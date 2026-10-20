using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 3f;
    private float currentHealth;

    [Header("Freeze Settings")]
    [SerializeField] private float freezeDuration = 1f;

    private EnemyAI enemyAI;

    private void Awake()
    {
        // Cache internal AI navigation script references on startup
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogWarning("EnemyAI component not found on this enemy!");
        }
    }

    // Handles fireball impact triggers to process status changes
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(HitFreeze());
        }
        else
        {
            // Fallback safety to stop movement tracking loops if needed
            if (enemyAI != null) enemyAI.enabled = false;
        }
    }

    // Temporarily disrupts runtime AI cycles to simulate physical weapon impact weight responses
    private IEnumerator HitFreeze()
    {
        if (enemyAI != null)
            // Pauses navigation cycles immediately
            enemyAI.enabled = false;

        yield return new WaitForSeconds(freezeDuration);

        if (enemyAI != null)
            // Restores active locomotion tracking
            enemyAI.enabled = true;
    }
}

