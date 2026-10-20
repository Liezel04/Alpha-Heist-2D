using UnityEngine;

public class EndPoint : MonoBehaviour
{
    // High-performance collision check framework executed strictly upon physical contact
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifies if the hitting object is designated as the primary player character
        if (collision.CompareTag("Player"))
        {
            // Safely verifies if the global loading screen instance is registered in memory
            if (LoadingScreen.Instance != null)
            {
                // Unloads active maps and executes transition sequence to the next layout index
                LoadingScreen.Instance.LoadNextScene();
            }
            else
            {
                // Fallback diagnostic note fired if structural loading systems are missing
                Debug.LogError("No LoadingScreen instance found in the scene!");
            }
        }
    }
}

