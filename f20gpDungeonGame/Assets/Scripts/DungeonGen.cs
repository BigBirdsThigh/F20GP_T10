using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    // Grid variables
    public int grid_width = 5;
    public int grid_height = 5;
    private string[,] grid;
    
    // Room variables
    static private int room_count = 2; // hardcoded uh oh
    private Room Room1x1 = new Room("Room1x1", 1, 1);
    private Room Room1x2 = new Room("Room1x2", 1, 2);
    private Room[] possible_rooms = new Room[room_count];
    
    
    // Other variables
    // go here

    void Start()
    {
        // Fill list of possible rooms, hardcoding unavoidable
        possible_rooms[0] = Room1x1;
        possible_rooms[1] = Room1x2;

        // create empty grid
        grid = new string[grid_height, grid_width];
        //PrintGrid(grid);
        int empty_count = grid_height * grid_width;

        // other variable for genning grid
        LinkedList<(int,int)> adjacent_rooms= new LinkedList<(int,int)>();
        int[] ROTATIONS = {0,90};

        // Add rooms until there are no more empty spaces
        while (empty_count > 0) {
            // Chose a room and location for the room
            Room room_choice  = possible_rooms[Random.Range(0, possible_rooms.Length)];
            int origin_x = Random.Range(0, grid_width);
            int origin_y = Random.Range(0, grid_height);
            
            
            
            
            
            empty_count -= 1;
        }


        

        
    

    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PrintGrid(string[,] grid)
    {
        string grid_text = "";

        for (int y = 0; y < grid.GetLength(0); y++) 
        { 
            for (int x = 0; x < grid.GetLength(1); x++) 
            { 
                grid_text += grid[y,x] + " "; 
            } 
            grid_text += "\n";
        }  

        Debug.Log(grid_text);
    }
}
