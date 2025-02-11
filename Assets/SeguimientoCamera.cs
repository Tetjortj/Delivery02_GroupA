using UnityEngine;

public class SeguimientoCamera : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
    }
}
