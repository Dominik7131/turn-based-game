using UnityEngine;

public class SkillHUDController : MonoBehaviour
{
    [SerializeField] private GameObject cooldownTextPrefab;

    private GameObject[] cooldownTexts;
    private GameObject[] skillButtons;
    private GameObject[] shortcutTexts;

    private GameObject cooldownsObject;
    private GameObject combatButtons;
    private GameObject shortcuts;
    private int[] cooldowns;
    private int skillsSlots;

    void Start()
    {
        skillsSlots = 8;
        cooldownTexts = new GameObject[skillsSlots];
        skillButtons = new GameObject[skillsSlots];
        shortcutTexts = new GameObject[skillsSlots];
        cooldowns = new int[skillsSlots];
        cooldownsObject = transform.GetChild(0).gameObject;
        combatButtons = transform.GetChild(1).gameObject;
        shortcuts = transform.GetChild(2).gameObject;

        for (int i = 0; i < skillsSlots; i++)
        {
            cooldownTexts[i] = cooldownsObject.transform.GetChild(i).gameObject;
            skillButtons[i] = combatButtons.transform.GetChild(i).gameObject;
            shortcutTexts[i] = shortcuts.transform.GetChild(i).gameObject;

            if (PlayerStatsController.ps.isSkillUnlocked[i])
            {
                skillButtons[i].SetActive(true);
                shortcutTexts[i].SetActive(true);
            }
            else
            {
                skillButtons[i].SetActive(false);
                shortcutTexts[i].SetActive(false);
            }
        }
    }

    public void SetCooldown(Skill skill, int cooldown)
    {
        int index = (int)skill;
        cooldowns[index] = cooldown;
        cooldownTexts[index].GetComponent<TextMesh>().text = cooldown.ToString();
        cooldownTexts[index].SetActive(true);
    }

    public bool IsSkillReady(Skill skill)
    {
        return cooldowns[(int)skill] == 0;
    }

    public void DecreseCooldowns()
    {
        for (int i = 1; i < cooldownTexts.Length; i++)
        {
            if (cooldowns[i] == 0)
            {
                continue;
            }
            else if (cooldowns[i] == 1)
            {
                cooldowns[i]--;
                cooldownTexts[i].SetActive(false);
            }
            else
            {
                cooldowns[i]--;
                cooldownTexts[i].GetComponent<TextMesh>().text = cooldowns[i].ToString();
            }
        }
    }
}
