using UnityEngine;

public class SeguimientoCamera : MonoBehaviour
{
    public Transform player;  // Asigna el personaje en Unity
    public Vector3 offset = new Vector3(0, 2, -10); // Ajusta la posición de la cámara
    public float smoothSpeed = 5f; // Velocidad de seguimiento

    void LateUpdate()
    {
        if (player != null)
        {
            // Movimiento suave hacia la posición del jugador
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
