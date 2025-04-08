using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Rigidbody rb;
    public bool isGrounded;
    public bool running = false;
    public int playerID = 1; // 1 para teclado, 2 para controle

    private Vector3 moveDirection;
    private Quaternion m_Rotation = Quaternion.identity;
    private PlayerControls controls;

    private Vector2 inputMovement;
    private bool jumpPressed = false;

    // Ataques
    private bool lightHit;
    private bool upperCut;
    private bool heavyHit;
    private bool grab;

    void Awake()
    {
        controls = new PlayerControls();

        // Aqui você pode diferenciar os bindings se estiver usando PlayerInput, ou deixar assim para teclado + controle
        controls.Gameplay.Move.started += ctx => inputMovement = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.performed += ctx => inputMovement = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => inputMovement = Vector2.zero;

        controls.Gameplay.Jump.performed += ctx => jumpPressed = true;

        controls.Gameplay.LightHit.performed += ctx => lightHit = true;
        controls.Gameplay.UpperCut.performed += ctx => upperCut = true;
        controls.Gameplay.HeavyHit.performed += ctx => heavyHit = true;
        controls.Gameplay.Grab.performed += ctx => grab = true;
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        Movement();
        Jump();
        HandleAttacks();
    }

    void Movement()
    {
        moveDirection = new Vector3(inputMovement.x, 0f, inputMovement.y).normalized;

        if (moveDirection != Vector3.zero)
        {
            m_Rotation = Quaternion.LookRotation(moveDirection);
            running = true;
        }
        else
        {
            running = false;
        }
    }

    void Jump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
        }

        jumpPressed = false;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        rb.MoveRotation(m_Rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void HandleAttacks()
    {
        if (lightHit)
        {
            Debug.Log("Light Hit");
            lightHit = false;
        }

        if (upperCut)
        {
            Debug.Log("Upper Cut");
            upperCut = false;
        }

        if (heavyHit)
        {
            Debug.Log("Heavy Hit");
            heavyHit = false;
        }

        if (grab)
        {
            Debug.Log("Grab");
            grab = false;
        }
    }
}
