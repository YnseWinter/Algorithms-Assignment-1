using UnityEngine;
public class GraphTester : MonoBehaviour
{
    void Start()
    {
        Graph<string> graph = new Graph<string>();
        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddNode("C");
        graph.AddNode("D");
        graph.AddNode("E");
        if(graph.GetNeighbors("E").Count == 0)
        {
            Debug.Log("nothetiebt");
        }
        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");
        graph.AddEdge("B", "D");
        graph.AddEdge("C", "D");
        Debug.Log("Graph Structure:");
        graph.PrintGraph();
    }
}
