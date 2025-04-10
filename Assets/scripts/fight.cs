using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class fight : MonoBehaviour
{
    public GameObject suicide;
    
    public Move movement;
    public atkmanager state;

    public bool isGrabbing;
    public bool shaking;

    // Animations
    public Animator moves; // animator
    public int grabLayerIndex;
    public int baseLayerIndex;

    // Attacks damages

    private bool atk;  // true during damage frames

    // Health
    public float hp;  // hitpoints
    private float damage;  // the damage applied
    public float hitvar;
    public bool gotHit;

    // Knockbacks
    public Rigidbody rb;  // rigidbody that moves player

    public float chain;
    public bool maxchain;

    public int score;

    public bool OnTheGroundHurt;
    public bool recovered;
   
    // Attack Hitboxes
    public atkmanager enemy;
    public Enemy grabbedEnemy;

    //sound effects
    AudioSource audioSource;
    public AudioClip HitClip;
    public AudioClip ShootClip;
    public AudioClip UppercutClip;


    public void Start()
    {
        PlayerManager.Instance.RegisterPlayer(transform);
        gotHit = false;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        baseLayerIndex = moves.GetLayerIndex("Base");
        grabLayerIndex = moves.GetLayerIndex("Grab");
        chain = 0;
        moves.SetBool("Alive", true);
    }

    public void Update()
    {
        if (hp <= 0)
        {
            suicide.SetActive(false);
            moves.SetBool("Alive", false);
            Debug.Log("Died");
        }
        
        // Corrida (animação)
        if (movement.running == true)
        {
            moves.SetBool("Running", true);
        }

        if (movement.running == false)
        {
            moves.SetBool("Running", false);
        }
        if(movement.isGrounded == true)
        {
            moves.SetBool("OnAir", false);
        }
        if (movement.isGrounded == false)
        {
            moves.SetBool("OnAir", true);
        }

        if (chain > 0)
        {
            StartCoroutine(chainResetTimer());
        }

        
        GrabCheck();
    }

    public void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Deadzone to avoid jitter on each axis
        float x = Mathf.Abs(velocity.x) < 0.05f ? 0f : velocity.x; // Horizontal (strafe)
        float y = Mathf.Abs(velocity.y) < 0.05f ? 0f : velocity.y; // Vertical (jump/fall)
        float z = Mathf.Abs(velocity.z) < 0.05f ? 0f : velocity.z; // Forward/back

        moves.SetFloat("HorizontalVelocity", x);
        moves.SetFloat("VerticalVelocity", y);       // For jump/fall
        moves.SetFloat("ForwardVelocity", z);
    }

    public void GrabCheck()
    {
        if (isGrabbing == true)
        {
            moves.SetLayerWeight(baseLayerIndex, 0);
            moves.SetLayerWeight(grabLayerIndex, 1);
        }
        else
        {
            moves.SetLayerWeight(grabLayerIndex, 0);
            moves.SetLayerWeight(baseLayerIndex, 1);
        }
    }

    #region player input

    public void LightAttackInput(InputAction.CallbackContext context)
    {
        if (context.started && state.atk == false && chain == 0 && !isGrabbing )
        {
            Debug.Log("first attack");
            moves.Play("Light1");
            chain += 1;
        }


        if (context.started && state.atk == false && chain == 1 && state.followup && !isGrabbing)
        {
            Debug.Log("second attack");
            moves.Play("Light3");
            chain += 1;
        }


        if (context.started && state.atk == false && chain == 2 && state.followup && !isGrabbing )
        {
            Debug.Log("third attack");
            moves.Play("Light2");
            chain = 0;
           
        }

    }

    public void HeavyAttackInput(InputAction.CallbackContext context )
    {
        if (context.started && state.atk == false && chain == 0 && !isGrabbing )
        {
            moves.Play("Heavy1");
            chain += 1;
        }

        if (context.started && state.atk == false && chain == 1 && state.followup && !isGrabbing)
        {
            moves.Play("Heavy2");
            chain += 1;
        }

        if (context.started && state.atk == false && chain == 2 && state.followup && !isGrabbing)
        {
            moves.Play("Heavy3");
            chain = 0;
        }
    }

    public void LauncherAttackInput(InputAction.CallbackContext context)
    {
        if (context.started && state.atk == false && !isGrabbing)
        {
        
            moves.Play("Uppercut");

        }
    }

    public void ShootAttackInput(InputAction.CallbackContext context)
    {
        if (context.started && state.atk == false && !isGrabbing)
        {

            Debug.Log("Shoot");
            moves.Play("Shoot");

        }
    }

    public void GrabAttackInput(InputAction.CallbackContext context)
    {
        if (context.started && state.atk == false)
        {
            if (isGrabbing)
            {
                Debug.Log("Throwing");
                moves.Play("Throw");
            }
            else
            {
                Debug.Log("Grabbing");
                moves.Play("Grab");
            }
           

        }
    }

    #endregion


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (gotHit == true)
            {
                Debug.Log("crashed into the ground");
                moves.SetBool("Hurt", false);
                OnTheGroundHurt = true;
                Recover();
            }

        }

    }

    public void Recover()
    {
  
        Debug.Log("Recovering");
        StartCoroutine(RecoverTimer());
    
    }   


    IEnumerator RecoverTimer()
    {
        yield return new WaitForSeconds(0.5f);
        gotHit = false;
        moves.SetBool("Recovered", true);
    }


    IEnumerator chainResetTimer()
    {
        yield return new WaitForSeconds(0.5f);
        chain = 0;
        //state.canWalk = true;
    }

    #region Getting hit

    public void Slowdown()
    {
        Debug.Log("player slowed down!!!!!");
        moves.speed = 0; // Reduce animation speed (0.2x slower)
        Debug.Log(moves.speed);
        rb.useGravity = false;
        StartCoroutine(RestoreSpeedCoroutine());
    }

    IEnumerator RestoreSpeedCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        moves.speed = 1f;
        rb.useGravity = true;

    }


    public void GetSlowdown(hitbox collision, AudioClip hitSound)
    {
        Debug.Log("slowed down!!!!!");
        moves.speed = 0; // Reduce animation speed (0.2x slower)
        Debug.Log(moves.speed);
        rb.useGravity = false;
        gotHit = true;
        PlaySound(hitSound);
        //rb.isKinematic = true;
        StartCoroutine(ShakeRoutine(2, collision));
        //StartCoroutine(RestoreSpeedCoroutine(collision));

    }


    IEnumerator RestoreSpeedCoroutine(hitbox collision)
    {
        yield return new WaitForSeconds(0.1f);
        moves.speed = 1f;
        rb.useGravity = true;
        GetHit(collision);
    }


    private IEnumerator ShakeRoutine(int shakeCount, hitbox collision)
    {
        shaking = true;
        Vector3 originalPosition = rb.position;
        float shakeSpeed = 0.03f; // Adjust for how fast the shake happens

        for (int i = 0; i < shakeCount; i++)
        {
            // Move slightly right
            rb.MovePosition(originalPosition + Vector3.right * 0.03f);
            yield return new WaitForSeconds(shakeSpeed);

            // Move slightly left
            rb.MovePosition(originalPosition + Vector3.left * 0.03f);
            yield return new WaitForSeconds(shakeSpeed);
            shakeSpeed -= 0.02f;
        }

        // Return to the original position
        //rigidbody2D.MovePosition(originalPosition);
        rb.isKinematic = false;
        shaking = false;
        StartCoroutine(RestoreSpeedCoroutine(collision));
    }


    public void GetHit(hitbox collision)
    {

        GameObject enemyObject = collision.transform.root.gameObject;

        hitvar = Random.Range(-1, 1);

        fight player = enemyObject.GetComponent<fight>();
        hitbox hitBoxObject = collision.GetComponent<hitbox>();



        if (hitBoxObject != null)
        {
            Debug.Log("found reference to hitbox data");
        }

        Debug.Log(hitBoxObject.HorizontalKnockback);

        hitvar = Random.Range(-1, 1);


        if (gotHit == true)
        {

            moves.SetBool("Hurt", true);



            //moves.Play("Hitted");
            //gotHit = false;

            Vector3 directionAwayFromAttacker = (transform.position - enemyObject.transform.position).normalized;

            // Break it down into local directions relative to THIS character
            Vector3 localKnockback = transform.InverseTransformDirection(directionAwayFromAttacker);

            Debug.Log($"Applied Knockback: {localKnockback}"); // Debugging

            Vector3 knockback =
             (transform.right * localKnockback.x * hitBoxObject.HorizontalKnockback) +       // Sideways
             (transform.forward * localKnockback.z * hitBoxObject.ForwardKnockback) +        // Forward/back
             (Vector3.up * hitBoxObject.VerticalKnockback);

            rb.linearVelocity = Vector3.zero;

            rb.linearVelocity = knockback;


            if (hitBoxObject.HorizontalKnockback < 2 && hitBoxObject.VerticalKnockback < 2)
            {
                {
                    moves.Play("Hitted");
                }
            }







        }




    }


    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}

#endregion 