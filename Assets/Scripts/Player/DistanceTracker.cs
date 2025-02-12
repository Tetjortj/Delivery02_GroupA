using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    private Vector3 lastPosition; // �ltima posici�n del personaje
    private float totalDistance = 0f; // Distancia recorrida

    public Text distanceText; // Asigna el texto en el Inspector

    void Start()
    {
        lastPosition = transform.position; // Guardar la posici�n inicial
    }

    void Update()
    {
        // Calcular la distancia recorrida en este frame
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        totalDistance += distanceThisFrame; // Sumar a la distancia total

        // Actualizar la �ltima posici�n
        lastPosition = transform.position;

        // Mostrar la distancia en la UI
        if (distanceText != null)
        {
            distanceText.text = "Distancia: " + totalDistance.ToString("F2") + "m";
        }
    }
}
