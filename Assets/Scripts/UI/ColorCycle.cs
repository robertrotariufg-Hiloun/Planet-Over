using UnityEngine;
using UnityEngine.UI;

public class ColorCycle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 0.2f;
    Image img;

    void Awake() => img = GetComponent<Image>();

    void Update() => img.color = Color.HSVToRGB(Mathf.Repeat(Time.time * speed, 1f), 0.5f, 1f);

}
