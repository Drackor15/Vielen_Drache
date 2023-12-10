using UnityEngine;

/// <summary>
/// Attach to Empty Game objects in the scene to define win areas on the LVL.
/// </summary>
public class WinGame : MonoBehaviour {
    #region Win Variables
    protected BoxCollider2D coll;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected Vector2 areaSize;
    #endregion

    protected virtual void Start() {
        coll = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update() {
        coll.size = areaSize;
        if(IsPlayerWinning()) {
            Win();
        }
    }

    #region Debugging Methods
    /// <summary>
    /// Draws where the various Detection colliders would be.
    /// Gizmos are only visible in the Unity Editor.
    /// </summary>
    protected virtual void OnDrawGizmos() {
        coll = GetComponent<BoxCollider2D>();
        if(coll != null) {
            // Draw Attack Box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(coll.bounds.center, new Vector2(areaSize.x, areaSize.y));
        }
    }
    #endregion

    #region Player Win Methods
    /// <summary>
    /// Checks if the Player is within the win zone on the level.
    /// </summary>
    /// <returns> returns <see langword="true"/> if Player is in win range;
    /// returns <see langword="false"/> if Player is not</returns>
    protected virtual bool IsPlayerWinning() {
        return Physics2D.BoxCast(coll.bounds.center, new Vector2(areaSize.x, areaSize.y), 0f, Vector2.left, 0f, playerLayer);
    }

    /// <summary>
    /// Makes the player win the game once called.
    /// </summary>
    protected void Win() {
        EventManager.WinGame();
    }
    #endregion
}
