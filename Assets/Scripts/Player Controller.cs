using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; }}

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

    // =========================

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();

        myAnimator = GetComponentInChildren<Animator>();
        mySpriteRender = GetComponentInChildren<SpriteRenderer>();

        playerCol = GetComponent<Collider2D>();

        checkpointPosition = transform.position;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    // =========================

    private void Update()
    {
        PlayerInput();
        HandleJump();
        UpdateJumpPhysics();
        UpdateVisualHeight();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    // =========================

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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

            myAnimator.SetTrigger("jump");
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

                // caiu dentro da poça
                if (estaSobrePoca)
                {
                    Die();
                }

                ReativarColisoesJumpable();
            }
        }

        myAnimator.SetBool("isGrounded", estaNoChao);
    }

    private void UpdateVisualHeight()
    {
        if (visual != null)
        {
            visual.localPosition = new Vector3(0, altura, 0);
        }
    }

    // =========================
    // JUMPABLE
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!estaNoChao && collision.gameObject.CompareTag("Jumpable"))
        {
            Physics2D.IgnoreCollision(playerCol, collision.collider, true);
        }
    }

    private void ReativarColisoesJumpable()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 2f);

        foreach (var c in cols)
        {
            if (c.CompareTag("Jumpable"))
            {
                Physics2D.IgnoreCollision(playerCol, c, false);
            }
        }
    }

    // =========================
    // POÇA
    // =========================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Puddle"))
        {
            estaSobrePoca = true;

            if (estaNoChao)
            {
                Die();
            }
        }

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

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        mySpriteRender.flipX = mousePos.x < playerScreenPoint.x;
    }

    // =========================

    private void Die()
    {
        Debug.Log("Morreu");

        Respawn();
    }

    // private void Respawn()
    // {
    //     transform.position = checkpointPosition;

    //     altura = 0f;
    //     velocidadeY = 0f;
    //     estaNoChao = true;

    //     estaSobrePoca = false;

    //     ReativarColisoesJumpable();
    // }

   private void Respawn()
{
    transform.position = checkpointPosition;

    altura = 0f;
    velocidadeY = 0f;
    estaNoChao = true;

    estaSobrePoca = false;

    ReativarColisoesJumpable();

    movement = Vector2.zero;
    rb.linearVelocity = Vector2.zero;

    
}




}