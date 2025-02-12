using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    private Vector3 lastPosition;
    private float totalDistance = 0f;

    public Text distanceText;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        totalDistance += distanceThisFrame;

        lastPosition = transform.position;

        if (distanceText != null)
        {
            distanceText.text = "Distancia: " + totalDistance.ToString("F2") + "m";
        }
    }
}
