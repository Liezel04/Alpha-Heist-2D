using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Output Components")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music Tracks")]
    [SerializeField] private AudioClip mainThemeAndJungleMusic;
    [SerializeField] private AudioClip volcanicScaryMusic;

    [Header("Character Sound Effects")]
    [SerializeField] private AudioClip meatMunchSound;
    [SerializeField] private AudioClip fireballCastSound;
    [SerializeField] private AudioClip enemyAttackSound;
    [SerializeField] private AudioClip playerDyingSound;

    [Header("Portal Sound Effects")]
    [SerializeField] private AudioClip portalOpenSound;
    [SerializeField] private AudioClip portalTeleportSound;

    private void Awake()
    {
        // Enforces singleton instance patterns across active scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Automatically launches the main level background loop on startup
        PlayMusicLoop(mainThemeAndJungleMusic, 1.0f);
    }

    // Handles track caching and audio checks before starting a looping music clip
    public void PlayMusicLoop(AudioClip clip, float volume)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Changes the active music source volume level manually
    public void SetMusicVolume(float targetVolume)
    {
        if (musicSource != null) musicSource.volume = targetVolume;
    }

    // Stops overlapping interface audio and swaps background music to the volcano map track
    public void TransitionToVolcanoMusic()
    {
        StopAllSFX();
        Debug.Log("[AUDIO]: Loading Level 2 environment. Switching background track.");
        PlayMusicLoop(volcanicScaryMusic, 0.8f);
    }

    // Fires a single sound clip instance overlay on the SFX mixer tracking bus
    public void PlayOneShotSFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Forces the sound effects channel source to stop all active playback tracks instantly
    public void StopAllSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    // Public encapsulation helpers called by external script engines safely
    public void PlayMunchNoise() => PlayOneShotSFX(meatMunchSound);
    public void PlayFireballNoise() => PlayOneShotSFX(fireballCastSound);
    public void PlayEnemyAttackNoise() => PlayOneShotSFX(enemyAttackSound);
    public void PlayPlayerDyingNoise() => PlayOneShotSFX(playerDyingSound);
    public void PlayPortalOpenNoise() => PlayOneShotSFX(portalOpenSound);
    public void PlayPortalTeleportNoise() => PlayOneShotSFX(portalTeleportSound);
}





