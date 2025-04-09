using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject suicide;
    public fight otherplayer;
    public Move movement;
    public atkmanager state;
    public bool isGrounded;
    public GameObject hurtbox;
    Rigidbody GrabRb;


    //Animations
    public Animator moves; //animator


    //attacks damages 
    public float LAD;//ligh attack damage
    public float HAD;//heavy attack damage
    public float HAHD;//heavy attack damage after hold
    public float CAD;//chain attacks damage
    public float SAD;//special attack damage
    public float UAD;//launcher attack damage
    public float UAHD;//laucher damage after hold
    private bool atk;//true during damage frames


    //heath
    public float hp; //hitpoints
    private float damage;//the damage applied
    public float hitvar;
    public bool gotHit;
    public bool gotGrabbed;
    public bool isBeingHit;
    public bool shaking;
    public bool Recovered;
    public bool OnTheGroundHurt;

    //knockbacks
    public Rigidbody rb;//rigidbody that moves player

    public float chain;
    public bool maxchain;


    //Attack Hitboxes
    public GameObject Lhit;//Light attack hitbox
    public GameObject Hhit;//Heavy attack hitbox
    public GameObject Shit;//Special attack hitbox
    public GameObject Uhit;//Launcher hitbox


    public atkmanager enemy;
    public grabHitbox hitBoxObject;

    public Transform PlayerTransform;
    public float moveSpeed = 5f;
    public bool isAttacking = false;

    Transform grabParent;
    



    public void Start()
    {
        hp = 10;
        //moves.SetBool("Alive", true);
        moves.SetBool("Hurt", false);
       
        gotHit = false;

        rb = GetComponent<Rigidbody>();

        Physics.IgnoreLayerCollision(9, 8);
        //Physics.IgnoreLayerCollision(0, 4);


    }

    public void GetHit(hitbox collision)
    {

        GameObject enemyObject = collision.transform.root.gameObject;

        hitvar = Random.Range(-1, 1);

        fight player = enemyObject.GetComponent<fight>();
        hitbox hitBoxObject = collision.GetComponent<hitbox>(); 


      
        if (hitBoxObject != null) {
            Debug.Log("found reference to hitbox data");
        }

        //Debug.Log(hitBoxObject.HorizontalKnockback);

        hitvar = Random.Range(-1, 1);


        gotHit = true;

        if (gotHit == true ) {

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



               // moves.SetFloat("HorizontalVelocity", normalizedDirection.x);
                //moves.SetFloat("VerticalVelocity", normalizedDirection.y);

                if (hitBoxObject.HorizontalKnockback <2 && hitBoxObject.VerticalKnockback < 2)
                {
                    {
                        moves.Play("Hitted");
                        gotHit = false;
                    }
                }

            


        }

}


    public void GetGrabbed(grabHitbox hitbox)
    {
       
        GameObject enemyObject = hitbox.transform.root.gameObject;
        Transform grabParent = enemyObject.transform;
        Rigidbody GrabRb = hitbox.GetComponent<Rigidbody>();

        hitvar = Random.Range(-1, 1);

        fight player = enemyObject.GetComponent<fight>();
        hitBoxObject = hitbox.GetComponent<grabHitbox>();



        if (hitBoxObject != null)
        {
            Debug.Log("found reference to hitbox data");
        }


        gotGrabbed = true;

        if (gotGrabbed == true)
        {


            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;

            transform.position = hitBoxObject.transform.position;
            transform.rotation = hitBoxObject.transform.rotation;

            gameObject.layer = LayerMask.NameToLayer("GrabbedObject");
            hurtbox.layer = LayerMask.NameToLayer("GrabbedObject");


            // Disable collisions with the player
            //Physics.IgnoreLayerCollision(0,8);

            //Physics.IgnoreCollision(GetComponent<Collider>(), hitbox.GetComponent<Collider>(), true);



        }

    }


    public void Update()
    {
        if (hp <= 0)
        {
            moves.SetBool("Alive", false);

            if (hp > 0)  {
                moves.SetBool("Alive", true);
            }
        }


        if (gotGrabbed)
        {
            rb.MovePosition(Vector3.Lerp(rb.position, hitBoxObject.transform.position, Time.fixedDeltaTime * 5f));
        }
        else if (!gotGrabbed && !isAttacking && !gotHit)
        {

            if (PlayerTransform != null)
            {
                transform.LookAt(PlayerTransform);

                Vector3 direction = (PlayerTransform.position - transform.position).normalized;
                rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

                float distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);

                if (distanceToPlayer <= 0.7f && isGrounded)
                {
                    isAttacking = true;
                    StartCoroutine(Attacking());
                }

            }
        }
}



    public IEnumerator Attacking()
    {
        isAttacking = true;

        moves.Play("Chain1");
        yield return new WaitForSeconds(2.0f);

        isAttacking = false;
    }


    public void GetThrown(fight player, GameObject playerObject, float horizontal_throwforce, float vertical_throwforce) 
    {
        moves.SetBool("Hurt", true);

        gotGrabbed = false;

        gameObject.layer = LayerMask.NameToLayer("CharacterLayer");
        hurtbox.layer = LayerMask.NameToLayer("Water");

        rb.isKinematic = false;

        rb.useGravity = true;

      
        Vector3 directionAwayFromAttacker = transform.position - playerObject.transform.position;
        directionAwayFromAttacker.y = 0;

        Vector3 verticalKnockback = Vector3.up * vertical_throwforce;

        Vector3 horizontalKnockback = directionAwayFromAttacker.normalized * horizontal_throwforce;

        rb.linearVelocity = Vector3.zero;

        rb.linearVelocity += horizontalKnockback * 2;
        rb.linearVelocity += verticalKnockback;

        Debug.Log(directionAwayFromAttacker);

        moves.SetFloat("HorizontalVelocity", 1);
        moves.SetFloat("VerticalVelocity", 0);

        //Debug.Log($"Applied Horizontal: {horizontalKnockback}, Applied Vertical: {verticalKnockback}");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            moves.SetBool("OnAir", false);
            if (gotHit == true)
            {
                Debug.Log("crashed into the ground");
                moves.SetBool("Hurt", false);
                OnTheGroundHurt = true;
                Recover();
            }
            
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            moves.SetBool("OnAir", true);
        }

    }

    public void Slowdown(hitbox collision)
    {
        Debug.Log("slowed down!!!!!");
        moves.speed = 0; // Reduce animation speed (0.2x slower)
        Debug.Log(moves.speed);
        rb.useGravity = false;
        //rb.isKinematic = true;
        StartCoroutine(ShakeRoutine(2, collision));
        //StartCoroutine(RestoreSpeedCoroutine(collision));

    }




    IEnumerator RestoreSpeedCoroutine(hitbox collision)
    {
        yield return new WaitForSeconds(0.2f);
        moves.speed = 1f;
        rb.useGravity = true;
        GetHit(collision);
    }

    public void Recover()
    {
        Debug.Log("Enemy is Recovering");
        StartCoroutine(RecoverTimer());
    }

    public void ResetRecover()
    {
        Debug.Log("Recovering");
        StartCoroutine(ResetRecoverTimer());
    }

    IEnumerator RecoverTimer()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Recovered");
        Recovered = true;
        OnTheGroundHurt = false;
        moves.SetBool("Recovered", true);
        ResetRecover();

    }

    public void callWaitTimeAttack()
    {
        StartCoroutine(WaitAttackTimer());
    }

    IEnumerator WaitAttackTimer()
    {
        yield return new WaitForSeconds(0.5f);
        moves.SetTrigger("EnemyAttack");

    }

    IEnumerator ResetRecoverTimer()
    {
        yield return new WaitForSeconds(0.1f);
        moves.SetBool("Recovered", true);
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






}
