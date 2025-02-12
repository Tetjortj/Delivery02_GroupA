using System.Collections;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    // GENERAL SETTINGS
    public float speedPatrol = 2f;                      // Velocidad de patrulla.
    public float speedChase = 4f;                       // Velocidad de persecución.
    private float _currentSpeed;                        // Velocidad actual.
    private SpriteRenderer _spriteRenderer;
    private EnemyState _currentState = EnemyState.Patrol;

    // PATROL
    [SerializeField] private GameObject[] waypointList;
    private int _currentWaypointTarget;

    // MOVE TO
    [SerializeField] private float detectionRange;       // Rango de detección de obstáculos.
    [SerializeField] private LayerMask whatIsNotAPlayer; // Máscara de capa para los obstáculos.
    private Orientation[] _directionsOrientation;        // Array de direcciones precalculadas.

    // DETECT
    [SerializeField] private LayerMask whatIsPlayer;     // Capa del jugador.
    [SerializeField] private float rangeDetectionPlayer; // Rango de detección del jugador.
    [SerializeField] private float visionAngle;          // Ángulo de visión del enemigo.

    private bool _playerDetected;                        // Indica si el jugador ha sido detectado.
    private GameObject _player;                          // Referencia al jugador.

    // CHASE
    private Vector3 _lastPlayerPosition;                 // Última posición conocida del jugador.
    private float _chaseCooldown = 5f;                  // Tiempo de enfriamiento para dejar de perseguir.
    private float _currentCooldown = 0f;                // Tiempo restante de enfriamiento.

    // STATES
    public enum EnemyState
    {
        Patrol,
        Alarm,
        Chase
    }

    private void Start()
    {
        // GENERAL SETTINGS
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentSpeed = speedPatrol; // Inicializa la velocidad a la de patrulla.

        // PATROL
        _currentWaypointTarget = 0;

        // DETECT PLAYER
        _player = GameObject.FindWithTag("Player");
        if (_player == null)
        {
            // Debug.LogError("No se encontró un objeto con el tag 'Player' en la escena.");
            enabled = false; // Desactiva el script si no hay jugador.
            return;
        }
        _playerDetected = false;

        StartCoroutine(DetectPlayerCoroutine()); // Inicia la corrutina de detección.
        StartCoroutine(DetectWallsCoroutine());  // Inicia la corrutina de detección de obstáculos.
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (_playerDetected)
                {
                    _currentState = EnemyState.Alarm;
                }
                break;
            case EnemyState.Alarm:
                UpdateAlarm();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
        }
    }

    // CLASS ORIENTATION - MAKE DIRECTIONS
    private void Awake()
    {
        // Ángulos predefinidos para las direcciones.
        float[] angles = { -135f, -90f, -45f, 45f, 90f, 135f, 180f, 360f };
        _directionsOrientation = new Orientation[angles.Length];

        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(angles[i], transform.forward) * transform.right;
            _directionsOrientation[i] = new Orientation(direction, false);
        }
    }

    // GENERAL FUNCTIONS 
    private void ChangeTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        _spriteRenderer.flipY = angle is > 90 and < 270;
    }

    private void MoveTo(Vector3 target)
    {
        Vector3 bestDirection = Vector3.zero;
        float shortestDistance = float.MaxValue;

        // Itera sobre todas las direcciones posibles para encontrar la mejor.
        foreach (var orientation in _directionsOrientation)
        {
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
        if (bestDirection != Vector3.zero)
        {
            Vector3 movePosition = transform.position + bestDirection * (detectionRange * 0.5f);
            transform.position = Vector2.MoveTowards(transform.position, movePosition, _currentSpeed * Time.fixedDeltaTime);
        }
    }

    // FUNCTION PATROL
    private void Patrol()
    {
        ChangeTarget(waypointList[_currentWaypointTarget].transform.position);

        if (Vector2.Distance(transform.position, waypointList[_currentWaypointTarget].transform.position) < 0.1f)
        {
            _currentWaypointTarget = (_currentWaypointTarget + 1) % waypointList.Length;
        }

        MoveTo(waypointList[_currentWaypointTarget].transform.position);
    }

    // DETECT
    private void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer <= rangeDetectionPlayer)
        {
            Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

            if (angleToPlayer <= visionAngle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, rangeDetectionPlayer, whatIsNotAPlayer);

                if (hit.collider == null || hit.collider.CompareTag("Player"))
                {
                    _playerDetected = true;
                    _lastPlayerPosition = _player.transform.position;
                    return;
                }
            }
        }

        _playerDetected = false;
    }

    private IEnumerator DetectPlayerCoroutine()
    {
        while (true)
        {
            DetectPlayer();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void DetectWalls()
    {
        foreach (var orientation in _directionsOrientation)
        {
            Vector3 direction = orientation.GetDirection();
            if (Physics2D.Raycast(transform.position, direction, rangeDetectionPlayer, whatIsNotAPlayer))
            {
                orientation.SetVisibility(true);
            }
            else
            {
                orientation.SetVisibility(false);
            }
        }
    }

    private IEnumerator DetectWallsCoroutine()
    {
        while (true)
        {
            DetectWalls();
            yield return new WaitForSeconds(0.5f);
        }
    }

    // CHASE TO PLAYER
    private void UpdateChase()
    {
        _currentCooldown -= Time.fixedDeltaTime;

        ChangeTarget(_lastPlayerPosition);
        MoveTo(_lastPlayerPosition);

        if (Vector2.Distance(transform.position, _lastPlayerPosition) < 0.9f || _currentCooldown <= 0)
        {
            if (!_playerDetected)
            {
                _currentState = EnemyState.Patrol;
                _currentSpeed = speedPatrol; // Vuelve a la velocidad de patrulla.
                _currentCooldown = _chaseCooldown;
            }
        }
    }

    private IEnumerator WaitForChase()
    {
        yield return new WaitForSeconds(1.5f);
        _currentState = EnemyState.Chase;
        _currentSpeed = speedChase; // Cambia a la velocidad de persecución.
    }

    // ALARM 
    private void UpdateAlarm()
    {
        StartCoroutine(WaitForChase());
    }
}