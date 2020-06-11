//This script is attached to a Room Game Object
//On Start will catalogue references to it's Parition Child Objects
    //In addition it will calculate all Connections between each Partition Child Object
    //Connections are used in Player Path finding between Partitions
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Partition> Partitions { get; private set; }

    void Start()
    {
        Partitions = new List<Partition>();

        foreach (Partition p in transform.GetComponentsInChildren<Partition>())
        {
            Partitions.Add(p);
        }

        AddConnections();
    }

    //Method creates connections between partitions
    public void AddConnections()
    {
        for (int i = 0; i < Partitions.Count - 1; i++)
        {
            for (int j = i + 1; j < Partitions.Count; j++)
            {
                if (Partitions[i].Nedge == Partitions[j].Sedge || Partitions[i].Sedge == Partitions[j].Nedge)
                {
                    if ((Partitions[i].Wedge >= Partitions[j].Wedge && Partitions[i].Wedge <= Partitions[j].Eedge) ||
                        (Partitions[i].Eedge >= Partitions[j].Wedge && Partitions[i].Eedge <= Partitions[j].Eedge))
                    {
                        Partitions[i].Connections.Add(new Connection(
                            Partitions[j],
                            Partitions[i].Nedge == Partitions[j].Sedge ? Partitions[i].Nedge : Partitions[i].Sedge,
                            false,
                            Mathf.RoundToInt(Mathf.Max(Partitions[i].Wedge, Partitions[j].Wedge)),
                            Mathf.RoundToInt(Mathf.Min(Partitions[i].Eedge, Partitions[j].Eedge))));
                        Partitions[j].Connections.Add(new Connection(
                            Partitions[i],
                            Partitions[i].Nedge == Partitions[j].Sedge ? Partitions[j].Sedge : Partitions[j].Nedge,
                            false,
                            Mathf.RoundToInt(Mathf.Max(Partitions[i].Wedge, Partitions[j].Wedge)),
                            Mathf.RoundToInt(Mathf.Min(Partitions[i].Eedge, Partitions[j].Eedge))));
                    }
                }
                if (Partitions[i].Wedge == Partitions[j].Eedge || Partitions[i].Eedge == Partitions[j].Wedge)
                {
                    if ((Partitions[i].Sedge >= Partitions[j].Sedge && Partitions[i].Sedge <= Partitions[j].Nedge) ||
                        (Partitions[i].Nedge >= Partitions[j].Sedge && Partitions[i].Nedge <= Partitions[j].Nedge))
                    {
                        Partitions[i].Connections.Add(new Connection(
                            Partitions[j],
                            Partitions[i].Wedge == Partitions[j].Eedge ? Partitions[i].Wedge : Partitions[i].Eedge,
                            true,
                            Mathf.RoundToInt(Mathf.Max(Partitions[i].Sedge, Partitions[j].Sedge)),
                            Mathf.RoundToInt(Mathf.Min(Partitions[i].Nedge, Partitions[j].Nedge))));
                        Partitions[j].Connections.Add(new Connection(
                            Partitions[i],
                            Partitions[i].Wedge == Partitions[j].Eedge ? Partitions[j].Eedge : Partitions[j].Wedge,
                            true,
                            Mathf.RoundToInt(Mathf.Max(Partitions[i].Sedge, Partitions[j].Sedge)),
                            Mathf.RoundToInt(Mathf.Min(Partitions[i].Nedge, Partitions[j].Nedge))));
                    }
                }
            }
        }
    }
    public Partition GetPartition(Vector3 v)
    {
        foreach (Partition p in Partitions)
        {
            if (p.Wedge <= v.x && v.x <= p.Eedge)
            {
                if (p.Sedge <= v.z && v.z <= p.Nedge)
                {
                    return p;
                }
            }
        }
        return Player.LatestSpawn.GetComponent<Spawn>().GetPartition();
    }
}