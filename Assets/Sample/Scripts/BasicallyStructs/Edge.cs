using UnityEngine;
using System.Collections;

namespace Sample
{
    public class Edge : MonoBehaviour
    {
        public Vertex v0;
        public Vertex v1;
        public int index;
        public LineRenderer lineRenderer;
        // Update is called once per frame
        void Update()
        {
            //draw line
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetColors(Color.blue, Color.blue);
            lineRenderer.SetWidth(0.025f, 0.025f);
            lineRenderer.SetPosition(0, v0.pos);
            lineRenderer.SetPosition(1, v1.pos);
        }

        public static int iteration = 0;
        public static Edge CreateEdge(Vertex v0, Vertex v1)
        {
            GameObject go = new GameObject("Edge " + iteration.ToString());

            Edge e = go.AddComponent<Edge>();
            e.v0 = v0;
            e.v1 = v1;
            e.lineRenderer = go.AddComponent<LineRenderer>();
            e.index = iteration;

            iteration++;
            return e;
        }
    }
}