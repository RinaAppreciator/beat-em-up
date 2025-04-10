using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using static Move;

public class Move : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Rigidbody rb;
    public bool isGrounded;
    public bool running = false;
    public fight player;

    public Mesh MagicGirl;
    public Mesh Soldier;
    public SkinnedMeshRenderer bodyMesh;
    public Material[] MagicGirlMat;
    public Material[] SoldierMat;
    public Transform playerBody;
    public Transform shadow;
    public float playerID;

    private Vector2 movement;

    private Vector3 moveDirection;
    private Quaternion m_Rotation = Quaternion.identity;
    [SerializeField] atkmanager state;
    [SerializeField] Animator moves;

    // Ataques
    private bool lightHit;
    private bool upperCut;
    private bool heavyHit;
    private bool grab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerID = PlayerManager.Instance.players.IndexOf(transform);

        switch (playerID)
        {
            case 0:
                bodyMesh.sharedMesh = MagicGirl;
                bodyMesh.materials = MagicGirlMat;
                break;
            case 1:
                bodyMesh.sharedMesh = Soldier;
                bodyMesh.materials = SoldierMat;
                break;
        }
    }

    void Update()
    {
        if (shadow.position.y != 0.25f)
        {
            Vector3 pos = shadow.position;
            pos.y = 0.25f;
            shadow.position = pos;
        }

        Movement();
    }

    public void Movement()
    {
        if (moveDirection != Vector3.zero && state.canWalk )
        {
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * 10f);
            running = true;
        }
        else
        {
            running = false;
        }

    }

    public void MovementInput(InputAction.CallbackContext context)
    {

       
         moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0f, context.ReadValue<Vector2>().y).normalized;
        

    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && state.canWalk )
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
            moves.Play("Jump");
            running = false;
            
        }
    }

    void FixedUpdate()
    {
        if (!state.atk && !player.gotHit)
        {
            rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
            rb.MoveRotation(m_Rotation);

        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
          
        }
    }

}

