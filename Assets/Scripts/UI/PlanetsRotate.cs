using UnityEngine;

public class Orbita : MonoBehaviour
{
    [Header("Órbita")]
    public Transform centro;            // el astro del medio
    public float velocidad = 30f;       // grados por segundo
    public bool antihorario = true;

    [Header("Opcional")]
    public bool girarSobreSiMismo = false;
    public float velocidadRotacionPropia = 20f;

    float radio;
    float angulo;   // en radianes

    void Start()
    {
        Vector3 offset = transform.position - centro.position;
        radio = new Vector2(offset.x, offset.y).magnitude;
        angulo = Mathf.Atan2(offset.y, offset.x);
    }

    void Update()
    {
        float dir = antihorario ? 1f : -1f;
        angulo += dir * velocidad * Mathf.Deg2Rad * Time.deltaTime;

        transform.position = centro.position + new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f) * radio;

        if (girarSobreSiMismo)
        {
            transform.Rotate(0f, 0f, velocidadRotacionPropia * Time.deltaTime);
        }
    }
}