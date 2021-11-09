using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }

    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;

    public float masterVolumeSFX = 0.1f;
    public float masterVolumeBGM = 0.1f;

    [SerializeField]
    private AudioClip mainBgmAudioClip; 

    [SerializeField]
    private AudioClip endingBgmAudioClip; 

    [SerializeField]
    private AudioClip[] sfxAudioClips;

    Dictionary<string, AudioClip> audioClipsDic = new Dictionary<string, AudioClip>(); //효과음 딕셔너리


    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject); //여러 씬에서 사용할 것.

        bgmPlayer = GameObject.Find("BGMSoundPlayer").GetComponent<AudioSource>();
        sfxPlayer = GameObject.Find("SFXSoundPlayer").GetComponent<AudioSource>();

        foreach (AudioClip audioclip in sfxAudioClips)
        {
            audioClipsDic.Add(audioclip.name, audioclip);
        }
    }

    public void Start()
    {
        PlayBGMSound();
    }

    public void PlaySFXSound(string name, float volume = 0.3f)
    {
        if (audioClipsDic.ContainsKey(name) == false)
        {
            Debug.Log(name + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipsDic[name], volume * masterVolumeSFX);
    }

    //BGM 사운드 재생 : 볼륨을 선택적 매개변수로 지정
    public void PlayBGMSound()
    {
        bgmPlayer.loop = true; //BGM 사운드이므로 루프설정
        bgmPlayer.volume = masterVolumeBGM;

        if (SceneManager.GetActiveScene().name == "MainStory" || SceneManager.GetActiveScene().name == "Title")
        {
            bgmPlayer.clip = mainBgmAudioClip;
            bgmPlayer.Play();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        masterVolumeSFX = volume;
        masterVolumeBGM = volume;

        bgmPlayer.volume = masterVolumeBGM;
        sfxPlayer.volume = masterVolumeSFX;
    }
}

