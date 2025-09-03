using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class autoTarget : MonoBehaviour
{
    public List<Transform> targets;
    public Transform target;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setTarget();
    }

    private void OnTriggerEnter(Collider collision)
    {
        Transform target = collision.gameObject.transform;

        fight playerScript = collision.gameObject.GetComponent<fight>();

        if (playerScript != null)
        {
            //Debug.Log("Potential Enemy detected");
            //Debug.Log("Target List" + targets);
            targets.Add(target);
            //CheckImpact(rb);
        }

        else
        {
            return;
        }


    }

    public void setTarget()
    {
        if (targets != null && targets.Count > 0 && targets[0] != null)
        {
            target = targets[0];
        }
        else
        {
            return;
        }

    }
}
