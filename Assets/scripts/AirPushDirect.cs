using UnityEngine;

public class AirPushDirect : MonoBehaviour
{

    public float raycastDistance = 1.5f;
    public BoxCollider collider;
    public LayerMask characterLayerMask;
    public LayerMask groundLayerMask;
    Vector3 originalColliderSize = new Vector3(0.3692338f, 0.9934251f, 0.3215933f);

    private Rigidbody rb;
    private bool isAirborne => !Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayerMask);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        if (isAirborne)
        {
            Debug.Log("is Airborne");
            collider.size = new Vector3(0.3692338f, 0.5f, 0.06f);
        }

        if (!isAirborne)
        {
            collider.size = originalColliderSize;


        }
    }
}

// Update is called once per frame
