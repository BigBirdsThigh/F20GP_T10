using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    static private int room_count = 2; // hardcoded uh oh
    private Dictionary<string, int> oneXone = new Dictionary<string, int>();
    private Dictionary<string, int> oneXtwo = new Dictionary<string, int>();
    private Dictionary<string, int>[] possible_rooms = new Dictionary<string, int>[room_count];
    
    void Start()
    {
        // oops more hardcoding, it's unavoidable
        oneXone.Add("width", 1); oneXone.Add("height", 1); possible_rooms[0] = oneXone;
        oneXtwo.Add("width", 1); oneXtwo.Add("height", 2); possible_rooms[1] = oneXtwo;


    

    
    
    
    
    
    
    
    
    
    
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
