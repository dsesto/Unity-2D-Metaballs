/// Author: David Sesto (GitHub: @dsesto)
/// Digging Dinosaurs Games (Twitter @digging_dinos)

using UnityEngine;

/*
 * Manager of our Metaball experiment. It generates the grid of samples, the Unity Mesh and the metaballs,
 * and computes (and draws) the new state on each frame.
 */
public class MetaballManager : MonoBehaviour {

    // Experiment options
    public int numMetaballs = 6;
    public int gridResolution = 100;
    public Rect boundaries = new Rect(-5, -5, 10, 10);
    [Range(0.5f, 3f)]
    public float metaballSpeed = 1f;
    [Range(0.2f, 2f)]
    public float metaballRadius = 0.5f;
    public bool smooth;
    public bool includeRectangle;

    private Metaball[] metaballs;
    public MeshGrid.Sample[,] gridSamples;
    private MeshGrid.Cell[,] gridCells;
    [HideInInspector]
    public Transform rectangle;

    [HideInInspector]
    public int numCols;
    [HideInInspector]
    public int numRows;
    private float gridCellHalfSize;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    public static MetaballManager instance { get; private set; }

    private void Awake() {
        // Singleton instantiation
        transform.parent = null;
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // Initialization of the experiment
        CreateMetaballs();
        CreateGrid();
        CreateMesh();
        EvaluateRectangle();
    }

    private void Update() {
        // Move metaballs
        foreach (Metaball metaball in metaballs) {
            metaball.Move();
        }

        // Update values of samples based on the new metaball positions
        foreach (MeshGrid.Sample gridSample in gridSamples) {
            gridSample.UpdateSampleValue(metaballs);
        }

        // Generate a new Mesh to replace the existing one, based on the new Marching Squares results
        UpdateMesh();
    }

    /// Generates all the metaballs that will be used in the experiment
    private void CreateMetaballs() {
        metaballs = new Metaball[numMetaballs];
        for (int i = 0; i < numMetaballs; i++) {
            metaballs[i] = new Metaball();
        }
    }

    /// Generates the grid (a matrix of vertices, or GridSamples) that will be used for the Marching Squares algorithm.
    /// It also groups GridSamples into GridCells (quads with 4 vertices, or GridSamples) for a better organization.
    private void CreateGrid() {
        numCols = gridResolution;
        numRows = gridResolution;

        float gridSampleWidth = boundaries.size.x / gridResolution;
        float gridSampleHeight = gridSampleWidth;
        gridCellHalfSize = gridSampleWidth * 0.5f;

        gridSamples = new MeshGrid.Sample[numRows, numCols];
        float x = boundaries.xMin;
        for (int c = 0; c < numCols; c++) {
            float y = boundaries.yMin;

            for (int r = 0; r < numRows; r++) {
                MeshGrid.Sample gridSample = new MeshGrid.Sample(r, c, x, y);
                gridSamples[r, c] = gridSample;

                y += gridSampleHeight;
            }

            x += gridSampleWidth;
        }

        // There's 1 cellColumn less than sampleColumns
        gridCells = new MeshGrid.Cell[numRows - 1, numCols - 1];
        for (int c = 0; c < numCols - 1; c++) {
            for (int r = 0; r < numRows - 1; r++) {
                MeshGrid.Cell gridCell = new MeshGrid.Cell(gridSamples[r, c]);
                gridCells[r, c] = gridCell;
            }
        }
    }

    /// Acts on the rectangle of the scene depending on whether its usage is activated.
    /// The MeshRenderer of the rectangle is unnecessary, as it will be rendered in this object's Mesh.
    private void EvaluateRectangle() {
        rectangle = GameObject.Find("Quad").transform;
        rectangle.GetComponent<MeshRenderer>().enabled = false;
    }

    #region Mesh
    /// Creates the Unity Mesh and the arrays that will be populated with the vertices and triangles
    private void CreateMesh() {
        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;

        // Create arrays of vertices and triangles, reserving for the mesh the maximum size
        int numVertices = numCols * numRows * 6;
        vertices = new Vector3[numVertices];
        int numTriangleVertices = numVertices;
        triangles = new int[numTriangleVertices];
    }

    /// Builds a new Mesh based on the new sample values taken after moving the metaballs
    private void UpdateMesh() {
        int vertexIndex = 4;
        int triangleIndex = 6;
        foreach (MeshGrid.Cell gridCell in gridCells) {
            int cellConfiguration = gridCell.GetCellConfiguration();

            if (cellConfiguration > 0) {
                Vector3 p = gridCell.sampleC.position; // Lower-left corner

                Vector3[] cellPoints = MarchingSquares.vertices[cellConfiguration];
                int[] cellTriangles = MarchingSquares.triangles[cellConfiguration];

                for (int i = 0; i < cellTriangles.Length; i += 3) {
                    int t1 = cellTriangles[i];
                    int t2 = cellTriangles[i + 1];
                    int t3 = cellTriangles[i + 2];

                    Vector3 p0 = cellPoints[t1];
                    Vector3 p1 = cellPoints[t2];
                    Vector3 p2 = cellPoints[t3];

                    if (smooth && cellConfiguration != 15) {
                        // Unless the Cell Configuration is 15 (full cell), interpolate the triangle points
                        // to smooth the curves.
                        p0 = gridCell.ApplyLinearInterpolation(p0);
                        p1 = gridCell.ApplyLinearInterpolation(p1);
                        p2 = gridCell.ApplyLinearInterpolation(p2);
                    }

                    vertices[vertexIndex] = p + (p0 * gridCellHalfSize);
                    vertices[vertexIndex + 1] = p + (p1 * gridCellHalfSize);
                    vertices[vertexIndex + 2] = p + (p2 * gridCellHalfSize);

                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + 2;

                    vertexIndex += 3;
                    triangleIndex += 3;
                }
            }
        }

        // Clear the arrays positions that had been reserved but ended up being unused in this iteration.
        // It's not necessary to clear them beforehand because the first N values (numMeshVertices and
        // numMeshTriangles, respectively) will already be replaced by the new values.
        System.Array.Clear(vertices, vertexIndex, vertices.Length - vertexIndex);
        System.Array.Clear(triangles, triangleIndex, triangles.Length - triangleIndex);

        // Update mesh to display its new structure
        mesh.Clear(false);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
    #endregion
}
