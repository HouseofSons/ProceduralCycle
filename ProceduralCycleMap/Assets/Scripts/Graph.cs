using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public PathNode (Vector3Int v)
    {
        location = v;
    }

    public Vector3Int location { get; private set; }
    public int h_value { get; set; }
    public int m_value { get; set; }
    public int g_value { get; set; }
    public int f_value { get; set; }

    public PathNode parent { get; set; }
}

public class Pathing
{
    private PathNode[,,] nodes = new PathNode[GameGrid.GameGridScale, GameGrid.GameGridScale, GameGrid.GameGridScale];

    public void InitializeNodes(Room[,,] roomNodes)
    {
        for (int i = 0; i < roomNodes.GetLength(0); i++)
        {
            for (int j = 0; j < roomNodes.GetLength(1); j++)
            {
                for (int k = 0; k < roomNodes.GetLength(2); k++)
                {
                    if (roomNodes[i, j, k] == null)
                    {
                        nodes[i, j, k] = new PathNode(new Vector3Int(i,j,k));
                    }
                }
            }
        }
    }

    public List<Vector3Int> AstarPath(Vector3Int start, Vector3Int end)
    {
        //for all nodes adjacent to current_node not on closed_list
        //    if node not in open_list
        //        set node.parent to current_node
        //        if node = destination track through parents and you're done
        //        set node.m_value = 1;
        //        set node.g_value = node.m_value + current_node.g_value
        //        set node.f_value = node.g_value + node.h_value
        //        add node to open_list
        //    else
        //        if (current_node.g_value + node.m_value < node.g_value)
        //        node.parent = current_node
        //set current_node = node with lowest f_value in open_list
        //add current_node to closed_list
        //remove current_node from open_list

        PathNode startNode = nodes[start.x, start.y, start.z];
        PathNode endNode = nodes[end.x, end.y, end.z];

        //calculate h_value for each node to destination
        foreach (PathNode p in nodes)
        {
            p.h_value =
                Mathf.Abs(endNode.location.x - p.location.x) +
                Mathf.Abs(endNode.location.y - p.location.y) +
                Mathf.Abs(endNode.location.z - p.location.z);
        }
        //Initialize Open and Closed Lists
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode currentNode;

        startNode.m_value = 0;
        startNode.g_value = 0;
        startNode.parent = startNode;
        closedList.Add(startNode);

        currentNode = startNode;

        //Finds neighbors of currentNode
        int a;
        int b;
        int c;

        PathNode neighbor;

        do
        {
            for (int i = 0; i < 6; i++)
            {
                a = (i == 0 ? -1 : (i == 1 ? 1 : 0));
                b = (i == 2 ? -1 : (i == 3 ? 1 : 0));
                c = (i == 4 ? -1 : (i == 5 ? 1 : 0));

                neighbor = nodes[currentNode.location.x + a, currentNode.location.y + b, currentNode.location.z + c];

                if (neighbor != null) //avoids non-empty nodes
                {
                    if (!closedList.Contains(neighbor))
                    {
                        if (!openList.Contains(neighbor))
                        {
                            neighbor.parent = currentNode;
                            if (neighbor == endNode)
                            {
                                List<Vector3Int> path = new List<Vector3Int>();
                                PathNode iterate = endNode;

                                do
                                {
                                    path.Add(iterate.location);
                                    iterate = iterate.parent;
                                } while (iterate.parent != iterate);
                                return path;
                            }
                            neighbor.m_value = 1;
                            neighbor.g_value = neighbor.m_value + currentNode.g_value;
                            neighbor.f_value = neighbor.g_value + neighbor.h_value;
                            openList.Add(neighbor);
                        }
                        else
                        {
                            if (currentNode.g_value + neighbor.m_value < neighbor.g_value)
                            {
                                neighbor.parent = currentNode;
                            }
                        }

                        currentNode = null;

                        foreach (PathNode p in openList)
                        {
                            if (currentNode == null || currentNode.f_value > p.f_value)
                            {
                                currentNode = p;
                            }
                        }
                        closedList.Add(currentNode);
                        openList.Remove(currentNode);
                    }
                }
            }
        } while (openList.Count > 0);
        return null;
    }
}


