using UnityEngine;


public class ShopController : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private TextMesh xp;
    [SerializeField] private TextMesh skillPoints;
    [SerializeField] private GameObject shopButtons;
    [SerializeField] private GameObject abilitiesTab;
    [SerializeField] private GameObject skillTreeTab;
    [SerializeField] private GameObject backgroundParrent;


    private GameObject abilitiesButton;
    private TextMesh abilityLevels;
    private TextMesh abilityPrices;

    private GameObject skillTreeButton;
    private TextMesh skillInfo;
    private GameObject skillPointAddButton;
    private GameObject skillPointRemoveButton;
    private GameObject[] skillButtons;

    private GameObject background;
    private GameObject backgroundBlured;

    Skill selectedSkill;

    #region Skill tree chart
    /* Basic skill:
    0: 0%
    1: 10%
    2: 20%
    3: 30%
    4: 40%
    5: 50% */
    public static readonly int[] basicEffect = { 0, 10, 20, 30, 40, 50 };

    /*Healing skill:                 
    0: 20%, cooldown: 10
    1: 25%, cooldown: 9
    2: 30%, cooldown: 9
    3: 35%, cooldown: 8
    4: 45%, cooldown: 8
    5: 50%, cooldown: 7 */
    public static readonly int[] healingEffect = { 20, 25, 30, 35, 45, 50 };
    public static readonly int[] healingCooldown = { 10, 9, 9, 8, 8, 7 };

    /* Stun skill:
    0: length: 2, cooldown: 8
    1: length: 2, cooldown: 7
    2: length: 3, cooldown: 7
    3: length: 3, cooldown: 6
    4: length: 4, cooldown: 6
    5: length: 4, cooldown: 5 */
    public static readonly int[] stunLength = { 2, 2, 3, 3, 4, 4 };
    public static readonly int[] stunCooldown = { 8, 7, 7, 6, 6, 5 };

    /* Areal skill:
    0: cooldown: 11
    1: cooldown: 10
    2: cooldown: 9
    3: cooldown: 8
    4: cooldown: 7
    5: cooldown: 6 */
    public static readonly int[] arealCooldown = { 11, 10, 9, 8, 7, 6 };

    /* Poison skill:
    0: 10%, length: 2, cooldown: 10
    1: 10%, length: 3, cooldown: 9
    2: 15%, length: 3, cooldown: 9
    3: 15%, length: 4, cooldown: 8
    4: 20%, length: 4, cooldown: 8
    5: 20%, length: 5, cooldown: 7 */
    public static readonly int[] poisonEffect = { 10, 10, 15, 15, 20, 20 };
    public static readonly int[] poisonLength = { 2, 3, 3, 4, 4, 5 };
    public static readonly int[] poisonCooldown = { 10, 9, 9, 8, 8, 7 };

    /* Shield skill:
    0: 30%, length: 2, cooldown: 12
    1: 40%, length: 2, cooldown: 12
    2: 50%, length: 3, cooldown: 11
    3: 60%, length: 3, cooldown: 11
    4: 70%, length: 4, cooldown: 11
    5: 80%, length: 4, cooldown: 10 */
    public static readonly int[] shieldEffect = { 30, 40, 50, 60, 70, 80 };
    public static readonly int[] shieldLength = { 2, 2, 3, 3, 4, 4 };
    public static readonly int[] shieldCooldown = { 12, 12, 11, 11, 11, 10 };

    /* Buff skill:
    0: 2x damage, length: 2, cooldown: 15
    1: 2x damage, length: 3, cooldown: 15
    2: 2x damage, length: 3, cooldown: 15
    3: 3x damage, length: 3, cooldown: 14
    4: 3x damage, length: 4, cooldown: 14
    5: 3x damage, length: 4, cooldown: 13 */
    public static readonly int[] buffEffect = { 2, 2, 2, 3, 3, 3 };
    public static readonly int[] buffLength = { 2, 3, 3, 3, 4, 4 };
    public static readonly int[] buffCooldown = { 15, 15, 15, 14, 14, 13 };

    /* Ultimate skill:
    0: 2x damage, cooldown: 15
    1: 2x damage, cooldown: 14
    2: 3x damage, cooldown: 14
    3: 3x damage, cooldown: 13
    4: 4x damage, cooldown: 13
    5: 4x damage, cooldown: 12 */
    public static readonly int[] ultimateEffect = { 2, 2, 3, 3, 4, 4 };
    public static readonly int[] ultimateCooldown = { 15, 14, 14, 13, 13, 12 };

    /* Poison chance = Second turn:
    0: 0%
    1: 5%
    2: 10%
    3: 15%
    4: 20%
    5: 25% */
    public static readonly int[] poisonChance = { 0, 5, 10, 15, 20, 25 };

    /* Thorns:
    0: 0% chance, -0% damage
    1: 10% chance, -20% damage
    2: 15% chance, -20% damage
    3: 20% chance, -25% damage
    4: 25% chance, -25% damage
    5: 30% chance, -30% damage */
    public static readonly int[] thornsChance = { 0, 10, 15, 20, 25, 30 };
    public static readonly int[] thornsEffect = { 0, 20, 20, 25, 25, 30 };

    /* Passive Healing:
    // 0: +0%
    // 1: +2%
    // 2: +4%
    // 3: +6%
    // 4: +8%
    // 5: +10% */
    public static readonly int[] passiveHealingEffect = { 0, 2, 4, 6, 8, 10 };
    #endregion

    private void Awake()
    {
        abilitiesButton = shopButtons.transform.GetChild(0).gameObject;
        skillTreeButton = shopButtons.transform.GetChild(1).gameObject;

        abilityLevels = abilitiesTab.transform.GetChild(2).GetComponent<TextMesh>();
        abilityPrices = abilitiesTab.transform.GetChild(3).GetComponent<TextMesh>();

        skillInfo = skillTreeTab.transform.GetChild(1).GetComponent<TextMesh>();
        skillPointAddButton = skillTreeTab.transform.GetChild(2).gameObject;
        skillPointRemoveButton = skillTreeTab.transform.GetChild(3).gameObject;
        skillButtons = new GameObject[PlayerStatsController.skillCount];

        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i] = skillTreeTab.transform.GetChild(i + 5).gameObject;
        }

        background = backgroundParrent.transform.GetChild(0).gameObject;
        backgroundBlured = backgroundParrent.transform.GetChild(1).gameObject;
    }

    void Start()
    {
        abilitiesTab.SetActive(false);
        skillTreeTab.SetActive(false);
        background.SetActive(true);
        backgroundBlured.SetActive(false);

        xp.text = "XP: " + PlayerStatsController.ps.xp.ToString();
        skillPoints.text = "SP: " + PlayerStatsController.ps.skillPoints.ToString();
    }

    private void ShowUnlockedButtons()
    {
        for (int i = 0; i < PlayerStatsController.skillCount; i++)
        {
            if (PlayerStatsController.ps.isSkillUnlocked[i])
            {
                skillButtons[i].SetActive(true);
            }
            else
            {
                skillButtons[i].SetActive(false);
            }
        }
    }

    public void OnAbilitiesOpenButton()
    {
        abilitiesButton.SetActive(false);
        skillTreeButton.SetActive(false);
        abilitiesTab.SetActive(true);
        background.SetActive(false);
        backgroundBlured.SetActive(true);

        UpdateAbilitiesTexts();
    }

    public void OnAbilitiesCloseButton()
    {
        abilitiesButton.SetActive(true);
        skillTreeButton.SetActive(true);
        abilitiesTab.SetActive(false);
        backgroundBlured.SetActive(false);
        background.SetActive(true);
    }

    public void OnSkillTreeOpenButton()
    {
        abilitiesButton.SetActive(false);
        skillTreeButton.SetActive(false);

        ShowUnlockedButtons();
        skillPointAddButton.SetActive(false);
        skillPointRemoveButton.SetActive(false);

        skillTreeTab.SetActive(true);
        background.SetActive(false);
        backgroundBlured.SetActive(true);
    }

    public void OnSkillTreeCloseButton()
    {
        abilitiesButton.SetActive(true);
        skillTreeButton.SetActive(true);
        skillTreeTab.SetActive(false);
        backgroundBlured.SetActive(false);
        background.SetActive(true);
    }

    private void UpdateAbilitiesTexts()
    {
        abilityLevels.text = PlayerStatsController.ps.strength.ToString() + "\n" + PlayerStatsController.ps.vitality.ToString() + "\n"
              + PlayerStatsController.ps.criticals.ToString() + "\n" + PlayerStatsController.ps.looting.ToString();
        abilityPrices.text = abilityLevels.text; // Right now same but prices can be modified
    }

    public void OnAddButton(int a)
    {
        Ability ability = (Ability)a;

        int price = 0;

        switch (ability)
        {
            case Ability.Strength:
                price = PlayerStatsController.ps.strength;
                if (PlayerStatsController.ps.xp < price) { return; }
                PlayerStatsController.ps.strength++;
                break;
            case Ability.Vitality:
                price = PlayerStatsController.ps.vitality;
                if (PlayerStatsController.ps.xp < price) { return; }
                PlayerStatsController.ps.vitality++;
                break;
            case Ability.Criticals:
                price = PlayerStatsController.ps.criticals;
                if (PlayerStatsController.ps.xp < price) { return; }
                PlayerStatsController.ps.criticals++;
                break;
            case Ability.Looting:
                price = PlayerStatsController.ps.looting;
                if (PlayerStatsController.ps.xp < price) { return; }
                PlayerStatsController.ps.looting++;
                break;
        }

        PlayerStatsController.ps.xp -= price;
        UpdateAbilitiesTexts();
        xp.text = "XP: " + PlayerStatsController.ps.xp.ToString();
    }

    public void OnRemoveButton(int a)
    {
        Ability ability = (Ability)a;
        int previousPrice = 0;

        switch (ability)
        {
            case Ability.Strength:
                previousPrice = PlayerStatsController.ps.strength - 1;
                if (PlayerStatsController.ps.strength <= 1) { return; }
                PlayerStatsController.ps.strength--;
                break;
            case Ability.Vitality:
                previousPrice = PlayerStatsController.ps.vitality - 1;
                if (PlayerStatsController.ps.vitality <= 1) { return; }
                PlayerStatsController.ps.vitality--;
                break;
            case Ability.Criticals:
                previousPrice = PlayerStatsController.ps.criticals - 1;
                if (PlayerStatsController.ps.criticals <= 1) { return; }
                PlayerStatsController.ps.criticals--;
                break;
            case Ability.Looting:
                previousPrice = PlayerStatsController.ps.looting - 1;
                if (PlayerStatsController.ps.looting <= 1) { return; }
                PlayerStatsController.ps.looting--;
                break;
        }

        PlayerStatsController.ps.xp += previousPrice;
        UpdateAbilitiesTexts();
        xp.text = "XP: " + PlayerStatsController.ps.xp.ToString();
    }

    public void OnSkillButton(int s)
    {
        selectedSkill = (Skill)s;
        int skillLevel = PlayerStatsController.ps.skillTreeLevels[s];
        string info = "Level: " + skillLevel + "/5 \n";

        skillPointAddButton.SetActive(true);
        skillPointRemoveButton.SetActive(true);

        switch (selectedSkill)
        {
            case Skill.Basic:
                info += "Increases damage for basic attack \n";
                info += "Current: " + (100 + basicEffect[skillLevel]).ToString() + "% damage \n";
                if (skillLevel < 5)
                {
                    info += "Next: " + (100 + basicEffect[skillLevel + 1]).ToString() + "% damage";
                }
                break;
            case Skill.Heal:
                info += "Increases heal skill effectivness and reducess cooldown \n";
                info += "Current: Heal: " + healingEffect[skillLevel].ToString() + "%, Cooldown: " + healingCooldown[skillLevel].ToString() + '\n';
                if (skillLevel < 5)
                {
                    info += "Next: Heal: " + healingEffect[skillLevel + 1].ToString() + "%, Cooldown: " + healingCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.Stun:
                info += "Increases stun effectivness and reducess cooldown \n";
                info += "Current: Length: " + stunLength[skillLevel].ToString() + " turns, Cooldown: " + stunCooldown[skillLevel].ToString() + "\n";
                if (skillLevel < 5)
                {
                    info += "Next: Length: " + stunLength[skillLevel + 1].ToString() + " turns, Cooldown: " + stunCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.Areal:
                info += "Decreses areal skill cooldown \n";
                info += "Current: Cooldown: " + arealCooldown[skillLevel].ToString() + '\n';
                if (skillLevel < 5)
                {
                    info += "Next: Cooldown: " + arealCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.Poison:
                info += "Increases poison skill effectivness and reducess cooldown \n";
                info += "Current: -" + poisonEffect[skillLevel].ToString() + "% HP per " + poisonLength[skillLevel].ToString() + " turns, Cooldown: " + poisonCooldown[skillLevel].ToString() + '\n';
                if (skillLevel < 5)
                {
                    info += "Next: -" + poisonEffect[skillLevel + 1].ToString() + "% HP per " + poisonLength[skillLevel + 1].ToString() + " turns, Cooldown: " + poisonCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.Shield:
                info += "Increases shield skill effectivness and reducess cooldown \n";
                info += "Current: " + shieldEffect[skillLevel].ToString() + "% damage reduction per " + shieldLength[skillLevel].ToString() + " turns, Cooldown: " + shieldCooldown[skillLevel].ToString() + '\n';
                if (skillLevel < 5)
                {
                    info += "Next: " + shieldEffect[skillLevel + 1].ToString() + "% damage reduction per " + shieldLength[skillLevel + 1].ToString() + " turns, Cooldown: " + shieldCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.Buff:
                info += "Increases buff skill effectivness and reducess cooldown \n";
                info += "Current: " + buffEffect[skillLevel].ToString() + "x damage per " + buffLength[skillLevel].ToString() + " turns, Cooldown: " + buffCooldown[skillLevel].ToString() + '\n';
                if (skillLevel < 5)
                {
                    info += "Next: " + buffEffect[skillLevel + 1].ToString() + "x damage per " + buffLength[skillLevel + 1].ToString() + " turns, Cooldown: " + buffCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.Ultimate:
                info += "Increases ultimate skill effectivness and reducess cooldown \n";
                info += "Current: " + ultimateEffect[skillLevel].ToString() + "x damage, Cooldown: " + ultimateCooldown[skillLevel].ToString() + '\n';
                if (skillLevel < 5)
                {
                    info += "Next: " + ultimateEffect[skillLevel + 1].ToString() + "x damage, Cooldown: " + ultimateCooldown[skillLevel + 1].ToString();
                }
                break;
            case Skill.SecondTurn:
                info += "Increases chance for second turn \n";
                // Same chance as poison chance
                info += "Current: " + poisonChance[skillLevel].ToString() + "% chance\n";
                if (skillLevel < 5)
                {
                    info += "Next: " + poisonChance[skillLevel + 1].ToString() + "% chance";
                }
                break;
            case Skill.PoisonChance:
                info += "Increases chance for basic attack to transform into poison attack \n";
                info += "Current: " + poisonChance[skillLevel].ToString() + "% chance\n";
                if (skillLevel < 5)
                {
                    info += "Next: " + poisonChance[skillLevel + 1].ToString() + "% chance";
                }
                break;
            case Skill.PassiveHealing:
                info += "Increases effectivness of passive healing every turn \n";
                info += "Current: +" + passiveHealingEffect[skillLevel] + "% HP\n";
                if (skillLevel < 5)
                {
                    info += "Next: +" + passiveHealingEffect[skillLevel + 1] + "% HP";
                }
                break;
            case Skill.Thorns:
                info += "Increases chance to reflect some enemy damage \n";
                info += "Current: " + thornsChance[skillLevel].ToString() + "% chance of " + thornsEffect[skillLevel] + "% reflected damage\n";
                if (skillLevel < 5)
                {
                    info += "Next: " + thornsChance[skillLevel + 1].ToString() + "% chance of " + thornsEffect[skillLevel + 1] + "% reflected damage";
                }
                break;
        }
        skillInfo.text = info;
    }

    public void OnSkillAddButton()
    {
        int index = (int)selectedSkill;

        if (PlayerStatsController.ps.skillTreeLevels[index] == 5 || PlayerStatsController.ps.skillPoints == 0)
        {
            return;
        }

        PlayerStatsController.ps.skillTreeLevels[index]++;
        PlayerStatsController.ps.skillPoints--;
        OnSkillButton(index); // Update text info
        skillPoints.text = "SP: " + PlayerStatsController.ps.skillPoints.ToString();
    }

    public void OnSkillRemoveButton()
    {
        int index = (int)selectedSkill;

        if (PlayerStatsController.ps.skillTreeLevels[index] == 0)
        {
            return;
        }

        PlayerStatsController.ps.skillTreeLevels[index]--;
        PlayerStatsController.ps.skillPoints++;
        OnSkillButton(index); // Update text info
        skillPoints.text = "SP: " + PlayerStatsController.ps.skillPoints.ToString();
    }
}
