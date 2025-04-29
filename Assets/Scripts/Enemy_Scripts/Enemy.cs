using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy properties: set in inspector
    [SerializeField] private string _name;
    [SerializeField] private int _health;
    [SerializeField] private float _speed;
    [SerializeField] private int _damage;
    [SerializeField] private int _orbAmount;
    [Header("Burn offset 0 is the current tile, 1 is previous tile, etc")]
    [SerializeField] private int _burnOffset;
    [SerializeField] private float _burnRadius;

    // Each enemy needs a path to follow
    private Path _path = null;
    private int _pathPointIndex = 0;
    private float _distanceTraveled = 0f;
    private float _percentAlongPath = 0f;

    // Tiles
    private GridTile _currentTile;
    private List<GridTile> _previousTiles = new List<GridTile>();

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
        _distanceTraveled = 0f;
        //Debug.Log($"This enemy's path distance is: {_path.TotalPathDistance}");

        // Place the enemy at the start of the path
        // Eventually allow enemies to be placed further along the path?
        // Like if an enemy spawns another enemy?
        transform.position = _path.Points[_pathPointIndex].transform.position;

        // Get the current tile
        _currentTile = GridManager.Instance.GetTileFromWorldPosition(transform.position);
        _previousTiles.Add(_currentTile);
    }

    private void Update()
    {
        // Have the enemy follow the path
        FollowPath();

        UpdatePreviousTilesList();

        GridTile burnTile = GetPeviousTile(_burnOffset);
        if (burnTile != null)
        {
            //DebugExtension.DebugCircle(burnTile.Position, -Vector3.forward, Color.white, 0.5f);

            // Burn tiles
            GridManager.Instance.BurnTilesInRange(burnTile.Position, _burnRadius);
        }

        

        // Burn tiles around it
        //Vector3 tile = GridManager.Instance.GetNearestTile(transform.position);
        //DebugExtension.DebugCircle(tile, -Vector3.forward, Color.white, 0.5f);
        //GridManager.Instance.BurnTilesInRange(transform.position, _burnRadius);
    }

    private void FollowPath()
    {
        // Check if there is a path to follow
        if (_path == null)
        {
            Debug.LogWarning($"Enemy: ({_name}) has no path to follow!");
            return;
        }

        //Debug.Log($"Enemy {_name} is following the path {_path.name}");

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
            //Debug.Log($"Enemy {_name} percent along path: {_percentAlongPath}");
            if (_percentAlongPath >= 0.98f)
            {
                //Debug.Log($"Enemy: ({_name}) reached the end!");
                EnemyManager.Instance.EnemyReachedEndOfPath(this);
            }
        }
    }

    // Tiles
    private void UpdatePreviousTilesList()
    {
        // Get the current tile
        _currentTile = GridManager.Instance.GetTileFromWorldPosition(transform.position);
        //DebugExtension.DebugCircle(_currentTile.Position, -Vector3.forward, Color.yellow, 0.5f);
        if (_currentTile != _previousTiles.Last())
        {
            // Entered a new tile
            _previousTiles.Add(_currentTile);
            
            //Debug.Log($"Enemy {_name} is now in tile {_currentTile.Position}");
        }
    }
    private GridTile GetPeviousTile(int index)
    {
        if (index >= _previousTiles.Count) return null;
        else return _previousTiles[_previousTiles.Count - index - 1];
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