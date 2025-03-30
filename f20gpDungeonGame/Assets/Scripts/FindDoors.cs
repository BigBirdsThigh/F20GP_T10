using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class FindDoors1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private LinkedList<GameObject> find_doors(){
        LinkedList<GameObject> doors = new LinkedList<GameObject>();
        int req_distance = 33;
        
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Door")) {
            if (Vector3.Distance(this.gameObject.transform.position, go.transform.position) < req_distance)
            {
                    doors.AddLast(go);
            }
        }

        return doors;
    }
}


