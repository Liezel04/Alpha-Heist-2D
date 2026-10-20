using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float Damage = 1f;

    private bool hit;
    private float direction;
    private BoxCollider2D boxCollider;
    private Animator animator;

    private void Awake()
    {
        // Cache internal component references on projectile initialization
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Halts translation vectors immediately if a collision impact registers
        if (hit) return;

        // Applies linear horizontal translation ticks over runtime frames
        float move = speed * Time.deltaTime * direction;
        transform.Translate(move, 0, 0);
    }

    // Handles impact triggers for environmental elements and active hazards
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;

        // Fires the explosion impact state parameters inside the animation controller
        animator.SetTrigger("Explode");

        // Verifies if the impacted boundary is designated as an enemy hazard
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Damage);
            }
            else
            {
                Debug.LogWarning("No EnemyHealth component found on " + collision.name);
            }
        }

        StartCoroutine(DeactivateAfterAnimation());
    }

    // Initializes trajectory values and bounds layout vectors dynamically upon generation
    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        // Flips the transform local layout boundaries to match active travel vectors
        Vector3 scale = transform.localScale;
        if (Mathf.Sign(scale.x) != _direction)
            scale.x = -scale.x;
        transform.localScale = scale;
    }

    // Delays disabling the object instance loop to allow visual effects to finish playing out
    private IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}

