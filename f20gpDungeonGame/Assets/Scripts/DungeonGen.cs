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
    public Vector3 spawn_location;

    // Player
    public GameObject player;
    private GameObject playerInstance;

    // Room variables
    static private int room_count = 17; // hardcoded uh oh
    public GameObject Door_obj;

    public GameObject Enemy1x1;
    private Room EnemyRoom1x1;

    public GameObject Enemy1x2;
    private Room EnemyRoom1x2;

    public GameObject StartRoom_obj;
    private Room StartRoom;
    public GameObject Room1x1_obj;
    private Room Room1x1;
    public GameObject Room1x2_obj;
    private Room Room1x2;

    // fire rooms
    public GameObject Room1x1_Fire_1_obj;
    private Room Room1x1_Fire_1;
    public GameObject Room1x1_Fire_2_obj;
    private Room Room1x1_Fire_2;
    public GameObject Room1x2_Fire_1_obj;
    private Room Room1x2_Fire_1;
    public GameObject Room1x2_Fire_2_obj;
    private Room Room1x2_Fire_2;

    // jump rooms
    public GameObject JumpRoom1x1_obj;
    private Room JumpRoom1x1;
    public GameObject JumpRoom1x2_obj;
    private Room JumpRoom1x2;

    // teleport room
    public GameObject TeleportRoom_obj;
    private Room TeleportRoom;

    // key
    public GameObject key_obj;
    int[] keyLocations;

    private Room[] possible_rooms = new Room[room_count];
    
    // Other variables
    // go here

    void Start()
    {
        generateDungeon();
    }

    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("resetting dungeon");
            generateDungeon();
        }
        */
    }

    public void generateDungeon() {

        // Delete previous dungeon if there is one
        deleteDungeon();

        //Door_obj.GetComponent<Animator>().Play("Idle_closedDoor");

        // Fill list of possible rooms, hardcoding unavoidable
        StartRoom = new Room("StartRoom", "s", 1, 1, StartRoom_obj);

        //test rooms
        Room1x1 = new Room("Room1x1", "1", 1, 1, Room1x1_obj);
        possible_rooms[0] = Room1x1; possible_rooms[1] = Room1x1;
        Room1x2 = new Room("Room1x2", "2", 1, 2, Room1x2_obj);
        possible_rooms[2] = Room1x2;


        //fire rooms
        Room1x1_Fire_1 = new Room("Room1x1_Fire_1", "3", 1, 1, Room1x1_Fire_1_obj);
        possible_rooms[3] = Room1x1_Fire_1;
        Room1x1_Fire_2 = new Room("Room1x1_Fire_2", "4", 1, 1, Room1x1_Fire_2_obj);
        possible_rooms[4] = Room1x1_Fire_2;
        Room1x2_Fire_1 = new Room("Room1x2_Fire_1", "5", 1, 2, Room1x2_Fire_1_obj);
        possible_rooms[5] = Room1x2_Fire_1;
        Room1x2_Fire_2 = new Room("Room1x2_Fire_2", "6", 1, 2, Room1x2_Fire_2_obj);
        possible_rooms[6] = Room1x2_Fire_2;

        //jump rooms
        JumpRoom1x1 = new Room("JumpRoom1x1", "j", 1, 1, JumpRoom1x1_obj);
        possible_rooms[7] = JumpRoom1x1; possible_rooms[8] = JumpRoom1x1;
        JumpRoom1x2 = new Room("JumpRoom1x2", "i", 1, 2, JumpRoom1x2_obj);
        possible_rooms[9] = JumpRoom1x2; possible_rooms[10] = JumpRoom1x2;

        //teleport room
        TeleportRoom = new Room("TeleportRoom", "t", 1, 1, TeleportRoom_obj);
        possible_rooms[11] = TeleportRoom; possible_rooms[12] = TeleportRoom;

        // enemy rooms
        EnemyRoom1x1 = new Room("EnemyRoom1x1", "z", 1,1, Enemy1x1);
        possible_rooms[13] = EnemyRoom1x1; possible_rooms[14] = EnemyRoom1x1;
        EnemyRoom1x2 = new Room("EnemyRoom1x2", "a", 1,2, Enemy1x2);
        possible_rooms[15] = EnemyRoom1x2; possible_rooms[16] = EnemyRoom1x2;

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
                room_choice = StartRoom.copy();
            else
                room_choice  = possible_rooms[Random.Range(0, possible_rooms.Length)].copy(); // todo copy not ref
            if (room_choice.Height == 2) {
                room_choice.Rotation = rotation = ROTATIONS[Random.Range(0,2)];
            }
            string letter = room_choice.Letter;
            
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
                    if (BoundsCheck(origin_y-1,origin_x) == false || grid[origin_y-1,origin_x] != "")
                        direction_valid = false; 
                    else {
                        grid[origin_y,origin_x] = letter; 
                        grid[origin_y-1,origin_x] = letter; 
                        empty_count-=2; // mark the room in the grid
                    }
                } else if (rotation == 90) {
                    if (BoundsCheck(origin_y,origin_x+1) == false || grid[origin_y,origin_x+1] != "")
                        direction_valid = false; 
                    else {
                        grid[origin_y,origin_x] = letter; 
                        grid[origin_y,origin_x+1] = letter; 
                        empty_count-=2; // mark the room in the grid
                    }
                }
            } else {
                grid[origin_y,origin_x] = letter; empty_count--;
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


        // Add the rooms and doors to the scene
        foreach (Room room in rooms)
        {
            //Quaternion quat = Quaternion.identity;
            //quat.y = room.Rotation;
            Instantiate(room.Obj, new Vector3(room.Origin.Item2 * 45, 0, room.Origin.Item1 * -45), Quaternion.Euler(new Vector3(0, room.Rotation, 0)), this.transform);
        }

        foreach(((int,int),(int,int)) br in bridges) { 
            float door_x = ((float) br.Item1.Item2 + (float) br.Item2.Item2)/2;
            float door_y = ((float) br.Item1.Item1 + (float) br.Item2.Item1)/2;
            Vector3 door_pos = new Vector3 (door_x*45, 0, door_y*-45);
            int door_rotation = 90;
            if (br.Item1.Item2 != br.Item2.Item2) { // if x coords are not equal door must be rotated the other way
                door_rotation = 0;
            }
            Instantiate(Door_obj, door_pos, Quaternion.Euler(new Vector3(0, door_rotation, 0)), this.transform);

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("DoorWall")) {
                if (Vector3.Distance(door_pos, go.transform.position) < 5)
                {
                    Destroy(go);
                }
            }
        }

        spawn_location = getSpawnRoom();
        playerInstance = Instantiate(player, getSpawnRoom(), player.transform.rotation, this.transform);

        // list of key spawn locations
        HashSet<int> keyLocations = new HashSet<int>();
        while (keyLocations.Count < 3)
        {
            int location = Random.Range(1, rooms.Count);
            keyLocations.Add(location);
            Debug.Log(location);
        }

        // traverse list of rooms
        LinkedListNode<Room> current = rooms.First;
        int index = 0;
        int keysSpawned = 0;

        // spawn 3 keys in the center of the randomly chosen rooms
        while (current != null && keysSpawned < 3)
        {
            if (keyLocations.Contains(index))
            {
                Instantiate(key_obj, new Vector3(current.Value.Origin.Item2 * 45, 0, current.Value.Origin.Item1 * -45), key_obj.transform.rotation, this.transform);
                //Debug.Log("Key Spawned at: " + index);
                keysSpawned++;
            }
            current = current.Next;
            index++;
        }
    }

    public void deleteDungeon(){
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public GameObject getPlayerInstance()
    {
        return playerInstance;
    }

    public Vector3 getSpawnRoom()
    {
        return new Vector3(rooms.First.Value.Origin.Item2 * 45, 1.5f, rooms.First.Value.Origin.Item1 * -45);
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
