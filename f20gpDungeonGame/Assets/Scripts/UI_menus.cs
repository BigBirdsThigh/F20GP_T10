using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_menus : MonoBehaviour
{
    //drag in the menus from scene into these variables
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject pauseScreen;

    //audio
    private AudioSource audioSource;
    private AudioSource menuSound;

    public AudioClip hoverSound;
    public AudioClip slashSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    bool hasPlayedWinLoseSound = false; //flag to stop win/lose sound from replaying constantly

    bool hasWon = false; //makes the win menu appear instead of the lose menu
    bool isGameEnded = false; //make this true if the player either wins or loses
    bool isPaused = false; //if the game has or hasnt been paused

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get audiosource from this manager
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSource = audioSources[0]; //first audio source
        menuSound = audioSources[1]; //second audio source

        //hide all screens on loading into the scene
        pauseScreen.SetActive(false);
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PauseMenuToggle();
        Menu_PauseMenu();

        if(isGameEnded)
        {
            ShowWinLose(true);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            //lose screen test
            isGameEnded = true;
            hasWon = false;
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            //win screen test
            isGameEnded = true;
            hasWon = true;
        }
    }

    public void ShowWinLose(bool hasWon)
    {
        if(!hasPlayedWinLoseSound)
        {
            if(hasWon)
            {
                menuSound.PlayOneShot(winSound);
                winScreen.SetActive(true);
            }
            else {
                menuSound.PlayOneShot(loseSound);
                loseScreen.SetActive(true);
            }
            Time.timeScale = 0f; //freeze the game
            hasPlayedWinLoseSound = true; //sound has played out
        }
    }


    void PauseMenuToggle()
    {
        //if the game hasnt ended, and the player presses the escape key, and its not already paused, switch pause boolean
        if (!isGameEnded && Input.GetKeyDown(KeyCode.Escape))
        {   
            if (isPaused == true) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            isPaused = !isPaused;
        }
    }

    void Menu_PauseMenu()
    {
        //if only isPaused is true
        if(isPaused)
        {
            pauseScreen.SetActive(true); //show pause menu
            Time.timeScale = 0f; //freeze the game
        }
        else {
            pauseScreen.SetActive(false); //hide pause menu
            Time.timeScale = 1f; //unfreeze the game
        }
    }

    public void Button_ReturnToGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        Time.timeScale = 1f; //unfreeze the game
        PlaySlash();
        pauseScreen.SetActive(false); //hide pause menu
    }

    public void Button_ReturnToMainMenu()
    {
        Time.timeScale = 1f; //unfreeze the game
        PlaySlash();
        SceneManager.LoadScene("MainMenu"); //go back to the main menu
    }

    //audio stuff
    private void PlaySlash()
    {
        audioSource.PlayOneShot(slashSound);
    }

    public void OnHoverSword()
    {
        audioSource.PlayOneShot(hoverSound);
    }
    
    /*
        Victory and Defeat Soundbites by Quazy1 -- https://freesound.org/s/778560/ -- License: Attribution 4.0
    */
}
