using System.Collections;
using UnityEngine;

public class ball : hitbox
{
    public GameObject projectile;
    bool gotHit;
    public Rigidbody body;
    public int ProjectileHealth;

    new
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        body = GetComponent<Rigidbody>();
    }


    public override void OnHit(Hurt hurt, hitbox h)
    {

        //hurt.enemy.GetHit(h);
        if (hurt.player != null && hurt != selfHurtBox)
        {
            Debug.Log("vertical knockback" + h.VerticalKnockback);
            Debug.Log("knocktype type " + knockback);
            Debug.Log("knocktype type " + vfxType);

            //acessa a variavel player do script Hurt se ela não for nula, e ativa uma função do script do jogador
            //utilizando todas as variaveis da hitbox e passando elas como parametros
            hurt.player.GetSlowdown(h, impactHit, damage, knockback, hitboxType, vfxType);


        }

        if (playerScript != null)
        {
            //faz o hit stun para o player que atingiu o inimigo
            playerScript.Slowdown();

        }

        Debug.Log("projectile hit the enemy");


        checkDestruction();
        //Destroy(projectile);

    }

    public void checkDestruction()

         {
        Debug.Log("checking destruction");
        ProjectileHealth -= 1;
        if (ProjectileHealth == 0)
        {
            StartCoroutine(startDestruction());
        }
 

        }

    public IEnumerator startDestruction()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(projectile);
        Debug.Log("destroying");
    }

    // Update is called once per frame
    public void GetHit(hitbox collision)
    {
        GameObject playerObject = collision.transform.root.gameObject;

        fight player = playerObject.GetComponent<fight>();
        hitbox hitBoxObject = collision.GetComponent<hitbox>();

        gotHit = true;

        if (gotHit == true)
        {

            Vector3 directionAwayFromAttacker = (transform.position - player.transform.position);

            // Apply knockback in that direction
            Vector3 knockbackDirection = (directionAwayFromAttacker * hitBoxObject.HorizontalKnockback) + (Vector3.up * hitBoxObject.VerticalKnockback);

            Debug.Log($"Applied Knockback: {knockbackDirection}"); // Debugging

            body.linearVelocity = Vector3.zero;

            body.linearVelocity = knockbackDirection;
        }

    }
}
