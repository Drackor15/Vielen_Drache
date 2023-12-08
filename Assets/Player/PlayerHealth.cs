using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    #region Player HP Fields
    //Just look at the names damn it
    [Header("HP")]
    [SerializeField] public int MaxHP;
    public int CurrentHP;

    // Heart Objects
    [Header("HP Vis")]
    public Object Heart1;
    public Object Heart2;
    public Object Heart3;
    public Object EH1;
    public Object EH2;
    public Object EH3;

    //Iframes time
    [Header("Iframes")]
    [SerializeField] private float IFrameDuration; // try 1.0f
    [SerializeField] private float IFrameTimer;// try 0.0f
    [SerializeField] private int NumFlashes;
    private bool isInvincible = false;
    private SpriteRenderer SRend;
    #endregion

    private void OnEnable() {
        EventManager.OnPlayerHealthChanged.AddListener(ModifyHealth);
    }

    void Start() {
        CurrentHP = MaxHP;
        SRend = GetComponent<SpriteRenderer>();
    }

    void Update() {
        //testing dmg and show health is working
        if(Input.GetKeyUp(KeyCode.Q)) {
            ModifyHealth(-1);
        }

        // Update the invincibility timer
        if(isInvincible) {
            IFrameTimer -= Time.deltaTime;

            // Check if invincibility has expired
            if(IFrameTimer <= 0.0f) {
                isInvincible = false;
            }
        }

        if (CurrentHP == 3)
        {
            EH1.GameObject().SetActive(false);
            EH2.GameObject().SetActive(false);
            EH3.GameObject().SetActive(false);
            Heart1.GameObject().SetActive(true);
            Heart2.GameObject().SetActive(true);
            Heart3.GameObject().SetActive(true);
        }
        else if (CurrentHP == 2) 
        {
            EH1.GameObject().SetActive(false);
            EH2.GameObject().SetActive(false);
            EH3.GameObject().SetActive(true);
            Heart1.GameObject().SetActive(true);
            Heart2.GameObject().SetActive(true);
            Heart3.GameObject().SetActive(false);
        }
        else if (CurrentHP == 1)
        {
            EH1.GameObject().SetActive(false);
            EH2.GameObject().SetActive(true);
            EH3.GameObject().SetActive(true);
            Heart1.GameObject().SetActive(true);
            Heart2.GameObject().SetActive(false);
            Heart3.GameObject().SetActive(false);
        } else if (CurrentHP == 0) 
        {
            EH1.GameObject().SetActive(true);
            EH2.GameObject().SetActive(true);
            EH3.GameObject().SetActive(true);
            Heart1.GameObject().SetActive(false);
            Heart2.GameObject().SetActive(false);
            Heart3.GameObject().SetActive(false);
        }
    }

    private void OnDisable() {
        EventManager.OnPlayerHealthChanged.RemoveListener(ModifyHealth);
    }

    /// <summary>
    /// Adds or Subtracts Player Health if amount is + or -, respectively.
    /// </summary>
    /// <param name="amount"></param>
    private void ModifyHealth(int amount) {
        if(!isInvincible) {
            CurrentHP += amount;
            CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);

            // Is Player taking damage?
            if(amount < 0) {
                // Is Player Dead?
                if(CurrentHP <= 0) {
                    //death, will just restart the level until gameover screen is done
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }
                StartCoroutine(DMGFlash());
                isInvincible = true;
                IFrameTimer = IFrameDuration;
            }

            Debug.Log("Player Health: " + CurrentHP);
        }
    }

    /// <summary>
    /// Briefly makes the Player Sprite flash red when taking DMG.
    /// </summary>
    IEnumerator DMGFlash() {
        for(int i = 0; i < NumFlashes; i++) {
            SRend.color = new Color(1, 0, 0);
            yield return new WaitForSeconds(0.1f);
            SRend.color = Color.white;
            yield return new WaitForSeconds(0.1f);

        }
    }
}