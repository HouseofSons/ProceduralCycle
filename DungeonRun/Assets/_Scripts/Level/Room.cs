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
    public static Vector3 TranslateToPlayerView(Vector3 v,Room r)
    {
        Ray ray;
        Vector3 origin;
        if(CollisionPath.Collisions.Count == 0)
        {
            origin = Player.PlayerTransform.position; 
        } else
        {
            origin = CollisionPath.Collisions[CollisionPath.Collisions.Count - 1];
        }
        ray = new Ray(origin, v - origin);

        List<Vector3> edgeIntersects = new List<Vector3>();
        edgeIntersects.Add(v); //if click does not hit wall
        Vector3 point,scratch;
        Plane plane;
        float distance;
        foreach (Partition p in r.Partitions)
        {
            if (ray.direction.z > 0)
            {
                plane = new Plane(Vector3.back, p.Vertices[0]);
                if (plane.Raycast(ray, out distance))
                {
                    point = ray.GetPoint(distance);
                    scratch = ray.GetPoint(distance - Random.Range(0.1f, 0.2f));
                    if (p.Wedge <= scratch.x && scratch.x <= p.Eedge)
                    {
                        if (!p.GetConnection(point))
                        {
                            edgeIntersects.Add(scratch);
                        }
                    }
                }
            }

            if (ray.direction.x > 0)
            {
                plane = new Plane(Vector3.left, p.Vertices[1]);
                if (plane.Raycast(ray, out distance))
                {
                    point = ray.GetPoint(distance);
                    scratch = ray.GetPoint(distance - Random.Range(0.1f, 0.2f));
                    if (p.Sedge <= scratch.z && scratch.z <= p.Nedge)
                    {
                        if (!p.GetConnection(point))
                        {
                            edgeIntersects.Add(scratch);
                        }
                    }
                }
            }

            if (ray.direction.z < 0)
            {
                plane = new Plane(Vector3.forward, p.Vertices[2]);
                if (plane.Raycast(ray, out distance))
                {
                    point = ray.GetPoint(distance);
                    scratch = ray.GetPoint(distance - Random.Range(0.1f, 0.2f));
                    if (p.Wedge <= scratch.x && scratch.x <= p.Eedge)
                    {
                        if (!p.GetConnection(point))
                        {
                            edgeIntersects.Add(scratch);
                        }
                    }
                }
            }
            if (ray.direction.x < 0)
            {
                plane = new Plane(Vector3.right, p.Vertices[3]);
                if (plane.Raycast(ray, out distance))
                {
                    point = ray.GetPoint(distance);
                    scratch = ray.GetPoint(distance - Random.Range(0.1f, 0.2f));
                    if (p.Sedge <= scratch.z && scratch.z <= p.Nedge)
                    {
                        if (!p.GetConnection(point))
                        {
                            edgeIntersects.Add(scratch);
                        }
                    }
                }
            }
        }
        edgeIntersects.Sort((v1, v2) => (v1 - origin).sqrMagnitude.CompareTo((v2 - origin).sqrMagnitude));
        GameObject.Find("Point").transform.position = edgeIntersects[0];
        return edgeIntersects[0];
    }
}