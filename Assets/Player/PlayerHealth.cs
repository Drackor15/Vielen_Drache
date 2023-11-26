using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthAll : MonoBehaviour
{
    //Just look at the names damn it
    [Header ("HP")]
    [SerializeField] public int MaxHP;
    public int CurrentHP;
    
    //Iframes time
    [Header("Iframes")]
    [SerializeField] private float IFrameTime;
    [SerializeField] private int NumFlashes;
    private SpriteRenderer SRend;


    // Start is called before the first frame update
    void Start()
    {
        CurrentHP = MaxHP;
        SRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //testing dmg and show health is working
        if (Input.GetKey(KeyCode.Q))
        {
            dmgTaken(1);
        }
    }

    void dmgTaken(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            //death, will just restart the level until gameover screen is done
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
        StartCoroutine(Invincible());
    }

    void HealTaken(int amount)
    {
        CurrentHP += amount;

        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;

        }
    }

    private IEnumerator Invincible()
    {
        //Iframes aren't work with the test, need an enemy to be able to use it correctly
        //We can change and discuss the color laster
        Physics2D.IgnoreLayerCollision(7, 8, true);
        for (int i = 0; i < NumFlashes; i++)
        {
            SRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(1);
            SRend.color = Color.white;
            yield return new WaitForSeconds(1);

        }
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

}
