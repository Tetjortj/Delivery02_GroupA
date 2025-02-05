using UnityEngine;

public class Movement : MonoBehaviour
{
    // GENERAL STINGS
    private Rigidbody2D _rb;

    // HEAL
    public bool isDead;
    public float live = 3f;

    // RUN 
    public float speed = 5f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Run();
    }

    private void Run()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal"); // Movimiento horizontal (izquierda/derecha)
        float moveInputY = Input.GetAxisRaw("Vertical");   // Movimiento vertical (arriba/abajo)

        // Calcular la velocidad
        Vector2 movement = new Vector2(moveInputX, moveInputY).normalized * speed;
        _rb.linearVelocity = movement;
    }
}
