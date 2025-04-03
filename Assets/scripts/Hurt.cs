using System.Collections;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    public Enemy enemy;
    private bool isColliding;



// Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
    {
        enemy = transform.parent.gameObject.GetComponent<Enemy>();

    }

 

}





