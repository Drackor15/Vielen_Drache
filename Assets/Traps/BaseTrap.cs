using UnityEngine;

/// <summary>
/// Attach to Empty Game objects in the scene to encompass trap areas on the LVL.
/// </summary>
public class TrapDMG : MonoBehaviour {
    #region Trap Variables
    protected BoxCollider2D coll;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask playerLayer;

    [Header("Attack Stats")]
    [SerializeField] protected int attackDMG;
    [SerializeField] protected Vector2 attackSize;
    #endregion

    protected virtual void Start() {
        coll = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update() {
        coll.size = attackSize;
        if(IsPlayerAttackable()) {
            Attack();
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
            Gizmos.DrawWireCube(coll.bounds.center, new Vector2(attackSize.x, attackSize.y));
        }
    }
    #endregion

    #region Player DMG Methods
    /// <summary>
    /// Checks if the Player is within the trap's attack range
    /// </summary>
    /// <returns> returns <see langword="true"/> if Player is in attack range;
    /// returns <see langword="false"/> if Player is not</returns>
    protected virtual bool IsPlayerAttackable() {
        return Physics2D.BoxCast(coll.bounds.center, new Vector2(attackSize.x, attackSize.y), 0f, Vector2.left, 0f, playerLayer);
    }

    /// <summary>
    /// DMGs the Player based on this Trap's attackDMG.
    /// </summary>
    protected void Attack() {
        EventManager.ModifyPlayerHealth(-attackDMG);
    }
    #endregion
}
