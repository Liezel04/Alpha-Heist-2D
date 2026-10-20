using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Configurations")]
    [SerializeField] private float Startinghealth;

    // Core vital tracks accessible by UI and enemy script systems
    public float currentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    private Animator animator;
    private bool dead;

    // Event system to alert the heart UI layer when health updates
    public delegate void HealthChanged();
    public event HealthChanged OnHealthChanged;

    private void Awake()
    {
        // Caches references and sets initial health pool
        currentHealth = Startinghealth;
        MaxHealth = Startinghealth;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Keyboard fallback to restart scene without UI button clicks
        if (dead && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Standards entry point for taking enemy damage
    public void takeDamage(float _damage)
    {
        // Prevents health from dropping below zero or exceeding max boundaries
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, MaxHealth);

        // Refreshes the visual hearts canvas layer
        OnHealthChanged?.Invoke();

        if (currentHealth > 0)
        {
            // Plays flinch state and activates physics layer bypass
            animator.SetTrigger("Hurt");
            StartCoroutine(Invulnerability());
        }
        else
        {
            // Ensures death sequence executes only once
            if (!dead)
            {
                // Triggers death audio if the global sound pipeline exists
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayPlayerDyingNoise();
                }

                animator.SetTrigger("die");

                // Kills input processing scripts immediately on fatal tick
                if (GetComponent<PlayerMovement>() != null)
                    GetComponent<PlayerMovement>().enabled = false;

                if (GetComponent<PlayerAttack>() != null)
                    GetComponent<PlayerAttack>().enabled = false;

                dead = true;

                // Scans active scene hierarchy for the primary UI Manager
                StartGame gameManager = null;
                StartGame[] allManagers = Object.FindObjectsByType<StartGame>(FindObjectsSortMode.None);

                foreach (StartGame mg in allManagers)
                {
                    if (mg.gameObject.name.Contains("UI Manager"))
                    {
                        gameManager = mg;
                        break;
                    }
                }

                // Activates the game over canvas overlay
                if (gameManager != null)
                {
                    gameManager.TriggerGameOverSequence();
                }
            }
        }
    }

    // Temporary collision bypass window to handle frames-per-second hit spacing
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        yield return new WaitForSeconds(1);
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
}










