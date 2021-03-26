using UnityEngine;

public class PlayerController : CharacterBase
{
    protected int criticalHit { get; set; }


    protected override void Start()
    {
        maxHP = PlayerStatsController.ps.GetMaxHealth();
        currentHP = PlayerStatsController.ps.currentHP;
        damage = PlayerStatsController.ps.strength;
        slideSpeed = PlayerStatsController.ps.slideSpeed;
        criticalHit = PlayerStatsController.ps.criticals;

        base.Start();
    }

    public void Attack(EnemyBase[] enemyStats, int selectedEnemy, Skill skill, CameraShakeController cameraShakeController)
    {
        int playerDamage = damage;
        Color attackColor = Color.white;

        int criticalChance = 10 + ((criticalHit - 1) * 2);
        if (Random.Range(1, 100) <= criticalChance)
        {
            // Critical hit
            float criticalMultiplier = 2 + (criticalHit / 10f);
            Debug.Log("Player damage: " + playerDamage);
            playerDamage = Mathf.RoundToInt(playerDamage * criticalMultiplier);
            Debug.Log("Critical multiplier: " + criticalMultiplier);
            Debug.Log("Critical damage: " + playerDamage);
            attackColor = Color.red;
            StartCoroutine(cameraShakeController.Shake(0.1f, 0.8f));
        }

        if (IsBuffed())
        {
            playerDamage *= 2;
        }

        if (skill == Skill.Areal)
        {
            // Damage all enemies
            foreach (EnemyBase eb in enemyStats)
            {
                if (!eb.isDestroyed)
                {
                    eb.TakeDamage(playerDamage, attackColor);
                }
            }
        }
        else
        {
            if (skill == Skill.Basic)
            {
                playerDamage += Mathf.RoundToInt(playerDamage * (PlayerStatsController.ps.basicEffect / 100f));

                // Chance for poison attack
                if (PlayerStatsController.ps.isSkillUnlocked[(int)Skill.PoisonChance] && Random.Range(1, 100) <= PlayerStatsController.ps.poisonChance)
                {
                    enemyStats[selectedEnemy].ApplyPoison(PlayerStatsController.ps.poisonLength);
                }
            }
            else if (skill == Skill.Stun)
            {
                playerDamage = Mathf.RoundToInt(playerDamage / 2f); // half damage
                enemyStats[selectedEnemy].ApplyStun(PlayerStatsController.ps.stunLength);
            }
            else if (skill == Skill.Ultimate)
            {
                playerDamage = Mathf.RoundToInt(playerDamage * PlayerStatsController.ps.ultimateEffect);
            }
            else if (skill == Skill.Poison)
            {
                enemyStats[selectedEnemy].ApplyPoison(PlayerStatsController.ps.poisonLength);
            }
            enemyStats[selectedEnemy].TakeDamage(playerDamage, attackColor);
        }
    }

    public void HealPercentage()
    {
        int value = Mathf.RoundToInt(maxHP * (PlayerStatsController.ps.healingEffect / 100f));
        HealAmmount(value);
    }

    public void PassiveHeal(PlayerController playerController)
    {
        int ammount = Mathf.RoundToInt(playerController.maxHP * (PlayerStatsController.ps.passiveHealingEffect / 100f));
        if (ammount != 0)
        {
            playerController.HealAmmount(ammount);
        }
    }

    public void DisplaySecondTurn()
    {
        GameObject points = Instantiate(damagePopUpPrefab, transform.position, Quaternion.identity);
        points.GetComponent<FloatingTextController>().Display("Second Turn", Color.yellow);
    }

    public void SaveCurrentHP()
    {
        PlayerStatsController.ps.currentHP = currentHP;
    }

    public override void Die()
    {
        base.Die();
        PlayerStatsController.ps.currentHP = 0;
        PlayerStatsController.ps.level = 1;
    }
}
