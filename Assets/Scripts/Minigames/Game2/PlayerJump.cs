using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 12f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private InputAction jumpAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction = new InputAction(binding: "<Keyboard>/space", type: InputActionType.Button);
        jumpAction.performed += ctx => TryJump();
        jumpAction.Enable();
    }

    private void TryJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.collider.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over!");    // TODO: Reemplazar con una pantalla de Game Over o reiniciar el juego
            Time.timeScale = 0f;        // Pausamos el juego
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnDestroy()
    {
        jumpAction.Disable();
        jumpAction.Dispose();
    }
}
