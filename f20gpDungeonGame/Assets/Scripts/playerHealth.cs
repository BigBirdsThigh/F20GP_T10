using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    //right, middle, left. Heart1 is 1hp or 0.5hp
    public Image heart1, heart2, heart3;

    public Sprite fullheart, halfheart, emptyheart;

    private int health;
    private int prevHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevHealth = health;

        //flip the hearts so it looks better teehee
    }

    // Update is called once per frame
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
