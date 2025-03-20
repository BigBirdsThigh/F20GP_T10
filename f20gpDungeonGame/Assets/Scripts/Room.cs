using UnityEngine;

public class Room
{

    public string Name { get;}
    public int Width { get;}
    public int Height { get;}
    public int Rotation { get; set;}
    public (int,int) Origin { get; set;}

    public Room(string _name, int _width, int _height) {
        Name = _name;
        Width = _width;
        Height = _height;
    }

    
}
