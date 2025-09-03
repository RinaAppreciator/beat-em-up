using UnityEngine;

public class romanCancel : hitbox
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void OnHit(Hurt hurt, hitbox h)
    {
        if (hurt.player != null && hurt != selfHurtBox)
        {
            //acessa a variavel player do script Hurt se ela não for nula, e ativa uma função do script do jogador
            //utilizando todas as variaveis da hitbox e passando elas como parametros
            hurt.player.GetSlowdown(h, impactHit, damage, knockback, hitboxType, vfxType);
            hurt.player.slowMotion = true;
            Debug.Log("enemy is slowed");


        }

        if (playerScript != null)
        {
            //faz o hit stun para o player que atingiu o inimigo
            playerScript.Slowdown();
            if (!movement.isGrounded)
            {
                movement.airHit = true;
            }
            playerScript.meter += 8;

        }
    }
}
