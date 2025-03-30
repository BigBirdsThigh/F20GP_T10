using UnityEngine;

public class Room
{

    public string Name { get;}
    public string Letter { get;}
    public int Width { get;}
    public int Height { get;}
    public GameObject Obj { get;}
    public int Rotation { get; set;}
    public (int,int) Origin { get; set;}
    

    public Room(string _name, string _letter, int _width, int _height, GameObject _obj) {
        Name = _name;
        Letter = _letter;
        Width = _width;
        Height = _height;
        Rotation = 0;
        Origin = (0,0);
        Obj = _obj;
    }

    public Room(string _name, string _letter, int _width, int _height, int _rotation, (int,int) _origin, GameObject _obj) {
        Name = _name;
        Letter = _letter;
        Width = _width;
        Height = _height;
        Rotation = _rotation;
        Origin = _origin;
        Obj = _obj;
    }

    public Room copy() {
        return new Room(Name, Letter, Width, Height, Rotation, Origin, Obj);
    }
    
}

