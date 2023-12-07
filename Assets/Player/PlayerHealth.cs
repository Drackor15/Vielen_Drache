using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    #region Player HP Fields
    //Just look at the names damn it
    [Header("HP")]
    [SerializeField] public int MaxHP;
    public int CurrentHP;

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