using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioSource ambienceSource;

    public AudioClip hoverSound;
    public AudioClip slashSound;
    public AudioClip ambience;

    //fade panel and how long the fade lasts
    public CanvasGroup fadePanel;
    public float fadeSpeed = 1.5f;

    public GameObject[] uiElements;
    public GameObject[] creditElements;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSource = audioSources[0]; //first audio source
        ambienceSource = audioSources[1]; //second audio source
        PlayAmbience();
        fadePanel.gameObject.SetActive(false); //make it false so we can press buttons
        fadePanel.alpha = 0; //make opacity 0

        //make credits hidden by default
        foreach (var creditElement in creditElements)
        {
            creditElement.SetActive(false);
            CanvasGroup canvasGroup = creditElement.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        PlaySlash();
        StartCoroutine(FadeToBlack());
        //SceneManager.LoadScene("MainGame");
    }

    public void ExitGame()
    {
        PlaySlash();
        Application.Quit();
    }

    public void OnHoverSword()
    {
        audioSource.PlayOneShot(hoverSound);

    }

    public void PlayAmbience()
    {
        ambienceSource.clip = ambience;
        ambienceSource.volume = 0.1f;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    private void PlaySlash()
    {
        audioSource.PlayOneShot(slashSound);
    }

    private IEnumerator FadeToBlack()
    {
        fadePanel.gameObject.SetActive(true);
        float timer = 0f;
        
        while (timer < fadeSpeed)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0, 1, timer / fadeSpeed); //slowly increase alpha
            yield return null;
        }

        fadePanel.alpha = 1;
        SceneManager.LoadScene("MainGame"); //load game after fade
    }

    public void ShowCredits()
    {
        //disable ui elements
        PlaySlash();
        StartCoroutine(TransToCredits());
    }

    public void HideCredits()
    {
        //disable ui elements
        PlaySlash();
        StartCoroutine(TransFromCredits());
    }

    private IEnumerator TransToCredits()
    {
        float timer = 0f;
        //set all credits to active
        foreach (var creditElement in creditElements)
        {
            creditElement.SetActive(true);  //set each credit to active
            CanvasGroup canvasGroup = creditElement.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0; //make transparent
            }
        }

        while (timer < fadeSpeed)
        {
            timer += Time.deltaTime;
            foreach (var uiElement in uiElements)
            {
                uiElement.SetActive(false); //set each element to inactive
                CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeSpeed);
                }
            }
            foreach (var creditElement in creditElements)
            {
                CanvasGroup canvasGroup = creditElement.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeSpeed);
                }
            }
            yield return null;
        }

        //make all elements transparent after fading
        foreach (var uiElement in uiElements)
        {
            CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }
    }

    private IEnumerator TransFromCredits()
    {
        float timer = 0f;
        //set all credits to active
        foreach (var uiElement in uiElements)
        {
            uiElement.SetActive(true);  //set each credit to active
            CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0; //make transparent
            }
        }

        while (timer < fadeSpeed)
        {
            timer += Time.deltaTime;
            foreach (var creditElement in creditElements)
            {
                creditElement.SetActive(false); //set each element to inactive
                CanvasGroup canvasGroup = creditElement.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeSpeed);
                }
            }
            foreach (var uiElement in uiElements)
            {
                CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeSpeed);
                }
            }
            yield return null;
        }

        //make all elements visible
        foreach (var uiElement in uiElements)
        {
            CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
            }
        }
        //make all credits invisible
        foreach (var creditElement in creditElements)
        {
            CanvasGroup canvasGroup = creditElement.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }
    }
}
