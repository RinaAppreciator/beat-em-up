using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class fight : MonoBehaviour
{
    

    public GameObject suicide;
    public fight otherplayer;
    public Move movement;
    public atkmanager state;


    public bool isGrabbing;

    //Animations
    public Animator moves; //animator
    public int grabLayerIndex;
    public int baseLayerIndex;


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

   //knockbacks
   public Rigidbody rb;//rigidbody that moves player
   public float lightK;//normal attack knockback
   public float heavyK;//big knockback
   public float upK;//upwards knockbak for launcher
   public float downK;
   public float K;
   public float Kup;
   public float chain;
   public bool maxchain;


    //Attack Hitboxes
   public GameObject Lhit;//Light attack hitbox
   public GameObject Hhit;//Heavy attack hitbox
   public GameObject Shit;//Special attack hitbox
   public GameObject Uhit;//Launcher hitbox


   public atkmanager enemy;

   public Enemy grabbedEnemy;


    public void Start()
    {
        gotHit=false;

        rb = GetComponent<Rigidbody>();
        baseLayerIndex = moves.GetLayerIndex("BaseLayer");
        grabLayerIndex = moves.GetLayerIndex("GrabLayer");
       
    }

    public void Gethit(Collider collision)
    {

        GameObject enemyObject = collision.transform.root.gameObject;

        hitvar = Random.Range(-1,1);

        Enemy enemy = enemyObject.GetComponent<Enemy>();


        //rb.linearVelocity = new Vector3(enemy.K, enemy.Kup, rb.linearVelocity.z);
        
        gotHit = true;
        if(hitvar>=0)
        {
            moves.Play("Hitted");
        }

        if(hitvar<0)
        {
            moves.Play("Hitted2");
        }

       // if (enemy != null)
           // rb.linearVelocity = new Vector3(enemy.K, enemy.Kup, rb.linearVelocity.z);
           // enemy.chain += 1;
       
        
    }
        
    

    //public void OnTriggerEnter(Collider collision)
    //{
        //if(collision.CompareTag("Attack"))
       // {
           // Gethit(collision);
        //}
    //}

    public void Update()
    {

  
        
        
        if(hp<=0)
        {
            suicide.SetActive(false);
            Debug.Log("Died");
        }

       
        if (Input.GetKeyDown(KeyCode.J) )
        {
            damage = LAD;
            Kup = 0;


        //if (Input.GetKeyDown(KeyCode.J) && chain==0 && !state.stunned )
        //{
           // Debug.Log("first attack");
           // moves.Play("Light Attack");
              //  chain += 1;
        //}

        if (Input.GetKeyDown(KeyCode.J) && state.atk == false )
            {
                Debug.Log("first attack");
                moves.Play("Light Attack");
                chain += 1;
            }

            //if (Input.GetKeyDown(KeyCode.F) && chain==1 && state.followup)
            // {
            //  Debug.Log("first chain");
            //  moves.Play("Chain1");
            //  chain += 1;
            // }
            //if (Input.GetKeyDown(KeyCode.F) && chain==2 && state.followup)
            //{
            //  Debug.Log("second chain");
            //  moves.Play("Chain2");

            // K= lightK*2;

            //}


            if (!state.followup)
        {
            chain=0;
        }
        }


         if (Input.GetKeyDown(KeyCode.K) && state.atk == false)
        {
            Debug.Log("heavy kick");
            moves.Play("Chain2");
            damage = HAD;
            K = lightK;
            Kup = 0;
            
        }

         if (Input.GetKeyDown(KeyCode.L) && state.atk == false )
        {
            moves.Play("Chain1");
            damage = SAD;
            K = lightK;
            Kup = 0;
            
        }


        if (Input.GetKeyDown(KeyCode.O) && state.atk == false )
        {
            moves.Play("Grab");
           
        }

        if (Input.GetKeyDown(KeyCode.O) && isGrabbing && state.atk == false)
        {
            moves.Play("Throw");
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            damage = UAD;
            K = lightK/2;
            Kup = upK;
           
            
        }

        if (movement.running == true )
        {
            moves.SetBool("Running", true);
        }

        if (movement.running == false)
        {
            moves.SetBool("Running", false);
        }


        GrabCheck();



    }

    public void GrabCheck()
    {
        if ( isGrabbing == true)
        {
            moves.SetLayerWeight(baseLayerIndex, 0);
            moves.SetLayerWeight(grabLayerIndex, 1);
        }
        if (isGrabbing == false)
        {
           moves.SetLayerWeight(grabLayerIndex, 0);
           moves.SetLayerWeight(baseLayerIndex, 1);
        }

    }

    public void Slowdown()
    {
        Debug.Log("player slowed down!!!!!");
        moves.speed = 0; // Reduce animation speed (0.2x slower)
        Debug.Log(moves.speed);
        rb.useGravity = false;
        //rb.isKinematic = true;
        StartCoroutine(RestoreSpeedCoroutine());
    }


    IEnumerator RestoreSpeedCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        moves.speed = 1f;
        rb.useGravity = true;

    }







}
