using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;

    public enum AudioChannel { Master, Sfx };
    AudioSource audioSource;

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }

    Transform audioListener;
    Transform playerT;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        audioListener = FindObjectOfType<AudioListener>().transform;
       

        if (FindObjectOfType<Player>() != null)
        {
            playerT = FindObjectOfType<Player>().transform;
        }
        
        audioSource = GetComponent<AudioSource>();

        masterVolumePercent = PlayerPrefs.GetFloat("master vol", 0.5f);
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 0.5f);
    }

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    void Update()
    {
        if(playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;

            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
        }

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlaySound(AudioClip clip, Vector3 pos, bool isPlayer)
    {
        if(clip != null)
        {
            if (isPlayer)
            {
                audioSource.PlayOneShot(clip, sfxVolumePercent * masterVolumePercent);
            }

            else
                AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }
}
