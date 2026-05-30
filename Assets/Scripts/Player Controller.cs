using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    private Vector2 lastDirection = Vector2.right;

    private bool isDead = false;

    [SerializeField] private float deathDuration = 1.5f;
    [SerializeField] private float reviveDuration = 1f;

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

    private void OnEnable()  { playerControls.Enable(); }
    private void OnDisable() { playerControls.Disable(); }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (isDead) return;

        PlayerInput();
        HandleJump();
        UpdateJumpPhysics();
        UpdateVisualHeight();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        Move();
        AdjustPlayerFacingDirection();
    }

    // =========================
    // INPUT
    // =========================

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        if (movement != Vector2.zero)
            lastDirection = movement;

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
        myAnimator.SetFloat("lastMoveX", lastDirection.x);
        myAnimator.SetFloat("lastMoveY", lastDirection.y);
    }

    // =========================
    // MOVIMENTO
    // =========================

    private void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // =========================
    // ANIMAÇÕES
    // =========================

    private void UpdateAnimations()
    {
        bool isMoving = movement != Vector2.zero;

        myAnimator.SetBool("isMoving", isMoving);
        myAnimator.SetBool("isGrounded", estaNoChao);
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);

        if (isMoving)
            lastDirection = movement.normalized;

        myAnimator.SetFloat("lastMoveX", lastDirection.x);
        myAnimator.SetFloat("lastMoveY", lastDirection.y);
    }

    // =========================
    // VIRAR PERSONAGEM
    // =========================

    private void AdjustPlayerFacingDirection()
    {
        if (movement.x > 0.1f)
            mySpriteRender.flipX = false;
        else if (movement.x < -0.1f)
            mySpriteRender.flipX = true;
    }

    // =========================
    // PULO — teclado (space) OU controle via InputReader
    // =========================

    private void HandleJump()
    {
        bool jumpPressed = InputReader.Instance != null
            ? InputReader.Instance.JumpPressed
            : Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        if (jumpPressed && estaNoChao)
        {
            velocidadeY = jumpForce;
            estaNoChao  = false;
        }
    }

    private void UpdateJumpPhysics()
    {
        if (!estaNoChao)
        {
            velocidadeY -= gravity * Time.deltaTime;
            altura      += velocidadeY * Time.deltaTime;

            if (altura <= 0f)
            {
                altura      = 0f;
                velocidadeY = 0f;
                estaNoChao  = true;

                if (estaSobrePoca)
                    Die();

                ReativarColisoesJumpable();
            }
        }
    }

    private void UpdateVisualHeight()
    {
        if (visual != null)
            visual.localPosition = new Vector3(0, altura, 0);
    }

    // =========================
    // JUMPABLE
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!estaNoChao && collision.gameObject.CompareTag("Jumpable"))
            Physics2D.IgnoreCollision(playerCol, collision.collider, true);
    }

    private void ReativarColisoesJumpable()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 2f);

        foreach (var c in cols)
        {
            if (c.CompareTag("Jumpable"))
                Physics2D.IgnoreCollision(playerCol, c, false);
        }
    }

    // =========================
    // TRIGGERS
    // =========================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Puddle"))
        {
            estaSobrePoca = true;

            if (estaNoChao)
                Die();
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
            estaSobrePoca = false;
    }

    // =========================
    // MORTE COM FADE
    // =========================

    public void Die()
    {
        if (isDead) return;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        if (movement != Vector2.zero)
            lastDirection = movement.normalized;

        myAnimator.SetFloat("lastMoveX", lastDirection.x);
        myAnimator.SetFloat("lastMoveY", lastDirection.y);

        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        playerControls.Disable();

        myAnimator.SetTrigger("death");

        yield return new WaitForSeconds(deathDuration);

        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeOut());

        Respawn();

        if (UIFade.Instance != null)
            yield return StartCoroutine(UIFade.Instance.FadeIn());

        myAnimator.SetTrigger("revive");

        yield return new WaitForSeconds(reviveDuration);

        playerControls.Enable();

        isDead = false;
    }

    // =========================
    // RESPAWN
    // =========================

    public void SetCheckpoint(Vector3 pos)
    {
        checkpointPosition = pos;
    }

    private void Respawn()
    {
        transform.position = checkpointPosition;

        altura        = 0f;
        velocidadeY   = 0f;
        estaNoChao    = true;
        estaSobrePoca = false;
        movement      = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        ReativarColisoesJumpable();
    }
}
