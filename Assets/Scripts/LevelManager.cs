/// <remarks>
/// Author: Chase Bennett-Hill
/// Date Created: May 2, 2024
/// Bugs: None known at this time.
/// </remarks>
// <summary>
/// This class manages the level, it will keep track of each teams score , the time remaining and the current state of the level
/// </summary>


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    //Note from Palin: I went through and hid the values that don't need to be shown in the inspector anymore
    //I also changed the values to the ones we have decided on
    [Header("Level Management")]
    private int playerScore = 0;
    private int enemyScore = 0;

    public int LevelLength = 1; //How long the level will last in seconds
    [HideInInspector]
    public int secondsRemaining; //The time remaining in the level

    private float delayTime = 5; //The delay time before the level starts

    private int targetScore = 15; //The score needed to win the level

    [Header("Colors")]
    public Color mediumColor;
    public Color criticalColor;

    public GameObject mainCamera;
    public GameObject endGameCamera;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;
    public TextMeshProUGUI playerEndScore;
    public TextMeshProUGUI enemyEndScore;

    public TextMeshProUGUI timerText;

    private float currentTime;
    public static LevelManager instance;
    public GameObject EndScreen;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject DrawPanel;
    public GameObject Trophy;
    public GameObject ContinueButton;
    public Animator ContinueAnimator;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameSettings.currentGameState = GameStates.PreGame;

        playerScoreText.text = playerScore.ToString();
        enemyScoreText.text = enemyScore.ToString();
        timerText.text = delayTime.ToString();
        currentTime = Time.time;
        secondsRemaining = LevelLength;
    }

    // Update is called once per frame
    void Update()
    {
        {
            //Update the timer every second if the round is not over
            if (
                Time.time > currentTime + 1
                && (
                    GameSettings.currentGameState == GameStates.PreGame
                    || GameSettings.currentGameState == GameStates.InGame
                )
            )
            {
                if (currentTime == 5) { PlayAnnouncerSounds.Instance.FinalCountdown(); }
                if (delayTime > 0)
                {
                    currentTime = Time.time;
                    delayTime -= 1;
                    timerText.text = delayTime.ToString();
                }
                else
                {
                    StartCountdown();

                    if (
                        secondsRemaining > 0
                        && playerScore < targetScore
                        && enemyScore < targetScore
                        && GameSettings.currentGameState == GameStates.InGame
                    )
                    {
                        currentTime = Time.time;
                        secondsRemaining -= 1;
                        timerText.text = secondsRemaining.ToString();

                        //Once the timer reaches either threshold, change the color of the text
                        if (secondsRemaining <= 5)
                        {
                            timerText.color = criticalColor;
                        }
                        else if (secondsRemaining <= 10)
                        {
                            timerText.color = mediumColor;
                        }
                    }
                    else
                    {
                        DisplayWinner(CheckForWinner());
                        GameSettings.currentGameState = GameStates.PostGame;
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method will display the winner of the match
    /// </summary>
    /// <param name="winner">The team that won the match</param>
    private void DisplayWinner(string winner)
    {
        mainCamera.SetActive(false);
        endGameCamera.SetActive(true);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Destroy(player);   
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] snowballs = GameObject.FindGameObjectsWithTag("Snowball");
        foreach (GameObject snowball in snowballs)
        {
            Destroy(snowball);
        }
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
            Destroy(turret);
        }

        EndScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(ContinueButton);
        ContinueAnimator.SetTrigger("pointerEnter");

        playerEndScore.text = playerScore.ToString();
        enemyEndScore.text = enemyScore.ToString();
        
        if (winner == "Player")
        {
            PlayAnnouncerSounds.Instance.PlayVictory();
            if(MusicManager.Instance != null)
            {
                MusicManager.Instance.SetTrack("Draw");
            }
            Trophy.SetActive(true);
        }
        else if (winner == "Enemy")
        {
            PlayAnnouncerSounds.Instance.PlayDefeat();
            if(MusicManager.Instance != null)
            {
                MusicManager.Instance.SetTrack("Draw");
            }
            WinPanel.SetActive(false);
            LosePanel.SetActive(true);
            DrawPanel.SetActive(false);
        }
        else
        {
            PlayAnnouncerSounds.Instance.PlayDraw();
            if(MusicManager.Instance != null)
            {
                MusicManager.Instance.SetTrack("Draw");
            }
            WinPanel.SetActive(false);
            LosePanel.SetActive(false);
            DrawPanel.SetActive(true);
        }   
    }

    /// <summary>
    /// This method will check for the winner of the match
    /// </summary>
    /// <returns>The winner as a string</returns>

    private string CheckForWinner()
    {
        if (secondsRemaining <= 0)
        {
            if (playerScore > enemyScore)
            {
                return "Player";
            }
            else if (enemyScore > playerScore)
            {
                return "Enemy";
            }
            else
            {
                return "Draw";
            }
        }
        else
        {
            if (playerScore >= targetScore)
            {
                return "Player";
            }
            else if (enemyScore >= targetScore)
            {
                return "Enemy";
            }
            else
            {
                return "Draw";
            }
        }
    }

    /// <summary>
    /// This method will start the countdown timer
    /// </summary>
    private void StartCountdown()
    {
        GameSettings.currentGameState = GameStates.InGame;
    }

    /// <summary>
    /// This method will update the score of the team that scored
    /// </summary>
    /// <param name="team">The name of the team which will be given points</param>
    public void UpdateScore(string team)
    {
        if (
            GameSettings.currentGameState != GameStates.InGame
            || playerScore == targetScore
            || enemyScore == targetScore
        )
            return;
        if (team == "Player")
        {
            playerScore += 1;
            playerScoreText.text = playerScore.ToString();
        }
        else if (team == "Enemy")
        {
            enemyScore += 1;
            enemyScoreText.text = enemyScore.ToString();
        }
        else
            return;
    }

    /// <summary>
    /// This method will restart the level
    /// </summary>
    public void RestartLevel()
    {
        MusicManager.Instance.SetTrack("Fight");
        SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// This method will return to the main menu
    /// </summary>
    public void Continue()
    {
        MusicManager.Instance.SetTrack("Menu");
        GameSettings.Player2Exists = false;
        SceneManager.LoadScene("MainMenu"); //Commented out until we merge
    }
}
