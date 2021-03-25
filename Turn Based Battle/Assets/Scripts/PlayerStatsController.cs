using UnityEngine;
using UnityEngine.SceneManagement;
using SC = ShopController;

public enum Skill { Basic, Heal, Stun, Areal, Poison, Shield, Buff, Ultimate, SecondTurn, PoisonChance, PassiveHealing, Thorns }
public enum Ability { Strength, Vitality, Criticals, Looting };


public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController ps;

    // Basic stats
    public int strength { get; set; }
    public int vitality { get; set; }
    public int currentHP { get; set; }
    public int slideSpeed { get; set; }
    public int criticals { get; set; }
    public int looting { get; set; }

    // Currency
    public int xp { get; set; }
    public int skillPoints { get; set; }

    public static int levelCount;
    public int level { get; set; }
    public int maxLevelReached { get; set; }

    // Skill stats length of effect on enemy
    public int stunLength { get; set; }
    public int poisonLength { get; set; }
    public int shieldLength { get; set; }
    public int buffLength { get; set; }

    // Skill's cooldown length
    public int healRecharge { get; set; }
    public int stunRecharge { get; set; }
    public int arealRecharge { get; set; }
    public int poisonRecharge { get; set; }
    public int shieldRecharge { get; set; }
    public int buffRecharge { get; set; }
    public int ultimateRecharge { get; set; }

    // Skill's efficiency
    public int basicEffect { get; set; }
    public int healingEffect { get; set; }
    public int poisonEffect { get; set; }
    public int shieldEffect { get; set; }
    public int buffEffect { get; set; }
    public int ultimateEffect { get; set; }
    public int passiveHealingEffect { get; set; }
    public int thornsEffect { get; set; }

    public int poisonChance { get; set; }
    public int secondTurnChance { get; set; }
    public int thornsChance { get; set; }

    public static readonly int skillCount = 12;
    public bool[] isSkillUnlocked { get; set; } // index = Skill enum index
    public int[] skillTreeLevels { get; set; }


    private void Awake()
    {
        if (ps == null)
        {
            ps = this;
        }
        else
        {
            Debug.LogWarning("PlayerStatsController destroyed");
            Destroy(ps);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Init player's stats
        vitality = strength = criticals = looting = level = maxLevelReached = 1;
        slideSpeed = 5;

        ps.skillTreeLevels = new int[skillCount];
        ps.isSkillUnlocked = new bool[skillCount];
        ps.isSkillUnlocked[(int)Skill.Basic] = true;

        xp = skillPoints = 0;

        LoadLobby();
    }

    private void UpdateSkills() // Alternative: assign skill properties immediately after buying them (but that would be long switches for add & remove)
    {
        ps.basicEffect = SC.basicEffect[ps.skillTreeLevels[(int)Skill.Basic]];

        ps.healingEffect = SC.healingEffect[ps.skillTreeLevels[(int)Skill.Heal]];
        ps.healRecharge = SC.healingCooldown[ps.skillTreeLevels[(int)Skill.Heal]];

        ps.stunLength = SC.stunLength[ps.skillTreeLevels[(int)Skill.Stun]];
        ps.stunRecharge = SC.stunCooldown[ps.skillTreeLevels[(int)Skill.Stun]];

        ps.arealRecharge = SC.arealCooldown[ps.skillTreeLevels[(int)Skill.Areal]];

        ps.poisonEffect = SC.poisonEffect[ps.skillTreeLevels[(int)Skill.Poison]];
        ps.poisonLength = SC.poisonLength[ps.skillTreeLevels[(int)Skill.Poison]];
        ps.poisonRecharge = SC.poisonCooldown[ps.skillTreeLevels[(int)Skill.Poison]];

        ps.shieldEffect = SC.shieldEffect[ps.skillTreeLevels[(int)Skill.Shield]];
        ps.shieldLength = SC.shieldLength[ps.skillTreeLevels[(int)Skill.Shield]];
        ps.shieldRecharge = SC.shieldCooldown[ps.skillTreeLevels[(int)Skill.Shield]];

        ps.buffEffect = SC.buffEffect[ps.skillTreeLevels[(int)Skill.Buff]];
        ps.buffLength = SC.buffLength[ps.skillTreeLevels[(int)Skill.Buff]];
        ps.buffRecharge = SC.buffCooldown[ps.skillTreeLevels[(int)Skill.Buff]];

        ps.ultimateEffect = SC.ultimateEffect[ps.skillTreeLevels[(int)Skill.Ultimate]];
        ps.ultimateRecharge = SC.ultimateCooldown[ps.skillTreeLevels[(int)Skill.Ultimate]];

        ps.secondTurnChance = SC.poisonChance[ps.skillTreeLevels[(int)Skill.SecondTurn]]; // same as poison chance
        ps.poisonChance = SC.poisonChance[ps.skillTreeLevels[(int)Skill.PoisonChance]];

        ps.thornsChance = SC.thornsChance[ps.skillTreeLevels[(int)Skill.Thorns]];
        ps.thornsEffect = SC.thornsEffect[ps.skillTreeLevels[(int)Skill.Thorns]];

        ps.passiveHealingEffect = SC.passiveHealingEffect[ps.skillTreeLevels[(int)Skill.PassiveHealing]];
    }

    public int GetMaxHealth()
    {
        return 10 + (ps.vitality * 5);
    }

    public void AddSkillPoints()
    {
        ps.skillPoints += 1 + (ps.level / 2);
    }

    public void CheatAddXPSP(int value)
    {
        ps.xp += value;
        ps.skillPoints += value;
    }

    public void CheatUnlockEverything()
    {
        for (int i = 0; i < skillCount; i++)
        {
            ps.isSkillUnlocked[i] = true;
        }
    }

    public void OnFightButton()
    {
        UpdateSkills();

        if (ps.currentHP == 0)
        {
            ps.currentHP = ps.GetMaxHealth();
        }
        SceneManager.LoadScene("Level");
    }

    public void LoadWin()
    {
        SceneManager.LoadScene("Win");
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void LoadShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void LoadCheats()
    {
        SceneManager.LoadScene("Cheats");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
