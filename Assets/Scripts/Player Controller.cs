using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft => facingLeft;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Pulo 2.5D")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Referências")]
    [SerializeField] private Transform visual;

    private bool facingLeft = false;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;

    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;

    private float altura = 0f;
    private float velocidadeY = 0f;
    private bool estaNoChao = true;

    private bool estaSobrePoca = false;

    private Collider2D playerCol;

    private Vector3 checkpointPosition;

    // direção salva pro idle/pulo
    private Vector2 lastDirection = Vector2.right;


//-------------------------------------------------------------------



    // =========================
    // AWAKE
    // =========================

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();

        rb = GetComponent<Rigidbody2D>();

        myAnimator = GetComponentInChildren<Animator>();
        mySpriteRender = GetComponentInChildren<SpriteRenderer>();
        mySpriteRender.flipX = false;
        
        playerCol = GetComponent<Collider2D>();

        checkpointPosition = transform.position;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        PlayerInput();

        HandleJump();

        UpdateJumpPhysics();

        UpdateVisualHeight();

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        Move();

        AdjustPlayerFacingDirection();
    }

    // =========================
    // INPUT
    // =========================

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // salva última direção
        if (movement != Vector2.zero)
        {
            lastDirection = movement;
        }
    }

    // =========================
    // MOVIMENTO
    // =========================

    private void Move()
    {
        rb.MovePosition(
            rb.position +
            movement * moveSpeed * Time.fixedDeltaTime
        );
    }

    // =========================
    // ANIMAÇÕES
    // =========================


    private void UpdateAnimations()
    {
        bool isMoving = movement != Vector2.zero;

        myAnimator.SetBool("isMoving", isMoving);
        myAnimator.SetBool("isGrounded", estaNoChao);

        // RUN usa movimento atual
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);

        // salva última direção válida
        if (isMoving)
        {
            lastDirection = movement.normalized;
        }

        // IDLE + JUMP usam última direção
        myAnimator.SetFloat("lastMoveX", lastDirection.x);
        myAnimator.SetFloat("lastMoveY", lastDirection.y);
    }

    // =========================
    // VIRAR PERSONAGEM
    // =========================

  private void AdjustPlayerFacingDirection()
    {
        if (movement.x > 0.1f)
        {
            mySpriteRender.flipX = false;
        }
        else if (movement.x < -0.1f)
        {
            mySpriteRender.flipX = true;
        }
    }

    // =========================
    // PULO
    // =========================

    private void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && estaNoChao)
        {
            velocidadeY = jumpForce;

            estaNoChao = false;

           // myAnimator.SetTrigger("jump");
        }
    }

    private void UpdateJumpPhysics()
    {
        if (!estaNoChao)
        {
            velocidadeY -= gravity * Time.deltaTime;

            altura += velocidadeY * Time.deltaTime;

            if (altura <= 0f)
            {
                altura = 0f;

                velocidadeY = 0f;

                estaNoChao = true;

                // caiu na poça
                if (estaSobrePoca)
                {
                    Die();
                }

                ReativarColisoesJumpable();
            }
        }
    }

    private void UpdateVisualHeight()
    {
        if (visual != null)
        {
            visual.localPosition = new Vector3(
                0,
                altura,
                0
            );
        }
    }

    // =========================
    // JUMPABLE
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!estaNoChao &&
            collision.gameObject.CompareTag("Jumpable"))
        {
            Physics2D.IgnoreCollision(
                playerCol,
                collision.collider,
                true
            );
        }
    }

    private void ReativarColisoesJumpable()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(
            transform.position,
            2f
        );

        foreach (var c in cols)
        {
            if (c.CompareTag("Jumpable"))
            {
                Physics2D.IgnoreCollision(
                    playerCol,
                    c,
                    false
                );
            }
        }
    }

    // =========================
    // TRIGGERS
    // =========================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // poça
        if (collision.CompareTag("Puddle"))
        {
            estaSobrePoca = true;

            if (estaNoChao)
            {
                Die();
            }
        }

        // checkpoint
        if (collision.CompareTag("Checkpoint"))
        {
            checkpointPosition = collision.transform.position;

            Debug.Log("Checkpoint salvo");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Puddle"))
        {
            estaSobrePoca = false;
        }
    }

    // =========================
    // MORTE
    // =========================

    public void Die()
    {
        Debug.Log("Morreu");

        Respawn();
    }

    // =========================
    // RESPAWN
    // =========================

    private void Respawn()
    {
        transform.position = checkpointPosition;

        altura = 0f;

        velocidadeY = 0f;

        estaNoChao = true;

        estaSobrePoca = false;

        movement = Vector2.zero;

        rb.linearVelocity = Vector2.zero;

        ReativarColisoesJumpable();
    }
}