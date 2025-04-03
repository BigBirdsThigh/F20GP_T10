using UnityEngine;

public class PlayerDie : MonoBehaviour, IKillable
{
    private UI_menus menu;
    private bool invulnerable = false;

    void Update()
    {
        // Press H to toggle invulnerability
        if (Input.GetKeyDown(KeyCode.H))
        {
            invulnerable = !invulnerable;
            Debug.Log("Invulnerability: " + (invulnerable ? "ON" : "OFF"));
        }
    }

    public void Die()
    {
        if (invulnerable)
        {
            Debug.Log("Invulnerable! Player death ignored.");
            return;
        }

        menu = FindObjectOfType<UI_menus>();
        if (menu != null)
        {
            menu.ShowWinLose(false);
        }

        Destroy(gameObject);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
