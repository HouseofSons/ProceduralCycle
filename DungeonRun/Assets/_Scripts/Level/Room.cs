//This script is attached to a Room Game Object
//On Start will catalogue references to it's Parition Child Objects
    //In addition it will calculate all Connections between each Partition Child Object
    //Connections are used in Player Path finding between Partitions
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Partition> Partitions { private get; set; }
    public List<Connection> Connections { private get; set; }

    void Start()
    {
        foreach (Partition p in transform.GetComponentsInChildren<Partition>())
        {
            Partitions.Add(p);
        }
        FindConnections();
    }
    //Method creates connections between partition Child Objects
    private void FindConnections()
    {
        for (int i = 0; i < Partitions.Count - 1; i++)
        {
            for (int j = i + 1; j < Partitions.Count; j++)
            {
                if (Partitions[i].Nedge == Partitions[j].Sedge)
                {
                    if ((Partitions[i].Wedge >= Partitions[j].Wedge && Partitions[i].Wedge <= Partitions[j].Eedge) ||
                        (Partitions[i].Eedge >= Partitions[j].Wedge && Partitions[i].Eedge <= Partitions[j].Eedge))
                    {
                        Connections.Add(new Connection(
                            Partitions[i],
                            Partitions[j],
                            Partitions[i].Nedge,
                            true,
                            Mathf.RoundToInt(Mathf.Max(Partitions[i].Wedge, Partitions[j].Wedge)),
                            Mathf.RoundToInt(Mathf.Min(Partitions[i].Eedge, Partitions[j].Eedge))));
                    }
                }
                if (Partitions[i].Wedge == Partitions[j].Eedge)
                {
                    if ((Partitions[i].Sedge >= Partitions[j].Sedge && Partitions[i].Sedge <= Partitions[j].Nedge) ||
                        (Partitions[i].Nedge >= Partitions[j].Sedge && Partitions[i].Nedge <= Partitions[j].Nedge))
                    {
                        Connections.Add(new Connection(
                            Partitions[i],
                            Partitions[j],
                            Partitions[i].Wedge,
                            false,
                            Mathf.RoundToInt(Mathf.Max(Partitions[i].Sedge, Partitions[j].Sedge)),
                            Mathf.RoundToInt(Mathf.Min(Partitions[i].Nedge, Partitions[j].Nedge))));
                    }
                }
            }
        }
    }
    //Method used by Player Script to determine if a player's path edge collision is a connection to a new Partition
    public bool PartitionConnection(Vector3 position, Partition currentPartition, out Partition enterPartition)
    {
        foreach(Connection o in Connections)
        {
            if (Mathf.RoundToInt(position.x) == o.NeighborEdge)
            {
                if (position.z <= o.MaxRange && position.z >= o.MinRange)
                {
                    if(currentPartition == o.Partition0)
                    {
                        enterPartition = o.Partition1;
                        return true;
                    } else
                    {
                        enterPartition = o.Partition0;
                        return true;
                    }
                }
            }
            if (Mathf.RoundToInt(position.z) == o.NeighborEdge)
            {
                if (position.z <= o.MaxRange && position.z >= o.MinRange)
                {
                    if (currentPartition == o.Partition0)
                    {
                        enterPartition = o.Partition1;
                        return true;
                    }
                    else
                    {
                        enterPartition = o.Partition0;
                        return true;
                    }
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
    public Partition Partition0 { private set; get; }
    public Partition Partition1 { private set; get; }
    public int NeighborEdge { private set; get; }
    public bool IsVertical { private set; get; }
    public int MinRange { private set; get; }
    public int MaxRange { private set; get; }

    public Connection(Partition p0, Partition p1, int edge, bool vert, int min, int max)
    {
        Partition0 = p0;
        Partition1 = p1;
        NeighborEdge = edge;
        IsVertical = vert;
        MinRange = min;
        MaxRange = max;
    }
}