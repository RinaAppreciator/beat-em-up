using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ground : MonoBehaviour
{
    [Header("Bouncing")]
    public float liftForce;
    public float groundDamping;// Upward force applied to objects
    public float shakeThreshold = 15f;   // Speed threshold for a heavy shake
    public float shakeMagnitude = 0.5f;
    public int maxBounces;
    public AudioSource audioSource;

    [Header("Skipping")]
    public float skipUpForce;  // Minimum velocity to trigger lift
    public float skipForwardForce;        // Upward force applied to objects
    public int maxSkips;

    public int skips;
    public int bounces;

    public bool hasPushed;

    public AudioClip groundExplosion;
    public AudioClip flatSound;

    [SerializeField] private cameraManager cameraManager;


    private HashSet<Rigidbody> touchingObjects = new HashSet<Rigidbody>();




    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(touchingObjects.Count);
    }

    public void screenShake(float amount, float time)
    {
        cameraManager.Shake(amount, time, 1);
    }

    //public void shakeGround(float amount, float time,  float force)
    //{
    //    playSound(groundExplosion);
    //    screenShake(amount, time);
    //    LiftObjects(force);
    //}

    //public void LiftObjects(float liftForce)
    //{

    //    // Apply upward force to all objects in contact with the ground
    //    foreach (Rigidbody rb in touchingObjects)
    //    {
       

         
    //        rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);
            

    //    }

    //}


    //private void OnCollisionEnter(Collision collision)
    //{
    //    Rigidbody rb = collision.rigidbody;


    //    fight playerScript = collision.gameObject.GetComponent<fight>();

    //    if (playerScript != null)
    //    {
    //        if (playerScript.canGroundBounce && playerScript.bounces < maxBounces )
    //        {

    //            // está causando um problema onde se a vitima for atingida no chão e abrir uma animação que permite levar bounce, ativa esta função.
    //            // quando idealmente, esta função deveria ser apenas chamada a vitima cai no chão, ou seja, entra em contato.
    //            pushObject(rb, playerScript);
    //            playerScript.bounceWall(false);
    //        }

    //        //else if (playerScript.canGroundBounce && maxBounces == playerScript.bounces && !hasPushed)
    //        //{
    //        //    playerScript.getKO();
    //        //    hasPushed = true;
    //        //    StartCoroutine(resetbounce());

    //        //}
    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;


        fight playerScript = collision.gameObject.GetComponent<fight>();

        if (playerScript.canGroundBounce && playerScript.bounces < maxBounces && !hasPushed)
        {

            // está causando um problema onde se a vitima for atingida no chão e abrir uma animação que permite levar bounce, ativa esta função.
            // quando idealmente, esta função deveria ser apenas chamada a vitima cai no chão, ou seja, entra em contato.
            pushObject(rb, playerScript);
            playerScript.bounceWall(false);
            hasPushed = true;
            StartCoroutine(resetbounce());
            playSound(groundExplosion);
            //Debug.Log("pushed off ground");
        }

        if (playerScript.canGroundBounce && maxBounces == playerScript.bounces && !hasPushed)
        {
            playSound(flatSound);
            //Debug.Log("KO");
            playerScript.getKO();
            hasPushed = true;
            StartCoroutine(resetbounce());
        }
    }

    public void pushObject(Rigidbody rb, fight playerScript)
    {
 
            //Debug.Log("Player bounces" + playerScript.bounces + " and max bounces " + maxBounces);

            
            Vector3 UpKnockback = new Vector3(0,0,0);

            switch(playerScript.bounces)
            {
                case 0:
                {
                    if (playerScript.lastKnockback.y < 0)
                    {
                             UpKnockback = (-playerScript.lastKnockback);
                    }

                    else
                    {
                        UpKnockback = (playerScript.lastKnockback);
                     
                    }

                    break;
                }
                      
                  

                case 1:
                {
                    if (playerScript.lastKnockback.y < 0)
                    {
                        UpKnockback = (-playerScript.lastKnockback / 1.5f);

                    }

                    else
                    {
                        UpKnockback = (playerScript.lastKnockback / 1.5f);
                    }

                    break;
                }

                

                case 2:
                    {

                    if(playerScript.lastKnockback.y < 0)
                    {
                        UpKnockback = (-playerScript.lastKnockback / 2f);
                    }

                    else
                    {
                        UpKnockback = (playerScript.lastKnockback / 2f);
                    }

                        break;
                }


        }

        //Debug.Log("last knockback applied" + playerScript.lastKnockback);

        if (playerScript.lastKnockback.x == 0|| playerScript.lastKnockback.z == 0)
        {
            //Debug.Log("stopped movement");
            rb.linearVelocity = Vector3.zero;
        }
        //rb.linearVelocity = Vector3.zero;
        rb.AddForce(new Vector3(0, UpKnockback.y, 0), ForceMode.Impulse);



        
    }

    public IEnumerator resetbounce()
    {
        yield return new WaitForSeconds(0.1f);
        hasPushed = false;
    }

    public void playSound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }

    #region unused

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Rigidbody rb = collision.rigidbody;
    //    if (rb != null && collision.contacts.Length > 0)
    //    {
    //        touchingObjects.Add(rb);
    //        //CheckImpact(rb);
    //    }

    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    Rigidbody rb = collision.rigidbody;
    //    if (rb != null )
    //    {
    //        touchingObjects.Remove(rb);
    //    }

    //}



    //void CheckImpact(Rigidbody rb)
    //{
    //    float verticalSpeed = Mathf.Abs(rb.linearVelocity.y);

    //    Debug.Log(verticalSpeed);


    //    if (verticalSpeed)
    //      {
    //        Debug.Log("heavy");
    //        shakeGround(2,0.1f,2);
    //      }

    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Rigidbody rb = collision.rigidbody;

    //    fight playerScript = collision.gameObject.GetComponent<fight>();

    //    if (playerScript != null)
    //    {
    //        if (playerScript.canBounce)
    //        {
    //            playSound(groundExplosion);
    //            //playerScript.bounceWall();
    //            Debug.Log("heavy");
    //            shakeGround(3, 0.5f, 3);
    //        }
    //    }

    //    else
    //    {
    //        Debug.Log("Player wasnt found");
    //    }


    //    //if (rb != null && collision.contacts.Length > 0)
    //    //{           
    //    //    CheckImpact(rb, playerScript);
    //    //}
    //}

    #endregion
}




