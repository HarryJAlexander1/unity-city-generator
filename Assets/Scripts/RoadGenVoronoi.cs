using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadGenVoronoi : MonoBehaviour
{

    List<Triangle> triangles;

    private class Vertex {

        private Vector3Int _position;
        private int _id;

        public Vertex(Vector3Int position, int id)
        {
            _position = position;
            _id = id;
        }

        // getters and setters
        public Vector3Int Position
        {

            get { return _position; }
            set { _position = value; }
        }

        public int Id
        {

            get { return _id; }
            set { _id = value; }
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

        private Vector3 _circumcenter;
        private float _circumradius;

        public Triangle(Vertex vertexA, Vertex vertexB, Vertex vertexC) {
            _vertexA = vertexA;
            _vertexB = vertexB;
            _vertexC = vertexC;

            Vector2 circumcenterVector2 = GetCircumcenter(this);
            _circumcenter = new(circumcenterVector2.x, 0, circumcenterVector2.y);
            _circumradius = Vector2.Distance(circumcenterVector2, new(VertexA.Position.x, VertexA.Position.z));

        }

        // getter and setters
        public Vertex VertexA {

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

        public Vector3 Circumcenter
        {

            get { return _circumcenter; }
            set { _circumcenter = value; }
        }
        public float Circumradius
        {

            get { return _circumradius; }
            set { _circumradius = value; }
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

        //foreach (int vertex in squareVertices) {
        //    Debug.Log(vertex);
        //}

        var dX = (maxX - minX) * 2;
        var dZ = (maxZ - minZ) * 2;

        Vertex v1 = new Vertex(new(minX - dX, 0,  minZ - dZ * 3), 1);
        Vertex v2 = new Vertex(new(minX - dX, 0, maxZ + dZ), 2);
        Vertex v3 = new Vertex(new(maxX + dX * 3, 0, maxZ + dZ), 3);

        //Edge e1 = new Edge(v1, v3);
        //Edge e2 = new Edge(v2, v3);
        //Edge e3 = new Edge(v1, v2);

        Triangle superTriangle = new Triangle(v1, v2, v3);

        //Debug.Log("Edge A: Vertex A "+ superTriangle.VertexA.Position);
        //Debug.Log("Edge A: Vertex B " + superTriangle.VertexB.Position);

        //Debug.Log("Edge B: Vertex A " + superTriangle.VertexA.Position);
        //Debug.Log("Edge B: Vertex B " + superTriangle.VertexB.Position);

        //Debug.Log("Edge C: Vertex A " + superTriangle.VertexA.Position);
        //Debug.Log("Edge C: Vertex B " + superTriangle.VertexB.Position);

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

        Vector3 circumcenter = triangle.Circumcenter;
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

        //triangles.Add(new Triangle(pointList[0], pointList[1], pointList[2]));


        //List<Triangle> badTriangles = new();

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
                triangles.Add(new Triangle(edges[x].VertexA, edges[x].VertexB, pointList[n]));
            }

        }
        triangles = triangles.Where(triangle =>
    triangle.VertexA != superTriangle.VertexA && triangle.VertexA != superTriangle.VertexB && triangle.VertexA != superTriangle.VertexC &&
    triangle.VertexB != superTriangle.VertexA && triangle.VertexB != superTriangle.VertexB && triangle.VertexB != superTriangle.VertexC &&
    triangle.VertexC != superTriangle.VertexA && triangle.VertexC != superTriangle.VertexB && triangle.VertexC != superTriangle.VertexC
).ToList();

        return triangles;
    }

    private void OnDrawGizmos()
    {
        if (triangles == null)
            return;

        Gizmos.color = Color.white;

        foreach (Triangle triangle in triangles)
        {
            Gizmos.DrawLine(triangle.VertexA.Position, triangle.VertexB.Position);
            Gizmos.DrawLine(triangle.VertexB.Position, triangle.VertexC.Position);
            Gizmos.DrawLine(triangle.VertexC.Position, triangle.VertexA.Position);
        }
    }

private void Awake()
    {
        Vertex v1 = new Vertex(new(130, 0, 320), 1);
        Vertex v2 = new Vertex(new(50, 0, 150), 2);
        Vertex v3 = new Vertex(new(110, 0, 210), 3);
        Vertex v4 = new Vertex(new(290, 0, 80), 4);
        Vertex v5 = new Vertex(new(300, 0, 668), 5);
        Vertex v6 = new Vertex(new(87, 0, 23), 6);
        Vertex v7 = new Vertex(new(279, 0, 51), 7);
        Vertex v8 = new Vertex(new(201, 0, 89), 8);
        Vertex v9 = new Vertex(new(65, 0, 173), 8);
        Vertex v10 = new Vertex(new(334, 0, 41), 8);
        Vertex v11 = new Vertex(new(96, 0, 152), 8);
        Vertex v12 = new Vertex(new(562, 0, 341), 8);
        Vertex v13 = new Vertex(new(222, 0, 370), 8);
        Vertex v14 = new Vertex(new(16, 0, 31), 8);
        Vertex v15 = new Vertex(new(169, 0, 475), 8);

        List<Vertex> points = new();
        points.Add(v1);
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

        //Edge e1 = new Edge(v1, v3);
        //Edge e2 = new Edge(v2, v3);
        //Edge e3 = new Edge(v1, v2);

        //Triangle testTriangle = new(v1,v2,v3);

        //Vertex testPoint = new(new(7, 0, 14), 1);

        //Debug.Log("Point is in circumcircle: " + IsPointInCircumcircle(testPoint, testTriangle));
        //Debug.Log("Circumcenter of triangle: " + testTriangle.Circumcenter);
        //Debug.Log("Circumradius of triangle: " + testTriangle.Circumradius);

        triangles = Triangulate(points);
        foreach (Triangle t in triangles) {
            Debug.Log("Triangle vertices: " + t.VertexA.Position + t.VertexB.Position + t.VertexC.Position);
        }
    }
}
