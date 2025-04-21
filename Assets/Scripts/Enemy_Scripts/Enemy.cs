using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy properties: set in inspector
    [SerializeField] private string _name;
    [SerializeField] private int _health;
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private int _orbAmount;
    [SerializeField] private float _burnRadius;

    // Each enemy needs a path to follow
    private Path _path = null;
    private int _pathPointIndex = 0;
    private float _distanceTraveled = 0f;
    private float _percentAlongPath = 0f;

    // Death
    private bool _hasDied = false;

    // Getters
    public string Name => _name;
    public int Health => _health;
    public float Speed => _speed;
    public int Damage => _damage;
    public int OrbAmount => _orbAmount;
    public float BurnRadius => _burnRadius;
    public float PercentAlongPath => _percentAlongPath;


    // Call this method when an enemy is spawned
    public void InitializeEnemy()
    {
        _hasDied = false;

        // Set this enemy's path
        _path = PathManager.Instance.ActivePath;

        // Place the enemy at the start of the path
        // Eventually allow enemies to be placed further along the path?
        // Like if an enemy spawns another enemy?
        transform.position = _path.Points[_pathPointIndex].transform.position;
    }

    private void Update()
    {
        // Have the enemy follow the path
        FollowPath();

        // Burn tiles around it
        //Vector3 tile = GridManager.Instance.GetNearestTile(transform.position);
        //DebugExtension.DebugCircle(tile, -Vector3.forward, Color.white, 0.5f);
        GridManager.Instance.BurnTilesInRange(transform.position, _burnRadius);
    }

    private void FollowPath()
    {
        // Check if there is a path to follow
        if (_path == null)
        {
            Debug.LogWarning($"Enemy: ({_name}) has no path to follow!");
            return;
        }

        // If the enemey hasn't reached the end of the path...
        if (_pathPointIndex <= _path.Points.Length - 1)
        {
            // Move towards the target point
            Vector2 previousPosition = transform.position;
            transform.position = Vector2.MoveTowards(transform.position,
                                                    _path.Points[_pathPointIndex].transform.position,
                                                    _speed * Time.deltaTime);

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

            // Check if the enemy reached the end of the path
            // Use 0.999 to allow some room for error
            if (_percentAlongPath >= 0.98f)
            {
                //Debug.Log($"Enemy: ({_name}) reached the end!");
                EnemyManager.Instance.EnemyReachedEndOfPath(this);
            }
        }
    }

    public void DamageEnemy(int damage)
    {
        _health -= damage;
        if (!_hasDied && _health <= 0)
        {
            _hasDied = true;

            //Debug.Log("Enemy died!");
            EnemyManager.Instance.EnemyDied(this);
        }
    }

}