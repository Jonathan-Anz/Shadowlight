using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TowerTargetMode
{
    First, Last, Strong, Weak
}
public enum TowerAttackType
{
    Melee, Ranged
}

public class Tower : MonoBehaviour
{
    // Tower properties: set in inspector
    [SerializeField] private string _name;
    [SerializeField] private float _range;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private TowerTargetMode _targetMode; // Change target mode to be modified in game?
    [SerializeField] private TowerAttackType _attackType;
    [Header("Melee Properties")]
    [SerializeField] private int _attackPower;
    [Header("Ranged Properties")]
    [SerializeField] private GameObject _projectilePrefab;


    // Each tower has a list of enemies in its range
    private List<Enemy> _enemiesInRange = new List<Enemy>();
    private bool _hasEnemiesInRange = false;

    // Attacking
    private float _attackTimer = 0f;
    private Enemy _currentTarget = null;

    // Range
    private CircleCollider2D _rangeCollider;

    // Getters
    public string Name => _name;
    public float Range => _range;
    public int AttackPower => _attackPower;
    public float AttackSpeed => _attackSpeed;

    // TEMP
    private void Awake()
    {
        InitializeTower();
    }

    // Call when spawning in the tower
    public void InitializeTower()
    {
        _attackTimer = 0f;
        _rangeCollider = GetComponent<CircleCollider2D>();

        // Changes range to value set in inspector
        ChangeRange(_range);
    }

    // Add enemies to enemies in range list when entering the tower's trigger collider
    // The tower needs a non-static rigidbody for this to work (set it to kinematic)
    private void OnTriggerEnter2D(Collider2D col)
    {
        // Check if the collider is an enemy
        if (1 << col.gameObject.layer == LayerMasks.EnemyMask)
        {
            //Debug.Log("New enemy in range!");
            _enemiesInRange.Add(col.GetComponent<Enemy>());

            if (!_hasEnemiesInRange) _hasEnemiesInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        // Check if the collider is an enemy
        if (1 << col.gameObject.layer == LayerMasks.EnemyMask)
        {
            //Debug.Log("Enemy left range!");
            _enemiesInRange.Remove(col.GetComponent<Enemy>());

            if (_enemiesInRange.Count <= 0) _hasEnemiesInRange = false;
        }
    }

    private void Update()
    {
        // Reduce the attack cooldown
        _attackTimer -= Time.deltaTime;

        // If there are any enemies in range...
        if (_hasEnemiesInRange)
        {
            // Find the current attack target
            _currentTarget = FindCurrentTarget();

            // DEBUG: highlight the current attack target
            Debug.DrawLine(transform.position, _currentTarget.transform.position, Color.green);

            // Attack
            if (_attackTimer <= 0f)
            {
                _attackTimer = _attackSpeed;
                Attack();
            }
        }
    }

    private Enemy FindCurrentTarget()
    {
        //Debug.Log($"Target mode: {_targetMode}");

        // Eventually add second condition if multiple enemies are the same strength
        // Second condition should be who is first

        switch (_targetMode)
        {
            // Default is first
            case TowerTargetMode.Strong:
                return _enemiesInRange.OrderBy(e => e.Damage).First();
            case TowerTargetMode.Weak:
                return _enemiesInRange.OrderByDescending(e => e.Damage).First();
            case TowerTargetMode.Last:
                return _enemiesInRange.OrderBy(e => e.PercentAlongPath).First();
            default:
                return _enemiesInRange.OrderByDescending(e => e.PercentAlongPath).First();
        }
    }

    private void Attack()
    {
        //Debug.Log("Attack!");
        if (_attackType == TowerAttackType.Melee)
        {
            // If melee tower:
            // Remove health from enemy
            // Play animation/sound

            _currentTarget.DamageEnemy(_attackPower);
        }
        else if (_attackType == TowerAttackType.Ranged)
        {
            // If ranged tower:
            // Spawn a projectile prefab
            // Play animation/sound

            Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.InitializeProjectile(_currentTarget);
        }
    }

    // Changes the range of the tower
    private void ChangeRange(float range)
    {
        // Divides by scale x to offset scale applied on collider
        _rangeCollider.radius = range / transform.localScale.x;
    }

    // DEBUG
    void OnDrawGizmos()
    {
        DrawTowerRange();
    }
    private void DrawTowerRange()
    {
        DebugExtension.DrawCircle(transform.position, -Vector3.forward, Color.blue, _range);
    }
}