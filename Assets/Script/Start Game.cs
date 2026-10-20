using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [Header("Game Over UI Links")]
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        // Resets the simulation clock back to full speed when a scene loads
        Time.timeScale = 1f;

        // Ensures user cursor layers remain completely visible for menu button clicks
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // secures the fallback application exit event hook tracking array
        GameObject quitButtonObj = GameObject.FindGameObjectWithTag("QuitButton");
        if (quitButtonObj != null)
        {
            Button btn = quitButtonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(QuitGame);
            }
        }
    }

    // Handles the primary user navigation pipeline into the initialization loading window
    public void Start_Game()
    {
        Debug.Log("[SYSTEM]: Play button verified. Advancing active scene matrix.");
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    // Terminates the active platform pipeline runtime loop depending on compilation environments
    public void QuitGame()
    {
        Debug.Log("[SYSTEM]: Exit signal active. Initiating application shutdown routines.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }

    // Primary victory endpoint escape sequence used to return to main title screen index 0
    public void ReturnToMenu()
    {
        Debug.Log("[SYSTEM]: Victory screen finished. Loading Main Menu index 0.");
        SceneManager.LoadScene(0);
    }

    // Halts active frame ticks and forces your custom defeat overlay panels completely visible
    public void TriggerGameOverSequence()
    {
        Debug.Log("[SYSTEM]: Player health depleted. Activating defeat screen state updates.");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    // instantly reloads your first gameplay scene by name path string
    public void ReloadLevelOne()
    {
        Debug.Log("[SYSTEM]: Retry command registered. Reloading Level one build asset.");
        SceneManager.LoadScene("Scenes/Level one");
    }
}





















