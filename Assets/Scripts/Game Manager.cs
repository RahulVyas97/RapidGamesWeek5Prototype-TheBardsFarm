using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // Singleton instance
    public GameObject startMenu;  // UI for the start menu
    public Playermovment playerMovement;  // Reference to player movement script
    public CinemachineFreeLook cinemachine;  // Reference to Cinemachine camera
    public Animal animalScript;  // Reference to the animal script
    public AnimalSpawner animalSpawnerScript;  // Reference to the animal spawner script
    public Image FadePanel;  // UI element for fading screen transitions
    public GameObject gameOverPanel;  // UI panel shown when the game is over
    public TextMeshProUGUI timerText;  // TextMeshPro element to display the timer
    public TextMeshProUGUI scoreText;  // TextMeshPro element to display the score
    public TextMeshProUGUI finalScoreText;  // TextMeshPro element to display the final score on Game Over panel
    public int score;  // Game score
    private float timer = 180f;  // Duration of the game in seconds
    private bool timerRunning = false;  // Flag to control the timer

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this instance alive across scenes
        }
        gameOverPanel.SetActive(false);  // Ensure the Game Over panel is initially hidden
    }

    public void StartGame()
    {
        FadeInOut(0, 0.5f);  // Fade from black
        startMenu.SetActive(false);  // Hide the start menu
        cinemachine.enabled = true;  // Enable the camera
        playerMovement.enabled = true;  // Enable player control
        animalScript.enabled = true;  // Enable animal behaviors
        animalSpawnerScript.enabled = true;  // Enable animal spawning
        Cursor.visible = false;  // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor to the center of the screen

        score = 0;  // Reset the score
        UpdateScoreDisplay();  // Update the score display at the start
        timer = 180f;  // Reset the timer to 3 minutes
        timerRunning = true;  // Start the timer countdown
    }

    void Update()
    {
        if (timerRunning)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;  // Decrease the timer
                UpdateTimerDisplay(timer);  // Update the UI display
            }
            else
            {
                timerRunning = false;
                timer = 0;
                EndGame();  // Trigger the end of the game
            }
        }
    }

    void UpdateTimerDisplay(float currentTime)
    {
        currentTime = Mathf.Max(currentTime, 0);  // Ensure the time doesn't go negative
        float minutes = Mathf.FloorToInt(currentTime / 60);  // Calculate the minutes
        float seconds = Mathf.FloorToInt(currentTime % 60);  // Calculate the seconds
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  // Display time in mm:ss format
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = "Score: " + score.ToString();  // Display score
    }

    void EndGame()
    {
        cinemachine.enabled = false;
        playerMovement.enabled = false;
        animalScript.enabled = false;
        animalSpawnerScript.enabled = false;
        gameOverPanel.SetActive(true);  // Show the game over panel
        finalScoreText.text = "Final Score: " + score.ToString();  // Update final score text
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;  // Unlock the cursor for UI interaction
    }

    public void QuitGame()
    {
        FadeInOut(1, 0.5f);  // Fade to black
        StartCoroutine(Quit_Delay());  // Delay the quitting to allow fade
    }

    IEnumerator Quit_Delay()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();  // Quit the application
    }

    public void FadeInOut(float targetAlpha, float duration)
    {
        if (FadePanel != null)
        {
            FadePanel.DOFade(targetAlpha, duration).SetUpdate(true);  // Perform the fade
        }
        else
        {
            Debug.LogError("FadePanel is not assigned in the inspector!");
        }
    }

    public void IncreaseScore()
    {
        score++;
        UpdateScoreDisplay();  // Update the UI whenever the score changes
    }

    public void DecreaseScore()
    {
        score--;
        UpdateScoreDisplay();  // Update the UI whenever the score changes
    }

    public void RestartGame()
    {
        FadeInOut(0, 0.5f);  // Fade from black
        gameOverPanel.SetActive(false);  // Hide the game over panel
        cinemachine.enabled = true;  // Re-enable the camera
        playerMovement.enabled = true;  // Re-enable player movement
        animalScript.enabled = true;  // Re-enable animal behaviors
        animalSpawnerScript.enabled = true;  // Re-enable animal spawning
        Cursor.visible = false;  // Hide the cursor again
        Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor to the center of the screen

        score = 0;  // Reset the score
        UpdateScoreDisplay();  // Update the score display immediately
        timer = 180f;  // Reset the timer to 3 minutes
        timerRunning = true;  // Start the timer countdown
    }
}