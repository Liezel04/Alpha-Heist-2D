// ============================================================================
// System: Automated Audio Event Dispatcher Matrix
// Component: SceneAudioTrigger.cs
// Architect: Magda Liezel Goosen
// Description: Monitors active scene levels to scale volumes or switch tracks.
// ============================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAudioTrigger : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnLevelTransitionLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLevelTransitionLoaded;
    }

    private void OnLevelTransitionLoaded(Scene loadedScene, LoadSceneMode loadingMode)
    {
        if (AudioManager.Instance == null) return;

        switch (loadedScene.buildIndex)
        {
            case 0: // Main Menu Scene
                AudioManager.Instance.SetMusicVolume(1.0f);
                break;

            case 1: // Instructions Prologue Screen
                AudioManager.Instance.SetMusicVolume(0.6f); // Lowers slightly for story reading!
                break;

            case 2: // adventuretime (Your Level 1 Jungle map!)
                AudioManager.Instance.SetMusicVolume(0.4f); // Softer ambient sound level during gameplay!
                break;

            case 3: // Level 2 Volcanic Lava Peaks
                AudioManager.Instance.TransitionToVolcanoMusic(); // Instantly swaps to your scary volcano theme loop!
                break;
        }
    }
}


