using System.Collections.Generic;

public class Path
{
    public Path()
    {
        PathTiles = new List<Tile>();
        Paths.Add(this);
    }

    public static List<Path> Paths = new List<Path>();
    public List<Tile> PathTiles { get; }

    public void AddPathTile(Tile t)
    {
        PathTiles.Add(t);
    }
}
