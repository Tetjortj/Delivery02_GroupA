using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    public bool isDead;
    public float lives = 3f;
    public float speed = 5f;
    public Transform spawnPoint;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint no asignado en el Inspector.");
            spawnPoint = transform;
        }
    }

    void Update()
    {
        if (!isDead)
        {
            Run();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            lives--;

            if (lives > 0)
            {
                if (spawnPoint == null)
                {
                    return;
                }

                _rb.linearVelocity = Vector2.zero;
                transform.position = spawnPoint.position;
            }
            else
            {
                isDead = true;
                SceneManager.LoadScene("Ending");
            }
        }
    }

    private void Run()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveInputX, moveInputY).normalized * speed;
        _rb.linearVelocity = movement;

        if (moveInputX != 0)
        {
            _spriteRenderer.flipX = moveInputX < 0;
        }
    }
}

