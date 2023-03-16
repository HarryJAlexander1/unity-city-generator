using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadGenVoronoi : MonoBehaviour
{
    List<Triangle> triangles;
    Dictionary<Triangle, List<Triangle>> trianglesAndNeighbours;
    List<Edge> voronoiEdges;

    public GameObject segmentPrefab;

    private class Vertex {

        private Vector3Int _position;
        public Vertex(Vector3Int position)
        {
            _position = position;
        }

        // getters and setters
        public Vector3Int Position
        {
            get { return _position; }
            set { _position = value; }
        }
    }

    private class Edge {

        // these variables represent the two endpoints of the edge
        private Vertex _vertexA;
        private Vertex _vertexB;

        private double _length;

        public Edge(Vertex vertexA, Vertex vertexB) {
            _vertexA = vertexA;
            _vertexB = vertexB;
            _length = Mathf.Sqrt(Mathf.Pow(vertexB.Position.x - vertexA.Position.x, 2) + Mathf.Pow(vertexB.Position.z - vertexA.Position.z, 2));
        }

        // getters and setters
        public Vertex VertexA
        {
            get { return _vertexA; }
            set { _vertexA = value; }
        }
        public Vertex VertexB
        {
            get { return _vertexB; }
            set { _vertexB = value; }
        }

        public double Length 
        {
            get { return _length;}
            set { _length = value; }
        }
    }

    private class Triangle {

        private Vertex _vertexA;
        private Vertex _vertexB;
        private Vertex _vertexC;

        private Vertex _circumcenter;
        private float _circumradius;

        public Triangle(Vertex vertexA, Vertex vertexB, Vertex vertexC) {

            _vertexA = vertexA;
            _vertexB = vertexB;
            _vertexC = vertexC;

            Vector2 circumcenterVector2 = GetCircumcenter(this);
            _circumcenter = new(new((int)circumcenterVector2.x, 0, (int)circumcenterVector2.y));
            _circumradius = Vector2.Distance(circumcenterVector2, new(VertexA.Position.x, VertexA.Position.z));
        }

        // getter and setters
        public Vertex VertexA 
        {
            get { return _vertexA; }
            set { _vertexA = value; }
        }
        public Vertex VertexB
        {
            get { return _vertexB; }
            set { _vertexB = value; }
        }
        public Vertex VertexC
        {
            get { return _vertexC; }
            set { _vertexC = value; }
        }

        public Vertex Circumcenter
        {
            get { return _circumcenter; }
            set { _circumcenter = value; }
        }
        public float Circumradius
        {
            get { return _circumradius; }
            set { _circumradius = value; }
        }
       
        public bool SharesEdge(Triangle other)
        {
            int sharedVertices = 0;
            if (_vertexA == other._vertexA || _vertexA == other._vertexB || _vertexA == other._vertexC)
            {
                sharedVertices++;
            }
            if (_vertexB == other._vertexA || _vertexB == other._vertexB || _vertexB == other._vertexC)
            {
                sharedVertices++;
            }
            if (_vertexC == other._vertexA || _vertexC == other._vertexB || _vertexC == other._vertexC)
            {
                sharedVertices++;
            }
            return sharedVertices == 2;
        }
    }
  //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private Triangle InitialiseSuperTriangle(List<Vertex> pointList) {

        var maxX = 0;
        var minX = 1000;

        var maxZ = 0;
        var minZ = 1000;

        List<int> squareVertices = new();
        foreach (Vertex point in pointList) {
            maxX = Mathf.Max(maxX, point.Position.x);
            minX = Mathf.Min(minX, point.Position.x);

            maxZ = Mathf.Max(maxZ, point.Position.z);
            minZ = Mathf.Min(minZ, point.Position.z);
        }

        squareVertices.Add(maxX);
        squareVertices.Add(minX);
        squareVertices.Add(maxZ);
        squareVertices.Add(minZ);

        var dX = (maxX - minX) * 2;
        var dZ = (maxZ - minZ) * 2;

        Vertex v1 = new Vertex(new(minX - dX, 0,  minZ - dZ * 3));
        Vertex v2 = new Vertex(new(minX - dX, 0, maxZ + dZ));
        Vertex v3 = new Vertex(new(maxX + dX * 3, 0, maxZ + dZ));

        Triangle superTriangle = new Triangle(v1, v2, v3);

        return superTriangle;
    }

    private static Vector2 GetCircumcenter(Triangle triangle)
    {
        Vector2 pointA = new(triangle.VertexA.Position.x, triangle.VertexA.Position.z);
        Vector2 pointB = new(triangle.VertexB.Position.x, triangle.VertexB.Position.z);
        Vector2 pointC = new(triangle.VertexC.Position.x, triangle.VertexC.Position.z);

        LinearEquation lineAB = new LinearEquation(pointA, pointB);
        LinearEquation lineBC = new LinearEquation(pointB, pointC);

        Vector2 midPointAB = Vector2.Lerp(pointA, pointB, .5f);
        Vector2 midPointBC = Vector2.Lerp(pointB, pointC, .5f);

        LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
        LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);

        Vector2 circumcenter = GetCrossingPoint(perpendicularAB, perpendicularBC);

        float circumRadius = Vector2.Distance(circumcenter, pointA);
        //Debug.Log(circumRadius);
        return circumcenter;
    }

    static Vector2 GetCrossingPoint(LinearEquation line1, LinearEquation line2)
    {
        float A1 = line1._A;
        float A2 = line2._A;
        float B1 = line1._B;
        float B2 = line2._B;
        float C1 = line1._C;
        float C2 = line2._C;

        //Cramer's rule
        float Determinant = A1 * B2 - A2 * B1;
        float DeterminantX = C1 * B2 - C2 * B1;
        float DeterminantY = A1 * C2 - A2 * C1;

        float x = DeterminantX / Determinant;
        float y = DeterminantY / Determinant;

        return new Vector2(x, y);
    }
    public class LinearEquation
    {
        public float _A;
        public float _B;
        public float _C;
        public LinearEquation() { }

        //Ax+By=C
        public LinearEquation(Vector2 pointA, Vector2 pointB)
        {
            float deltaX = pointB.x - pointA.x;
            float deltaY = pointB.y - pointA.y;
            _A = deltaY; //y2-y1
            _B = -deltaX; //x1-x2
            _C = _A * pointA.x + _B * pointA.y;
        }

        public LinearEquation PerpendicularLineAt(Vector3 point)
        {
            LinearEquation newLine = new LinearEquation();

            newLine._A = -_B;
            newLine._B = _A;
            newLine._C = newLine._A * point.x + newLine._B * point.y;

            return newLine;
        }
    }

    private bool IsPointInCircumcircle(Vertex point, Triangle triangle) {

        Vector3 circumcenter = triangle.Circumcenter.Position;
        float circumradius = triangle.Circumradius;

        float R = Mathf.Sqrt(Mathf.Pow(circumcenter.x - point.Position.x, 2) + Mathf.Pow(circumcenter.z - point.Position.z, 2));

        if (R <= circumradius) {
            return true;
        }
        else {
            return false;
        }
    }

    private List<Triangle> Triangulate(List<Vertex> pointList) {

        List<Triangle> triangles = new();

        Triangle superTriangle = InitialiseSuperTriangle(pointList);

        triangles.Add(superTriangle);

        for (int n = 0; n < pointList.Count; n++) {
            List<Edge> edges = new List<Edge>();
            
            for (int i = triangles.Count-1; i >= 0; i--) {
                // if point is inside triangle circumcircle
                if (IsPointInCircumcircle(pointList[n], triangles[i])) {
                    // add triangle's edges to edges
                    edges.Add(new Edge(triangles[i].VertexA, triangles[i].VertexB));
                    edges.Add(new Edge(triangles[i].VertexB, triangles[i].VertexC));
                    edges.Add(new Edge(triangles[i].VertexC, triangles[i].VertexA));
                    // remove current triangle from list
                    triangles.RemoveAt(i);
                }
            }

            // Remove any duplicate edges from the list
            for (int j = 0; j < edges.Count; j++) {
                for (int k = j+1; k < edges.Count; k++) {
                    if (edges[j].VertexA == edges[k].VertexB && edges[j].VertexB == edges[k].VertexA) {
                        edges.RemoveAt(k);
                        edges.RemoveAt(j);
                        break;
                    }
                }
            }
        
            // Create new triangles from the edges and current point and add to list
            for (int x = 0; x < edges.Count; x++) {
                Edge ab = new(edges[x].VertexA, edges[x].VertexB);
                Edge bc = new(edges[x].VertexB, pointList[n]);
                Edge ca = new(pointList[n], edges[x].VertexA);

                triangles.Add(new Triangle(edges[x].VertexA, edges[x].VertexB, pointList[n]));
            }

        }
        triangles = triangles.Where(triangle =>
        triangle.VertexA != superTriangle.VertexA && triangle.VertexA != superTriangle.VertexB && triangle.VertexA != superTriangle.VertexC &&
        triangle.VertexB != superTriangle.VertexA && triangle.VertexB != superTriangle.VertexB && triangle.VertexB != superTriangle.VertexC &&
        triangle.VertexC != superTriangle.VertexA && triangle.VertexC != superTriangle.VertexB && triangle.VertexC != superTriangle.VertexC).ToList();

        return triangles;
    }

    private Dictionary<Triangle, List<Triangle>> CalculateNeighbors(List<Triangle> triangles)
    {
        Dictionary<Triangle, List<Triangle>> neighbors = new Dictionary<Triangle, List<Triangle>>();

        foreach (Triangle t in triangles)
        {
            neighbors[t] = new List<Triangle>();
        }
        foreach (Triangle t in triangles)
        {
            foreach (Triangle u in triangles)
            {
                if (t != u && t.SharesEdge(u))
                {
                    neighbors[t].Add(u);
                }
            }
        }
        return neighbors;
    }

    private List<Edge> CreateVoronoiDiagram(Dictionary<Triangle, List<Triangle>> trianglesAndNeighbours)
    {
        List<Edge> voronoiEdges = new();

        foreach (KeyValuePair<Triangle, List<Triangle>> entry in trianglesAndNeighbours)
        {
            // iterate through the list of strings associated to the key
            foreach (Triangle value in entry.Value)
            {
                // Create edges between each triangle's circumcenter and its neighbour's circumcenter
                Edge voronoiEdge = new(entry.Key.Circumcenter, value.Circumcenter);
                voronoiEdges.Add(voronoiEdge);
            }
        }
        return voronoiEdges;
    }

    private void SpawnRoads(List<Edge> edges) {

        List<Vector3> midpoints = new();
        bool duplicatePosition;

        foreach (Edge edge in edges) {
            duplicatePosition = false;
            // Calculate the midpoint and set the position
            Vector3 midpoint = (edge.VertexA.Position + edge.VertexB.Position) / 2;

            // iterate through list of previous midpoints to prevent duplicate road segments
            for (int i = 0; i < midpoints.Count; i++) {
                if (midpoint == midpoints[i]) { duplicatePosition = true; }
            }

            if (!duplicatePosition) {

                // Calculate the rotation based on the direction of the edge
                Vector3 direction = ((Vector3)edge.VertexB.Position - (Vector3)edge.VertexA.Position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                // Instantiate a new road prefab
                GameObject road = Instantiate(segmentPrefab, midpoint, rotation);
                midpoints.Add(midpoint);

                // Set the length
                road.transform.localScale = new(road.transform.localScale.x * 5, road.transform.localScale.y, (road.transform.localScale.z * (float)edge.Length));
            }  
        }
    }

  /*  private void OnDrawGizmos()
    {
        if (triangles == null)
            return;

        Gizmos.color = Color.white;
        for (int i = 0; i < triangles.Count; i++)
        {
            Gizmos.DrawLine(triangles[i].VertexA.Position, triangles[i].VertexB.Position);
            Gizmos.DrawLine(triangles[i].VertexB.Position, triangles[i].VertexC.Position);
            Gizmos.DrawLine(triangles[i].VertexC.Position, triangles[i].VertexA.Position);

            //Gizmos.DrawSphere(triangles[i].Circumcenter.Position, 3f);
        }

        Gizmos.color = Color.black;
        foreach (Edge edge in voronoiEdges)
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawSphere(edge.VertexA.Position, 3f);
            //Gizmos.DrawSphere(edge.VertexB.Position, 3f);
            Gizmos.DrawLine(edge.VertexA.Position, edge.VertexB.Position);
        }

        Vertex v1 = new Vertex(new(200, 0, 1));
        Vertex v2 = new Vertex(new(100, 0, 150));
        Vertex v3 = new Vertex(new(300, 0, 75));
        Vertex v4 = new Vertex(new(50, 0, 50));
        Vertex v5 = new Vertex(new(100, 0, 100));
        Vertex v6 = new Vertex(new(175, 0, 20));
        Vertex v7 = new Vertex(new(180, 0, 180));
        Vertex v8 = new Vertex(new(201, 0, 89));
        Vertex v9 = new Vertex(new(65, 0, 173));
        Vertex v10 = new Vertex(new(334, 0, 98));
        Vertex v11 = new Vertex(new(-100, 0, 152));
        Vertex v12 = new Vertex(new(250, 0, 500));
        Vertex v13 = new Vertex(new(-100, 0, -200));
        Vertex v14 = new Vertex(new(16, 0, 31));
        Vertex v15 = new Vertex(new(169, 0, 475));
        Vertex v16 = new Vertex(new(300, 0, -200));
        Vertex v17 = new Vertex(new(120, 0, -234));

        List<Vertex> testPoints = new();
        //testPoints.Add(v1);
        testPoints.Add(v2);
        testPoints.Add(v3);
        testPoints.Add(v4);
        testPoints.Add(v5);
        testPoints.Add(v6);
        testPoints.Add(v7);
        testPoints.Add(v8);
        testPoints.Add(v9);
        testPoints.Add(v10);
        testPoints.Add(v11);
        testPoints.Add(v12);
        testPoints.Add(v13);
        testPoints.Add(v14);
        testPoints.Add(v15);
        testPoints.Add(v16);
        testPoints.Add(v17);

        Gizmos.color = Color.red;
        foreach (Vertex point in testPoints)
        {
            Gizmos.DrawSphere(point.Position, 3);
        }
    }*/

private void Awake()
    {
        Vertex v1 = new Vertex(new(2000, 0, 10));
        Vertex v2 = new Vertex(new(1000, 0, 1500));
        Vertex v3 = new Vertex(new(3000, 0, 750));
        Vertex v4 = new Vertex(new(500, 0, 500));
        Vertex v5 = new Vertex(new(1000, 0, 1000));
        Vertex v6 = new Vertex(new(1750, 0, 200));
        Vertex v7 = new Vertex(new(1800, 0, 1800));
        Vertex v8 = new Vertex(new(2010, 0, 890));
        Vertex v9 = new Vertex(new(650, 0, 1730));
        Vertex v10 = new Vertex(new(3340, 0, 980));
        Vertex v11 = new Vertex(new(-1000, 0, 1520));
        Vertex v12 = new Vertex(new(2500, 0, 5000));
        Vertex v13 = new Vertex(new(-1000, 0, -2000));
        Vertex v14 = new Vertex(new(160, 0, 310));
        Vertex v15 = new Vertex(new(1690, 0, 4750));
        Vertex v16 = new Vertex(new(3000, 0, -2000));
        Vertex v17 = new Vertex(new(1200, 0, -2340));
        List<Vertex> points = new();
        //points.Add(v1);
        points.Add(v2);
        points.Add(v3);
        points.Add(v4);
        points.Add(v5);
        points.Add(v6);
        points.Add(v7);
        points.Add(v8);
        points.Add(v9);
        points.Add(v10);
        points.Add(v11);
        points.Add(v12);
        points.Add(v13);
        points.Add(v14);
        points.Add(v15);
        points.Add(v16);
        points.Add(v17);

        triangles = Triangulate(points);
        trianglesAndNeighbours = CalculateNeighbors(triangles);
        voronoiEdges = CreateVoronoiDiagram(trianglesAndNeighbours);
        SpawnRoads(voronoiEdges);
    }
}
