using UnityEngine;

public class PhysicsMaterialSwapper : MonoBehaviour
{

    public CapsuleCollider playerCollider;

    public PhysicsMaterial normalState;
    public PhysicsMaterial bounceState;
    public fight playerScript;
    public Move movementScript;
    public bool hasSwapped;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        swapMaterial(normalState);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("true or false" + playerScript.canGroundBounce);
        if (playerScript.canGroundBounce )
        {
            Debug.Log("can ground bounce");
            swapMaterial(bounceState);
        }

        if (!playerScript.canGroundBounce && hasSwapped)
        {
            Debug.Log("returned back to normal state");
            swapMaterial(normalState);
        }
    }

    public void swapMaterial(PhysicsMaterial material)
    {
        if(material == bounceState)
        {
            hasSwapped = true;
            Debug.Log("Can bounce");
        }

        playerCollider.material = material;
    }
}
