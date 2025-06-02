using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryGame : MonoBehaviour
{
    [SerializeField] public GameObject gameVictoryPanel;   
    [SerializeField] public Button restartButton;      
    [SerializeField] public Button exitButton;
    [SerializeField] public Button NextLevelButton;

     void Start()
    {
        gameVictoryPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);
        NextLevelButton.onClick.AddListener(NextLevel);
    }

    public void Setup()
    {
        gameVictoryPanel.SetActive(true);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game_menu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level2", LoadSceneMode.Single);
    }

}
