using UnityEngine;

public class SeguimientoCamera : MonoBehaviour
{
    public Transform player;  // Asigna el personaje en Unity
    public Vector3 offset = new Vector3(0, 2, -10); // Ajusta la posici�n de la c�mara
    public float smoothSpeed = 5f; // Velocidad de seguimiento

    void LateUpdate()
    {
        if (player != null)
        {
            // Movimiento suave hacia la posici�n del jugador
            Vector3 targetPosition = player.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
