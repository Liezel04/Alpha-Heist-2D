using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack }

    [Header("AI Settings")]
    [SerializeField] private Transform patrolPointsParent;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform groundCheck;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float heightThreshold = 0.5f;
    [SerializeField] private float jumpCooldown = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private EnemyState currentState = EnemyState.Patrol;
    private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    private float currentHealth;
    private float lastAttackTime;
    private float lastJumpTime;
    private bool isDead = false;

    private void Start()
    {
        // Cache core physics components and look for player tracking transform references
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player not found! Ensure Player GameObject has 'Player' tag.");

        // Automatically populate and map path destination nodes out of the layout hierarchy parent
        if (patrolPointsParent != null)
        {
            patrolPoints = new Transform[patrolPointsParent.childCount];
            for (int i = 0; i < patrolPointsParent.childCount; i++)
            {
                patrolPoints[i] = patrolPointsParent.GetChild(i);
            }
            if (patrolPoints.Length == 0) Debug.LogWarning("No patrol points found under parent object!");
        }
        else
        {
            Debug.LogError("PatrolPointsParent not assigned in Inspector!");
            patrolPoints = new Transform[0];
        }

        currentHealth = maxHealth;
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        // Safety guard checks to instantly drop movement if dead or frozen states trigger
        if (isDead || currentHealth <= 0)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // State machine processor updating behaviors dynamically based on player proximity
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distanceToPlayer <= chaseRange) currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                Chase();
                if (distanceToPlayer > chaseRange) currentState = EnemyState.Patrol;
                else if (distanceToPlayer <= attackRange) currentState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                Attack();
                if (distanceToPlayer > attackRange) currentState = EnemyState.Chase;
                break;
        }

        // Keeps locomotion animation variables constantly updated
        if (anim != null)
        {
            anim.SetBool("Walking", Mathf.Abs(rb.velocity.x) > 0.1f);
        }
    }

    // Standard looping navigation matching local index tracks
    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    // Hard-wires locomotion path headings directly toward player coordinates
    private void Chase()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    // Handles core horizontal velocity vectors, sprite flipping, and vertical navigation updates
    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        // Flips the sprite scale parameters to match travel trajectories
        if (direction.x > 0.1f)
            transform.localScale = new Vector3(0.12f, 0.14f, 1);
        else if (direction.x < -0.1f)
            transform.localScale = new Vector3(-0.12f, 0.14f, 1);

        // Vertical platform auto-jumping check logic structures
        if (IsGrounded() && Time.time - lastJumpTime >= jumpCooldown)
        {
            float deltaY = target.y - transform.position.y;
            if (deltaY > heightThreshold)
            {
                float g = -Physics2D.gravity.y * rb.gravityScale;
                float maxJumpHeight = (jumpForce * jumpForce) / (2f * g);

                if (deltaY > maxJumpHeight + 0.1f)
                {
                    if (currentState == EnemyState.Patrol)
                    {
                        currentPatrolIndex = (currentPatrolIndex - 1 + patrolPoints.Length) % patrolPoints.Length;
                        Debug.Log("Cannot reach height, reversing to previous patrol point.");
                    }
                    return;
                }

                // Checks for ceiling grid geometries via raycast arrays before breaking ground contact
                Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
                RaycastHit2D ceilingHit = Physics2D.Raycast(rayOrigin, Vector2.up, deltaY + 1f, groundLayer);
                Debug.DrawRay(rayOrigin, Vector2.up * (deltaY + 1f), Color.blue, 1f);

                if (ceilingHit.collider != null)
                {
                    if (currentState == EnemyState.Patrol)
                    {
                        currentPatrolIndex = (currentPatrolIndex - 1 + patrolPoints.Length) % patrolPoints.Length;
                        Debug.Log("Ceiling obstructed, reversing to previous patrol point.");
                    }
                    return;
                }

                // Applies instantaneous upward forces to execute jump routine transitions
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                lastJumpTime = Time.time;
                if (anim != null)
                {
                    anim.SetTrigger("Jump");
                }
                Debug.Log("Performing jump to higher target.");
            }
        }

        // Traces debugging lines dynamically straight into the editor viewport panel grid
        Debug.DrawRay(groundCheck.position, Vector2.down * checkDistance, Color.green);
        Debug.DrawRay(wallCheck.position, (transform.localScale.x > 0 ? Vector2.right : Vector2.left) * checkDistance, Color.red);
    }

    // Verifies solid contact layer metrics underneath the object base tracking point
    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
    }

    // Scans frontal space layer bounds to identify incoming wall obstructions
    private bool IsHittingWall()
    {
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        return Physics2D.Raycast(wallCheck.position, dir, checkDistance, groundLayer);
    }

    // Identifies drops or upcoming ledge boundaries before passing outer collision tiles
    private bool IsLedgeAhead()
    {
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)wallCheck.position + dir * 0.05f;
        return !Physics2D.Raycast(origin, Vector2.down, checkDistance + 0.05f, groundLayer);
    }

    // Halts horizontal motion vectors and invokes overlapping combat metrics to strike player health cards
    private void Attack()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            if (hit != null)
            {
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.takeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}
