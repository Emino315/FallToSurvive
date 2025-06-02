using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Sources ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    [SerializeField] public AudioClip backgroundLevel1;
    [SerializeField] public AudioClip backgroundMenu;
    [SerializeField] public AudioClip backgroundLevel2;

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}