using System.Collections;
using UnityEngine;
using PSC = PlayerStatsController;

public class BattleController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject levelsPrefab;
    [SerializeField] private GameObject activeCirclePrefab;
    [SerializeField] private SkillHUDController skillHUDController;
    [SerializeField] private CameraShakeController cameraShakeController;

    private GameObject currentLevel;
    private int currentRound;
    private int roundsCount;

    private GameObject activeCircle;

    private PlayerController playerController;
    private EnemyBase[] enemyStats;

    private TextMesh roundText;
    private TextMesh tutorialText;
    private GameObject unlockTextGO;

    private readonly Vector3 playerSpawnPosition = new Vector3(-6, -1, 0);
    private readonly Vector3[] enemySpawnPosition = { new Vector3(6, -1, 0), new Vector3(7f, 1.5f, 0), new Vector3(7f, -3.5f, 0) };

    private enum BattleState { START, PLAYERTURN, ENEMYTURN }
    private BattleState state;

    private int enemyCount;
    private int selectedEnemy; // player's current target: 0 = middle enemy, 1 = top, 2 = botom
    private bool isSecondTurn;
    IEnumerator playerSlideBackCoroutine;
    IEnumerator[] enemySlideBackCoroutine;


    private void Awake()
    {
        roundText = transform.GetChild(1).GetComponent<TextMesh>();
        tutorialText = transform.GetChild(2).GetComponent<TextMesh>();
        unlockTextGO = transform.GetChild(3).gameObject;
    }

    void Start()
    {
        currentRound = 0;
        SetupLevel();
        SetupPlayer();
        SetupRound();
    }

    void SetupLevel()
    {
        // Set level's text
        transform.GetChild(0).GetComponent<TextMesh>().text = "Level " + PSC.ps.level.ToString();

        // Set game's level count
        if (PSC.levelCount == 0)
        {
            PSC.levelCount = levelsPrefab.transform.childCount;
        }

        currentLevel = levelsPrefab.transform.GetChild(PSC.ps.level - 1).gameObject;

        // Set tutorial message
        if (PSC.ps.maxLevelReached == PSC.ps.level)
        {
            switch (PSC.ps.level)
            {
                case 1:
                    tutorialText.text = "Click with left mouse button on the skill or use shortcut [Q].";
                    break;
                case 2:
                    tutorialText.text = "Tip: Enemies get stronger every level";
                    break;
                case 3:
                    tutorialText.text = "Tip: Some enemies have special abilities (red one is stronger).";
                    break;
                case 4:
                    tutorialText.text = "Press TAB to switch between enemies.";
                    break;
                default:
                    tutorialText.text = "";
                    break;
            }
        }
        else
        {
            tutorialText.text = "";
        }

        activeCircle = Instantiate(activeCirclePrefab, enemySpawnPosition[0], Quaternion.identity);
    }

    void SetupPlayer()
    {
        playerController = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity).GetComponent<PlayerController>();
    }

    void SetupRound()
    {
        GameObject round = Instantiate(currentLevel.transform.GetChild(currentRound).gameObject);

        roundsCount = currentLevel.transform.childCount;
        enemyCount = round.transform.childCount;
        roundText.text = "Round: " + (currentRound + 1).ToString() + '/' + roundsCount.ToString();

        SetupEnemies(round);
        activeCircle.SetActive(true);
        activeCircle.transform.position = enemySpawnPosition[0];

        selectedEnemy = 0; // target middle enemy

        state = BattleState.PLAYERTURN;
    }

    void SetupEnemies(GameObject round)
    {
        enemySlideBackCoroutine = new IEnumerator[enemyCount];
        enemyStats = new EnemyBase[enemyCount];
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = round.transform.GetChild(i).gameObject;
            enemy.transform.Translate(enemySpawnPosition[i]);
            enemyStats[i] = enemy.GetComponent<EnemyBase>();
        }
    }


    #region SkillButtons
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN || !PSC.ps.isSkillUnlocked[(int)Skill.Basic]) { return; }

        state = BattleState.ENEMYTURN;
        StartCoroutine(PlayerAttack(Skill.Basic));
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Heal) || !PSC.ps.isSkillUnlocked[(int)Skill.Heal]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Heal, PSC.ps.healRecharge);
        StartCoroutine(PlayerHeal());
    }
    public void OnStunButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Stun) || !PSC.ps.isSkillUnlocked[(int)Skill.Stun]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Stun, PSC.ps.stunRecharge);
        StartCoroutine(PlayerAttack(Skill.Stun));
    }
    public void OnArealAttackButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Areal) || !PSC.ps.isSkillUnlocked[(int)Skill.Areal]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Areal, PSC.ps.arealRecharge);
        StartCoroutine(PlayerAttack(Skill.Areal));
    }
    public void OnPoisonButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Poison) || !PSC.ps.isSkillUnlocked[(int)Skill.Poison]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Poison, PSC.ps.poisonRecharge);
        StartCoroutine(PlayerAttack(Skill.Poison));
    }
    public void OnShieldButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Shield) || !PSC.ps.isSkillUnlocked[(int)Skill.Shield]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Shield, PSC.ps.shieldRecharge);
        StartCoroutine(PlayerUseShield());
    }
    public void OnBuffButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Buff) || !PSC.ps.isSkillUnlocked[(int)Skill.Buff]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Buff, PSC.ps.buffRecharge);
        StartCoroutine(PlayerUseBuff());
    }
    public void OnUltimateButton()
    {
        if (state != BattleState.PLAYERTURN || !skillHUDController.IsSkillReady(Skill.Ultimate) || !PSC.ps.isSkillUnlocked[(int)Skill.Ultimate]) { return; }

        state = BattleState.ENEMYTURN;
        skillHUDController.SetCooldown(Skill.Ultimate, PSC.ps.ultimateRecharge);
        StartCoroutine(PlayerAttack(Skill.Ultimate));
    }
    #endregion


    IEnumerator PlayerAttack(Skill skill)
    {
        if (playerSlideBackCoroutine != null) // stop sliding back if still executing
        {
            StopCoroutine(playerSlideBackCoroutine);
        }
        StartCoroutine(playerController.MoveTo(enemySpawnPosition[selectedEnemy], 1.5f));
        playerController.PlayAttack();
        yield return new WaitForSeconds(0.5f); // Wait for slide attack

        playerController.Attack(enemyStats, selectedEnemy, skill, cameraShakeController);

        playerSlideBackCoroutine = playerController.MoveTo(playerSpawnPosition, 0.01f);
        StartCoroutine(playerSlideBackCoroutine);


        yield return new WaitForSeconds(0.5f); // Wait for slide back

        if (IsAnyEnemyDead())
        {
            if (IsRoundOver())
            {
                StartCoroutine(EndRound());
                yield break;
            }
            else
            {
                TargetAnotherEnemy();
            }
        }

        // Second turn chance - can happen only once per player's turn
        if (Random.Range(1, 101) <= PSC.ps.secondTurnChance && !isSecondTurn)
        {
            isSecondTurn = true; // disable second turn chance next round
            skillHUDController.DecreseCooldowns();
            playerController.DisplaySecondTurn();
            state = BattleState.PLAYERTURN;
        }
        else
        {
            isSecondTurn = false;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerHeal()
    {
        playerController.HealPercentage();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerUseShield()
    {
        playerController.ApplyShield(PSC.ps.shieldLength);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerUseBuff()
    {
        playerController.ApplyBuff(PSC.ps.buffLength);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(EnemyTurn());
    }

    private bool IsAnyEnemyDead()
    {
        bool result = false;
        foreach (EnemyBase eb in enemyStats)
        {
            if (eb.IsDead() && !eb.isDestroyed)
            {
                eb.Die();
                eb.isDestroyed = true;

                result = true;
            }
        }
        return result;
    }

    private void TargetAnotherEnemy()
    {
        if (!enemyStats[(selectedEnemy + 1) % enemyCount].IsDead())
        {
            selectedEnemy = (selectedEnemy + 1) % enemyCount;
        }
        else
        {
            selectedEnemy = (selectedEnemy + 2) % enemyCount;
        }
        activeCircle.transform.position = enemySpawnPosition[selectedEnemy];
    }

    IEnumerator EnemyTurn()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyStats[i].IsDead()) { continue; }

            if (enemyStats[i].IsPoisoned())
            {
                enemyStats[i].DamageByPoison(PSC.ps.poisonEffect);

                if (enemyStats[i].IsDead())
                {
                    enemyStats[i].Die();
                    enemyStats[i].isDestroyed = true;

                    if (IsRoundOver())
                    {
                        StartCoroutine(EndRound());
                        yield break;
                    }
                    else
                    {
                        TargetAnotherEnemy();
                        continue;
                    }
                }
            }

            if (enemyStats[i].IsStuned())
            {
                enemyStats[i].ProcessStun();
                continue;
            }

            if (enemySlideBackCoroutine[i] != null) // stop sliding back if still executing
            {
                StopCoroutine(enemySlideBackCoroutine[i]);
            }
            StartCoroutine(enemyStats[i].MoveTo(playerSpawnPosition, 1.5f));
            enemyStats[i].PlayAttack();
            yield return new WaitForSeconds(0.5f); // Wait for slide attack

            enemyStats[i].Attack(playerController);

            // Thorns skill
            if (PSC.ps.isSkillUnlocked[(int)Skill.Thorns])
            {
                if (Random.Range(1, 101) <= PSC.ps.thornsChance)
                {
                    enemyStats[i].DamageByThorns(PSC.ps.thornsEffect);
                }
            }

            enemySlideBackCoroutine[i] = enemyStats[i].MoveTo(enemySpawnPosition[i], 0.01f);
            StartCoroutine(enemySlideBackCoroutine[i]);
            yield return new WaitForSeconds(0.5f); // Wait for slide back

            if (playerController.IsDead())
            {
                playerController.Die();
                yield return new WaitForSeconds(1);
                PSC.ps.LoadShop();
                yield break;
            }

            // Check death by thorns
            if (enemyStats[i].IsDead())
            {
                if (enemySlideBackCoroutine[i] != null) // stop sliding back if still executing
                {
                    StopCoroutine(enemySlideBackCoroutine[i]);
                }

                enemyStats[i].Die();
                enemyStats[i].isDestroyed = true;

                if (IsRoundOver())
                {
                    StartCoroutine(EndRound());
                    yield break;
                }
                else
                {
                    TargetAnotherEnemy();
                }
            }
        }

        skillHUDController.DecreseCooldowns();

        // Passive healing skill
        if (PSC.ps.isSkillUnlocked[(int)Skill.PassiveHealing])
        {
            playerController.PassiveHeal(playerController);
        }

        if (playerController.IsShielded())
        {
            playerController.ProcessShield();
        }

        if (playerController.IsStuned())
        {
            playerController.ProcessStun();
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = BattleState.PLAYERTURN;
        }
    }


    IEnumerator EndRound()
    {
        state = BattleState.START;
        activeCircle.SetActive(false);

        if (currentRound != roundsCount - 1)
        {
            yield return new WaitForSeconds(1);
            currentRound++;
            SetupRound(); // Start next round
        }
        else  // Last round just finished
        {
            yield return new WaitForSeconds(1);

            playerController.SaveCurrentHP();

            // Last level finished
            if (PSC.ps.level == PSC.levelCount)
            {
                PSC.ps.LoadWin();
            }
            else
            {
                int level = PSC.ps.level;

                if (level == PSC.ps.maxLevelReached)
                {
                    PSC.ps.maxLevelReached++;
                    PSC.ps.AddSkillPoints();
                }

                // Unlock standard skill (total count = 8)
                if (level <= 8 && !PSC.ps.isSkillUnlocked[level])
                {
                    unlockTextGO.SetActive(true);
                    unlockTextGO.GetComponent<TextMesh>().text = "New skill unlocked!";

                    PSC.ps.isSkillUnlocked[level] = true;
                    yield return new WaitForSeconds(2);
                }

                // Unlock special skill every even round (total count = 4)
                int specialSkillIndex = 7 + (level / 2);
                if (level % 2 == 0 && !PSC.ps.isSkillUnlocked[specialSkillIndex])
                {
                    // Reset text to play the animation again
                    unlockTextGO.SetActive(false);
                    unlockTextGO.SetActive(true);

                    unlockTextGO.GetComponent<TextMesh>().text = "New special unlocked!";
                    PSC.ps.isSkillUnlocked[specialSkillIndex] = true; // indexes 0-7 skills, 8-11 special skills
                    yield return new WaitForSeconds(2);
                }

                PSC.ps.level++;
                PSC.ps.LoadLobby();
            }
        }
    }

    bool IsRoundOver()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (!enemyStats[i].IsDead())
            {
                return false;
            }
        }
        return true;
    }


    void Update()
    {
        // Skills shortcuts
        if (Input.GetKeyDown(KeyCode.Q)) { OnAttackButton(); }
        if (Input.GetKeyDown(KeyCode.W)) { OnHealButton(); }
        if (Input.GetKeyDown(KeyCode.E)) { OnStunButton(); }
        if (Input.GetKeyDown(KeyCode.R)) { OnArealAttackButton(); }
        if (Input.GetKeyDown(KeyCode.A)) { OnPoisonButton(); }
        if (Input.GetKeyDown(KeyCode.S)) { OnShieldButton(); }
        if (Input.GetKeyDown(KeyCode.D)) { OnBuffButton(); }
        if (Input.GetKeyDown(KeyCode.F)) { OnUltimateButton(); }


        // Enemy target switching
        if (Input.GetKeyDown(KeyCode.Tab) && state == BattleState.PLAYERTURN && !IsRoundOver())
        {
            for (int i = 0; i < enemyCount; i++)
            {
                selectedEnemy = (selectedEnemy + 1) % enemyCount;
                if (!enemyStats[selectedEnemy].IsDead())
                {
                    break;
                }
            }
            activeCircle.transform.position = enemySpawnPosition[selectedEnemy];
        }
    }
}