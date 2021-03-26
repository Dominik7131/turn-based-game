using UnityEngine;

public class DarkKnightController : EnemyBase
{
    private int stunCooldown;
    private int stunRecharge;
    private int stunLength;

    protected override void Start()
    {
        base.Start();
        stunRecharge = 6;
        stunCooldown = 0;
        stunLength = 2;
    }

    public override void Attack(PlayerController playerController)
    {
        int enemyDamage = damage;

        // Stun attack
        if (stunCooldown == 0 && !playerController.IsStuned())
        {
            enemyDamage = (enemyDamage / 2) + 1; // half damage
            playerController.ApplyStun(stunLength);
            stunCooldown = stunRecharge;
        }
        if (playerController.IsShielded())
        {
            enemyDamage = Mathf.RoundToInt(enemyDamage * (1 - (PlayerStatsController.ps.shieldEffect / 100f)));
        }

        playerController.TakeDamage(enemyDamage, Color.white);
        if (stunCooldown > 0)
        {
            stunCooldown--;
        }
    }

    // Immune to stun
    public override void ApplyStun(int rounds)
    {
        stunRemaining = 0;
    }
}
