using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 20f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private InputAction jumpAction;

    private Animator animator;             // Referencia al componente Animator para controlar las animaciones  

    public MinigameEnd endController;      // Referencia al script que maneja el fin del minijuego


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpAction = new InputAction(type: InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");                  // Salto en PC o Mac
        jumpAction.AddBinding("<Touchscreen>/primaryTouch/press");  // Salto en movil

        jumpAction.performed += ctx => TryJump();        
    }

    private void Start()
    {
        animator.SetBool("isRun", true);    // Iniciamos corriendo
        animator.SetBool("isJump", false);
    }

    private void OnEnable() => jumpAction.Enable();
    private void OnDisable() => jumpAction.Disable();


    private void TryJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            animator.SetBool("isRun", false);
            animator.SetBool("isJump", true);

            if (AudioManager.Instance != null)            {
                AudioManager.Instance.PlayJumpSound();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;

            animator.SetBool("isRun", true);
            animator.SetBool("isJump", false);
        }

        if (collision.collider.CompareTag("Obstacle"))
        {
            HandleFinJuego();
        }
    }

    private void HandleFinJuego()
    {
        // StartCoroutine(SequencceDie());

        int finalscore = ScoreManager.Instance.GetFinalScore();
        MinigameFlowController.Instance.EndGame(finalscore, EndType.Death);
    }


    /* // Secuencia refactorizada en MinigameFlowController
    
    private IEnumerator SequencceDie()
    {
        animator.SetBool("isRun", false);
        animator.SetBool("isJump", false);
        animator.SetTrigger("die");             // Activar la animación de muerte

        // animator.Play("Die", -1, 0f);

        jumpAction.Disable();                   // Desactivar el salto
        this.enabled = false;                   // Desactivar este script para evitar más interacciones


        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StopScore();

        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
            spawner.StopAllCoroutines();        // Detener la generación de obstáculos

        
        ParallaxController parallax = FindObjectOfType<ParallaxController>();
        if (parallax != null){
            parallax.globalSpeed = 0;
        }

        // Delay
        yield return new WaitForSeconds(1f);

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetFinalScore() : 0;
        if (endController != null)
        {
            endController.FinishMinigameWithScore(finalScore);
        }
        else
        {
            Debug.LogError("No se ha asignado el EndController al script PlayerJump.");
        }        

        // rb.linearVelocity = Vector2.zero;        // Detener el movimiento del jugador
        // rb.simulated = false;                    // Detener la física del jugador

        
    }
    */

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

    public void DisableJumping()
    {
        jumpAction.Disable();
    }
}
