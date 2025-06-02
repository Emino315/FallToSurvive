using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] public GameObject gameOverPanel;   
    [SerializeField] public Button restartButton;       
    [SerializeField] public Button exitButton;          
    AudioManager audioManager;


    void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);      
    }
    public void Setup()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
       
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        audioManager.PlayMusic(audioManager.backgroundMenu);
        SceneManager.LoadScene("Game_menu");
        
    }
}
