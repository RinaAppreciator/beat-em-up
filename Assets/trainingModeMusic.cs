using UnityEngine;

public class trainingModeMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

}
