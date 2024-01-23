//////////////////////////////////////////////
//Assignment/Lab/Project: 3D PacMan
//Name: Shelby Leggott
//Section: SGD285.4171
//Instructor: Aurore Locklear
//Date: 01/21/2024
/////////////////////////////////////////////

// GameManager Script: For managing general gameplay loop mechanics

using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform[] pellets;

    // can access these variables but cannot set the score in game, will be done automatically
    public int score { get; private set; }
    public int lives { get; private set; }
    public int livesLostOnRound { get; private set; }
    public int levelCounter { get; private set; }
    public int pelletCounter = 0;

    // Audio variables
    int pelletInterval = 0; // pacman has two different waka waka sounds that will play based on if the int is set to 0 or 1
    [SerializeField] private AudioClip pacmanStartupSFX; // assigned in editor
    [SerializeField] private AudioClip pacmanMonch1SFX; // assigned in editor
    [SerializeField] private AudioClip pacmanMonch2SFX; // assigned in editor
    [SerializeField] private AudioClip pacmanEatenSFX; // assigned in editor
    [SerializeField] private AudioClip pacmanGhostSirenSFX; // assigned in editor
    [SerializeField] private AudioSource audioSource1; // assigned in editor - used for ghost sirens (CAN ALSO BE USED FOR GHOST FRIGHTENED MODE)
    [SerializeField] private AudioSource audioSource2; // assigned in editor - used for startup sounds, pacman waka wakas, and death noise

    // UI variables
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text pelletsText;
    [SerializeField] private GameObject gameOverPanel;
    private int pelletsInScene = 0;

    private int behaviorSwapCount = 0;
    private bool gameOver = false;

    // can change this variable to decide who has priority in pellet counting to leave ghost home
    public int priority = 0;

    private void Awake()
    {
        audioSource1 = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (gameOver)
        {
            // check if any key is pressed to start a new game (or Escape to quit the game)
            if (lives <= 0 && Input.anyKeyDown && !(Input.GetKeyDown(KeyCode.Escape)))
            {
                StartCoroutine(StartGame());
            }
            else if (lives <= 0 && Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    // Function will be called at the start of a new game
    private IEnumerator StartGame()
    {
        gameOver = false;
        gameOverPanel.SetActive(false);
        pacman.gameObject.SetActive(false);
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }

        if (audioSource2 != null && pacmanStartupSFX != null) 
        {
            audioSource2.PlayOneShot(pacmanStartupSFX, 1f);
        }

        yield return new WaitForSeconds(pacmanStartupSFX.length);

        levelCounter = 0;
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    // Function is called when pacman runs out of lives
    private void GameOver()
    {
        // set pacman and ghosts to inactive in new stage
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }
        pacman.gameObject.SetActive(false);

        gameOver = true;
        gameOverPanel.SetActive(true);
    }

    // Function for setting the game state when a new wave of pacman has started (on game start or when all pellets are collected)
    private void NewRound()
    {
        pelletsInScene = 0;

        // Set pellets back to active/visible again
        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
            pelletsInScene++;
        }
        levelCounter++; // increment the level counter starting at 1 by default
        livesLostOnRound = 0;
        behaviorSwapCount = 0;
        pelletsText.text = pelletsInScene.ToString();

        ResetState();
    }

    // called when pacman dies (pacman and ghosts are reset to their spawn points)
    private void ResetState()
    {
        // set pacman and ghosts back to active in new stage
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }
        pacman.ResetState();

        priority = 0;
        pelletCounter = 0;
        StartCoroutine(ScatterToChase());

        if (audioSource1 != null && pacmanGhostSirenSFX != null)
        {
            audioSource1.clip = pacmanGhostSirenSFX;
            audioSource1.loop = true;
            audioSource1.Play();
        }
    }

    // Function for changing the current score of the game
    private void SetScore(int inputtedScore)
    {
        score = inputtedScore;
        scoreText.text = "SCORE: " + score.ToString();
    }

    // Function for changing the current lives of pacman
    private void SetLives(int inputtedLives) 
    { 
        lives = inputtedLives;
        livesText.text = lives.ToString();
    }

    // Function for when pacman is eaten
    public void PacmanEaten()
    {
        audioSource1.Stop();
        audioSource2.PlayOneShot(pacmanEatenSFX, 1f);
        StopAllCoroutines();

        // hide pacman when eaten
        pacman.gameObject.SetActive(false);

        SetLives(lives - 1);
        livesLostOnRound++;

        // Reset the game state if pacman still has lives remaining, else player loses
        if (lives > 0) 
        {
            Invoke(nameof(ResetState), 2.0f);
        }
        else
        {
            GameOver();
        }
    }

    // Handles the event that occurs when a pellet is eaten
    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(score + pellet.points);
        pelletsInScene--;
        pelletsText.text = pelletsInScene.ToString();
        pelletCounter++;

        if (pelletInterval == 0)
        {
            audioSource2.PlayOneShot(pacmanMonch1SFX, 1f);
            pelletInterval = 1;
        }
        else if (pelletInterval == 1) 
        { 
            audioSource2.PlayOneShot(pacmanMonch2SFX, 1f);
            pelletInterval = 0;
        }

        if (!RemainingPellets())
        {
            audioSource1.Stop();
            pacman.gameObject.SetActive(false); // disable pacman so he cannot be killed after winning the game
            Invoke(nameof(NewRound), 2.0f);
        }
    }

    // returns a bool that checks if any pellets are still in the scene
    private bool RemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) // checks if a pellet is active, is so game continues
            { return true; }
        }
        return false;
    }

    private IEnumerator ScatterToChase()
    {
        float timeTilSwitch = 7f;
        // behavior time for scatter will change to a shorter duration each time the behavior is swapped
        if (behaviorSwapCount >= 2 || levelCounter >= 5)
        {
            timeTilSwitch = 5f; // per original pacman code
        }
        if (levelCounter >= 2 && behaviorSwapCount >= 3)
        {
            timeTilSwitch = 0.0167f; // 1/60th of a second per original pacman code
        }

        yield return new WaitForSeconds(timeTilSwitch);

        // swap all ghosts to chase mode
        for (int i = 0; i < ghosts.Length; i++)
        {
            // only swap ghosts that are currently in scatter mode
            if (ghosts[i].behaviorScript.currentBehavior != GhostBehavior.CurrentBehavior.Home && ghosts[i].behaviorScript.currentBehavior != GhostBehavior.CurrentBehavior.LeavingHome)
            {
                ghosts[i].behaviorScript.currentBehavior = GhostBehavior.CurrentBehavior.Chase;
            }
        }
        StartCoroutine(ChaseToScatter());
    }

    private IEnumerator ChaseToScatter()
    {
        float timeTilSwitch = 20f;
        // behavior time for scatter will change to a shorter duration each time the behavior is swapped
        if (levelCounter >= 2 && levelCounter <= 4 && behaviorSwapCount >= 3)
        {
            timeTilSwitch = 1033f; // per original pacman code
        }
        if (levelCounter >= 5 && behaviorSwapCount >= 3)
        {
            timeTilSwitch = 1037f; // per original pacman code
        }

        yield return new WaitForSeconds(timeTilSwitch);

        // swap all ghosts to scatter mode (if behaviorSwapCount is >= 4, then chase remains indefinitely, per original pacman code)
        if (!(behaviorSwapCount >= 4))
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                // only swap ghosts that are currently in chase mode
                if (ghosts[i].behaviorScript.currentBehavior != GhostBehavior.CurrentBehavior.Home && ghosts[i].behaviorScript.currentBehavior != GhostBehavior.CurrentBehavior.LeavingHome)
                {
                    ghosts[i].behaviorScript.currentBehavior = GhostBehavior.CurrentBehavior.Scatter;
                }
            }
        }

        // increment behavior swap count
        behaviorSwapCount++;
        StartCoroutine(ScatterToChase());
    }
}
