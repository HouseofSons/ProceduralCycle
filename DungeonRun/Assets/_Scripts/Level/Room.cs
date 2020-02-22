using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Partition> Partitions { private get; set; }
    public List<Overlap> Overlaps { private get; set; }

    void Start()
    {
        foreach (Partition p in transform.GetComponentsInChildren<Partition>())
        {
            Partitions.Add(p);
        }
        FindOverlaps();
    }

    private void FindOverlaps()
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
                        Overlaps.Add(new Overlap(
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
                        Overlaps.Add(new Overlap(
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

    public bool PartitionOverlap(Vector3 position, Partition currentPartition, out Partition enterPartition)
    {
        foreach(Overlap o in Overlaps)
        {
            if (position.x == o.NeighborEdge)
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
            if (position.z == o.NeighborEdge)
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

public class Overlap
{
    public Partition Partition0 { private set; get; }
    public Partition Partition1 { private set; get; }
    public int NeighborEdge { private set; get; }
    public bool IsVertical { private set; get; }
    public int MinRange { private set; get; }
    public int MaxRange { private set; get; }

    public Overlap(Partition p0, Partition p1, int edge, bool vert, int min, int max)
    {
        Partition0 = p0;
        Partition1 = p1;
        NeighborEdge = edge;
        IsVertical = vert;
        MinRange = min;
        MaxRange = max;
    }
}