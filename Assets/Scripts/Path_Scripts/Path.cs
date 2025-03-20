using UnityEngine;

public class Path : MonoBehaviour
{
    // Path properties
    [SerializeField] private Transform[] _points; // Set in inspector
    [SerializeField, Range(0f, 5f)] private float _pathRadius; // Set in inspector
    private float _totalPathDistance;

    // Debug properties
    [Header("DEBUG")]
    [SerializeField] private bool _drawPath; // Set in inspector
    [SerializeField] private Color _pathColor; // Set in inspector

    // Getters
    public Transform[] Points => _points;
    public float PathRadius => _pathRadius;
    public float TotalPathDistance => _totalPathDistance;


    // Call on initialization
    public void InitializePath()
    {
        // Calculate the path's total distance
        CalculatePathDistance();
        //Debug.Log($"Total path distance: {_totalPathDistance}");
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

    // DEBUG
    private void OnDrawGizmos()
    {
        if (_drawPath)
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
            Gizmos.DrawWireSphere(_points[i].position, _pathRadius);

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
                Vector3 left = _points[i].position + (-rightDir * _pathRadius);
                Vector3 right = _points[i].position + (rightDir * _pathRadius);

                // Draw the lines that connect the points
                Gizmos.DrawRay(left, forwardDir * dist);
                Gizmos.DrawRay(right, forwardDir * dist);
            }
        }
    }

}