using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpspeed;
    private Rigidbody2D body;
    private Animator animator;
    private bool Jumping;
    public MeatCollectable Meat;

    [Header("Portal Mechanics")]
    [SerializeField] private GameObject portalObject;

    private int internalMeatCount = 0;

    private void Awake()
    {
        // Cache internal physics components on initialization
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Processess horizontal layout movement vector tracking
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // Flips player sprite scale to match horizontal locomotion vectors
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3((float)1.2, (float)2, 1);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3((float)-1.2, (float)2, 1);

        if (Input.GetKey(KeyCode.Space) && Jumping)
            Jump();

        // Syncs local parameter tags inside the animation controller state machine
        animator.SetBool("Running", horizontalInput != 0);
        animator.SetBool("Jumping", Jumping);
    }

    // Applies vertical upward impulses to launch the character capsule
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpspeed);
        Jumping = false;
        animator.SetTrigger("Jump");
    }

    // Detects ground contact boundaries to refresh jumping availability metrics
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            Jumping = true;
    }

    // Accessess node tracking system called by external attack modules
    public bool canAttack()
    {
        return true;
    }

    // Processes boundary overlaps for active environmental interactable assets
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Core collectible item tracking pipeline
        if (other.gameObject.CompareTag("Meat"))
        {
            // Executes crunch sound FX overlay via the audio mixer
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMunchNoise();
            }

            Destroy(other.gameObject);

            // Increments tracking totals across both local variables and UI components
            internalMeatCount++;
            if (Meat != null)
            {
                Meat.meat++;
            }

            // Verifies if game unlock scores match requirements to render the exit portal
            if (internalMeatCount >= 6 || (Meat != null && Meat.meat >= 6))
            {
                if (portalObject != null && !portalObject.activeSelf)
                {
                    portalObject.SetActive(true);

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayPortalOpenNoise();
                    }
                }
            }
        }

        // Scene exit flow management pipeline
        if (other.gameObject.CompareTag("Portal") || (portalObject != null && other.gameObject == portalObject))
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPortalTeleportNoise();
            }

            // Restricts movement arrays dynamically during map transition cycles
            body.velocity = Vector2.zero;
            this.enabled = false;

            // Advances active scene index parameter variables forward to load the victory panel
            int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextLevelIndex);
        }
    }
}



