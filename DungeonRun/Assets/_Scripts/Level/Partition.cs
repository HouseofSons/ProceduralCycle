//This script is attached to a Partition Game Object
//On Start up will catalogue all parameters for Path Finding from the Partition Game Object
//These parameters are used for Player Path Finding
using System.Collections.Generic;
using UnityEngine;

public class Partition : MonoBehaviour
{
    //Initialized by Parent Room Object
    public List<Connection> Connections { get; set; }

    public Vector3Int Origin            { get; private set; }
    public int Width                    { get; private set; }
    public int Depth                    { get; private set; }
    public List<Vector3> Vertices       { get; private set; }

    public int Nedge                    { get; private set; }
    public int Eedge                    { get; private set; }
    public int Sedge                    { get; private set; }
    public int Wedge                    { get; private set; }

    void Awake()
    {
        Connections = new List<Connection>();

        Origin = new Vector3Int(
            Mathf.RoundToInt(this.transform.GetChild(0).position.x),
            Mathf.RoundToInt(this.transform.GetChild(0).position.y),
            Mathf.RoundToInt(this.transform.GetChild(0).position.z));
        Width = Mathf.RoundToInt(this.transform.GetChild(0).lossyScale.x * 10);
        Depth = Mathf.RoundToInt(this.transform.GetChild(0).lossyScale.z * 10);
        Vertices = new List<Vector3>();

        Nedge = Mathf.RoundToInt(Origin.z + (Depth / 2));
        Eedge = Mathf.RoundToInt(Origin.x + (Width / 2));
        Sedge = Mathf.RoundToInt(Origin.z - (Depth / 2));
        Wedge = Mathf.RoundToInt(Origin.x - (Width / 2));

        Vertices.Add(new Vector3(Wedge, 0, Nedge));
        Vertices.Add(new Vector3(Eedge, 0, Nedge));
        Vertices.Add(new Vector3(Eedge, 0, Sedge));
        Vertices.Add(new Vector3(Wedge, 0, Sedge));
    }
    //Method used by Player Script to determine if a player's path edge collision is a connection to a new Partition
    public bool GetConnection(Vector3 position, out Partition enterPartition)
    {
        foreach (Connection c in Connections)
        {
            if (c.IsVertical)
            {
                if (position.x == c.NeighborEdge)
                {
                    if (position.z <= c.MaxRange && position.z >= c.MinRange)
                    {
                        enterPartition = c.PartitionNeighbor;
                        return true;
                    }
                }
            }
            else
            {
                if (position.z == c.NeighborEdge)
                {
                    if (position.x <= c.MaxRange && position.x >= c.MinRange)
                    {
                        enterPartition = c.PartitionNeighbor;
                        return true;
                    }
                }
            }
        }
        enterPartition = null;
        return false;
    }
    //Overloaded method
    public bool GetConnection(Vector3 position)
    {
        foreach (Connection c in Connections)
        {
            if (c.IsVertical)
            {
                if (position.x == c.NeighborEdge)
                {
                    if (position.z <= c.MaxRange && position.z >= c.MinRange)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (position.z == c.NeighborEdge)
                {
                    if (position.x <= c.MaxRange && position.x >= c.MinRange)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

//Class which holds all needed Connection information
public class Connection
{
    public Partition PartitionNeighbor  { get; private set; }
    public int NeighborEdge             { get; private set; }
    public bool IsVertical              { get; private set; }
    public int MinRange                 { get; private set; }
    public int MaxRange                 { get; private set; }

    public Connection(Partition p, int edge, bool vert, int min, int max)
    {
        PartitionNeighbor = p;
        NeighborEdge = edge;
        IsVertical = vert;
        MinRange = min;
        MaxRange = max;
    }
}