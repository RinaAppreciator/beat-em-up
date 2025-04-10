using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Image healthBar;
    public TextMeshProUGUI scoreText;
    public fight player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    public void Start()
    {
        scoreText.text = player.score.ToString();
    }
    void Update()
    {
        
    }

    public void ChangeHealth(float damage)
    {
        if (damage < 0)
        {
            healthBar.fillAmount = player.hp / 100f ;
        }
    }

    public void ChangeScore(float score)
    {
      
        scoreText.text = player.score.ToString();
        
    }

}
