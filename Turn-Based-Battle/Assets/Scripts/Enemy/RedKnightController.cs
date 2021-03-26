
public class RedKnightController : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        damage *= 2;
    }
}