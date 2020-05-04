using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all the sounds, inherits from singleton to make the class into a singleton
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    /// <summary>
    /// A reference to the audios source
    /// </summary>
    private AudioSource musicSource;

    [SerializeField] private List<AudioClip> soundtrack = null;

    /// <summary>
    /// A reference to the sfx Audiosource
    /// </summary>
    private AudioSource sfxSource;

    /// <summary>
    /// A dictionary for all the SFX clips
    /// </summary>
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private Slider sfxSlider = null;

    [SerializeField] private PlayerController player = null;

    private PlayerStates.MovementState lastPlayerState = PlayerStates.MovementState.Idle;

    private bool playedJumpSound = false;

    // Use this for initialization
    void Start()
    {
        //Find the SFX and Music Sources
        sfxSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        musicSource = gameObject.transform.GetChild(1).GetComponent<AudioSource>();

        LoadVolume();

        //Instantiates the SFXClips array by loading all audioclips from the assetfolder
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/SFX") as AudioClip[];

        //Stores all the audio clips
        foreach (AudioClip clip in clips)
        {
            sfxClips.Add(clip.name, clip);
        }

        if (musicSlider)
        {
            musicSlider.onValueChanged.AddListener(delegate { UpdateVolume(); });
            sfxSlider.onValueChanged.AddListener(delegate { UpdateVolume(); });
        }

        //Play the music of the level the player is playing
        musicSource.clip = soundtrack[SceneManager.GetActiveScene().buildIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            ObservePlayer();
        }

        if (musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = soundtrack[SceneManager.GetActiveScene().buildIndex];
        musicSource.Play();
    }

    /// <summary>
    /// Plays an sfx sound
    /// </summary>
    /// <param name="trackName">The name of the sound file</param>
    public void PlaySFX(string trackName)
    {
        StopSFX();
        //Plays the clip once
        sfxSource.PlayOneShot(sfxClips[trackName]);
    }

    private void StopSFX()
    {
        sfxSource.Stop();
    }

    private bool SFXPlaying()
    {
        return sfxSource.isPlaying;
    }

    /// <summary>
    /// Updates the volumes according to the sliders
    /// </summary>
    private void UpdateVolume()
    {
        //Sets the music volume
        musicSource.volume = musicSlider.value;

        //Sets the sfx volume
        sfxSource.volume = sfxSlider.value;

        //Saves the values
        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }

    /// <summary>
    /// Loads the volumes 
    /// </summary>
    private void LoadVolume()
    {
        //Loads the sfx volume
        sfxSource.volume = PlayerPrefs.GetFloat("SFX", 0.75f);
        //Loads the music volumes
        musicSource.volume = PlayerPrefs.GetFloat("Music", 0.5f);

        //Updates the sliders
        if (!musicSlider) return;
        musicSlider.value = musicSource.volume;
        sfxSlider.value = sfxSource.volume;
    }

    private void ObservePlayer()
    {
        switch (player.State)
        {
            case PlayerStates.MovementState.Moving:
                playedJumpSound = false;
                break;
            case PlayerStates.MovementState.Sliding:
                if (lastPlayerState != PlayerStates.MovementState.Sliding)
                {
                    PlaySFX("Slide");
                    playedJumpSound = false;

                }
                break;
            case PlayerStates.MovementState.Vaulting:
                if (lastPlayerState != PlayerStates.MovementState.Vaulting)
                {
                    PlaySFX($"Vault{Random.Range(1, 4).ToString()}");
                    playedJumpSound = false;
                }
                break;
            case PlayerStates.MovementState.HookShotThrowing:
                if (lastPlayerState != PlayerStates.MovementState.HookShotThrowing)
                {
                    PlaySFX("Hookshot");
                    playedJumpSound = false;
                }
                break;
        }

        //Prevent the jump noise playing more than once
        if (player.playerInput.Jump() && !playedJumpSound)
        {
            PlaySFX($"Jump{Random.Range(1, 5).ToString()}");
            playedJumpSound = true;
        }

        if (player.GrowShrinkState.ToString().Contains("growing"))
        {
            PlaySFX($"Growing{Random.Range(1, 7).ToString()}");
        }
        else if (player.GrowShrinkState.ToString().Contains("shrinking"))
        {
            PlaySFX($"Shrinking{Random.Range(1, 7)}");
        }
        lastPlayerState = player.State;
    }
}