using UnityEngine;
using Unity.Collections;
using System.Collections;

public class hitbox : MonoBehaviour
{
   
    public GameObject player;
    public fight playerScript;
    public Animator anim;
    public bool hiti;
    public float VerticalKnockback;
    public float HorizontalKnockback;
    public float ForwardKnockback;
    public float damage;
    public LayerMask layerMask;

    public AudioClip impactHit;
    public AudioClip soundHit;

    public void Start()
    {
 
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (layerMask == (layerMask | (1 << collision.transform.gameObject.layer)))
        {
            Debug.Log("found hitbox");
            Debug.Log(collision.name);
            hitbox h = this;


            Hurt hurt = collision.GetComponent<Hurt>();
            ballHurtbox ballhurt = collision.GetComponent<ballHurtbox>();

            if (hurt != null)
            {
                Debug.Log("hitting");
                OnHit(hurt, h);
            }
            if (ballhurt != null)
            {
                Debug.Log("hit ball");
                OnBallHit(ballhurt, h);
            }

        }
    }

    void RestoreSpeed()
    {
       // anim.speed = 1f;
    }

    protected virtual void OnHit(Hurt hurt, hitbox h)
    {
        //hurt.enemy.GetHit(h);
        if (hurt.player != null)
        {
            Debug.Log("hitting player");

            hurt.player.GetSlowdown(h, impactHit);
        }
        if (hurt.enemy != null)
        {
            hurt.enemy.Slowdown(h, impactHit, damage);
        }
        playerScript.Slowdown();
        //anim.speed = 0.6f; // Reduce animation speed (0.2x slower)
        //Invoke("RestoreSpeed", 1f); // Restore normal speed after 2 seconds
    }

    protected virtual void OnBallHit(ballHurtbox ballhurt, hitbox h)
    {
        ballhurt.ball.GetHit(h);
        anim.speed = 0.6f; // Reduce animation speed (0.2x slower)
        Invoke("RestoreSpeed", 1f); // Restore normal speed after 2 seconds
    }

   



}
