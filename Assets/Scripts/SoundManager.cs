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

    [SerializeField] private List<AudioClip> soundtrack;

    /// <summary>
    /// A reference to the sfx Audiosource
    /// </summary>
    private AudioSource sfxSource;

    /// <summary>
    /// A dictionary for all the SFX clips
    /// </summary>
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    [SerializeField] private PlayerController player;

    private PlayerStates.MovementState lastPlayerState;

    private bool playedJumpSound = false;

    // Use this for initialization
    void Start()
    {
        //Find the SFX and Music Sources
        sfxSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        musicSource = gameObject.transform.GetChild(1).GetComponent<AudioSource>();


        //Instantiates the SFXClips array by loading all audioclips from the assetfolder
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/SFX") as AudioClip[];

        //Stores all the audio clips
        foreach (AudioClip clip in clips)
        {
            sfxClips.Add(clip.name, clip);
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

        if (musicSource.isPlaying) return;
        musicSource.clip = soundtrack[SceneManager.GetActiveScene().buildIndex];
        musicSource.Play();
    }

    /// <summary>
    /// Plays an sfx sound
    /// </summary>
    /// <param name="name">The name of the sound file</param>
    public void PlaySFX(string name)
    {
        //Plays the clip once
        sfxSource.PlayOneShot(sfxClips[name]);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    private bool SFXPlaying()
    {
        return sfxSource.isPlaying;
    }

    private void ObservePlayer()
    {
        switch (player.State)
        {
            case PlayerStates.MovementState.idle:
                break;
            case PlayerStates.MovementState.moving:
                if (!SFXPlaying())
                {
                    PlaySFX("Walking");
                    playedJumpSound = false;

                }
                break;
            case PlayerStates.MovementState.crouching:
                break;
            case PlayerStates.MovementState.sliding:
                if (lastPlayerState != PlayerStates.MovementState.sliding)
                {
                    PlaySFX("Sliding");
                    playedJumpSound = false;

                }
                break;
            case PlayerStates.MovementState.climbingLadder:
                if (!SFXPlaying())
                {
                    PlaySFX("Ladder");
                    playedJumpSound = false;
                }

                break;
            case PlayerStates.MovementState.wallRunning:
                if (lastPlayerState != PlayerStates.MovementState.wallRunning)
                {
                    PlaySFX("Wallrunning");
                    playedJumpSound = false;

                }
                break;
            case PlayerStates.MovementState.grabbedLedge:
                break;
            case PlayerStates.MovementState.climbingLedge:
                break;
            case PlayerStates.MovementState.vaulting:
                break;
            case PlayerStates.MovementState.hookShotThrowing:
                if (lastPlayerState != PlayerStates.MovementState.hookShotThrowing)
                {
                    PlaySFX("Hookshot");
                    playedJumpSound = false;

                }
                break;
            case PlayerStates.MovementState.hookShotFlying:
                break;
        }


        if (player.playerInput.Jump() && !playedJumpSound)
        {
            switch (Random.Range(1, 4))
            {
                case 1:
                    PlaySFX("Jump1");
                    break;
                case 2:
                    PlaySFX("Jump2");
                    break;
                case 3:
                    PlaySFX("Jump3");
                    break;
            }
            playedJumpSound = true;
        }
        lastPlayerState = player.State;
    }
}