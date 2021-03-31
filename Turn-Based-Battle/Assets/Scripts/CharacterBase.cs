using System.Collections;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected int currentHP { get; set; }
    protected int maxHP { get; set; }
    protected int slideSpeed { get; set; }
    protected int damage { get; set; }

    protected int stunRemaining { get; set; }
    protected int poisonRemaining { get; set; }
    protected int shieldRemaining { get; set; }
    protected int buffRemaining { get; set; }

    [SerializeField] protected GameObject damagePopUpPrefab;
    [SerializeField] private GameObject destructionSplashPrefab;
    [SerializeField] private GameObject shield;

    protected HealthController healthController;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        healthController = GetComponent<HealthController>();
    }

    protected virtual void Start()
    {
        healthController.SetMaxHealth(maxHP);
        healthController.SetHealth(currentHP);
    }

    public void TakeDamage(int value, Color color)
    {
        currentHP -= value;
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        GameObject points = Instantiate(damagePopUpPrefab, transform.position, Quaternion.identity);
        points.GetComponent<FloatingTextController>().Display(value.ToString(), color);
        healthController.SetHealth(currentHP);
    }

    public void HealAmmount(int value)
    {
        currentHP += value;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        GameObject points = Instantiate(damagePopUpPrefab, transform.position, Quaternion.identity);
        points.GetComponent<FloatingTextController>().Display('+' + value.ToString(), Color.green);
        healthController.SetHealth(currentHP);
    }

    public virtual void ApplyStun(int rounds)
    {
        stunRemaining = rounds;
    }

    public virtual void ApplyPoison(int rounds)
    {
        poisonRemaining = rounds;
        healthController.SetColor(Color.green);
    }

    public void ApplyShield(int rounds)
    {
        shieldRemaining = rounds;
        ShowShield();
    }

    public void ShowShield()
    {
        shield.SetActive(true);
    }

    public void HideShield()
    {
        shield.SetActive(false);
    }

    public void ApplyBuff(int rounds)
    {
        buffRemaining = rounds;
    }

    public bool IsStuned()
    {
        return stunRemaining != 0;
    }

    public bool IsPoisoned()
    {
        return poisonRemaining != 0;
    }

    public bool IsShielded()
    {
        return shieldRemaining != 0;
    }

    public bool IsBuffed()
    {
        return buffRemaining != 0;
    }

    public void ProcessStun()
    {
        DisplayStun();
        stunRemaining--;
    }

    public void ProcessPoison()
    {
        poisonRemaining--;
        if (poisonRemaining == 0)
        {
            healthController.SetColor(Color.red);
        }
    }

    public void ProcessShield()
    {
        shieldRemaining--;
        if (shieldRemaining == 0)
        {
            HideShield();
        }
    }

    public void DamageByPoison(int percentage)
    {
        int damage = Mathf.RoundToInt((percentage / 100f) * maxHP);
        TakeDamage(damage, new Color(0.1f, 0.5f, 0)); // Dark green
        ProcessPoison();
    }

    public void DamageByThorns(int percentage)
    {
        int ammount = Mathf.RoundToInt((percentage / 100f) * damage);
        if (ammount != 0)
        {
            TakeDamage(ammount, Color.black);
        }
    }

    public void DisplayStun()
    {
        GameObject points = Instantiate(damagePopUpPrefab, transform.position, Quaternion.identity);
        points.GetComponent<FloatingTextController>().Display("Stunned: " + stunRemaining.ToString(), Color.white);
    }

    public void DisplayCooldown(int ammount)
    {
        GameObject points = Instantiate(damagePopUpPrefab, transform.position, Quaternion.identity);
        points.GetComponent<FloatingTextController>().Display("Cooldown: " + ammount.ToString(), Color.white);
    }

    public bool IsDead()
    {
        return currentHP == 0;
    }

    public void PlayAttack()
    {
        anim.Play("Attack");
    }

    public virtual void Die()
    {
        healthController.Hide();
        transform.GetChild(0).gameObject.SetActive(false); // hide name

        anim.Play("Die");
        GameObject destructionSplash = Instantiate(destructionSplashPrefab, transform.position, Quaternion.identity);
        Destroy(destructionSplash, 2f);
        Destroy(gameObject, 0.25f);
    }

    public IEnumerator MoveTo(Vector3 target, float offset)
    {
        while (Vector2.Distance(transform.position, target) > offset)
        {
            transform.position = Vector2.Lerp(transform.position, target, slideSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
