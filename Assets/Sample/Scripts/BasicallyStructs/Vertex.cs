using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Sample
{

    public class Vertex : MonoBehaviour
    {
        public int index;
        public Vector3 pos = new Vector3();
        public List<Edge> edges = new List<Edge>();

        //public float tempCost = 9999f;

        //for pathfinding
        public List<Vertex> neighbors = new List<Vertex>();

        public void findNeighbors()
        {
            neighbors.Clear();
            foreach (Edge e in edges)
            {
                if (e.v0 == this) neighbors.Add(e.v1);
                else if (e.v0 != this) neighbors.Add(e.v0);
            }
        }

        public void OnMouseOver()
        {
            Debug.Log("index: " + index.ToString());
        }

        public static int iteration = 0;
        public static Vertex CreateVertex(Vector3 pos)
        {
            GameObject go = new GameObject("Vetex " + iteration.ToString());
            go.transform.position = pos;

            BoxCollider bc = go.AddComponent<BoxCollider>();

            bc.size = new Vector3(0.1f, 0.1f, 0.1f);

            Vertex v = go.AddComponent<Vertex>();
            v.index = iteration;
            v.pos = pos;

            iteration++;
            return v;
        }
    }
}