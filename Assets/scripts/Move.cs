using UnityEngine;

public class Move : MonoBehaviour
{
   
    public float moveSpeed = 5f; 
    public float jumpForce = 7f; 
    public Rigidbody rb;
    private float moveInput;
    public bool isGrounded;
    public atkmanager state;
    public bool Player2;
    float moveX;
    float moveZ;
    public bool running = false;


    public float turnSpeed = 1000f;
    private Vector3 moveDirection;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
      
    }


    void Movement()
    {
        moveX = 0f;
        moveZ = 0f;

        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;

        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        //rotation

        //Debug.Log(moveDirection);

        if (moveDirection != Vector3.zero)
        {
            m_Rotation = Quaternion.LookRotation(moveDirection);
     
            running = true;
        }

        if (moveDirection == Vector3.zero )
        {
            running = false;
        }


    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            isGrounded = false;
        }
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

}
