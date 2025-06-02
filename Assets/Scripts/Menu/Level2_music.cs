using UnityEngine;

public class Level2_music : MonoBehaviour
{
    AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.backgroundLevel2);
    }
}