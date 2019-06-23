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
    private static PathNode[,,] nodes = new PathNode[GameGrid.GameGridScale, GameGrid.GameGridScale, GameGrid.GameGridScale];

    public static int xMin { get; private set; }
    public static int yMin { get; private set; }
    public static int zMin { get; private set; }

    public static int xMax { get; private set; }
    public static int yMax { get; private set; }
    public static int zMax { get; private set; }

    public static void InitializeNodes(Vector3Int start,Vector3Int end, Room[,,] roomNodes)
    {
        //Clean old nodes
        nodes = new PathNode[GameGrid.GameGridScale, GameGrid.GameGridScale, GameGrid.GameGridScale];

        xMin = Mathf.Max(Mathf.Min(start.x, end.x) - 6,0);
        yMin = Mathf.Max(Mathf.Min(start.y, end.y) - 6,0);
        zMin = Mathf.Max(Mathf.Min(start.z, end.z) - 6,0);

        xMax = Mathf.Min(Mathf.Max(start.x, end.x) + 6, GameGrid.GameGridScale);
        yMax = Mathf.Min(Mathf.Max(start.y, end.y) + 6, GameGrid.GameGridScale);
        zMax = Mathf.Min(Mathf.Max(start.z, end.z) + 6, GameGrid.GameGridScale);

        for (int i = xMin; i < xMax; i++)
        {
            for (int j = yMin; j < yMax; j++)
            {
                for (int k = zMin; k < zMax; k++)
                {
                    if (roomNodes[i, j, k] == null)
                    {
                        nodes[i, j, k] = new PathNode(new Vector3Int(i,j,k));
                        //calculate h_value for each node to destination
                        nodes[i, j, k].h_value =
                            Mathf.Abs(end.x - nodes[i, j, k].location.x) +
                            Mathf.Abs(end.y - nodes[i, j, k].location.y) +
                            Mathf.Abs(end.z - nodes[i, j, k].location.z);
                    }
                }
            }
        }
    }

    public static List<Vector3Int> AstarPath(Vector3Int start, Vector3Int end, Room[,,] roomNodes)
    {
        //Initialize Nodes!!!
        InitializeNodes(start, end, roomNodes);

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
        if (start == end)
        {
            return new List<Vector3Int> { start };
        }

        PathNode startNode = nodes[start.x, start.y, start.z];
        PathNode endNode = nodes[end.x, end.y, end.z];
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

                if (currentNode.location.x + a >= 0 && currentNode.location.x + a < roomNodes.GetLength(0) &&
                    currentNode.location.y + b >= 0 && currentNode.location.y + b < roomNodes.GetLength(1) &&
                    currentNode.location.z + c >= 0 && currentNode.location.z + c < roomNodes.GetLength(2))
                {
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
                                    path.Add(iterate.location);
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
                        }
                    }
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
            
        } while (openList.Count > 0);
        
        return new List<Vector3Int>();
    }
}


