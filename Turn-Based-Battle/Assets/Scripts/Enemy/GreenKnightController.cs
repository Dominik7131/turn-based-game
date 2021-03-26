using UnityEngine;

public class GreenKnightController : EnemyBase
{
    private int poisonCooldown;
    private int poisonRecharge;
    private int poisonLength;

    protected override void Start()
    {
        base.Start();
        poisonRecharge = 6;
        poisonCooldown = 0;
        poisonLength = 2;
    }

    // Poison attack
    public override void Attack(PlayerController playerController)
    {
        int enemyDamage = damage;

        if (playerController.IsShielded())
        {
            enemyDamage = Mathf.RoundToInt(enemyDamage * (1 - (PlayerStatsController.ps.shieldEffect / 100f)));
        }

        if (poisonCooldown == 0)
        {
            enemyDamage = Mathf.RoundToInt(enemyDamage / 2f); // half damage
            playerController.ApplyPoison(poisonLength);
            poisonCooldown = poisonRecharge;
        }
        playerController.TakeDamage(enemyDamage, Color.white);
        poisonCooldown--;
    }

    // Immune to poison
    public override void ApplyPoison(int rounds)
    {
        stunRemaining = 0;
    }
}
