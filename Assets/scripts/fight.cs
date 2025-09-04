using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using static hitbox;
using Unity.Mathematics;


public class fight : MonoBehaviour
{

    public float meter = 100;
    public float maxMeter = 100;

    public GameObject hitboxes;
    
    public Move movement;
    public atkmanager state;
    public CharacterState characterState;

    public bool isGrabbing;
    public bool shaking;

    // Animations
    public Animator moves; // animator
    public int grabLayerIndex;
    public int baseLayerIndex;

    public int characterID;


    // Attacks damages

    private bool atk;  // true during damage frames

    // Health
    public float hp;  // hitpoints
    public string FighterName;
    public float maxHP = 150;
    private float damage;  // the damage applied
    public float hitvar;
    public bool gotHit;
    public bool gotHitInAir;
    public int score;

    public Rigidbody rb;  // rigidbody that moves player

    public float chain;
    public bool maxchain;

    public bool OnTheGroundHurt;
    public bool recovered;
   
    // Attack Hitboxes
    public atkmanager enemy;
    public Enemy grabbedEnemy;

    //sound effects
    AudioSource audioSource;
    public AudioClip HitClip;
    public AudioClip ShootClip;
    public AudioClip UppercutClip;
    public AudioClip FinishingBlow;
    public AudioClip blockClip;

    //inputs
    private bool KickInputBuffered = false;
    private bool HeavyInputBuffered = false;
    private bool SlashInputBuffered = false;
    private bool PunchInputBuffered = false;
    private bool launcherInputBuffered = false;
    private bool modifierInputBuffered = false;
  
    public float inputBufferTime = 0.3f;
    public float modifierBufferTime = 0.6f;
    private float modifierBufferTimer = 0f;
    private float bufferTimer = 0f;

    //prefabs and projectiles
    public GameObject crossPrefab;
    public GameObject cross2Prefab;
    public GameObject crossSpawnPoint;
    public GameObject VFXred;
    public GameObject VFXexplosion;
    public GameObject greenGlow;
    public GameObject block;
    public GameObject VFXspawnPoint;

    public autoTarget targetManager;

    public bool canBounce;

    public bool canGroundBounce;

    public bool slowMotion;

    public bool canStick;

    public float wallStickTime;

    public int bounces;

    public Vector3 lastKnockback;

    public float wallHitCount;

    float x;
    float y;
    float z;


    #region misc
    public void Start()
    {
        meter = 0;
        bounces = 0;
        wallHitCount = 0;
        hp = maxHP;
        gotHit = false;
        gotHitInAir = false;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
       
        chain = 0;
        moves.SetBool("Alive", true);
    }

    public void Update()
    {
        //Debug.Log("hit count: " + wallHitCount);
        //PlayerManager.Instance.RegisterPlayer(transform);
        
        checkBuffers();
        attack();
        specialAttack();


        //if (transform.position.y < -10.0f)
        //{
        //    hp = 0;
        //}

        if (hp <= 0)
        {
            moves.SetBool("Alive", false);
       
        }

        //CheckRecover();

        if (state.ableBodied == false)
        {
            hitboxes.SetActive(false);
          
        }

        if (characterState.Enabled == true)
        {
            //hitboxes.SetActive(true);
            wallHitCount = 0;
         
        }

        if (chain > 0)
        {
            //StartCoroutine(chainResetTimer());
        }

        if (wallHitCount < 5)
        {
            canStick = false;
        }
        
   

        AnimatorStateInfo stateInfo = moves.GetCurrentAnimatorStateInfo(0);

        // Get the full path hash of the current state
        int fullPathHash = stateInfo.fullPathHash;

        // You can then use this hash to compare with known states
        // For example, if you have a state named "MyState"
        int forwardKnockback = Animator.StringToHash("Base Layer.ForwardKnockback");
        int spinningKnockback = Animator.StringToHash("Base Layer.RotationKnockback");
        int downKnockback = Animator.StringToHash("Base Layer.FallDown");
        int groundState = Animator.StringToHash("Base Layer.GroundState");
        int KOState = Animator.StringToHash("Base Layer.GroundState");
        int WallHitState = Animator.StringToHash("Base Layer.WallFallHit");
        int WallBounce = Animator.StringToHash("Base Layer.WallHit");

        if (fullPathHash == spinningKnockback || fullPathHash == forwardKnockback || fullPathHash == downKnockback || fullPathHash == groundState || fullPathHash == KOState || fullPathHash == WallHitState || fullPathHash == WallBounce)
        {
            if (movement.wasAirborne)
            {
                //Debug.Log("Can ground bounce");
                canGroundBounce = true;
            }

         
            
            canBounce = true;
        }

        else
        {
            canBounce = false;
            canGroundBounce = false;
        }


    }

