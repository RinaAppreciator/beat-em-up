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
        //player.grabbedEnemy.GetThrown(player, playerObject, 6f, 2f);
        player.isGrabbing = false;
        atk = false;
   
    }
   

    public void attackReset()
   {
    atk = false;

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
