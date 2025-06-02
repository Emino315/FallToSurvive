using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameVictory : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen; 
    private Slime[] allSlimes;
    private Mushroom[] allMushrooms;
    private int nbSlimes;
    public static GameVictory Instance { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        allSlimes = FindObjectsOfType<Slime>();
        allMushrooms = FindObjectsOfType<Mushroom>();
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Level1") {
            nbSlimes = allSlimes.Length; }
        else
        {
            nbSlimes = allMushrooms.Length;
        }
    }

    public void addOneDead()
    {
        nbSlimes = nbSlimes -1;
        CheckVictory();
    }

    public IEnumerator ShowVictoryScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        victoryScreen.SetActive(true);
        Time.timeScale = 0f; 
    }   

    public void CheckVictory()
    {
        if (nbSlimes > 0) return;
        StartCoroutine(ShowVictoryScreenAfterDelay(1.5f));   
    }


}
