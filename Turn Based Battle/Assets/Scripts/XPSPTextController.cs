using UnityEngine;

public class XPSPTextController : MonoBehaviour
{
    private TextMesh textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "XP: " + PlayerStatsController.ps.xp.ToString() + ", SP: " + PlayerStatsController.ps.skillPoints.ToString();
    }
}
