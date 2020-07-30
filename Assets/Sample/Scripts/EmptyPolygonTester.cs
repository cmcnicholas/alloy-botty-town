using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sample;
public class EmptyPolygonTester : MonoBehaviour {
    List<Vertex> vertices = new List<Vertex>();
    List<Edge> edges = new List<Edge>();

	void Awake () {
        CreatePolygon();

        ShowMedialPoints();
	}

    void ShowMedialPoints()
    {
        var medialPoints = Skeleton.Finder.getSkeleton(vertices.Select(x => x.pos).ToList()).ToList();

        foreach (var p in medialPoints)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.localScale = new Vector3(0.175f, 0.175f, 0.175f);
            go.transform.position = p;
            go.name = "medial point";
            go.GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    void CreatePolygon()
    {
        vertices.Add(Vertex.CreateVertex(new Vector3(0.0f, 2.0f, 0.0f)));
        vertices.Add(Vertex.CreateVertex(new Vector3(1.0f, 2.0f, 0.0f)));
        vertices.Add(Vertex.CreateVertex(new Vector3(1.0f, 1.0f, 0.0f)));
        vertices.Add(Vertex.CreateVertex(new Vector3(2.0f, 1.0f, 0.0f)));
        vertices.Add(Vertex.CreateVertex(new Vector3(2.0f, 0.0f, 0.0f)));
        vertices.Add(Vertex.CreateVertex(new Vector3(0.0f, 0.0f, 0.0f)));
        
        edges.Add(Edge.CreateEdge(vertices[0], vertices[1]));
        edges.Add(Edge.CreateEdge(vertices[1], vertices[2]));
        edges.Add(Edge.CreateEdge(vertices[2], vertices[3]));
        edges.Add(Edge.CreateEdge(vertices[3], vertices[4]));
        edges.Add(Edge.CreateEdge(vertices[4], vertices[5]));
        edges.Add(Edge.CreateEdge(vertices[5], vertices[0]));
    }
}
