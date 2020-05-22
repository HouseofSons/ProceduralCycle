//This script is attached to a Partition Game Object
//On Start up will catalogue all parameters for Path Finding from the Partition Game Object
//These parameters are used for Player Path Finding
using System.Collections.Generic;
using UnityEngine;

public class Partition : MonoBehaviour
{
    public List<Connection> Connections { set; get; }

    public Vector3Int Origin { private set; get; }
    public int Width { private set; get; }
    public int Depth { private set; get; }

    public int Nedge { private set; get; }
    public int Eedge { private set; get; }
    public int Sedge { private set; get; }
    public int Wedge { private set; get; }

    public int WidthStart { private set; get; }
    public int WidthEnd { private set; get; }
    public int DepthStart { private set; get; }
    public int DepthEnd { private set; get; }

    void Awake()
    {
        Origin = new Vector3Int(
            Mathf.RoundToInt(this.transform.position.x),
            Mathf.RoundToInt(this.transform.position.y),
            Mathf.RoundToInt(this.transform.position.z));
        Width = Mathf.RoundToInt(this.transform.lossyScale.x * 10);
        Depth = Mathf.RoundToInt(this.transform.lossyScale.z * 10);

        Nedge = Mathf.RoundToInt(Origin.z + (Depth / 2));
        Eedge = Mathf.RoundToInt(Origin.x + (Width / 2));
        Sedge = Mathf.RoundToInt(Origin.z - (Depth / 2));
        Wedge = Mathf.RoundToInt(Origin.x - (Width / 2));

        Connections = new List<Connection>();
    }

    //Method used by Player Script to determine if a player's path edge collision is a connection to a new Partition
    public bool GetConnection(Vector3 position, out Partition enterPartition)
    {
        foreach (Connection o in Connections)
        {
            if (Mathf.RoundToInt(position.x) == o.NeighborEdge)
            {
                if (position.z <= o.MaxRange && position.z >= o.MinRange)
                {
                    enterPartition = o.PartitionNeighbor;
                    return true;
                }
            }
            if (Mathf.RoundToInt(position.z) == o.NeighborEdge)
            {
                if (position.z <= o.MaxRange && position.z >= o.MinRange)
                {
                    enterPartition = o.PartitionNeighbor;
                    return true;
                }
            }
        }
        enterPartition = null;
        return false;
    }
}

//Class which holds all needed Connection information
public class Connection
{
    public Partition PartitionNeighbor { private set; get; }
    public int NeighborEdge { private set; get; }
    public bool IsVertical { private set; get; }
    public int MinRange { private set; get; }
    public int MaxRange { private set; get; }

    public Connection(Partition p, int edge, bool vert, int min, int max)
    {
        PartitionNeighbor = p;
        NeighborEdge = edge;
        IsVertical = vert;
        MinRange = min;
        MaxRange = max;
        Debug.Log("Shared Edge " + NeighborEdge + " Vertical?: " + IsVertical + " min: " + MinRange + " max: " + MaxRange);
    }
}