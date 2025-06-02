using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] public Button playButton;       
    [SerializeField] public Button exitButton;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.backgroundMenu);
        playButton.onClick.AddListener(PlayGame);
        exitButton.onClick.AddListener(ExitGame);
        
    }
    public void PlayGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }
    public void ExitGame()
    {
       Application.Quit();
    }

}
