using System;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class EnemyIA : MonoBehaviour
{
    // GENERAL STINGS
    public float speed;
    private SpriteRenderer _spriteRenderer;

    // PATROL
    [SerializeField] private GameObject[] waypointList;
    private int _CWaypointTarget;
    private bool _direction;

    // MOVE TO
    [SerializeField] private float detectionRange;                              // Rango de detección de obstáculos.
    [SerializeField] private LayerMask whatIsNotAPlayer;                       // Máscara de capa para los obstáculos.
    private Orientation[] _directionsOrientation;                             // Array de direcciones precalculadas.

    private void Start()
    {
        // GENERAL STINGS
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // PATROL
        _CWaypointTarget = 0;
        _direction = true;
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    // Class Orientation
    private void Awake()
    {
        float[] angles = {  -135f, -90f, -45f, 45f, 90f, 135f, 180f, 360f};     // Ángulos predefinidos para las direcciones.
        _directionsOrientation = new Orientation[angles.Length];

        for (int i = 0; i < angles.Length; i++) {
            Vector3 direction = Quaternion.AngleAxis(angles[i], transform.forward) * transform.right;
            _directionsOrientation[i] = new Orientation(direction, false);
        }
    }

    // GENERAL FUNCTIONS 
    private void ChangeTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        _spriteRenderer.flipY = transform.rotation.eulerAngles.z is > 90 and < 270;
    }

    private void MoveTo(Vector3 target)
    {
        Vector3 bestDirection = Vector3.zero;
        float shortestDistance = float.MaxValue;

        // Itera sobre todas las direcciones posibles para encontrar la mejor.
        foreach (var orientation in _directionsOrientation) {
            Vector3 direction = orientation.GetDirection();
            Vector3 finalPosition = transform.position + direction * detectionRange;

            // Verifica si hay obstáculos en la dirección actual.
            if (!Physics2D.OverlapCircle(finalPosition, 0.4f, whatIsNotAPlayer))
            {
                // Calcula la distancia al objetivo en esta dirección.
                float distanceToTarget = Vector2.Distance(finalPosition, target);

                // Si esta dirección es mejor que las anteriores, actualiza la mejor dirección.
                if (distanceToTarget < shortestDistance)
                {
                    bestDirection = direction;
                    shortestDistance = distanceToTarget;
                }
            }
        }

        // Si se encontró una dirección válida, mueve al enemigo en esa dirección.
        if (bestDirection != Vector3.zero) {
            Vector3 movePosition = transform.position + bestDirection * (detectionRange * 0.5f); // Ajusta el rango de movimiento.
            transform.position = Vector2.MoveTowards(transform.position, movePosition, speed * Time.fixedDeltaTime);
        }
    }

    // FUNCTION PATROL
    private void Patrol()
    {
        ChangeTarget(waypointList[_CWaypointTarget].transform);

        if (Vector2.Distance(transform.position, waypointList[_CWaypointTarget].transform.position) < 0.1f)
        {
            if (_direction) {
                _CWaypointTarget++;
            } else {
                _CWaypointTarget--;
            }

            // Cambia la dirección si llega al final o al inicio de la lista de waypoints.
            if (_CWaypointTarget == waypointList.Length - 1) {
                _direction = false;
            } else if (_CWaypointTarget == 0) {
                _direction = true;
            }
        }

        // Mueve al enemigo hacia el waypoint actual.
        MoveTo(waypointList[_CWaypointTarget].transform.position);
    }


}
