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
