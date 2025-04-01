using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TeleportRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject[,] pads = new GameObject[6,6];
    public GameObject telepad_obj;
    void Start()
    {
        //Vector3 origin = this.gameObject.transform.position;
        for (int y = 0; y < 6; y++) {
            for (int x = 0; x < 6; x++) {
                float xn = (-16.5f + (x*7));
                float zn = (-16.5f + (y*7));
                pads[y,x] = Instantiate(telepad_obj);
                pads[y,x].transform.parent = transform;
                pads[y,x].transform.localPosition = new Vector3(xn,0,zn);
                pads[y,x].transform.rotation = Quaternion.Euler(-90,0,0);

                /*
                GameObject gFloor = Instantiate(Floor);
                gFloor.transform.parent = transform;
                gFloor.transform.localPosition = new Vector3(x,0,z) * fSpacing;
                gFloor.transform.rotation = Quaternion.Euler(-90,0,0);
                gFloor.name = "Floor_" + z + x ;
                */
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
