using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour 
{
    #region Audio Variables
    // Damage Received (Audio/SFX Variable)
    [SerializeField] private AudioSource damageReceivedSoundEffect;

    // Death (Audio/SFX Variable)
    [SerializeField] private AudioSource deathSoundEffect;
    #endregion

    #region Player HP Fields
    // HP Fields (Just look at the names, damn it!)
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
    // Iframes Time (Iframes being when the player receives damage, they are granted a
    //  brief moment of invincibility to reach safety before the next instance of damage.)
    [Header("Iframes")]
    [SerializeField] private float IFrameDuration;  // Try 1.0f
    [SerializeField] private float IFrameTimer;     // Try 0.0f
    [SerializeField] private int NumFlashes;
    private bool isInvincible = false;
    private SpriteRenderer SRend;
    #endregion

    #region Runtime
    private void OnEnable() {
        EventManager.OnPlayerHealthChanged.AddListener(ModifyHealth);
    }

    void Start() {
        CurrentHP = MaxHP;
        SRend = GetComponent<SpriteRenderer>();
    }

    void Update() {
        // (TESTING) Use 'Q' to damage the player and show that the health system is working.
        // if (Input.GetKeyUp(KeyCode.Q)) {
        //    ModifyHealth(-1);
        // }

        // Update the invincibility timer.
        if (isInvincible) {
            IFrameTimer -= Time.deltaTime;

            // Check if invincibility has expired.
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
    #endregion

    #region Event Manager Misc
    private void OnDisable() {
        EventManager.OnPlayerHealthChanged.RemoveListener(ModifyHealth);
    }
    #endregion

    #region Modify Health
    /// <summary>
    /// Adds to or subtracts from the player health total if amount is '+' or '-' (respectively).
    /// </summary>
    /// <param name="amount"></param>
    private void ModifyHealth(int amount) {
        if(!isInvincible) {
            CurrentHP += amount;
            CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);

            if(amount < 0) // Is the player taking damage?
            {    
                if(CurrentHP <= 0) // Is the player dead?
                {    
                    // If the player dies, the death sound effect will play.
                    deathSoundEffect.Play();

                    // Death! (Will just restart the level every time until the "Gameover" screen is done.)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }

                // If just damage taken, the damage received sound effect will play.
                damageReceivedSoundEffect.Play();

                // Apply visual damage indicator
                StartCoroutine(DMGFlash());
                isInvincible = true;
                IFrameTimer = IFrameDuration;
            }

            Debug.Log("Player Health: " + CurrentHP);
        }
    }
    #endregion

    #region Damage Flash
    /// <summary>
    /// Briefly makes the player sprite flash red when taking damage.
    /// </summary>
    IEnumerator DMGFlash() {
        for(int i = 0; i < NumFlashes; i++) {
            SRend.color = new Color(1, 0, 0);
            yield return new WaitForSeconds(0.1f);
            SRend.color = Color.white;
            yield return new WaitForSeconds(0.1f);

        }
    }
    #endregion
}