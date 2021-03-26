using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    private TextMesh textMesh;

    private void Awake()
    {
        textMesh = transform.GetChild(0).GetComponent<TextMesh>();
    }

    private void Start()
    {
        Destroy(gameObject, 1f); // Destroy after 1 second
        transform.localPosition += new Vector3(0, 0.5f, 0);
    }

    public void Display(string text, Color color)
    {
        textMesh.color = color;
        textMesh.text = text;
    }
}
