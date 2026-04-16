using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 12f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private InputAction jumpAction;

    public MinigameEnd endController;      // Referencia al script que maneja el fin del minijuego

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction = new InputAction(type: InputActionType.Button);

        jumpAction.AddBinding("<Keyboard>/space");                  // Salto en PC o Mac
        jumpAction.AddBinding("<Touchscreen>/primaryTouch/press");  // Salto en movil

        jumpAction.performed += ctx => TryJump();        
    }

    private void OnEnable()
    {
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
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
            HandleFinJuego();
        }
    }

    private void HandleFinJuego()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopScore();
        }

        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.StopAllCoroutines();        // Detener la generación de obstáculos
        }

        jumpAction.Disable();                   // Desactivar el salto

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetFinalScore() : 0;

        if (endController != null)
        {
            endController.FinishMinigameWithScore(finalScore);
        }
        else
        {
            Debug.LogError("No se ha asignado el EndController al script PlayerJump.");
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
        // jumpAction.Disable();
        jumpAction.Dispose();
    }
}
