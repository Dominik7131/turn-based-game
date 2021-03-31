using UnityEngine;

public class EnemyBase : CharacterBase
{
    [SerializeField] private GameObject xpPrefab;
    public bool isDestroyed { get; set; }


    protected override void Start()
    {
        maxHP = PlayerStatsController.ps.level * 4;
        currentHP = maxHP;
        damage = (PlayerStatsController.ps.level * 2) - 1;
        slideSpeed = 7;

        base.Start();
    }

    public virtual void Attack(PlayerController playerController)
    {
        Debug.Log("EnemyBase attack");
        int enemyDamage = damage;
        if (playerController.IsShielded())
        {
            enemyDamage = Mathf.RoundToInt(enemyDamage * (1 - (PlayerStatsController.ps.shieldEffect / 100f)));
        }
        playerController.TakeDamage(enemyDamage, Color.white);
    }

    public override void Die()
    {
        base.Die();

        // Get n XP for each enemy in Level n + looting ability bonus
        int value = PlayerStatsController.ps.level + PlayerStatsController.ps.looting - 1;
        for (int i = 0; i < value; i++)
        {
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        }
    }
}
