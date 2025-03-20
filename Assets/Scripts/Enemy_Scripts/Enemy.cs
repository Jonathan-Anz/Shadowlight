using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy properties: set in inspector
    [SerializeField] private string _name;
    [SerializeField] private int _health;
    [SerializeField] private float _speed;

    // Each enemy needs a path to follow
    private Path _path = null;
    private int _pathPointIndex = 0;
    private float _distanceTraveled = 0f;
    private float _percentAlongPath = 0f;

    // Getters
    public string Name => _name;
    public int Health => _health;
    public float Speed => _speed;
    public float PercentAlongPath => _percentAlongPath;


    // Call this method when an enemy is spawned
    public void InitializeEnemy(Path path)
    {
        // Set this enemy's path
        _path = path;

        // Place the enemy at the start of the path
        // Eventually allow enemies to be placed further along the path?
        // Like if an enemy spawns another enemy?
        transform.position = _path.Points[_pathPointIndex].transform.position;
    }

    private void Update()
    {
        // Have the enemy follow the path
        FollowPath();
    }

    private void FollowPath()
    {
        // Check if there is a path to follow
        if (_path == null)
        {
            Debug.LogWarning($"Enemy: {Name} has no path to follow!");
            return;
        }

        // If the enemey hasn't reached the end of the path...
        if (_pathPointIndex <= _path.Points.Length - 1)
        {
            // Move towards the target point
            Vector2 previousPosition = transform.position;
            transform.position = Vector2.MoveTowards(transform.position,
                                                    _path.Points[_pathPointIndex].transform.position,
                                                    Speed * Time.deltaTime);

            // Calculate the total distance traveled
            _distanceTraveled += Vector2.Distance(previousPosition, transform.position);
            //Debug.Log($"Distance traveled: {_distanceTraveled}");

            // Calcualte the total percentage traveled
            _percentAlongPath = _distanceTraveled / _path.TotalPathDistance;
            //Debug.Log($"Percent along path: {_percentAlongPath}");

            // If the enemy reached the target point, set the target as the next one
            if (transform.position == _path.Points[_pathPointIndex].transform.position)
            {
                _pathPointIndex++;
            }
        }
    }

}