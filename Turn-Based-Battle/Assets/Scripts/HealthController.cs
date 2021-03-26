using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMesh hpText;
    [SerializeField] private Image hpImage;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        hpText.text = health.ToString();
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        hpText.text = health.ToString();
    }

    public void SetColor(Color color)
    {
        hpImage.color = color;
    }

    public void Hide()
    {
        hpText.transform.parent.gameObject.SetActive(false);
    }
}
