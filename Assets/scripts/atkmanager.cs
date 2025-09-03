using UnityEngine;
using Unity.Collections;

public class atkmanager : MonoBehaviour
{
    public bool atk;
    public bool atkTransition;
    public bool airAtk;
    public bool stunned;
    private Animator moveset;
    public Rigidbody rb;
    public Move movement;
    public fight player;
    public GameObject playerObject;
    public bool followup;
    public bool isBeingHit;
    public bool canWalk;
    public int chain;
    public bool ableBodied;
    public bool canRotate;
    public bool canJump;
    public int type;
    public bool SlowDown;
    public bool KnockedOut;

    public CharacterController controller;

    public MoveProfile neutralProfile;
    public CharacterState state;


    public void Start()
    {
        moveset = GetComponent<Animator>();
        canWalk = true;
        canRotate = false;
        ableBodied = true;
        canJump = true;
    
    }

    public void ApplyMoveProfile(MoveProfile profile)
    {
        if (state != null && profile != null)
        {
            state.ApplyProfile(profile);
        }
    }


    private void FixedUpdate()
    {
        if(atkTransition == false)
        {
            moveset.SetBool("Attacking", false);
        }

        if(atkTransition == true)
        { 
            moveset.SetBool("Attacking", true);
        }
    }

    public void Dash()
    {
        Debug.Log("dashed");
        if (movement.moveDirection == Vector3.zero)
        {
            rb.AddForce(500 * transform.right, ForceMode.Acceleration);
        }

        else
        {
            rb.AddForce(750 * movement.moveDirection, ForceMode.Acceleration);
        }
     
    }

    public void DashAttack()
    {
        rb.AddForce(-1000 * transform.right, ForceMode.Acceleration);

    }

    public void spawnProjectile(int type)
    {
    
        
            player.spawnProjectile(type);
        

    }

    //public void enableDashing()
    //{
    //    canJump = true;
    //}

    //public void disableDashing()
    //{
    //    canJump = false;
    //}

    //public void finishAtk() 
    //{
    //    atkTransition = true;
    //}

    //public void disableWalk()
    //{
   
    //    canWalk = false;
    //}

    //public void canWalkAgain()
    //{
        
    //    canWalk = true;
    //}

    //public void enableRotation()
    //{
        
    //    canRotate = true;
    //}

    //public void disableRotation()
    //{
        
    //    canRotate = false;
    //}

    //public void atacking()
    //{
    //    atk = true;
    //    player.moves.SetBool("Attacking", true);
    //    followup = false;
    //}

    //public void airAttacking()
    //{
    //    airAtk = true;
    //}

    //public void ResetAirAtk()
    //{
    //    airAtk = false;
    //}

    public void throwing()
    {
        player.grabbedEnemy.GetThrown(player, playerObject, 6f, 2f);
        player.isGrabbing = false;
        atk = false; 
    }

    // Code for dropping items 


    //public void Disable()
    //{
    //    Debug.Log("got disabled");
    //    ableBodied = false;
    //}

    //public void Enable()
    //{
    //    Debug.Log("got enabled");
    //    ableBodied = true;
    //}


    public void dropping()
    {
        //player.grabbedEnemy.GetThrown(player, playerObject, 6f, 2f);
        player.isGrabbing = false;
        atk = false;
   
    }
   

   // public void attackReset()
   //{
   // atk = false;
   //     player.moves.SetBool("Attacking", false);

   // }

   // public void Super()
   // {
   //     atk = true;

   // }


}
