using UnityEngine;

public class PlayerDie : MonoBehaviour, IKillable
{
    private UI_menus menu;
    // this script solely just handles player death and loss condition
    public void Die()
    {
        menu = FindObjectOfType<UI_menus>();
        menu.ShowWinLose(false);
        Destroy(gameObject); // destroy this object after triggering UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
