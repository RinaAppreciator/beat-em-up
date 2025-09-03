
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Move;

public class Move : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Rigidbody rb;
    public bool isGrounded;
    public bool wasGrounded;
    public bool nearGround;
    public bool running = false;
    public fight player;
    public float groundCheckDistance = 1f; // Slightly more than your desired "early detect" buffer
    public float groundCollisionDistance;
    public LayerMask groundLayer;
    public bool flipped;

    public bool canBlock;

    public Transform playerBody;
    public Transform shadow;
    public float playerID;

    public Vector3 scale;

    public Vector3 moveDirection;
    private Vector3 rotationDirection;
    private Quaternion m_Rotation = Quaternion.identity;
    [SerializeField] atkmanager state;
    [SerializeField] Animator moves;
    [SerializeField] CharacterState characterState;

    public CapsuleCollider collision;

    public bool frozenState;

    public bool onTheAirHurt;

    public float customGravity;
    public float gravityScale;

    public int characterID;

    public bool JumpInputBuffered = false;

    private float inputBufferTime = 0.3f;
    private float bufferTimer = 0f;

    public int timesJumped = 0;
    public int maxJumps = 2;

    public bool airHit;
    public float gravityTimer;
    public float gravityModifier;

    private float jumpCooldown = 0.1f; // Time in seconds before another jump can be registered
    private float lastJumpTime = -1f;

    public float  friction;

    public bool wasAirborne;

    public bool slowMotion;

    public autoTarget TargetManager;






    //o motivo do jogador evitar o hit freeze em ataques giratórios é porque o hit freeze não afeta a rotação do personagem, mas somente a velocidade da animação.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
       
        timesJumped = 0;
        maxJumps = 1;
        rotationDirection = Vector3.zero;
    
    }

    void Update()
    {
      
        if (JumpInputBuffered)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                JumpInputBuffered = false;
            }
        }


        if (state.ableBodied == true)
        {

            wasAirborne = false;
        }

        if (isGrounded)
        {
            moves.SetBool("IsGrounded", true);
            state.airAtk = false;
            timesJumped = 0;
            //gravityChange(false);
            //wasAirborne = false;
            
        }

        if (player.gotHit)
        {
            gravityScale = customGravity;
           
        }

        if (playerID ==2)
        {
            if(isGrounded)
            {
                //Debug.Log("Enemy is grounded");
            }

            //if (slowMotion)
            //{
            //    Debug.Log("Enemy is slowed down");
            //}

        }

        if (isGrounded == false)
        {
            moves.SetBool("IsGrounded", false);
            StartCoroutine(validateAir());
            //Debug.Log("is airborne");
        }

        if (running == true)
        {
            moves.SetBool("Running", true);

        }

        if (running == false)
        {
            moves.SetBool("Running", false);

        }

        //if (player.gotHit)
        //{
        //    if (slowMotion)
        //    {
        //        gravityChange(false);
        //    }
        //}

 


    }

    public IEnumerator validateAir()
    {
        yield return new WaitForSeconds(0.05f);
        //Debug.Log("air validated");
        airBorneActivator();
    }

    public void airBorneActivator()
    {
        wasAirborne = true;
    }
    public void gravityChange(bool slowMotion)
    {
        if (slowMotion)
        {
            Debug.Log("modified gravity");
            gravityScale = gravityScale * gravityModifier;
            this.slowMotion = true;
            StartCoroutine(resetGravity());
        }

      
    }   

    public void Rotation()
    {
        if (moveDirection != Vector3.zero)
        {

            if (characterState.CanWalk == false && characterState.CanRotate)
            {
                playerBody.rotation = Quaternion.Slerp(playerBody.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * 10f);
                //Debug.Log("rotating while attacking");
            }

            if (characterState.CanWalk && characterState.CanRotate )
            {
                //Debug.Log("normal rotation");
                playerBody.rotation = Quaternion.Slerp(playerBody.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * 10f);
                running = true;
            }

            else
            {
                //Debug.Log("tweaking out");
                return;
            }
            
        }

        else
        {

            running = false;
        }
    }

    public void Jump()
    {

        if ( JumpInputBuffered )
        {

            if (characterState.CanJump == true && characterState.Enabled)
            {

                if (Time.time - lastJumpTime < jumpCooldown)
                {
                    Debug.Log("trying to jump too fast");
                    return;
                }


                if (timesJumped < maxJumps)
                    {
                        
                        timesJumped += 1;
                        //rb.AddForce(rb.linearVelocity.x, jumpForce , rb.linearVelocity.z, ForceMode.Impulse);

                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
                        JumpInputBuffered = false;
                        moves.Play("Jump");

                       lastJumpTime = Time.time;


                    running = false;
                    }


                else
                {
                    
                    return;
                }


            }
        }
      
    }

    public void move(Vector3 currentVelocity)
    {
        Vector3 inputVelocity = new Vector3(moveDirection.x * moveSpeed, currentVelocity.y, moveDirection.z * moveSpeed);
        rb.linearVelocity = inputVelocity;
        //rb.MoveRotation(m_Rotation);
    }

    public void MovementInput(InputAction.CallbackContext context)
    {


        if (context.canceled)
        {
            moveDirection = Vector3.zero;
            return;
        }


        if ( characterState.Enabled == false )
        {
          
               
                return;
        }

        else
        {
            moveDirection = new Vector3(context.ReadValue<Vector2>().x, 0f, context.ReadValue<Vector2>().y).normalized;
            
        }


    }

    public void JumpInput(InputAction.CallbackContext context)
    {
       

        if (context.started)
        {
            
            JumpInputBuffered = true;
            bufferTimer = inputBufferTime;

           

        }


    }

    public void RunInput(InputAction.CallbackContext context)
    {
        if (/*characterState.CanWalk == false || */!characterState.Enabled)
        {
            return;
        }

        if (context.started)
        {
            dash();

        }
    }

    public IEnumerator resetGravity()
    {
        yield return new WaitForSeconds(gravityTimer);
        //Debug.Log("gravity back to normal");
        gravityScale = customGravity;
    }

    void FixedUpdate()
    {

        Jump();
        Rotation();
        //gravityChange();


        //if (!nearGround)
        //{
        //    collision.enabled = false;
        //}

        ////if the enemy goes to the ground too quickly, it passes through the ground. I gotta add a higher raycast that checks if the enemy is near the ground to activate 
        ////the collision again.

        //if (nearGround)
        //{
        //    collision.enabled = true;
        //}


        if (!isGrounded && !frozenState && characterState.AffectedByGravity)
        {
            //ativa a gravidade custom
            //se o jogador estiver no ar, sem estar congelado ou machucado, aplicar a gravidade de maneira normal
            rb.linearVelocity += Vector3.down * gravityScale * Time.fixedDeltaTime;
     
        }

        if (frozenState)
        {
            
            rb.linearVelocity = Vector3.zero;
        }



        Vector3 currentVelocity = rb.linearVelocity;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer) ;

        nearGround = Physics.Raycast(transform.position, Vector3.down, groundCollisionDistance, groundLayer);


        // Uma maneira interessante de implementar isso seria outros scripts diretamente afetar a gravity scale para aumentar ou diminuir a gravidade.





        // Only apply input-based movement if canWalk is true
        if (characterState.CanWalk && characterState.Enabled && !frozenState)
        {
            //Debug.Log("can walk in theory");
            move(currentVelocity);

        }
        else if (!isGrounded && !characterState.Enabled && !characterState.KnockedOut && !characterState.SlowedDown)
        {
            //Se o jogador não poder se mover, deixe ele se mover na direção natural do knockback
            rb.linearVelocity = new Vector3(currentVelocity.x, currentVelocity.y, currentVelocity.z);
            
        }

        else if (characterState.SlowedDown && !characterState.KnockedOut)
        {
            rb.linearVelocity = new Vector3(currentVelocity.x, currentVelocity.y, currentVelocity.z) / 1.2f;
        }

        else if (characterState.KnockedOut)
        {
            rb.linearVelocity = new Vector3(currentVelocity.x, currentVelocity.y, currentVelocity.z) / 1.5f;

            //if (!JumpInputBuffered)
            //{
            //    rb.linearVelocity = new Vector3(currentVelocity.x * friction, currentVelocity.y, currentVelocity.z * friction);
            //}
            //this cancels out jump

        }

    }
 
    public void dash()
    {
        Debug.Log("Dash Input Sent");
        moves.Play("Dash");
        //rb.linearVelocity = new Vector3(25, rb.linearVelocity.y, 0);
    }

    public void faceEnemy()
    {
 
        transform.LookAt(new Vector3(TargetManager.target.position.x, transform.position.y, TargetManager.target.position.z));
        
       
    }

}
