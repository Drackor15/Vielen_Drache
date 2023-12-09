using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using Pathfinding;

/// <summary>
/// A base class for all basic enemies. Classes that inherit this class should
/// base.Start() or base.Update() if they wish to extend but not fully override
/// the core functionality of this class.
/// 
/// As of 11/23/23 the scope of this class has been drastically reduced. If you want
/// to build upon the previously conceived enemy class (enemies were going to be more interesting/complex),
/// check out commit ee6451a
/// </summary>
public class BaseEnemy : MonoBehaviour {
    #region Enemy Variables
    protected Rigidbody2D rb;
    protected BoxCollider2D coll;
    protected float dirX = 0f;
    protected bool isPausingPatrol = false;
    protected float patrolPauseStart;
    protected SpriteRenderer sprite;
    protected enum AnimationState { idle, patrol, patrolPause, dying };
    protected Animator animator;
    protected bool isDying = false;

    // (For Audio)
    // Initializing the footstepController field (a timer for footstep SFX).
    public FootstepController footstepController;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask playerLayer;

    [Header("Patrol & Idle Settings")]
    [Tooltip("Set True if this Enemy is to patrol. False if they are to Idle.")]
    [SerializeField] protected bool isDefaultPatrol = true;
    [Tooltip("Directional Multiplier used to determine in which direction the enemy should " +
             "begin their patrols. 1 = positive x direction. -1 = negative x direction.")]
    [SerializeField] protected int patrolDirection = 1;
    [Tooltip("How long an Enemy pauses (in seconds) when reaching one end of a patrol.")]
    [SerializeField] protected int pausePatrolTime;
    [SerializeField] protected LayerMask walkableGround;

    [Header("Enemy Stats")]
    [SerializeField] protected int maxHp;
    [SerializeField] protected float patrolSpeed;
    [Tooltip("The hitbox which the Player can collide with, to DMG this Enemy")]
    [SerializeField] protected Vector2 weakspotSize;
    [Tooltip("Ignore the Z dimension")]
    [SerializeField] protected Vector3 weakspotOffset;
    [SerializeField] protected int dmgTakenFromPlayer;
    protected int currentHp;

    [Header("Attack Stats")]
    [SerializeField] protected int attackDMG;
    [SerializeField] protected Vector2 attackSize;
    #endregion

    #region Audio Variables
    // Grunt Death Audio Sound Effect Variable
    [SerializeField] private AudioSource gruntDeathSoundEffect;
    #endregion

