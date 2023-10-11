using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// A base class for all basic enemies. Classes that inherit this class should
/// base.Start() or base.Update() if they wish to extend but not fully override
/// the core functionality of this class.
/// </summary>
public class BaseEnemy : MonoBehaviour {
    #region Enemy Variables
    protected Rigidbody2D rb;
    protected BoxCollider2D coll;
    protected float dirX = 0f;

    [SerializeField] protected LayerMask playerLayer;

    [SerializeField] protected int hp;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float playerDetectRange;
    [SerializeField] protected float playerChaseRange;
    [SerializeField] protected int attackDMG;
    [SerializeField] protected Vector2 attackRange;
    [SerializeField] protected Vector2 attackSize;
    [SerializeField] protected float telegraphSpeed;
    [SerializeField] protected float attackSpeed;
    #endregion

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update() {
        ChasePlayer();
    }

    #region Player Detection Methods
    /// <summary>
    /// Checks for the Player within a certain radius.
    /// Override this method to use an alternative detection shape.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsPlayerDetected() {
        return Physics2D.CircleCast(coll.bounds.center, playerDetectRange, Vector2.left, playerDetectRange, playerLayer);
    }

    protected virtual bool IsPlayerInAttackRange() {
        return Physics2D.BoxCast(coll.bounds.center, new Vector2(attackRange.x, attackRange.y), 0f, Vector2.left, 0f, playerLayer);
    }

    /// <summary>
    /// If Player is detected, cast a larger circle & chase the player.
    /// </summary>
    protected virtual void ChasePlayer() {
        if(IsPlayerDetected()) {
            if(IsPlayerInAttackRange()) {
                //Attack();
            }
            else {
                // Find a path to the player's position
                //List<Vector2> path = FindPathToPlayer();

                //if(path.Count > 0) {
                    // Move towards the next waypoint in the path
                    //MoveTo(path[0]);
                //}
            }
        }
    }

    //DUMMY
    protected List<Vector2> FindPathToPlayer() {
        return new List<Vector2>();
    }

    //DUMMY
    protected void MoveTo(List<Vector2> path) {
    
    }
    #endregion

    #region Enemy Attack Methods
    protected void Attack() {
        
    }
    #endregion

    #region Debugging Methods
    /// <summary>
    /// Draws where the various Detection colliders would be.
    /// Gizmos are only visible in the Unity Editor.
    /// </summary>
    protected virtual void OnDrawGizmos() {
        coll = GetComponent<BoxCollider2D>();
        if(coll != null) {
            // Draw Detect Radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(coll.bounds.center, playerDetectRange);
            // Draw Chase Radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(coll.bounds.center, playerChaseRange);

            //// Draw Attack Box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(coll.bounds.center, new Vector3(attackRange.x, attackRange.y));
        }
    }
    #endregion

    #region Enemy Health Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void TakeDamage(int damage) {
        // Implement damage logic here
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Die() {
        // Implement death logic here
    }
    #endregion
}
