using System;
using UnityEngine;

public class VisionDetector : MonoBehaviour
{
    private bool _isPlayerVisible;
    private GameObject _player;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsNotAPlayer;
    [SerializeField] private float detectionRange;
    [SerializeField] private float visionAngle;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Vector2 forward = transform.right;
        Vector2 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2) * forward;
        Vector2 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2) * forward;
            
        Gizmos.DrawRay(transform.position, leftBoundary * detectionRange);
        Gizmos.DrawRay(transform.position, rightBoundary * detectionRange);
        Gizmos.color = Color.white;
    }

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        _isPlayerVisible = false; // Por defecto, no detecta al jugador

        if (_player == null) return;
            
        if (!PlayerInRange()) return;
        if (!PlayerInAngle()) return;
        if (!IsVisible()) return;

        _isPlayerVisible = true;
    }

    private bool PlayerInRange()
    {
        return Physics2D.OverlapCircle(transform.position, detectionRange, whatIsPlayer) != null;
    }

    private bool PlayerInAngle()
    {
        Vector2 directionToPlayer = (_player.transform.position - transform.position).normalized;
        float angle = Vector2.Angle(transform.right, directionToPlayer);
        return angle < visionAngle / 2;
    }

    private bool IsVisible()
    {
        if (_player == null) return false;
        return !Physics2D.Linecast(transform.position, _player.transform.position, whatIsNotAPlayer);
    }

    public bool IsPlayerDetected() => _isPlayerVisible;
}


