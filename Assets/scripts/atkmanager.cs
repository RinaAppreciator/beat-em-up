using UnityEngine;
using Unity.Collections;

public class atkmanager : MonoBehaviour
{
    public bool atk;
    public bool stunned;
    private Animator moveset;
    public Rigidbody rb;
    public Move movement;
    public fight player;
    public GameObject playerObject;
    public bool followup;
    public bool cooldown;
    public Flip flipped;
    public bool isBeingHit;
    public Enemy enemy;


    public GameObject Lhitbox;//Light attack hitbox
    public GameObject Hhitbox;//Heavy attack hitbox
    public GameObject Chitbox;//Chain attack hitbox
    public GameObject Shitbox;//Special attack hitbox
    public GameObject Uhitbox;//Launcher hitbox


    public void Start()
    {
        moveset = GetComponent<Animator>();
    }

  

    public void atacking()
    {
        atk = true;
        followup = true;
    }

    public void throwing()
    {
        player.grabbedEnemy.GetThrown(player, playerObject, 6f, 2f);
        player.isGrabbing = false;
        atk = false; 
    }

    // Code for dropping items 

    public void dropping()
    {
        //                                           ( inside this function)
        //player.grabbedEnemy.GetThrown(player, playerObject, 2f, 0.5f);

        //                                           ( inside enemy script )

        //gotGrabbed = false;

        //rb.isKinematic = false;

        //rb.useGravity = true;

        // Vector3 directionAwayFromAttacker = (transform.position - playerObject.transform.position);

        // Apply knockback in that direction
        //Vector3 knockbackDirection = (directionAwayFromAttacker * horizontal_throwforce) + (Vector3.up * vertical_throwforce);

        //Debug.Log($"Applied Knockback: {knockbackDirection}"); // Debugging

        //  rb.linearVelocity = Vector3.zero;

        // rb.linearVelocity = knockbackDirection;
    }
   

    public void attackReset()
   {
    atk = false;

   }
   
   void Latk()
   {
    if(movement.isGrounded)
            {
            movement.rb.linearVelocity = new Vector3(player.K , movement.rb.linearVelocity.y, movement.rb.linearVelocity.z);
            }
   }

   void Hatk()
   {
     Hhitbox.SetActive(true);
     if(movement.isGrounded)
            {
            movement.rb.linearVelocity = new Vector3(player.K , movement.rb.linearVelocity.y, movement.rb.linearVelocity.z);
            }
   }

   void Catk()
   {
    Chitbox.SetActive(true);
    if(movement.isGrounded)
            {
            movement.rb.linearVelocity = new Vector3(player.K , movement.rb.linearVelocity.y, movement.rb.linearVelocity.z);
            }
   }

   void Satk()
   {
    Shitbox.SetActive(true);
   }

   void Uatk()
   {
     Uhitbox.SetActive(true);
   }

    void attaked()
    {
        stunned = true;
        followup = false;
    }

    void stunend()
    {
        stunned = false;
    }

    void ChainEnd()
    {
        followup =false;
    }

    void CooldownStart()
    {
        Debug.Log("no cooldown");
        //cooldown= true;
    }

    void CooldownEnd()
    {
        //cooldown=false;
    }

    void slowdown()
    {
        //enemy.Slowdown();
        Debug.Log("slowdown");

    }

    //void RestoreSpeed()
    //{
       // isBeingHit = false;
       // moveset.speed = 1f;
       // rb.useGravity = true;
    //}

}
