using UnityEngine;

public class Level1_music : MonoBehaviour
{
    AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.backgroundLevel1);
    }
}