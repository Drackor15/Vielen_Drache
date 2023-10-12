using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Pathfinding;

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
    protected Path path;
    protected Seeker seeker;
    protected int currentWaypoint;
    protected bool reachedEndOfPath;
    protected bool isChasing;
    protected float pathfindingRefreshTimer;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected Transform playerTransform;
    [SerializeField] protected float pathfindingUpdateFrequency = 0.2f;
    [SerializeField] protected float nextWaypointDistance;
    [SerializeField] protected float playerDetectRange;
    [SerializeField] protected float chaseMultiplier = 1f;

    [Header("Enemy Stats")]
    [SerializeField] protected int hp;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Vector2 maxLeap;
    [SerializeField] private LayerMask jumpableGround;

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

    }

    protected virtual void Update() {
        TryUpdatePathToPlayer();
        EnemyAI();
    }

    #region Enemy AI Methods
    /// <summary>
    /// If Player is detected, then increase Detection radius & Chase;
    /// Otherwise decrease Detection radius & stop moving
    /// </summary>
    protected virtual void EnemyAI() {
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
        else if(DoesNeedToLeap() && IsGrounded()) {
            LeapTo();
        }
        else {
            MoveOnPath();
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

    protected virtual void LeapTo() {
        // Calculate the point on the A* path where the enemy should leap.
        Vector3 leapPoint = CalculateLeapPoint();

        // Calculate the vector force needed to reach the leap point.
        Vector2 leapForce = CalculateLeapForce(leapPoint);

        // Apply the force to the enemy's rigidbody to make it leap.
        rb.AddForce(leapForce, ForceMode2D.Impulse);
    }

    // Calculate the point on the A* path where the enemy should leap.
    protected virtual Vector3 CalculateLeapPoint() {
        // You can use your pathfinding algorithm to find a point on the path
        // that is within the range defined by maxLeap.x and maxLeap.y.
        // For example, you can loop through the path waypoints and find the
        // first waypoint that is at an appropriate distance from the enemy.

        // Here is a simplified example. You can modify this as needed.
        for(int i = 0; i < path.vectorPath.Count; i++) {
            Vector3 waypoint = path.vectorPath[i];
            float distanceX = Mathf.Abs(waypoint.x - transform.position.x);
            float distanceY = Mathf.Abs(waypoint.y - transform.position.y);

            if(distanceX <= maxLeap.x && distanceY <= maxLeap.y) {
                return waypoint;
            }
        }

        // If no suitable point is found, return the last waypoint as a fallback.
        return path.vectorPath[path.vectorPath.Count - 1];
    }

    // Calculate the vector force needed to reach the leap point.
    protected virtual Vector2 CalculateLeapForce(Vector3 leapPoint) {
        // Calculate the direction from the current position to the leap point.
        Vector2 direction = (leapPoint - transform.position).normalized;

        // Calculate the force needed to reach the leap point based on the distance.
        float distance = Vector3.Distance(transform.position, leapPoint);
        Vector2 leapForce = direction * moveSpeed * distance;

        return leapForce;
    }

    /// <summary>
    /// Subtracts Enemy's Transform Height from Player's Transform Height
    /// to determine if the Enemy needs to jump.
    /// </summary>
    /// <returns>returns <see langword="true"/> if difference is greater than 3
    /// ; otherwise returns <see langword="false"/></returns>
    protected virtual bool DoesNeedToLeap() {
        if(playerTransform.position.y - transform.position.y > 3) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates a Box Collider (equal in size to the Enemy BoxCollider)
    /// slightly below the Enemy collider to detect if the enemy is on jumpable ground.
    /// </summary>
    /// <returns>True if on jumpable ground; False if anywhere else</returns>
    private bool IsGrounded() {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
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
