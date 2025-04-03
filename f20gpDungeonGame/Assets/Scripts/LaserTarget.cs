using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public LaserRoomTrigger linkedRoom;

    public void HitByLaser()
    {
        
        if (linkedRoom != null)
        {
            linkedRoom.TriggerOpen();
        }
    }
}
