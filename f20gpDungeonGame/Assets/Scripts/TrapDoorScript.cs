using UnityEngine;

public class TrapDoorScript : MonoBehaviour
{

    private KeyScript ks;
    private UI ui;
    private UI_menus um;

    int keyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ks = GameObject.FindGameObjectWithTag("key").GetComponent<KeyScript>();
        ui = FindObjectOfType<UI>();
        um = FindObjectOfType<UI_menus>();
    }

    // Update is called once per frame
    void Update()
    {
        keyCount = ui.getKeyCount();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("KEY COUNT: " + keyCount);
        Debug.Log("HELLO!");

        if (other.tag == "Player")
        {
            Debug.Log("CHECKING KEYS");
            if (keyCount == 3)
            {
                Debug.Log("YOU WIN!");
                um.ShowWinLose(true);
            }
            else
            {
                //tell player they need more keys.
            }
        }
    }
}
