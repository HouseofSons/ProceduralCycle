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
        bool[] visited = new bool[V];

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
        visited[v] = true;

        foreach (int x in adjListArray[v])
        {
            if (!visited[x])
            {
                DFSUtil(x, visited);
            }
        }
    }
}