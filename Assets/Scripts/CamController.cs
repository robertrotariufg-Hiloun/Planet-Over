using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] 
    private Transform player;
    [SerializeField] 
    private Vector3 camOffset = new Vector3(0, 0, -10);

    [Header("Limites")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void LateUpdate()
    {
        float x = Mathf.Clamp(player.position.x + camOffset.x, minX, maxX);
        float y = Mathf.Clamp(player.position.y + camOffset.y, minY, maxY);

        transform.position = new Vector3(x, y, camOffset.z);
    }
}
