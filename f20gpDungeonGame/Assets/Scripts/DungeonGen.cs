using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    // Grid variables
    public int grid_width = 5;
    public int grid_height = 5;
    private string[,] grid;
    private LinkedList<((int,int),(int,int))> bridges;
    private LinkedList<Room> rooms;
    private (int,int)[] adjacent_rooms = new (int,int)[4];
    
    // Room variables
    static private int room_count = 3; // hardcoded uh oh
    private Room Room1x1 = new Room("Room1x1", 1, 1);
    private Room Room1x2 = new Room("Room1x2", 1, 2);
    private Room StartRoom = new Room("Start", 1, 1);
    private Room[] possible_rooms = new Room[room_count];
    
    
    
    // Other variables
    // go here

    void Start()
    {
        // Fill list of possible rooms, hardcoding unavoidable
        
        possible_rooms[0] = StartRoom;
        possible_rooms[1] = Room1x1;
        possible_rooms[2] = Room1x2;

        // create empty grid
        grid = new string[grid_height, grid_width];
        for (int y = 0; y < grid.GetLength(0); y++) { 
            for (int x = 0; x < grid.GetLength(1); x++) { 
                grid[y,x] = "";
            } 
        }  
        bridges = new LinkedList<((int,int),(int,int))>();
        rooms = new LinkedList<Room>();
        int empty_count = grid_height * grid_width;

        // other variable for genning grid
        int[] ROTATIONS = {0,90};
        bool first_room = true;

        // Add rooms until there are no more empty spaces
        while (empty_count > 0) {

            // Default variables - get assigned later but need value to check for errors
            int rotation = 0;
            (int,int) connected_room = (0,0);
            adjacent_rooms = new (int,int)[4];
            adjacent_rooms[0]=(0,0);adjacent_rooms[1]=(0,0);adjacent_rooms[2]=(0,0);adjacent_rooms[3]=(0,0);
            Room room_choice;

            // Chose a room and location for the room
            if (first_room) 
                room_choice = possible_rooms[0];
            else
                room_choice  = possible_rooms[Random.Range(0, possible_rooms.Length)]; // todo copy not ref
            if (room_choice.Height == 2) {
                rotation = ROTATIONS[Random.Range(0,2)];
            }
            
            int origin_x = Random.Range(0, grid_width);
            int origin_y = Random.Range(0, grid_height);
            room_choice.Origin = (origin_y, origin_x);
        
            // make sure origin point is empty
            if (grid[origin_y, origin_x] != "")
                continue;

            // make sure there is enough space left
            int room_volume = room_choice.Height * room_choice.Width;
            if (room_volume > empty_count)
                continue;

            // make sure there is a room beside the chosen 
            int adj_count = 0;
            if (first_room == false) { // first room doesnt need to be placed beside another room
                for (int y = -1; y <= 1; y++){ // loop around the room's origin
                    for (int x = -1; x <= 1; x++){
                        if ((!(x == 0 && y == 0) && (x == 0 || y == 0))) {// they cannot both be 0, but 1 must be
                            Debug.Log(string.Format("{0},{1}",y,x));
                            if (BoundsCheck(origin_y+y, origin_x+x)) {
                                if (grid[origin_y+y, origin_x+x] != "") {
                                    //Debug.Log(adj_count);
                                    adjacent_rooms[adj_count] = (origin_y+y, origin_x+x);
                                    adj_count++;
                                }
                            }
                        } 
                    }
                }
                if (adj_count == 0)
                    continue;
                else
                    //Debug.Log(adj_count);
                    connected_room = adjacent_rooms[(Random.Range(0,adj_count))]; // chose a random room beside new room to connect via door
            }

            
            bool direction_valid = true;
            if (room_choice.Height == 2) { // if room is a 1x2, check that the extra space is unoccupied
                if (rotation == 0) {
                    if (BoundsCheck(origin_y+1,origin_x) == false || grid[origin_y+1,origin_x] != "")
                        direction_valid = false; 
                    else {
                        grid[origin_y,origin_x] = "#"; 
                        grid[origin_y+1,origin_x] = "#"; 
                        empty_count-=2; // mark the room in the grid
                    }
                } else if (rotation == 90) {
                    if (BoundsCheck(origin_y,origin_x+1) == false || grid[origin_y,origin_x+1] != "")
                        direction_valid = false; 
                    else {
                        grid[origin_y,origin_x] = "#"; 
                        grid[origin_y,origin_x+1] = "#"; 
                        empty_count-=2; // mark the room in the grid
                    }
                }
            } else {
                grid[origin_y,origin_x] = "#"; empty_count--;
            }

            if (direction_valid) { // room placement is garuanteeded to be ok
                rooms.AddLast(room_choice); // add room to list of rooms
                
                if (first_room == false) {
                    bridges.AddLast((connected_room, (origin_y, origin_x))); // add connection between rooms
                }
                first_room = false;
            }

            
        } // end while


        PrintGrid(grid);

        
    

    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PrintGrid(string[,] grid)
    {
        string grid_text = "";

        for (int y = 0; y < grid.GetLength(0); y++) { 
            for (int x = 0; x < grid.GetLength(1); x++) { 
                grid_text += grid[y,x] + " "; 
            } 
            grid_text += "\n";
        }  

        Debug.Log(grid_text);

        foreach(((int,int),(int,int)) br in bridges) { 
          Debug.Log(br); 
        }

        
    }

    private bool BoundsCheck(int y, int x) {
        if (y < 0 || y >= grid_height || x < 0 || x >= grid_width) {
            return false;
        } else {
            return true;
        }
    }
}
