using UnityEngine;

public class LevelTextController : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMesh>().text = PlayerStatsController.ps.level.ToString();
    }
}