    #region (Protected) Runtime
    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        footstepController = GetComponentInChildren<FootstepController>();
        currentHp = maxHp;
    }

    protected virtual void Update() {
        EnemyAI();
        WalkingCheck();
    }
    #endregion

    #region Enemy AI Methods
    /// <summary>
    /// If Player is detected, then increase Detection radius & Chase;
    /// Otherwise decrease Detection radius & patrol or stop moving
    /// </summary>
    protected virtual void EnemyAI() {
        FlipSprite();

        if(isDefaultPatrol && !isDying) {
            EnemyPatrol();
        }
        else {
            animator.SetInteger("animState", (int)AnimationState.idle);
        }
        if(IsPlayerInAttackable() && !IsPlayerInWeakspot() && !isDying) {
            Attack();
        }
        if(IsPlayerInWeakspot() && !isDying) {
            TakeDamage(dmgTakenFromPlayer);
        }
        if(currentHp == 0) {
            isDying = true;
            rb.bodyType = RigidbodyType2D.Static;
            coll.enabled = false;
            animator.SetInteger("animState", (int)AnimationState.dying);
        }
    }

    /// <summary>
    /// Stops when moving towards a ledge and inits Pause Timer variables.
    /// Counts down until it can move in the opposite direction to patrol.
    /// </summary>
    protected virtual void EnemyPatrol() {
        if(IsNearLedge() && IsMoving()) {
            StopVelocity();
            InitPausePatrolTimer();
        }
        else if(IsNearLedge() && isPausingPatrol) {
            animator.SetInteger("animState", (int)AnimationState.patrolPause);
            PausePatrolTimer();
        }
        else {
            animator.SetInteger("animState", (int)AnimationState.patrol);
            PatrolMove();
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
            // Draw Attack Box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(coll.bounds.center, new Vector2(attackSize.x, attackSize.y));

            // Draw IsNearLedge Collider
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(coll.transform.position.x + (patrolDirection * 1.5f), coll.transform.position.y - 1f), new Vector3(0.75f, 1));

            // Draw IsNearLedge Collider
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(coll.bounds.center + weakspotOffset, new Vector2(weakspotSize.x, weakspotSize.y));
        }
    }
    #endregion

    #region Enemy Movement Methods
    /// <summary>
    /// Enemy moves in a direction based on patrolSpeed
    /// </summary>
    protected virtual void PatrolMove() {
        rb.AddForce(new Vector2(patrolDirection * patrolSpeed * (Time.deltaTime * 100), rb.velocity.y));
    }

    /// <summary>
    /// Stops enemy X and Y velocities completely
    /// </summary>
    protected virtual void StopVelocity() {
        rb.velocity = new Vector2(0,0);
    }
    #endregion

    #region Patrol Methods
    /// <summary>
    /// Checks if the Enemy if moving a significant amount in the X direction.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsMoving() {
        return Mathf.Abs(rb.velocity.x) > 0.01f;
    }

    /// <summary>
    /// Checks for walkable ground in the direction that the enemy walks
    /// WARNING: the Gizmo that is drawing this collider is not tied to it at all.
    /// So if you change values in here, you'll have to change the Gizmo too
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsNearLedge() {
        return !Physics2D.BoxCast(new Vector2(coll.transform.position.x + (patrolDirection * 1.5f), coll.transform.position.y),
                                 new Vector2(0.75f,1), 0f, Vector2.down, 1f, walkableGround);
    }

    /// <summary>
    /// Init patrolPauseStart with current time & set isPausingPatrol to true.
    /// </summary>
    protected virtual void InitPausePatrolTimer() {
        patrolPauseStart = Time.time;
        isPausingPatrol = true;
    }

    /// <summary>
    /// Checks if enough time has passed before resuming the patrol
    /// in the opposite direction
    /// </summary>
    protected virtual void PausePatrolTimer() {
        float timePassed = Time.time - patrolPauseStart;
        if(timePassed >= pausePatrolTime) {
            isPausingPatrol = false;
            patrolDirection *= -1;
        }
    }
    #endregion

    #region Enemy Attack Methods
    /// <summary>
    /// Checks if the Player is within the enemy's attack range
    /// </summary>
    /// <returns> returns <see langword="true"/> if Player is in attack range;
    /// returns <see langword="false"/> if Player is not</returns>
    protected virtual bool IsPlayerInAttackable() {
        return Physics2D.BoxCast(coll.bounds.center, new Vector2(attackSize.x, attackSize.y), 0f, Vector2.left, 0f, playerLayer);
    }

    /// <summary>
    /// DMGs the Player based on this Enemy's attackDMG.
    /// </summary>
    protected void Attack() {
        EventManager.ModifyPlayerHealth(-attackDMG);
    }
    #endregion

    #region Enemy Health Methods
    /// <summary>
    /// Checks if the Player collides with the enemy's weakspot
    /// </summary>
    /// <returns> returns <see langword="true"/> if Player is colliding;
    /// returns <see langword="false"/> if Player is not</returns>
    protected virtual bool IsPlayerInWeakspot() {
        return Physics2D.BoxCast(coll.bounds.center + weakspotOffset, new Vector2(weakspotSize.x, weakspotSize.y), 0f, Vector2.left, 0f, playerLayer);
    }

    /// <summary>
    /// Subtracts from currentHP
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void TakeDamage(int damage) {
        currentHp -= damage;
        gruntDeathSoundEffect.Play(); // Play death SFX.
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

    /// <summary>
    /// Attatch this to the end of Enemy Death animations so that the
    /// instance of this game object is destroyed AFTER the death animation
    /// has played.
    /// </summary>
    protected virtual void DestroyEnemy() {
        Destroy(gameObject);
    }
    #endregion

    #region Enemy Audio Section
    private void WalkingCheck()
    {
        if (dirX != 0)
        {
            footstepController.StartWalking();
        }

        else
        {
            footstepController.StopWalking();
        }
    }
    #endregion
}
