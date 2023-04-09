
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadGenVoronoi : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vertex> points;
    public HashSet<Triangle> triangleList;
    public Dictionary<Triangle, HashSet<Triangle>> trianglesAndNeighbours;
    public List<Edge> voronoiEdgeList;
    public List<Cell> voronoiCells;
    public List<Vertex> uniqueTriangleVertices;
    public List<Vertex> convexHull = new();
    public List<GameObject> roadSegments;
    public List<GameObject> meshGenerators;
    //List<Edge> infiniteEdges;
    public List<GameObject> pointGameObjects = new();

    public GameObject segmentPrefab;
    public GameObject gameManager;

    private GameObject city;

    public class Vertex {

        private int _id;
        private Vector3 _position;
        public Vertex(Vector3 position, int id)
        {
            _position = position;
            _id = id;
        }

        // getters and setters
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public int Id {
            get { return _id; }
            set { _id = value; }
        }
    }

    public class Edge {

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
            get { return _length; }
            set { _length = value; }
        }
    }

    public class Triangle {

        private Vertex _vertexA;
        private Vertex _vertexB;
        private Vertex _vertexC;

        private Edge _edgeAB;
        private Edge _edgeBC;
        private Edge _edgeCA;

        private List<Edge> _edges = new();

        private Vertex _circumcenter;
        private float _circumradius;

        public Triangle(Vertex vertexA, Vertex vertexB, Vertex vertexC) {

            _vertexA = vertexA;
            _vertexB = vertexB;
            _vertexC = vertexC;

            Vector2 circumcenterVector2 = GetCircumcenter(this);
            _circumcenter = new(new(circumcenterVector2.x, 0, circumcenterVector2.y), 0);
            _circumradius = Vector2.Distance(circumcenterVector2, new(VertexA.Position.x, VertexA.Position.z));

            _edgeAB = new(_vertexA, _vertexB);
            _edgeBC = new(_vertexB, _vertexC);
            _edgeCA = new(_vertexC, _vertexA);

            _edges.Add(_edgeAB);
            _edges.Add(_edgeBC);
            _edges.Add(_edgeCA);
        }
        public Edge EdgeAB {
            get { return _edgeAB; }
            set { _edgeAB = value; }
        }
        public Edge EdgeBC
        {
            get { return _edgeBC; }
            set { _edgeBC = value; }
        }
        public Edge EdgeCA
        {
            get { return _edgeCA; }
            set { _edgeCA = value; }
        }

        public List<Edge> GetEdges() {
            return _edges;
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

    public class Cell
    {
        private List<Vertex> _vertices;
     
        private Vertex _site;

        private float _maxX;
        private float _minX;

        private float _maxZ;
        private float _minZ;
        public Cell(List<Vertex> vertices, Vertex site)
        {
            _vertices = vertices;
            _site = site;
   
        }

        public List<Vertex> Vertices {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public Vertex Site {
            get { return _site; }
            set { _site = value; }
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public List<Vertex> CreatePointsFromGameObjectPositions(List<GameObject> pointGOs) {
        // initialise list of vertices
        List<Vertex> vertices = new();
        // iterate through each pointGO
        for (int i = 0; i < pointGOs.Count; i++) {
            // instantiate new Vertex
            Vertex p = new(pointGOs[i].transform.position, i);
            vertices.Add(p);
            pointGameObjects.Add(pointGOs[i]);
        }
        return vertices;
    }
    private Triangle InitialiseSuperTriangle(List<Vertex> pointList) {

        var maxX = 0f;
        var minX = 10000f;

        var maxZ = 0f;
        var minZ = 10000f;

        List<float> squareVertices = new();
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

        Vertex v1 = new Vertex(new(minX - dX, 0, minZ - dZ * 3), 1);
        Vertex v2 = new Vertex(new(minX - dX, 0, maxZ + dZ), 2);
        Vertex v3 = new Vertex(new(maxX + dX * 3, 0, maxZ + dZ), 3);

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

    private bool IsPointInCircumcircle(Vertex point, Triangle triangle)
    {
        Vector3 circumcenter = triangle.Circumcenter.Position;
        float circumradius = triangle.Circumradius;

        float R = Mathf.Sqrt(Mathf.Pow(circumcenter.x - point.Position.x, 2) + Mathf.Pow(circumcenter.z - point.Position.z, 2));

        //float epsilon = 1e-12f; // You can adjust this value based on the level of precision you need

        if (R < circumradius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public HashSet<Triangle> Triangulate(List<Vertex> pointList)
    {
        HashSet<Triangle> triangles = new();

        Triangle superTriangle = InitialiseSuperTriangle(pointList);

        triangles.Add(superTriangle);

        for (int n = 0; n < pointList.Count; n++)
        {
            HashSet<Triangle> badTriangles = new();

            for (int i = 0; i < triangles.Count; i++)
            {
                // if point is inside triangle circumcircle
                if (IsPointInCircumcircle(pointList[n], triangles.ToList()[i]))
                {
                    badTriangles.Add(triangles.ToList()[i]);
                }
            }
            List<Edge> polygon = new();
            // iterate through all bad triangles
            for (int i = 0; i < badTriangles.Count; i++)
            {
                // iterate through edges
                foreach (Edge edge in badTriangles.ToList()[i].GetEdges())
                {
                    // set invalidEdge to false
                    bool invalidEdge = false;
                    // look at all other triangles in list
                    for (int j = 0; j < badTriangles.Count; j++)
                    {
                        if (i == j) { continue; }
                        // get all the edges of all other triangles
                        foreach (Edge edge1 in badTriangles.ToList()[j].GetEdges())
                        {
                            //compare the vertices of both edges
                            if (edge.VertexA == edge1.VertexA && edge.VertexB == edge1.VertexB || edge.VertexA == edge1.VertexB && edge1.VertexA == edge.VertexB)
                            {
                                // if both vertex A and B are the same, set invalidEdge to true
                                invalidEdge = true;
                            }
                        }
                    }

                    if (!invalidEdge)
                    {
                        polygon.Add(edge);
                        //Debug.Log("This edge has been added to the polygon");
                    }
                }
            }
            foreach (Triangle triangle in badTriangles)
            {
                triangles.Remove(triangle);
            }
            foreach (Edge edge in polygon)
            {
                Triangle newTriangle = new(edge.VertexA, edge.VertexB, pointList[n]);
                triangles.Add(newTriangle);
            }
        }
        // remove triangles whose edge is connected to the supertriangle
        triangles = triangles.Where(triangle =>
        triangle.VertexA != superTriangle.VertexA && triangle.VertexA != superTriangle.VertexB && triangle.VertexA != superTriangle.VertexC &&
        triangle.VertexB != superTriangle.VertexA && triangle.VertexB != superTriangle.VertexB && triangle.VertexB != superTriangle.VertexC &&
        triangle.VertexC != superTriangle.VertexA && triangle.VertexC != superTriangle.VertexB && triangle.VertexC != superTriangle.VertexC).ToHashSet<Triangle>();

        return triangles;
    }

    private void CalculateNeighbors(HashSet<Triangle> triangles)
    {
        Dictionary<Triangle, HashSet<Triangle>> neighbors = new Dictionary<Triangle, HashSet<Triangle>>();

        foreach (Triangle t in triangles)
        {
            neighbors[t] = new HashSet<Triangle>();
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
        trianglesAndNeighbours = neighbors;
    }

    private void ComputeConvexHull(List<Vertex> points)
    {

        if (points.Count < 4)
        {
            throw new System.Exception("Convex hull requires at least 4 points.");
        }

        int leftMostIndex = 0;
        for (int i = 1; i < points.Count; i++)
        {
            if (points[i].Position.x < points[leftMostIndex].Position.x)
            {
                leftMostIndex = i;
            }
        }

        HashSet<Vertex> hull = new HashSet<Vertex>();
        int currentIndex = leftMostIndex;
        int nextIndex;

        do
        {
            hull.Add(points[currentIndex]);
            nextIndex = (currentIndex + 1) % points.Count;

            for (int i = 0; i < points.Count; i++)
            {
                if (Orientation(points[currentIndex], points[nextIndex], points[i]) < 0)
                {
                    nextIndex = i;
                }
            }

            currentIndex = nextIndex;
        } while (currentIndex != leftMostIndex);

        convexHull = new List<Vertex>(hull);
    }

    private float Orientation(Vertex a, Vertex b, Vertex c)
    {
        return (b.Position.x - a.Position.x) * (c.Position.z - a.Position.z) - (b.Position.z - a.Position.z) * (c.Position.x - a.Position.x);
    }
    // DOES NOT WORK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //private List<Edge> CalculateOuterEdges(HashSet<Triangle> trianglulation, int maxLength) {
    //    List<Edge> outerEdges = new();
    //    List<Vertex> triangleVertices = new();
    //    foreach (Triangle t in trianglulation)
    //    {
    //        triangleVertices.Add(t.VertexA);
    //        triangleVertices.Add(t.VertexB);
    //        triangleVertices.Add(t.VertexC);
    //    }
    //    HashSet<Vector3> uniquePositions = new HashSet<Vector3>();
    //    List<Vertex> uniqueVertices = new List<Vertex>();

    //    foreach (Vertex vertex in triangleVertices)
    //    {
    //        if (uniquePositions.Add(vertex.Position))
    //        {
    //            uniqueVertices.Add(vertex);
    //        }
    //    }

    //    // Now uniqueVertices contains only the unique vertices based on their positions
    //    Debug.Log("Unique vertices count: " + uniqueVertices.Count);

    //    triangulationConvexHull = ComputeConvexHull(uniqueVertices);

    //    HashSet<Edge> visitedEdges = new HashSet<Edge>(); // To avoid processing the same edge twice
    //    // iterate through each triangle in triangulation
    //    foreach (Triangle t in trianglulation) {
    //        // get edges of each triangle
    //        foreach (Edge e in t.GetEdges()) {
    //            // if edge hasnt been processed and convexhull contains vertices A and B of edge 
    //            if (!visitedEdges.Contains(e) && triangulationConvexHull.Contains(e.VertexA) && triangulationConvexHull.Contains(e.VertexB)) {

    //                // get circumcenter of triangle
    //                Vertex startPoint = t.Circumcenter;
    //                // get edges of the triangle
    //                List<Edge> triangleEdges = new();
    //                foreach (Edge edge in t.GetEdges()) {
    //                    triangleEdges.Add(edge);
    //                }
    //                Vector3 crossProduct = CrossProduct(triangleEdges[0], triangleEdges[1]);

    //                // Normalize the cross product
    //                Vector3 normal = crossProduct.normalized;
    //                int length = 3000;
    //                Vector3Int normalTimesLength = new((int)normal.x * length, (int)normal.y * length, (int)normal.z * length);
    //                Vector3Int endpointPosition = startPoint.Position + normalTimesLength;

    //                // Create an edge between the Voronoi vertex and the endpoint
    //                // You can use the endpoint and the Voronoi vertex to create a line segment or an edge in Unity or other game engines
    //                Vertex endpoint = new(endpointPosition, 1);

    //                Edge outerEdge = new(startPoint, endpoint); 
    //                outerEdges.Add(outerEdge);
    //            }
    //            visitedEdges.Add(e);
    //        }
    //    }
    //    return outerEdges;
    //}

  /*  static Vector3 CrossProduct(Edge edge1, Edge edge2)
    {
        // Convert the vertices to float arrays
        float[] v1 = new float[] { edge1.VertexA.Position.x, edge1.VertexA.Position.y, edge1.VertexA.Position.z };
        float[] v2 = new float[] { edge1.VertexB.Position.x, edge1.VertexB.Position.y, edge1.VertexB.Position.z };
        float[] v3 = new float[] { edge2.VertexB.Position.x, edge2.VertexB.Position.y, edge2.VertexB.Position.z };

        // Calculate the edge vectors
        float[] e1 = new float[] { v2[0] - v1[0], v2[1] - v1[1], v2[2] - v1[2] };
        float[] e2 = new float[] { v3[0] - v1[0], v3[1] - v1[1], v3[2] - v1[2] };

        // Calculate the cross product of the edge vectors
        float[] cross = new float[] {
            e1[1] * e2[2] - e1[2] * e2[1],
            e1[2] * e2[0] - e1[0] * e2[2],
            e1[0] * e2[1] - e1[1] * e2[0]
        };

        // Return the cross product as a Vector3 object
        return new Vector3(cross[0], cross[1], cross[2]);
    }*/

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Create Edge objects for the diagram by looking at each triangle and its neighbours
    private void GetVoronoiEdges(Dictionary<Triangle, HashSet<Triangle>> trianglesAndNeighbours)
    {
        List<Edge> voronoiEdges = new();

        foreach (KeyValuePair<Triangle, HashSet<Triangle>> entry in trianglesAndNeighbours)
        {
            // iterate through the list of strings associated to the key
            foreach (Triangle value in entry.Value)
            {
                // Create edges between each triangle's circumcenter and its neighbour's circumcenter
                Edge voronoiEdge = new(entry.Key.Circumcenter, value.Circumcenter);
                voronoiEdges.Add(voronoiEdge);
            }
        }
        voronoiEdgeList = voronoiEdges;
    }

    // Creates Cell objects from the cells in the voronoi diagram by looking at the original sites and triangulation
    private void GetVoronoiCells(List<Vertex> sites, HashSet<Triangle> triangles)
    {
        List<Cell> cells = new();

        foreach (Vertex site in sites) {

            List<Vertex> circumcenters = new();

            foreach (Triangle triangle in triangles) {

                if (triangle.VertexA.Position == site.Position || triangle.VertexB.Position == site.Position || triangle.VertexC.Position == site.Position) {
                    circumcenters.Add(triangle.Circumcenter);
                }
            }
            Cell cell = new(circumcenters, site);
            cells.Add(cell);
        }
        voronoiCells = cells;
    }

    public HashSet<Triangle> ComputeBuildingMesh(Cell cell) {

        float height = Random.Range(10f, 160f);
        // get the vertices for the bottom side of mesh
        List<Vertex> meshBottomVertices = new();
        // get vertices for the top side of mesh
        List<Vertex> meshTopVertices = new();
        foreach (Vertex v in cell.Vertices) {

            meshBottomVertices.Add(v);
            Vertex v2 = new(new(v.Position.x, v.Position.y + height, v.Position.z), 0);
            meshTopVertices.Add(v2);
        }

        List<Vertex> completeMeshVertices = new();
        // combine vertices
        foreach (Vertex v in meshBottomVertices) {
            completeMeshVertices.Add(v);
        }
        foreach (Vertex v in meshTopVertices)
        {
            completeMeshVertices.Add(v);
        }

        // triangulate complete vertices list
        HashSet<Triangle> completeMeshTriangles = Triangulate(completeMeshVertices);


        // triangulate vertices
        HashSet<Triangle> bottomMeshTriangles = Triangulate(meshBottomVertices);
        //HashSet<Triangle> topMeshTriangles = Triangulate(meshTopVertices);

        // combine meshes

        // return triangles for mesh generation
        return completeMeshTriangles;
    }

    public void SpawnRoads(List<Edge> edges) {

        List<Vector3> midpoints = new();
        bool duplicatePosition;

        foreach (Edge edge in edges) {
            duplicatePosition = false;
            // Calculate the midpoint and set the position
            Vector3 midpoint = (edge.VertexA.Position + edge.VertexB.Position) / 2;

            // iterate through list of previous midpoints to prevent duplicate road segments
            for (int i = 0; i < midpoints.Count; i++)
            {
                if (midpoint == midpoints[i]) { duplicatePosition = true; }
            }

            if (!duplicatePosition) {

                // Calculate the rotation based on the direction of the edge
                Vector3 direction = (edge.VertexB.Position - edge.VertexA.Position).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction);

                // Instantiate a new road prefab
                GameManager gameManagerScript = gameManager.GetComponent<GameManager>();
                GameObject road = gameManagerScript.CreateRoadSegment();
                road.transform.position = new(midpoint.x, midpoint.y + 1, midpoint.z);
                road.transform.rotation = rotation;
                roadSegments.Add(road);
                midpoints.Add(midpoint);

                // Set the length
                road.transform.localScale = new(road.transform.localScale.x * 4, road.transform.localScale.y, (road.transform.localScale.z * (float)edge.Length));
            }
        }
    }

    public void CalculateUniqueTriangleVertices(HashSet<Triangle> triangles) {

        List<Vertex> triangleVertices = new();
        foreach (Triangle t in triangles)
        {
            triangleVertices.Add(t.VertexA);
            triangleVertices.Add(t.VertexB);
            triangleVertices.Add(t.VertexC);
        }
        HashSet<Vector3> uniquePositions = new HashSet<Vector3>();
        List<Vertex> uniqueVertices = new List<Vertex>();

        foreach (Vertex vertex in triangleVertices)
        {
            if (uniquePositions.Add(vertex.Position))
            {
                uniqueVertices.Add(vertex);
            }
        }
        uniqueTriangleVertices = uniqueVertices;
    }

    public void DestroyRoads(List<GameObject> roadList) {
        if (roadList.Count > 0) {
            foreach (GameObject road in roadList)
            {
                // delete road segments

                Destroy(road);
            }
            roadList.Clear();
        }
    }
    public void Pipeline(List<GameObject> pointGameObjects) {

        DestroyEdges();
        DestroyMeshGenerators();

        points = CreatePointsFromGameObjectPositions(pointGameObjects);

        triangleList = Triangulate(points);

        CalculateNeighbors(triangleList);

        GetVoronoiEdges(trianglesAndNeighbours);
        GetVoronoiCells(points, triangleList);

        CalculateUniqueTriangleVertices(triangleList);

        ComputeConvexHull(points);

        DrawConvexHull(convexHull);

        List<Edge> triangleEdges = new();
        foreach (Triangle triangle in triangleList)
        {
            // get edges
            

            triangleEdges.Add(triangle.EdgeAB);
            triangleEdges.Add(triangle.EdgeBC);
            triangleEdges.Add(triangle.EdgeCA);
        }

        DrawEdges(triangleEdges, Color.red);
        DrawEdges(voronoiEdgeList, Color.blue);
        //-------------------------------------------------------------------------------------------------------------------------
        foreach (Cell c in voronoiCells)
        {
            // compute mesh triangulation
            HashSet<Triangle> cellMeshData = ComputeBuildingMesh(c);

            GameManager gameManagerScript = gameManager.GetComponent<GameManager>();

            // spawn meshGenerator gameObject
            // Vector3 spawnPos = CalculateCenter(c.Vertices);
            GameObject meshGenerator = gameManagerScript.CreateMeshGenerator(c.Site.Position);
            meshGenerators.Add(meshGenerator);
            //meshGenerators.Add(meshGenerator); no need to add because they are temporary

            // send cellMeshData
            MeshGenerator meshGeneratorScript = meshGenerator.GetComponent<MeshGenerator>();

            List<Vector3> cellMeshDataVertices = new();
            for (int i = 0; i < cellMeshData.Count; i++)
            {
                cellMeshDataVertices.Add(new(cellMeshData.ToList()[i].VertexA.Position.x, cellMeshData.ToList()[i].VertexA.Position.y + 1, cellMeshData.ToList()[i].VertexA.Position.z));
                cellMeshDataVertices.Add(new(cellMeshData.ToList()[i].VertexB.Position.x, cellMeshData.ToList()[i].VertexB.Position.y + 1, cellMeshData.ToList()[i].VertexB.Position.z));
                cellMeshDataVertices.Add(new(cellMeshData.ToList()[i].VertexC.Position.x, cellMeshData.ToList()[i].VertexC.Position.y + 1, cellMeshData.ToList()[i].VertexC.Position.z));
            }

            meshGeneratorScript.worldSpacePoints = cellMeshDataVertices.ToArray();
        }
        //--------------------------------------------------------------------------------------------------------------------------
    }

    public void DestroyMeshGenerators() {
        if (meshGenerators.Count != 0) {
            for (int i = 0; i < meshGenerators.Count; i++)
            {
                Destroy(meshGenerators[i]);
            }
        }
    }

    public void DrawEdges(List<Edge> edges, Color colour)
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        GameManager gameManagerScript = gameManager.GetComponent<GameManager>();
        foreach (Edge edge in edges) {
            gameManagerScript.CreateEdge(gameManagerScript.edgePrefab, edge.VertexA.Position, edge.VertexB.Position, colour);
        }
    }

    public void DestroyEdges() {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager");
        GameManager gameManagerScript = gameManager.GetComponent<GameManager>();

        gameManagerScript.DestroyEdges();
    }

    public void GroupIntoCity(string cityName) {
        city = new GameObject(cityName);
        city.tag = "City";

        foreach (GameObject seg in roadSegments) {
            seg.transform.parent = city.transform;
        }
    }

    public void DrawConvexHull(List<Vertex> convexHull) {

        foreach (GameObject p in pointGameObjects) {

            // Reset all vertices to red
            Material material = p.GetComponent<Renderer>().material;
            material.color = Color.red;

            // if vertex is part of convex hull, change colour to purple
            foreach (Vertex v in convexHull)
            {
                if (p.transform.position == v.Position)
                {
                    // Get the Material component of the GameObject
                    material = p.GetComponent<Renderer>().material;
                    // Set the color of the Material to purple
                    material.color = Color.magenta;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (triangleList != null)
        {
            for (int i=0; i< triangleList.Count; i++)
            {
                Vector3 circumcenter = triangleList.ToList()[i].Circumcenter.Position;
                float circumradius = triangleList.ToList()[i].Circumradius;

                // Set the Gizmo color
                Gizmos.color = Color.yellow;

                // Draw the circumcircle
                DrawCircleGizmo(circumcenter, circumradius);
                Gizmos.DrawSphere(circumcenter, 1f);
                Gizmos.DrawLine(triangleList.ToList()[i].VertexA.Position, triangleList.ToList()[i].VertexB.Position);
                Gizmos.DrawLine(triangleList.ToList()[i].VertexB.Position, triangleList.ToList()[i].VertexC.Position);
                Gizmos.DrawLine(triangleList.ToList()[i].VertexC.Position, triangleList.ToList()[i].VertexA.Position);
            }
        }
    }

    private void DrawCircleGizmo(Vector3 center, float radius)
    {
        int numSegments = 128;
        float angleStep = 360f / numSegments;

        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= numSegments; i++)
        {
            float angle = i * angleStep;
            float radAngle = Mathf.Deg2Rad * angle;

            Vector3 newPoint = new Vector3(
                center.x + radius * Mathf.Cos(radAngle),
                center.y,
                center.z + radius * Mathf.Sin(radAngle)
            );

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }


    //private void Awake()
    //{
    //    Vertex v1 = new Vertex(new(2000, 0, 10), 1);
    //    Vertex v2 = new Vertex(new(1000, 0, 1500), 2);
    //    Vertex v3 = new Vertex(new(3000, 0, 750), 3);
    //    Vertex v4 = new Vertex(new(500, 0, 500), 4);
    //    Vertex v5 = new Vertex(new(1000, 0, 1000), 5);
    //    Vertex v6 = new Vertex(new(1750, 0, 200), 6);
    //    Vertex v7 = new Vertex(new(1800, 0, 1800), 7);
    //    Vertex v8 = new Vertex(new(2010, 0, 890), 8);
    //    Vertex v9 = new Vertex(new(650, 0, 1730), 9);
    //    Vertex v10 = new Vertex(new(3340, 0, 980), 10);
    //    Vertex v11 = new Vertex(new(-1000, 0, 1520), 11);
    //    Vertex v12 = new Vertex(new(2500, 0, 5000), 12);
    //    Vertex v13 = new Vertex(new(-1000, 0, -2000), 13);
    //    Vertex v14 = new Vertex(new(160, 0, 310), 14);
    //    Vertex v15 = new Vertex(new(1690, 0, 4750), 15);
    //    Vertex v16 = new Vertex(new(3000, 0, -2000), 16);
    //    Vertex v17 = new Vertex(new(1200, 0, -2340), 17);
    //    points = new();
    //    //points.Add(v1);
    //    points.Add(v2);
    //    points.Add(v3);
    //    points.Add(v4);
    //    points.Add(v5);
    //    points.Add(v6);
    //    points.Add(v7);
    //    points.Add(v8);
    //    /*points.Add(v9);
    //    points.Add(v10);
    //    points.Add(v11);
    //    points.Add(v12);
    //    points.Add(v13);
    //    points.Add(v14);
    //    points.Add(v15);
    //    points.Add(v16);
    //    points.Add(v17);*/

    //    triangles = Triangulate(points);
    //    trianglesAndNeighbours = CalculateNeighbors(triangles);
    //    voronoiEdges = GetVoronoiEdges(trianglesAndNeighbours);
    //    SpawnRoads(voronoiEdges);
    //    voronoiCells = GetVoronoiCells(points, triangles);

    //    List<Vertex> triangleVertices = new();
    //    foreach (Triangle t in triangles)
    //    {
    //        triangleVertices.Add(t.VertexA);
    //        triangleVertices.Add(t.VertexB);
    //        triangleVertices.Add(t.VertexC);
    //    }
    //    HashSet<Vector3> uniquePositions = new HashSet<Vector3>();
    //    List<Vertex> uniqueVertices = new List<Vertex>();

    //    foreach (Vertex vertex in triangleVertices)
    //    {
    //        if (uniquePositions.Add(vertex.Position))
    //        {
    //            uniqueVertices.Add(vertex);
    //        }
    //    }
    //    triangulationConvexHull = ComputeConvexHull(uniqueVertices);

    //}
}
