using System.Collections;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    // General Settings
    public float speedPatrol = 2f;
    public float speedChase = 4f;
    private float _currentSpeed;
    private SpriteRenderer _spriteRenderer;
    private EnemyState _currentState = EnemyState.Patrol;

    // Patrol
    [SerializeField] private GameObject[] waypointList;
    private int _currentWaypointTarget;

    // Move to
    [SerializeField] private float detectionRange;
    [SerializeField] private LayerMask whatIsNotAPlayer;
    private Orientation[] _directionsOrientation;

    // Detect
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private float rangeDetectionPlayer;
    [SerializeField] private float visionAngle;

    private bool _playerDetected;
    private GameObject _player;

    // Chase
    private Vector3 _lastPlayerPosition;
    private float _chaseCooldown = 5f;
    private float _currentCooldown = 0f;

    // States
    public enum EnemyState
    {
        Patrol,
        Alarm,
        Chase
    }

    private void Start()
    {
        // General Settings
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentSpeed = speedPatrol;

        // Patrol
        _currentWaypointTarget = 0;

        // Detect Player
        _player = GameObject.FindWithTag("Player");
        if (_player == null)
        {
            enabled = false;
            return;
        }
        _playerDetected = false;

        StartCoroutine(DetectPlayerCoroutine());
        StartCoroutine(DetectWallsCoroutine());
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

    // Make Directions
    private void Awake()
    {
        float[] angles = { -135f, -90f, -45f, 45f, 90f, 135f, 180f, 360f };
        _directionsOrientation = new Orientation[angles.Length];

        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(angles[i], transform.forward) * transform.right;
            _directionsOrientation[i] = new Orientation(direction, false);
        }
    }

    // General Functions 
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

        foreach (var orientation in _directionsOrientation)
        {
            Vector3 direction = orientation.GetDirection();
            Vector3 finalPosition = transform.position + direction * detectionRange;

            if (!Physics2D.OverlapCircle(finalPosition, 0.4f, whatIsNotAPlayer))
            {
                float distanceToTarget = Vector2.Distance(finalPosition, target);

                if (distanceToTarget < shortestDistance)
                {
                    bestDirection = direction;
                    shortestDistance = distanceToTarget;
                }
            }
        }

        if (bestDirection != Vector3.zero)
        {
            Vector3 movePosition = transform.position + bestDirection * (detectionRange * 0.5f);
            transform.position = Vector2.MoveTowards(transform.position, movePosition, _currentSpeed * Time.fixedDeltaTime);
        }
    }

    // Patrol
    private void Patrol()
    {
        ChangeTarget(waypointList[_currentWaypointTarget].transform.position);

        if (Vector2.Distance(transform.position, waypointList[_currentWaypointTarget].transform.position) < 0.1f)
        {
            _currentWaypointTarget = (_currentWaypointTarget + 1) % waypointList.Length;
        }

        MoveTo(waypointList[_currentWaypointTarget].transform.position);
    }

    // Detect
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

    // Chase to player
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
                _currentSpeed = speedPatrol;
                _currentCooldown = _chaseCooldown;
            }
        }
    }

    private IEnumerator WaitForChase()
    {
        yield return new WaitForSeconds(1.5f);
        _currentState = EnemyState.Chase;
        _currentSpeed = speedChase;
    }

    // Alarm 
    private void UpdateAlarm()
    {
        StartCoroutine(WaitForChase());
    }
}