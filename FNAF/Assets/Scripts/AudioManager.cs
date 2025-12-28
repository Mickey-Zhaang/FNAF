using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource animatronicSource;
    [SerializeField] private AudioSource jumpscareSource;

    [Header("Ambient Sounds")]
    [SerializeField] private AudioClip ambientOffice;
    [SerializeField] private AudioClip powerOut;

    [Header("Animatronic Sounds")]
    [SerializeField] private List<SoundClip> bonnieSounds = new List<SoundClip>();
    [SerializeField] private List<SoundClip> chicaSounds = new List<SoundClip>();
    [SerializeField] private List<SoundClip> freddySounds = new List<SoundClip>();
    [SerializeField] private List<SoundClip> foxySounds = new List<SoundClip>();

    [Header("Jumpscare Sounds")]
    [SerializeField] private AudioClip bonnieJumpscare;
    [SerializeField] private AudioClip chicaJumpscare;
    [SerializeField] private AudioClip freddyJumpscare;
    [SerializeField] private AudioClip foxyJumpscare;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip cameraStatic;
    [SerializeField] private AudioClip doorClose;
    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip lightOn;
    [SerializeField] private AudioClip lightOff;

    [Header("Other Sounds")]
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip nightCompleteSound;
    [SerializeField] private AudioClip phoneCall;

    private Dictionary<string, List<SoundClip>> animatronicSoundDict = new Dictionary<string, List<SoundClip>>();

    private void Awake()
    {
        // Create audio sources if they don't exist
        if (ambientSource == null)
        {
            GameObject ambientObj = new GameObject("AmbientAudioSource");
            ambientObj.transform.SetParent(transform);
            ambientSource = ambientObj.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.spatialBlend = 0f; // 2D sound
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXAudioSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.spatialBlend = 0f;
        }

        if (animatronicSource == null)
        {
            GameObject animObj = new GameObject("AnimatronicAudioSource");
            animObj.transform.SetParent(transform);
            animatronicSource = animObj.AddComponent<AudioSource>();
            animatronicSource.spatialBlend = 0f;
        }

        if (jumpscareSource == null)
        {
            GameObject jumpObj = new GameObject("JumpscareAudioSource");
            jumpObj.transform.SetParent(transform);
            jumpscareSource = jumpObj.AddComponent<AudioSource>();
            jumpscareSource.spatialBlend = 0f;
            jumpscareSource.volume = 1f;
        }

        InitializeSoundDictionary();
    }

    private void Start()
    {
        PlayAmbientSound();
    }

    private void InitializeSoundDictionary()
    {
        animatronicSoundDict["Bonnie"] = bonnieSounds;
        animatronicSoundDict["Chica"] = chicaSounds;
        animatronicSoundDict["Freddy"] = freddySounds;
        animatronicSoundDict["Foxy"] = foxySounds;
    }

    public void PlayAmbientSound()
    {
        if (ambientSource != null && ambientOffice != null)
        {
            ambientSource.clip = ambientOffice;
            ambientSource.Play();
        }
    }

    public void PlayPowerOutSound()
    {
        if (sfxSource != null && powerOut != null)
        {
            sfxSource.PlayOneShot(powerOut);
        }
    }

    public void PlayAnimatronicSound(string animatronicName, int soundIndex = -1)
    {
        if (!animatronicSoundDict.ContainsKey(animatronicName))
            return;

        List<SoundClip> sounds = animatronicSoundDict[animatronicName];
        if (sounds == null || sounds.Count == 0)
            return;

        SoundClip soundToPlay;
        if (soundIndex >= 0 && soundIndex < sounds.Count)
        {
            soundToPlay = sounds[soundIndex];
        }
        else
        {
            soundToPlay = sounds[Random.Range(0, sounds.Count)];
        }

        if (animatronicSource != null && soundToPlay.clip != null)
        {
            animatronicSource.PlayOneShot(soundToPlay.clip, soundToPlay.volume);
        }
    }

    public void PlayJumpscareSound(string animatronicName)
    {
        AudioClip jumpscare = null;

        switch (animatronicName)
        {
            case "Bonnie":
                jumpscare = bonnieJumpscare;
                break;
            case "Chica":
                jumpscare = chicaJumpscare;
                break;
            case "Freddy":
                jumpscare = freddyJumpscare;
                break;
            case "Foxy":
                jumpscare = foxyJumpscare;
                break;
        }

        if (jumpscareSource != null && jumpscare != null)
        {
            jumpscareSource.PlayOneShot(jumpscare);
        }
    }

    public void PlayFoxySound(int stage)
    {
        if (foxySounds != null && stage >= 0 && stage < foxySounds.Count)
        {
            if (animatronicSource != null && foxySounds[stage].clip != null)
            {
                animatronicSource.PlayOneShot(foxySounds[stage].clip, foxySounds[stage].volume);
            }
        }
    }

    public void PlayFoxyRunSound()
    {
        PlayJumpscareSound("Foxy");
    }

    public void PlayButtonClick()
    {
        if (sfxSource != null && buttonClick != null)
        {
            sfxSource.PlayOneShot(buttonClick);
        }
    }

    public void PlayCameraStatic()
    {
        if (sfxSource != null && cameraStatic != null)
        {
            sfxSource.PlayOneShot(cameraStatic);
        }
    }

    public void PlayDoorClose()
    {
        if (sfxSource != null && doorClose != null)
        {
            sfxSource.PlayOneShot(doorClose);
        }
    }

    public void PlayDoorOpen()
    {
        if (sfxSource != null && doorOpen != null)
        {
            sfxSource.PlayOneShot(doorOpen);
        }
    }

    public void PlayLightOn()
    {
        if (sfxSource != null && lightOn != null)
        {
            sfxSource.PlayOneShot(lightOn);
        }
    }

    public void PlayLightOff()
    {
        if (sfxSource != null && lightOff != null)
        {
            sfxSource.PlayOneShot(lightOff);
        }
    }

    public void PlayGameOverSound()
    {
        if (sfxSource != null && gameOverSound != null)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }
    }

    public void PlayNightCompleteSound()
    {
        if (sfxSource != null && nightCompleteSound != null)
        {
            sfxSource.PlayOneShot(nightCompleteSound);
        }
    }

    public void PlayPhoneCall()
    {
        if (sfxSource != null && phoneCall != null)
        {
            sfxSource.PlayOneShot(phoneCall);
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
    }

    public void SetAmbientVolume(float volume)
    {
        if (ambientSource != null)
        {
            ambientSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}

