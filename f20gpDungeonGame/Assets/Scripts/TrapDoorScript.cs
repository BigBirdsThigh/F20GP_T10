using UnityEngine;

public class TrapDoorScript : MonoBehaviour
{

    private KeyScript ks;
    private UI_menus ui;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ks = GameObject.FindGameObjectWithTag("key").GetComponent<KeyScript>();
        ui = FindObjectOfType<UI_menus>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (ks.getKeyCount() == 3)
            {
                Debug.Log("YOU WIN!");
                ui.ShowWinLose(true);
            }
            else
            {
                //tell player they need more keys.
            }
        }
    }
}
