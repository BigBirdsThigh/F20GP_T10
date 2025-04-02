using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TeleportRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private (GameObject,(int,int))[,] pads = new (GameObject,(int,int))[6,6]; // contains the telepad and the destination of that pad
    private (int,int)[,] connections = {{(3,1), (0,2), (0,1), (2,2), (4,5), (1,3)},
                                        {(5,4), (4,2), (2,5), (0,5), (5,0), (3,0)},
                                        {(5,2), (4,0), (0,3), (5,1), (3,2), (1,2)},
                                        {(1,5), (0,0), (2,4), (4,3), (5,3), (4,4)},
                                        {(2,1), (5,5), (1,1), (3,3), (3,5), (0,4)},
                                        {(1,4), (2,3), (2,0), (3,4), (1,0), (4,1)}};

    public GameObject telepad_obj;
    private Dictionary<GameObject,(int,int)> lookup = new Dictionary<GameObject,(int,int)>(); // used to find the telepad in the list pads without looping through every time
    private GameObject player;
    private bool teleportable = true;
    private GameObject targetPad;


    void Start()
    {
        AddTelepads();


        
    }

    private void AddTelepads() {
        for (int y = 0; y < 6; y++) {
            for (int x = 0; x < 6; x++) {
                float xn = (-16.5f + (x*7));
                float zn = (-16.5f + (y*7));
                GameObject obj = Instantiate(telepad_obj);
                obj.transform.parent = transform;
                obj.transform.localPosition = new Vector3(xn,0,zn);
                obj.transform.rotation = Quaternion.Euler(-90,0,0);
                pads[y,x] = (obj,connections[y,x]);
                lookup.Add(obj,(y,x));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        } else {
            foreach ((GameObject,(int,int)) pad in pads) {
                if (teleportable) { 
                    if (Vector3.Distance(player.transform.position, pad.Item1.transform.position) < 2) {
                        targetPad = pads[pad.Item2.Item1,pad.Item2.Item2].Item1;
                        Vector3 targetLocation = targetPad.transform.position;
                        targetLocation.x -= 1; targetLocation.z -= 1; targetLocation.y = 0.5f;
                        player.transform.position = targetLocation;
                        teleportable = false;
                        //Debug.Log("Teleported to " + pad.Item2.Item1 + ", " + pad.Item2.Item2);
                    }
                } else {
                    if (Vector3.Distance(player.transform.position, targetPad.transform.position) > 3){
                        teleportable = true;
                        //Debug.Log("reset");
                    }
                }
            }
        }
    }
}
