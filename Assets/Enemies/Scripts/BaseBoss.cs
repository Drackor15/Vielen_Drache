using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Pathfinding;

/// <summary>
/// A base class for all bosses. Classes that inherit this class should
/// base.Start() or base.Update() if they wish to extend but not fully override
/// the core functionality of this class.
/// </summary>
public class BaseBoss : MonoBehaviour {
    #region Enemy Variables
    protected Rigidbody2D rb;
    protected BoxCollider2D coll;
    protected float dirX = 0f;
    protected Path path;
    protected Seeker seeker;
    protected int currentWaypoint;
    protected bool reachedEndOfPath;
    protected bool isChasing;
    protected float pathfindingRefreshTimer;
    protected SpriteRenderer sprite;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected float pathfindingUpdateFrequency = 0.2f;
    [SerializeField] protected float nextWaypointDistance;
    [SerializeField] protected float playerDetectRange;
    [Tooltip("How much the Player Detection Range increases (percentage in float)")]
    [SerializeField] protected float chaseMultiplier = 1f;

    [Header("Enemy Stats")]
    [SerializeField] protected int hp;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Vector2 maxSpeed;

    [Header("Attack Stats")]
    [SerializeField] protected int attackDMG;
    [SerializeField] protected Vector2 attackRange;
    [SerializeField] protected Vector2 attackSize;
    [SerializeField] protected float telegraphSpeed;
    [SerializeField] protected float attackSpeed;
    #endregion

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        seeker = GetComponent<Seeker>();
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update() {
        ClampVelocity();
        TryUpdatePathToPlayer();
        EnemyAI();
    }

    #region Enemy AI Methods
    /// <summary>
    /// If Player is detected, then increase Detection radius & Chase;
    /// Otherwise decrease Detection radius & patrol or stop moving
    /// </summary>
    protected virtual void EnemyAI() {
        FlipSprite();

        if(IsPlayerDetected()) {
            IncreaseDetectRadius();
            ChasePlayer();
        }
        else {
            DecreaseDetectRadius();
        }
    }

    /// <summary>
    /// Chase the Player until in Attack Range.
    /// Once the Player is in Attack Range, stop moving & attack.
    /// </summary>
    protected virtual void ChasePlayer() {
        if(IsPlayerInAttackRange()) {
            //Attack();
        }
        else {
            MoveOnPath();
        }
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

            // Draw Attack Box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(coll.bounds.center, new Vector3(attackRange.x, attackRange.y));
        }
    }
    #endregion

    #region Player Detection Methods
    /// <summary>
    /// Checks for the Player within a certain radius.
    /// Override this method to use an alternative detection shape.
    /// </summary>
    /// <returns>returns <see langword="true"/> if Player is in radius;
    /// returns <see langword="false"/> if Player is not</returns>
    protected virtual bool IsPlayerDetected() {
        return Physics2D.CircleCast(coll.bounds.center, playerDetectRange, Vector2.left, 0f, playerLayer);
    }

    /// <summary>
    /// Increases Detection Radius Once
    /// </summary>
    protected virtual void IncreaseDetectRadius() {
        if(!isChasing) {
            playerDetectRange *= chaseMultiplier;
            isChasing = true;
        }
    }

    /// <summary>
    /// Decreases Detection Radius Once
    /// </summary>
    protected virtual void DecreaseDetectRadius() {
        if(isChasing) {
            playerDetectRange /= chaseMultiplier;
            isChasing = false;
        }
    }

    /// <summary>
    /// Update Path to Player after a fixed amount of time.
    /// </summary>
    protected virtual void TryUpdatePathToPlayer() {
        pathfindingRefreshTimer += Time.fixedDeltaTime;

        if(pathfindingRefreshTimer >= pathfindingUpdateFrequency) {
            pathfindingRefreshTimer = 0f;
            UpdatePathToPlayer();
        }
    }

    /// <summary>
    /// Updates Path to Player if not already doing so.
    /// </summary>
    protected virtual void UpdatePathToPlayer() {
        if(IsPlayerDetected() && !IsPlayerInAttackRange() && seeker.IsDone()) {
            seeker.StartPath(rb.position, playerTransform.position, OnPathComplete);
        }
    }

    /// <summary>
    /// Inits path variables as long as nothing went wrong with path generation.
    /// </summary>
    /// <param name="p"></param>
    protected virtual void OnPathComplete(Path p) {
        if(!p.error) {
            path = p;
            currentWaypoint = 0;
        }
    }
    #endregion

    #region Enemy Movement Methods
    /// <summary>
    /// Makes sure that Enemy X and Y velocities don't exceed maxSpeed.x & maxSpeed.y
    /// </summary>
    protected virtual void ClampVelocity() {
        float clampedX = Mathf.Clamp(rb.velocity.x, -maxSpeed.x, maxSpeed.x);
        float clampedY = Mathf.Clamp(rb.velocity.y, -maxSpeed.y, maxSpeed.y);
        rb.velocity = new Vector2(clampedX, clampedY);
    }

    /// <summary>
    /// Adds force in the direction of the next waypoint on the path
    /// </summary>
    protected virtual void MoveOnPath() {
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.AddForce(new Vector2(Mathf.RoundToInt(direction.x) * moveSpeed * (Time.deltaTime * 100), rb.velocity.y));
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance) {
            currentWaypoint++;
        }
    }

    /// <summary>
    /// Stops enemy X and Y velocities completely
    /// </summary>
    protected virtual void StopVelocity() {
        rb.velocity = new Vector2(0,0);
    }
    #endregion

    #region Enemy Attack Methods
    /// <summary>
    /// Checks if the Player is within the enemy's attack range
    /// </summary>
    /// <returns> returns <see langword="true"/> if Player is in attack range;
    /// returns <see langword="false"/> if Player is not</returns>
    protected virtual bool IsPlayerInAttackRange() {
        return Physics2D.BoxCast(coll.bounds.center, new Vector2(attackRange.x, attackRange.y), 0f, Vector2.left, 0f, playerLayer);
    }

    protected void Attack() {
        
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

    #region Enemy Animation Methods
    /// <summary>
    /// Flips Enemy Sprite based on movement direction.
    /// </summary>
    private void FlipSprite() {
        if(rb.velocity.x > 0.2f) { sprite.flipX = false; }
        else if(rb.velocity.x < -0.2f) { sprite.flipX = true; }
    }
    #endregion
}
