using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    public Canvas UI;
    public Image heart1, heart2, heart3;

    public Sprite fullheart, halfheart, emptyheart;

    private float health;
    private float prevHealth;

    void Start()
    {
        prevHealth = health;

        // Find the UI Canvas by tag
        GameObject UIObject = GameObject.FindWithTag("UI");
        if (UIObject != null)
        {
            UI = UIObject.GetComponent<Canvas>();
            
            //  search for heart UI elements inside the Canvas
            foreach (Image img in UI.GetComponentsInChildren<Image>(true)) // 'true' includes inactive objects
            {
                if (img.CompareTag("heart1")) heart1 = img;
                else if (img.CompareTag("heart2")) heart2 = img;
                else if (img.CompareTag("heart3")) heart3 = img;
            }
        }
        else
        {
            Debug.LogError("UI Canvas not found with tag 'UI'");
        }

        // Check assignments
        Debug.Log($"Heart1: {heart1}, Heart2: {heart2}, Heart3: {heart3}");
    }

    void Update()
    {
        health = gameObject.GetComponent<Health>().getHealth();
        if (health != prevHealth)
        {
            switch (health)
            {
                case 0:
                    heart1.sprite = emptyheart;
                    heart2.sprite = emptyheart;
                    heart3.sprite = emptyheart;
                    break;
                case 1:
                    heart1.sprite = halfheart;
                    heart2.sprite = emptyheart;
                    heart3.sprite = emptyheart;
                    break;
                case 2:
                    heart1.sprite = fullheart;
                    heart2.sprite = emptyheart;
                    heart3.sprite = emptyheart;
                    break;
                case 3:
                    heart1.sprite = fullheart;
                    heart2.sprite = halfheart;
                    heart3.sprite = emptyheart;
                    break;
                case 4:
                    heart1.sprite = fullheart;
                    heart2.sprite = fullheart;
                    heart3.sprite = emptyheart;
                    break;
                case 5:
                    heart1.sprite = fullheart;
                    heart2.sprite = fullheart;
                    heart3.sprite = halfheart;
                    break;
                case 6:
                    heart1.sprite = fullheart;
                    heart2.sprite = fullheart;
                    heart3.sprite = fullheart;
                    break;
            }
            prevHealth = health;
        }
    }
}
