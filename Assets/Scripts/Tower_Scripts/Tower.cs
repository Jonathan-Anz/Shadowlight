using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // Tower properties: set in inspector
    [SerializeField] private string _name;
    [SerializeField] private float _range;
    [SerializeField] private int _damage;
    [SerializeField] private float _attackSpeed;

    // Each tower has a list of enemies in its range
    private ContactFilter2D _enemyFilter;
    private List<Collider2D> _collidersInRange = new List<Collider2D>();
    //private List<Enemy> _enemiesInRange = new List<Enemy>();

    // Getters
    public string Name => _name;
    public float Range => _range;
    public int Damage => _damage;
    public float AttackSpeed => _attackSpeed;

    // TEMP
    private void Awake()
    {
        InitializeTower();
    }

    // Call when spawning in the tower
    public void InitializeTower()
    {
        _enemyFilter.layerMask = LayerMasks.EnemyMask;
    }

    private void Update()
    {
        // Update the enemies in range list each frame
        ScanForEnemiesInRange();
    }

    private void ScanForEnemiesInRange()
    {
        int num = Physics2D.OverlapCircle(transform.position, _range, _enemyFilter, _collidersInRange);
        Debug.Log($"Number of enemies found in range: {num}");
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