using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueController : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private float displayDuration = 10f;

    private void Start()
    {
        // Starts the countdown timer when the scene loads
        StartCoroutine(AutomatedCountdownRoutine());
    }

    // Coroutine that waits for the duration before advancing the scene index
    private IEnumerator AutomatedCountdownRoutine()
    {
        yield return new WaitForSeconds(displayDuration);

        // Advances to the next level in the build settings sequence
        int gameplaySceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(gameplaySceneIndex);
    }
}


