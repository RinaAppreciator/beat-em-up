using System.Collections;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    public Enemy enemy;
    public fight player;
    private bool isColliding;



// Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
    {
        if (enemy != null)
        {
            enemy = transform.parent.gameObject.GetComponent<Enemy>();
        }

        if (player != null)
             player = transform.parent.gameObject.GetComponent<fight>();

    }

 

}





