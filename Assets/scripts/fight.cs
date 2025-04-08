using UnityEngine;
using UnityEngine.InputSystem;

public class fight : MonoBehaviour
{
    public GameObject suicide;
    public fight otherplayer;
    public Move movement;
    public atkmanager state;

    public bool isGrabbing;

    // Animations
    public Animator moves; // animator
    public int grabLayerIndex;
    public int baseLayerIndex;

    // Attacks damages
    public float LAD;  // light attack damage
    public float HAD;  // heavy attack damage
    public float HAHD;
    public float CAD;  // chain attacks damage
    public float SAD;  // special attack damage
    public float UAD;  // launcher attack damage
    public float UAHD;
    private bool atk;  // true during damage frames

    // Health
    public float hp;  // hitpoints
    private float damage;  // the damage applied
    public float hitvar;
    public bool gotHit;

    // Knockbacks
    public Rigidbody rb;  // rigidbody that moves player
    public float lightK;  // normal attack knockback
    public float heavyK;  // big knockback
    public float upK;     // upward knockback for launcher
    public float downK;
    public float K;
    public float Kup;
    public float chain;
    public bool maxchain;

    // Attack Hitboxes
    public GameObject Lhit; // Light attack hitbox
    public GameObject Hhit; // Heavy attack hitbox
    public GameObject Shit; // Special attack hitbox
    public GameObject Uhit; // Launcher hitbox

    public atkmanager enemy;
    public Enemy grabbedEnemy;

    // Input Actions
    private PlayerControls controls;
    private bool lightPressed, heavyPressed, upperPressed, grabPressed;

    void Awake()
    {
        controls = new PlayerControls();

        // Mapear os botões aos eventos
        controls.Gameplay.LightHit.performed += ctx => lightPressed = true;
        controls.Gameplay.HeavyHit.performed += ctx => heavyPressed = true;
        controls.Gameplay.UpperCut.performed += ctx => upperPressed = true;
        controls.Gameplay.Grab.performed += ctx => grabPressed = true;
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public void Start()
    {
        gotHit = false;
        rb = GetComponent<Rigidbody>();
        baseLayerIndex = moves.GetLayerIndex("BaseLayer");
        grabLayerIndex = moves.GetLayerIndex("GrabLayer");
    }

    public void Gethit(Collider collision)
    {
        GameObject enemyObject = collision.transform.root.gameObject;
        hitvar = Random.Range(-1, 1);
        gotHit = true;

        if (hitvar >= 0)
        {
            moves.Play("Hitted");
        }

        if (hitvar < 0)
        {
            moves.Play("Hitted2");
        }
    }

    public void Update()
    {
        if (hp <= 0)
        {
            suicide.SetActive(false);
            Debug.Log("Died");
        }

        // Light Attack
        if (lightPressed)
        {
            damage = LAD;
            Kup = 0;

            if (state.atk == false)
            {
                Debug.Log("first attack");
                moves.Play("Light Attack");
                chain += 1;
            }

            if (!state.followup)
            {
                chain = 0;
            }

            lightPressed = false;
        }

        // Heavy Attack
        if (heavyPressed && state.atk == false)
        {
            Debug.Log("heavy kick");
            moves.Play("Chain2");
            damage = HAD;
            K = lightK;
            Kup = 0;

            heavyPressed = false;
        }

        // Special Attack (usando o mesmo botão do heavy neste exemplo)
        if (controls.Gameplay.HeavyHit.triggered && state.atk == false)
        {
            moves.Play("Chain1");
            damage = SAD;
            K = lightK;
            Kup = 0;
        }

        // Grab
        if (grabPressed)
        {
            if (isGrabbing)
            {
                moves.Play("Throw");
            }
            else
            {
                moves.Play("Grab");
            }

            grabPressed = false;
        }

        // Launcher / Uppercut
        if (upperPressed && state.atk == false)
        {
            Debug.Log("uppercut");
            moves.Play("Chain2"); // <- Coloque o nome certo da animação aqui
            damage = UAD;
            K = lightK / 2;
            Kup = upK;

            upperPressed = false; // <- Reset
        }

        // Corrida (animação)
        if (movement.running == true)
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
        if (isGrabbing == true)
        {
            moves.SetLayerWeight(baseLayerIndex, 0);
            moves.SetLayerWeight(grabLayerIndex, 1);
        }
        else
        {
            moves.SetLayerWeight(grabLayerIndex, 0);
            moves.SetLayerWeight(baseLayerIndex, 1);
        }
    }
}
