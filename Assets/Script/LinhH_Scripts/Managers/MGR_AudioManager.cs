// 
// Member: LinhH
// Date: 29/11/2025


using UnityEngine;
using UnityEngine.Audio;


public class MGR_AudioManager : MonoBehaviour
{
    public static MGR_AudioManager Instance;

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] public AudioMixer audioMixer;


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        LoadAudioSetting();
    }


    public void PlaySFX(AudioClip sfx)
    {
        sfxAudioSource.PlayOneShot(sfx);
    }


    public void ChangeMusicVolume(float value)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(value) * 20);
    }


    public void ChangeSfxVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }


    private void LoadAudioSetting()
    {
        GameConfig gameConfig = GameConfig.LoadGameConfig();

        ChangeMusicVolume(gameConfig.musicVolume);
        ChangeSfxVolume(gameConfig.sfxVolume);
    }
}
