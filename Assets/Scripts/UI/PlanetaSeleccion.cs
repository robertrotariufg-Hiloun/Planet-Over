using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlanetaSeleccion : MonoBehaviour
{
    [SerializeField] private string escenaDestino;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Cargar);
    }

    private void Cargar()
    {
        if (string.IsNullOrEmpty(escenaDestino))
        {
            Debug.LogWarning($"{name}: falta la escena de destino.", this);
        }
        else
        {
            SceneManager.LoadScene(escenaDestino);
        }
    }
}