    public void checkBuffers()
    {
        if (KickInputBuffered || HeavyInputBuffered || SlashInputBuffered || PunchInputBuffered || launcherInputBuffered)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                clearBuffers();
            }
        }

        if (modifierInputBuffered)
        {
            modifierBufferTimer -= Time.deltaTime;
            if (modifierBufferTimer <= 0f)
            {
                modifierInputBuffered = false;
            }
        }

      

    }

    public void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Deadzone to avoid jitter on each axis
        x = Mathf.Abs(velocity.x) < 0.05f ? 0f : velocity.x; // Horizontal (strafe)
        y = Mathf.Abs(velocity.y) < 0.05f ? 0f : velocity.y; // Vertical (jump/fall)
        z = Mathf.Abs(velocity.z) < 0.05f ? 0f : velocity.z; // Forward/back

        moves.SetFloat("VerticalVelocity", y);
  

    }

    public void spawnProjectile(int type)
    {
        if (type == 1)
        {
            Debug.Log("first cross spawned");
            GameObject cross = Instantiate(crossPrefab, crossSpawnPoint.transform.position, Quaternion.identity);
            cross.transform.Rotate(-90, 90, 0);
            Rigidbody crossRb = cross.GetComponent<Rigidbody>();
            crossRb.AddForce(100 * transform.forward, ForceMode.Acceleration);
        }
        
        if (type == 2)
        {
            Debug.Log("second cross spawned");
            GameObject cross = Instantiate(cross2Prefab, crossSpawnPoint.transform.position, Quaternion.identity);
            //cross.transform.Rotate(-90, 90, 0);
            Rigidbody crossRb = cross.GetComponent<Rigidbody>();
            //cross2Prefab.transform.position = Vector3.Lerp(targetManager.target.position, crossSpawnPoint.transform.position, 1);
            Vector3 direction = (targetManager.target.position - transform.position).normalized;
            crossRb.AddForce(1000 * direction, ForceMode.Acceleration);
            Debug.Log(targetManager.target.position);
        }
       
    }

    public void CheckRecover()
    {
        if (state.ableBodied)
        {
            recovered = true;
            //gotHit = false;
            OnTheGroundHurt = false;
            //moves.SetBool("Recovered", true);


            //StartCoroutine(ResetRecover());
        }
    }

    #endregion 
    public void attack()
    {
        // Y e A dao no mesmo ataque
        
        if (SlashInputBuffered && !modifierInputBuffered)
        {
            faceEnemy();
            //Debug.Log("slash");
            if (characterState.CanNormalAttack && characterState.Enabled && movement.isGrounded)
            {
                moves.Play("Slash");
            }

            else if (characterState.CanNormalAttack && characterState.Enabled && !movement.isGrounded)
            {
                moves.Play("AirSlash");
            }

         
        }

        if (KickInputBuffered && !modifierInputBuffered)
        {
            faceEnemy();
            //Debug.Log("Kick");
            if (characterState.CanNormalAttack && characterState.Enabled && movement.isGrounded)
            {
                moves.Play("Kick");
            }

            else if (characterState.CanNormalAttack && characterState.Enabled && !movement.isGrounded)
            {
                //moves.Play("AirLight");
            }
        }

        if (HeavyInputBuffered && !modifierInputBuffered)
        {
            faceEnemy();
            //Debug.Log("heavy");
            if (characterState.CanNormalAttack && characterState.Enabled && movement.isGrounded)
            {
                moves.Play("Heavy");
            }

            else if (characterState.CanNormalAttack && characterState.Enabled && !movement.isGrounded)
            {
                moves.Play("AirHeavy");
            }
        }

        if (PunchInputBuffered && !modifierInputBuffered)
        {
            faceEnemy();
            if (characterState.CanNormalAttack && characterState.Enabled && movement.isGrounded)
            {
              
                moves.Play("punch");
            }

            else if (characterState.CanNormalAttack && characterState.Enabled && !movement.isGrounded)
            {
                //Debug.Log("punched in the air, can normal attack?" + characterState.CanNormalAttack);
                moves.Play("AirPunch");
            }
        }
        
        if (launcherInputBuffered && !modifierInputBuffered)
        {
            faceEnemy();
            if (characterState.CanNormalAttack && characterState.Enabled && movement.isGrounded)
            {
                moves.Play("Charge");
            }

            else if (characterState.CanNormalAttack && characterState.Enabled && !movement.isGrounded)
            {
                moves.Play("AirDust");
            }

        }

       
    }

    public void specialAttack()
    {
        if (modifierInputBuffered && HeavyInputBuffered)
        {
            faceEnemy();
            //Debug.Log("special 1 played");
            moves.Play("Special 1");
          
        }

        if (PunchInputBuffered && modifierInputBuffered)
        {
            faceEnemy();
            //Debug.Log("special 2 played");
            moves.Play("Special 2");
        
        }

        if (KickInputBuffered && modifierInputBuffered)
        {
            faceEnemy();
            //Debug.Log("special 3 played");
            moves.Play("Special 3");

        }

        if (modifierInputBuffered && SlashInputBuffered)
        {
            moves.Play("Block");
        }

        if (SlashInputBuffered && KickInputBuffered && PunchInputBuffered && HeavyInputBuffered)
        {
            faceEnemy();
            moves.Play("Super");
        }


    }

    public void faceEnemy()
    {
        movement.faceEnemy();
    }

    #region player input        

    public void X_AttackInput(InputAction.CallbackContext context)                                   //verifica se o jogador está no chão ou no ar para
                                                                                                        //tocar o move correto
    {
        if (context.started )
        {
            PunchInputBuffered = true;
            bufferTimer = inputBufferTime;
        }

    }

    public void B_AttackInput(InputAction.CallbackContext context )
    {
        if (context.started)
        {
            HeavyInputBuffered = true;
            bufferTimer = inputBufferTime;
        }
    }

    public void Y_AttackInput(InputAction.CallbackContext context)
    {
        if (context.started )
        {

            SlashInputBuffered = true;
            bufferTimer = inputBufferTime;

        }
    }

    public void A_AttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            KickInputBuffered = true;
            bufferTimer = inputBufferTime;
        }
    
    }

    public void RBAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            launcherInputBuffered = true;
            bufferTimer = inputBufferTime;
        }

    }

    public void SuperAttackInput(InputAction.CallbackContext context)
    {
        if (context.started && state.atk == false && state.ableBodied && state.airAtk == false && meter >= 50)
        {

            if (movement.isGrounded == true)
            {
                meter -= 50;
                moves.Play("Super");
            }

        }
    }

    public void RTAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("modifier turn on");
            modifierInputBuffered = true;
            clearBuffers();
            modifierBufferTimer = modifierBufferTime;
        }

    }


    public void pauseInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {

        }
    }
    public void selectInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void clearBuffers()
    {
        KickInputBuffered = false;
        PunchInputBuffered = false;
        SlashInputBuffered = false;
        HeavyInputBuffered = false;
        launcherInputBuffered = false;
    }

    #endregion

    #region atualmente nao utilizado

    public void Recover()                                   //antes era utilizado para levantar o jogador caso ele ficasse preso no chão
    {
        Debug.Log("recovering now");
        //player is supposedly playing the recover animation at this moment
        //moves.SetBool("Recovering", true);
        StartCoroutine(RecoverTimer());
    
    }   

    IEnumerator RecoverTimer()
    {
        //For when the player is on the ground, hurt
        yield return new WaitForSeconds(1);
        //Debug.Log("recovered");
        moves.SetBool("Recovered", true);
        bounces = 0;
        wallHitCount = 0;
        storeHitbox(new Vector3(0, 0, 0));
        gotHit = false;
        StartCoroutine(ResetRecover());
    }
    IEnumerator chainResetTimer()
    {

        //antes era usado para usar chain attacks, mas a versão atual do jogo não utiliza chain attacks
        // por causa da personagem usada não possuir attacks em chain.
        // a implementação tradicional de utilizar chains por meio de eventos tem um erro:
        // o chain tem que ser frame-perfect, ou seja, o jogador tem que apertar o botão nos frames exatos da animação
        //a solução ideal seria criar um input buffer , que significaria um sistema que "guarda" inputs por um tempo determinado
        //antes de jogar eles fora.

        //atualização : completo. Parabéns!


        yield return new WaitForSeconds(5f);
        chain = 0;
        //state.canWalk = true;
    }

    #endregion

    #region Hit stun quando o jogador acerta alguem

    public void Slowdown()
    {
        // ativada quando o jogador bate em alguem. Ela desativa a gravidade e desacelera a animação do jogador
        DisableAnimation();
        DisableGravity();
        StartCoroutine(ResetAttacker());

    }

    public void DisableAnimation()
    {
        //Ela desacelera a animação do jogador
        moves.speed = 0;
        
    }

    public void DisableGravity()
    {

        //desativa uma variavel que afeta uma função da gravidade no script "Move"
        movement.frozenState = true;
     
    }
    IEnumerator ResetAttacker()

    {
        // jogador atacante volta ao normal após 0.2 segundos
        yield return new WaitForSeconds(0.2f);
        RestoreSpeed();
        movement.frozenState = false;

    }

    #endregion 

    #region Hit stun quando o jogador leva dano

    public void GetSlowdown(hitbox collision, AudioClip hitSound, float damage, KnockbackType knockback , HitboxType hitboxType, VFXType vfxType)
    {


        transform.LookAt(new Vector3(collision.transform.position.x, transform.position.y, collision.transform.position.z));

        wallHitCount ++;
        
        if ( wallHitCount >= 6)
        {
            canStick = true;
        }
        //esta função ativa quando um jogador é atingido. Funciona de maneira um pouco diferente de Slowdown()

        if (damage >= hp && !movement.canBlock)
        {


            if (knockback == KnockbackType.Upward)
            {
                moves.SetTrigger("UpwardHurt");
                
            }

            if (knockback == KnockbackType.Forward)
            {
                moves.Play("ForwardKnockback");
            }
            if (knockback == KnockbackType.Spinning)
            {
                moves.Play("RotationKnockback");

            }
            //if (knockback == KnockbackType.Ground)
            //{
            //    collision.HorizontalKnockback = 2;
            //    collision.VerticalKnockback = 1;
            //    moves.SetTrigger("ForwardHurt");
            //}

            if (knockback == KnockbackType.Downward)
            {
                moves.SetTrigger("DownwardHurt");
            }
            //ativa o slowmotion chamando a instance do Roundmanager.

            PlaySound(FinishingBlow);
           //RoundManager.Instance.SlowdownGame(1);
           
        }

        if (damage < hp && !movement.isGrounded)
        {
            gotHitInAir = true;
            //a ideia aqui é transformar todo move normal do atacante em um move que dá knockback vertical automaticamente se o oponente estiver no ar.
            //porém, há problemas com essa solução que ainda não sei resolver, não está muito consistente.
           
            if (knockback == KnockbackType.Ground)
            {
                Debug.Log("hititng a normal on  the air");
                moves.SetTrigger("UpwardHurt");
                collision.VerticalKnockback = 7;
            }

        }

        if (damage < hp && !movement.canBlock )
        {
            //Se o dano não for um finishing blow, ativa um trigger de animação de acordo com o tipo de animação da hitbox.

            if (knockback == KnockbackType.Upward)
            {
                moves.SetTrigger("UpwardHurt");
            }

            if (knockback == KnockbackType.Forward)
            {
                moves.Play("ForwardKnockback");
            }
            if (knockback == KnockbackType.Spinning)
            {
                moves.Play("RotationKnockback");
            }
            if (knockback == KnockbackType.Ground)
            {
                moves.Play("GroundHurt");
               
            }

        }

        if (movement.canBlock)
        {
            //damage = 0;
            //moves.Play("Block");
            //GameObject blockSpawn = Instantiate(block, crossSpawnPoint.transform.position, Quaternion.identity);
            //if (movement.flipped == false)
            //{
            //    Vector3 scale = blockSpawn.transform.localScale;
            //    float rotationY = blockSpawn.transform.eulerAngles.y;
            //    rotationY = Mathf.Round(rotationY);

            //    scale.z *= -1;
            //    blockSpawn.transform.eulerAngles = new Vector3(0, 180, 0);

            //}
            //gotHit = true;
            //PlaySound(blockClip);
            //collision.VerticalKnockback = 0;
            //collision.HorizontalKnockback = 2;
            //StartCoroutine(ShakeRoutine(2, collision));
        }

        else
        {

            if (vfxType == VFXType.Explosion)
            {
                GameObject explosionSpawn = Instantiate(VFXexplosion, VFXspawnPoint.transform.position, Quaternion.identity);
            }

            if (vfxType == VFXType.greenExplosion)
            {
                GameObject greenSpawn = Instantiate(greenGlow, VFXspawnPoint.transform.position, Quaternion.identity);
            }

            if (vfxType == VFXType.bigRedExplosion)
            {
                GameObject redSpawn = Instantiate(VFXred, VFXspawnPoint.transform.position, Quaternion.identity);
            }

            if (vfxType == VFXType.megaHit)
            {
                GameObject explosionSpawn = Instantiate(VFXexplosion, VFXspawnPoint.transform.position, Quaternion.identity);
                GameObject greenSpawn = Instantiate(greenGlow, VFXspawnPoint.transform.position, Quaternion.identity);
                GameObject redSpawn = Instantiate(VFXred, VFXspawnPoint.transform.position, Quaternion.identity);
            }

            if (collision.knockback == KnockbackType.Downward)
            {
                canBounce = true;
            }


            if (hitboxType != HitboxType.RomanCancel)
            {
                //Debug.Log("clean hit");
                gotHit = true;
                PlaySound(hitSound);
                hp -= damage;
                //hitboxes.SetActive(false);

                StartCoroutine(ShakeRoutine(2, collision));
            }

            else
            {
                //Debug.Log("roman cancel hit");
                movement.gravityChange(true);
                PlaySound(hitSound);
                StartCoroutine(ShakeRoutine(2, collision));
            }


            //    Debug.Log("clean hit");
            //    gotHit = true;
            //    PlaySound(hitSound);
            //    hp -= damage;
            //    //hitboxes.SetActive(false);

            //    StartCoroutine(ShakeRoutine(2, collision));
            //}

        }
    }
    private IEnumerator ShakeRoutine(int shakeCount, hitbox collision)
    {
        //cria o hit stun : o inimigo vibra no mesmo lugar para dar uma sensação de impacto na pessoa que levou o dano

        

        DisableAnimation();
        DisableGravity();
        shaking = true;
        Vector3 originalPosition = rb.position;
        float shakeSpeed = 0.03f; 

        for (int i = 0; i < shakeCount; i++)
        {
            // Vai para a direita
            rb.MovePosition(originalPosition + Vector3.right * 0.03f);
            yield return new WaitForSeconds(shakeSpeed);

            // Vai para a esquerda
            rb.MovePosition(originalPosition + Vector3.left * 0.03f);
            yield return new WaitForSeconds(shakeSpeed);
            shakeSpeed -= 0.02f;                                        // diminui a velocidade do shake a cada ciclo completo
        }

        
        yield return new WaitForSeconds(0.2f);              //duração do hitstop


        rb.position = originalPosition;
        shaking = false;

        //somente após o personagem terminar de vibrar , a gravidade e a velocidade das animações voltam ao normal
        RestoreSpeed();
        RestoreGravity(collision);

       
    }

    public void RestoreSpeed()
    {
        //volta a velocidade normal da animação
        moves.speed = 1f;
    }

    public void RestoreGravity(hitbox collision)
    {
        //Restora a gravidade, e apenas após isso, GetHit é chamado para ativar o knockback.
        movement.frozenState = false;
        GetHit(collision);
    }

    IEnumerator ResetRecover()
    {
       
        // Acho que não está sendo usado?
        Debug.Log("recovery reset");
        yield return new WaitForSeconds(1f);
        moves.SetBool("Recovered", false);  
    }
    public void GetHit(hitbox collision)
    {

        GameObject enemyObject = collision.transform.root.gameObject;                                   //vai procurar a raiz do objeto que tem a hitbox

        fight player = enemyObject.GetComponent<fight>();                                               //pega o script Fight do objeto raiz 
        hitbox hitBoxObject = collision.GetComponent<hitbox>();                                         //pega o script hitbox 

       

        if (gotHit == true || collision.hitboxType == HitboxType.RomanCancel)
        {
            //desativa a ação do outro jogador
            state.ableBodied = false;
            state.atk = false;
            gotHitInAir = false;
            gotHit = false;

            Vector3 directionAwayFromAttacker = (transform.position - enemyObject.transform.position).normalized;

            Vector3 localKnockback = transform.InverseTransformDirection(directionAwayFromAttacker);

           
     
            Vector3 knockback =
             (transform.right * localKnockback.x * hitBoxObject.HorizontalKnockback) +       // Sideways
             (transform.forward * localKnockback.z * hitBoxObject.ForwardKnockback) +        // Forward/back
             (Vector3.up * hitBoxObject.VerticalKnockback);

            

            //rb.AddForce(knockback, ForceMode.VelocityChange);

            rb.linearVelocity = Vector3.zero;

            rb.linearVelocity = knockback;

            storeHitbox(knockback);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void bounceWall(bool Wall)
    {
        if (Wall)
        {
            moves.Play("WallHit");
            transform.LookAt(new Vector3(transform.right.x, transform.position.y, transform.forward.z));
        }

        else
        {
            moves.Play("GroundHit");
            transform.LookAt(new Vector3(transform.right.x, transform.position.y, transform.forward.z));
            bounces++;
            
        }

    }

    public void wallStick()
    {
        moves.Play("WallStick");
        movement.frozenState = true;
        StartCoroutine(resetWall());
    }

    public void getKO()
    {
        moves.Play("KO");
        StartCoroutine(RecoverTimer());
    }

    public IEnumerator resetWall()
    {
        yield return new WaitForSeconds(wallStickTime);
        movement.frozenState = false;
        moves.SetTrigger("WallFall");


    }

    public void storeHitbox(Vector3 knockback)
    {
        lastKnockback = knockback;
    }
}

#endregion

