using UnityEditor.SceneManagement;
using UnityEngine;

public class Path : MonoBehaviour
{
    // Path properties
    [SerializeField] private Transform[] _points; // Set in inspector
    [SerializeField, Range(0f, 5f)] private float _pathWidth; // Set in inspector
    //[SerializeField] private int _pathCheckDistance = 15; // Set in inspector
    private float _totalPathDistance;
    private LineRenderer _lineRenderer;

    // Debug properties
    [Header("DEBUG")]
    [SerializeField] private bool _drawPath; // Set in inspector
    [SerializeField] private Color _pathColor; // Set in inspector

    // Getters
    public Transform[] Points => _points;
    public float PathWidth => _pathWidth;
    public float TotalPathDistance => _totalPathDistance;


    // Call on initialization
    public void InitializePath()
    {
        // Calculate the path's total distance
        CalculatePathDistance();
        //Debug.Log($"Total path distance: {_totalPathDistance}");

        RenderPath();

        // Set the path tiles as invalid
        SetPathTiles();
    }

    private void CalculatePathDistance()
    {
        // Loop over every path segment
        for (int i = 0; i < _points.Length - 1; i++)
        {
            _totalPathDistance += Vector2.Distance(_points[i].transform.position,
                                                    _points[i + 1].transform.position);
        }
    }

    // Uses LineRenderer to create a path in the game screen
    private void RenderPath()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _points.Length;
        _lineRenderer.widthMultiplier = _pathWidth;

        // Sets the vertices of the line from the path points
        for (int i = 0; i < _points.Length; i++)
        {
            _lineRenderer.SetPosition(i, _points[i].position);
        }
    }

    private void SetPathTiles()
    {
        // Create a mesh and add a mesh collider
        Mesh mesh = new Mesh();
        MeshCollider col = gameObject.AddComponent<MeshCollider>();

        // Set the new mesh to the line renderer
        _lineRenderer.BakeMesh(mesh);
        
        // Set the collider to match the new mesh
        col.sharedMesh = mesh;

        // Check for path tiles by raycasting against the collider
        GridManager.Instance.InitializePathTiles();

        // Remove the collider
        Destroy(col);
    }

    // DEBUG
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && _drawPath)
        {
            Gizmos.color = _pathColor;

            DrawPath();
        }
    }
    // DEBUG: Draws capsules around each path segment
    private void DrawPath()
    {
        // Loop over every path segment
        for (int i = 0; i < _points.Length; i++)
        {
            // Draw sphere at each point
            Gizmos.DrawWireSphere(_points[i].position, _pathWidth * 0.5f);

            // Draw line segments connecting each point
            // Exclude the last point
            if (i < _points.Length - 1)
            {
                // Get the direction and distance to the next point
                Vector3 forwardDir = _points[i + 1].position - _points[i].position;
                float dist = forwardDir.magnitude;
                forwardDir = forwardDir.normalized;

                // Get the direction perpendicular to the direction to the next point
                // To place lines on the left and right of each point
                Vector3 rightDir = Vector3.Cross(forwardDir, Vector3.forward);

                // Get left and right positions
                Vector3 left = _points[i].position + (-rightDir * _pathWidth * 0.5f);
                Vector3 right = _points[i].position + (rightDir * _pathWidth * 0.5f);

                // Draw the lines that connect the points
                Gizmos.DrawRay(left, forwardDir * dist);
                Gizmos.DrawRay(right, forwardDir * dist);
            }
        }
    }

}