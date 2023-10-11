using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    #region Enemy Variables
    /* Enemy Variables
     * 
     */
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private float dirX = 0f;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private int hp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float playerDetectRange;
    [SerializeField] private float playerChaseRange;
    [SerializeField] private int attackDMG;
    [SerializeField] private Vector2 attackRange;
    [SerializeField] private Vector2 attackSize;
    [SerializeField] private float telegraphSpeed;
    [SerializeField] private float attackSpeed;
    #endregion

    /// <summary>
    /// Called by Start. The purpose of this method is so that child classes
    /// can use, but extend the base behaviour of BaseEnemy. If the code within
    /// this method was simply put in Start(), child classes are forced into an
    /// 'all or nothing' situation where they are either using BaseEnemy's Start method
    /// or their own.
    /// </summary>
    protected virtual void OnStart() {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Called by Update. The purpose of this method is so that child classes
    /// can use, but extend the base behaviour of BaseEnemy. If the code within
    /// this method was simply put in Update(), child classes are forced into an
    /// 'all or nothing' situation where they are either using BaseEnemy's Update method
    /// or their own.
    /// </summary>
    protected virtual void OnUpdate() {
        ChasePlayer();
    }

    void Start() {
        OnStart();
    }

    void Update() {
        OnUpdate();
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
        // Draw Detect Radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(coll.bounds.center, playerDetectRange);
        // Draw Chase Radius
        Gizmos.color = Color.yellow;

        // Draw Attack Box
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(coll.bounds.center, new Vector3(attackRange.x, attackRange.y));
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
