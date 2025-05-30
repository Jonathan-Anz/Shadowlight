using System;
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

public enum TowerType 
{ 
    Bear, Bat, Bird, Squirrel, Fox, Beehive
}
public class Tower : MonoBehaviour
{
    // Tower properties: set in inspector
    [SerializeField] private string _name;
    [SerializeField] private float _range;
    [SerializeField] private int _attackPower;
    // The time between attacks
    [SerializeField] private float _attackSpeed;
    [SerializeField] private TowerAttackType _attackType;
    private TowerTargetMode _targetMode = TowerTargetMode.First;

    // Special effects
    [Header("Special Effects")]
    [SerializeField] private bool _stunsEnemies = false;
    [SerializeField] private float _stunDuration = 0f;
    [SerializeField] private bool _slowsEnemies = false;
    [SerializeField, Range(0f, 1f)] private float _slowReduction = 0f;
    [SerializeField] private float _slowDuration = 0f;


    [Header("Projectile (Only used if tower is ranged)")]
    [SerializeField] private GameObject _projectilePrefab;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer[] _towerVisuals;
    [SerializeField] private SpriteRenderer _rangeVisual;

    [Header("Selling")]
    [SerializeField] private float _sellMultiplier = 0.75f;
    private int _orbValue;

    // Each tower has a list of enemies in its range
    private List<Enemy> _enemiesInRange = new List<Enemy>();
    private bool _hasEnemiesInRange = false;

    // Attacking
    private bool _isDisabled = true;
    private float _attackTimer = 0f;
    private Enemy _currentTarget = null;
    [SerializeField] private Animator _attackAnimator;

    // Range
    private CircleCollider2D _rangeCollider;

    // Getters
    public string Name => _name;
    public float Range => _range;
    public int AttackPower => _attackPower;
    public float AttackSpeed => _attackSpeed;
    public TowerTargetMode TargetMode => _targetMode;
    //public Sprite TowerVisual => _towerVisual.sprite;
    public bool StunsEnemies => _stunsEnemies;
    public float StunDuration => _stunDuration;
    public bool SlowsEnemies => _slowsEnemies;
    public float SlowReduction => _slowReduction;
    public float SlowDuration => _slowDuration;
    public int SellValue => Mathf.RoundToInt(_orbValue * _sellMultiplier);
    public bool IsDisabled => _isDisabled;


    // Call when spawning in the tower
    public void InitializeTower(int orbValue)
    {
        _attackTimer = 0f;
        _rangeCollider = GetComponent<CircleCollider2D>();
        _isDisabled = false;
        _orbValue = orbValue;

        // Changes range to value set in inspector
        ChangeRange(_range);

        //Debug.Log($"{_name} has been initialized");
    }

    private void Update()
    {
        AttackCheck();
    }

    #region Attacking
    public void DisableTower(bool value) => _isDisabled = !value;

    private void AttackCheck()
    {
        if (_isDisabled) return;

        // Reduce the attack cooldown
        _attackTimer -= Time.deltaTime;

        // If there are any enemies in range...
        if (_hasEnemiesInRange)
        {
            // Find the current attack target
            _currentTarget = FindCurrentTarget();

            // Flip to face the current target
            TurnToFaceTarget();

            // DEBUG: highlight the current attack target
            //Debug.DrawLine(transform.position, _currentTarget.transform.position, Color.green);

            // Attack
            if (_attackTimer <= 0f)
            {
                _attackTimer = _attackSpeed;
                Attack();
            }
        }
    }

    private void TurnToFaceTarget()
    {
        // For each tower visual sprite
        for (int i = 0; i < _towerVisuals.Length; i++)
        {
            // Get the current scale
            Vector3 scale = _towerVisuals[i].transform.localScale;

            // If there is no target
            if (_currentTarget == null)
            {
                // Look left
                scale.x = Mathf.Abs(scale.x);
                _towerVisuals[i].transform.localScale = scale;
                continue;
            }

            // Check if the current target is on the left or right
            if (_currentTarget.transform.position.x <= transform.position.x)
            {
                // Look left
                scale.x = Mathf.Abs(scale.x);
            }
            else
            {
                // Look right
                scale.x = -Mathf.Abs(scale.x);
            }

            // Reapply the scale
            _towerVisuals[i].transform.localScale = scale;
        }

    }
    
    // Changes the target mode of the tower (in a cycle)
    public void ChangeTargetMode()
    {
        int num = (int)_targetMode;
        num = (num + 1) % Enum.GetNames(typeof(TowerTargetMode)).Length;
        _targetMode = (TowerTargetMode)num;
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
            _currentTarget.DamageEnemy(_attackPower);
            if (_stunsEnemies) _currentTarget.StunEnemy(_stunDuration);
            if (_slowsEnemies) _currentTarget.SlowEnemy(_slowReduction, _slowDuration);

            // Play animation/sound
            if (_attackAnimator != null) _attackAnimator.SetTrigger("Attack");
        }
        else if (_attackType == TowerAttackType.Ranged)
        {
            // If ranged tower:

            // Spawn a projectile prefab
            Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.InitializeProjectile(this, _currentTarget, _attackPower);

            // Play animation/sound
            if (_attackAnimator != null) _attackAnimator.SetTrigger("Attack");
        }
    }
    #endregion

    #region Range
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
    
    // Changes the range of the tower
    private void ChangeRange(float range)
    {
        _rangeCollider.radius = range;

        // Need to double the range for the visual because the sprite works in diameter, not radius
        _rangeVisual.transform.localScale = new Vector3(_range * 2f, _range * 2f, 1f);
    }
    public void ToggleRangeVisual(bool value) => _rangeVisual.enabled = value;
    #endregion
    
    // DEBUG
    void OnDrawGizmos()
    {
        //DrawTowerRange();
    }
    private void DrawTowerRange()
    {
        DebugExtension.DrawCircle(transform.position, -Vector3.forward, Color.blue, _range);
    }
}