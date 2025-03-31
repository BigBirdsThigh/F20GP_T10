using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    private int health = 6;
    private int keyCount = 0;
    public TMP_Text keyText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void increaseKeyCount()
    {
        keyCount++;
        keyText.text = (keyCount + "/3");
    }
}
