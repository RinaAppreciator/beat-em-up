using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class wall : MonoBehaviour

{

    public float impactThreshold;
    public float liftForce;        // Upward force applied to objects
    public float shakeThreshold = 15f;   // Speed threshold for a heavy shake
    public float shakeMagnitude = 0.5f;
    public AudioSource audioSource;
    public bool gotAttacked;
    public bool hasPushed;

    public AudioClip groundExplosion;

    public AudioClip wallBounceSound;

    [SerializeField] private cameraManager cameraManager;

    public GameObject wallPlayer;


    private HashSet<Rigidbody> touchingObjects = new HashSet<Rigidbody>();


    public enum WallType
    {

        //quais das animações irão tocar 
        Upward,
        Downward,
        Forward,
        Spinning,
        Ground
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        //Debug.Log("enemy hugging the wall");
        Vector3 wallNormal = collision.contacts[0].normal;

        Vector3 absNormal = new Vector3(Mathf.Abs(wallNormal.x), Mathf.Abs(wallNormal.y), Mathf.Abs(wallNormal.z));

        bool isXDominant = absNormal.x > absNormal.z; // mostly left/right wall
        bool isZDominant = absNormal.z >= absNormal.x; // mostly front/back wall

        fight playerScript = collision.gameObject.GetComponent<fight>();

        if (playerScript != null)
        {
            if (playerScript.canBounce && !playerScript.canStick)
            {
                pushObject(rb, playerScript, isXDominant, isZDominant);
                playerScript.bounceWall(true);
                playSound(wallBounceSound);
                shakeGround(0.1f, 0.5f, 0.1f);
                //Debug.Log("Can bounce");
            }

            if (playerScript.canStick)
            {
                if (!hasPushed)
                {
                    playSound(groundExplosion);
                    shakeGround(0.2f, 1, 0.2f);
                    playerScript.wallStick();
                    hasPushed = true;
                    resetBounce(true);

                }
                
            }
        }
    }

    public void pushObject(Rigidbody rb, fight playerScript, bool Xdominant, bool Zdominant)
    {
        //Debug.Log("pushed");

        if (!hasPushed)
        {
            //Debug.Log("enemy pushed away");

            Vector3 knockback = new Vector3(0, 0, 0);
            //Vector3 formalKnockback = new Vector3(0, 0, 0);

            knockback = (playerScript.lastKnockback);

            if (Xdominant)
            {
                knockback.z = 0f; // only X + Y movement
                knockback.x = -knockback.x;
            }

            else if (Zdominant)
            {
                knockback.x = 0;
                knockback.z = -knockback.z;
            }


            if (playerScript.lastKnockback.y <= 1)
            {
               knockback.y = 1;
            }

            rb.linearVelocity = Vector3.zero;
            rb.AddForce(knockback/1.5f, ForceMode.Impulse);
            
            
            playerScript.lastKnockback = knockback;

            hasPushed = true;
            StartCoroutine(resetBounce(false));


            //Debug.Log("wall knockback" + knockback);
        }
    }

    public IEnumerator resetBounce(bool Stick)
    {

        if(Stick)
        {
            yield return new WaitForSeconds(5f);
            resetBounceFunction();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            resetBounceFunction();
        }
    
        
    }


    public void resetBounceFunction()
    {
        hasPushed = false;
    }

    public void screenShake(float amount, float time, float frequency)
    {
        cameraManager.Shake(amount, time, frequency);
    }

    public void shakeGround(float amount, float time, float frequency)
    {
        //playSound(groundExplosion);
        screenShake(amount, time, frequency);
        //LiftObjects(force);
    }

    public void playSound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }

}
