using UnityEngine;

public class XPController : MonoBehaviour
{
    private Transform target;
    private int moveSpeed = 4;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        transform.position = new Vector2(transform.position.x + Random.Range(-1f, 2f), transform.position.y + Random.Range(-1f, 2f));
    }

    void Update()
    {
        // Attract XP to target (player)
        transform.position += (target.position - transform.position) * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStatsController.ps.xp += 1;
        Destroy(gameObject);
    }
}
