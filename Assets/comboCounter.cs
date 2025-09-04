using System.Collections;
using UnityEngine;

public class comboCounter : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public fight playerScript;
    public float comboCount;
    public bool hasActivated;
    public bool hasDeactivated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //comboCount = playerScript.wallHitCount;
        hasActivated = false;
        hasDeactivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        comboCount = playerScript.wallHitCount;

        
        switch (comboCount)
        {
            case 0:
                {
                    Debug.Log("white");
                    spriteRenderer.color = new Color(255, 255, 255);
                    break;
                }
            case 1:
                {
                    Debug.Log("blue");
                    spriteRenderer.color = new Color(0, 255, 255);
                    break;
                }
            case 2:
                {
                    Debug.Log("green");
                    spriteRenderer.color = new Color(0, 255, 0);
                    break;
                }
            case 3:
                {
                    Debug.Log("yellow");
                    spriteRenderer.color = new Color(255, 255, 0);
                    break;
                }
            case 4:
                {
                    Debug.Log("purple");
                    spriteRenderer.color = new Color(255, 0, 255);
                    break;
                }
            case 5:
                {
                    Debug.Log("purple");
                    spriteRenderer.color = new Color(255, 0, 0);
                    break;
                }
            case 6:
                {
                    Debug.Log("red");
                    spriteRenderer.color = new Color(255, 0, 0);
                    break;
                }

        }

        if (comboCount > 0 && !hasActivated )
        {
            spriteRenderer.enabled = true;
            //Debug.Log("zacktivated");
            hasActivated = true;
            hasDeactivated = false;
      
        }

        //else if (comboCount == 0 && !hasDeactivated && hasActivated)
        //{
        //    Debug.Log("Dezacktivated");
        //    spriteRenderer.enabled = false;
        //    hasDeactivated = true;
        //    hasActivated = false;
        //    //StartCoroutine(resetActivationTimer());
        //}
    }

}

 
