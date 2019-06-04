using System;
using System.Collections.Generic;

public class Graph
{
    private int V;
    private List<int>[] adjListArray;

    public Graph(int V)
    {
        this.V = V;

        adjListArray = new List<int>[V];

        for (int i = 0; i < V; i++)
        {
            adjListArray[i] = new List<int>();
        }
    }

    public void AddEdge(int src, int dest)
    {
        if (!adjListArray[src].Contains(dest))
        {
            adjListArray[src].Add(dest);
            adjListArray[dest].Add(src);
        }
    }

    public bool IsGraphConnected()
    {
        // Mark all the vertices as not visited 
        bool[] visited = new bool[V];
        // print all reachable vertices 
        // from 0 
        DFSUtil(0, visited);
        foreach(bool b in visited)
        {
            if(!b)
            {
                return false;
            }
        }
        return true;
    }

    private void DFSUtil(int v, bool[] visited)
    {
        // Mark the current node as visited
        visited[v] = true;

        // Recur for all the vertices 
        // adjacent to this vertex 
        foreach (int x in adjListArray[v])
        {
            if (!visited[x])
            {
                DFSUtil(x, visited);
            }
        }
    }

    // Driver code 
    //public static void Main(String[] args)
    //{
    //    // Create a graph given in the above diagram  
    //    Graph g = new Graph(5); // 5 vertices numbered from 0 to 4  
    //    g.addEdge(1, 0);
    //    g.addEdge(2, 3);
    //    g.addEdge(3, 4);
    //    Console.WriteLine("Following are connected components");
    //    g.connectedComponents();
    //}
}