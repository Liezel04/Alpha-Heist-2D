using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [Header("Visual Elements")]
    [SerializeField] private GameObject loadingPanel;

    [Header("Timing Configurations")]
    [SerializeField] private float displayTime = 3f;

    private void Awake()
    {
        // Enforces singleton instance visibility parameters across runtime transitions
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Ensures the panel elements are hidden by default on level initialization
        loadingPanel.SetActive(false);
    }

    // Public method interface called by portals or triggers to start the transition pipeline
    public void LoadNextScene()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    // Handles the loading canvas overlay toggle and asynchronous timing delay loops
    private IEnumerator ShowLoadingScreen()
    {
        loadingPanel.SetActive(true);

        yield return new WaitForSeconds(displayTime);

        // Advances build index order sequence by one node cleanly
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

