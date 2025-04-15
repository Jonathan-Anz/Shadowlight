using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public enum ProjectileMovementType
{
    StraightLine, Seeking
}

public class Projectile : MonoBehaviour
{
    // Projectile properties: set in inspector
    [SerializeField] private ProjectileMovementType _movementType;
    [SerializeField] private int _damage;
    [SerializeField] private float _speed;
    [SerializeField] private int _pierce;
    [SerializeField] private float _lifetime;

    // Each projectile has a target to follow
    private Enemy _target;
    private Vector3 _moveDir = Vector3.zero;


    // Initializer (call when the projectile is spawned)
    public void InitializeProjectile(Enemy target)
    {
        _target = target;

        // If this is a straightline projectile, set its movement direction
        if (_movementType == ProjectileMovementType.StraightLine)
        {
            _moveDir = (_target.transform.position - transform.position).normalized;
        }
    }

    private void Update()
    {
        // Update lifetime
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Check if its target has already been destroyed
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move
        if (_movementType == ProjectileMovementType.StraightLine)
        {
            // If this is a straightline projectile, move along the initial direction
            transform.position += _moveDir * _speed * Time.deltaTime;
        }
        else if (_movementType == ProjectileMovementType.Seeking)
        {
            // If this is a seeking projectile, follow the target
            transform.position = Vector3.MoveTowards(   transform.position,
                                                        _target.transform.position,
                                                        _speed * Time.deltaTime );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer == LayerMasks.EnemyMask)
        {
            // If the projectile hits an enemy...
            //Debug.Log("Projectile hit enemy!");

            // Damage the enemy
            collision.gameObject.GetComponent<Enemy>().DamageEnemy(_damage);

            // If the projectile is seeking and the enemy it hit is the target, destroy it
            if (_movementType == ProjectileMovementType.Seeking &&
                collision.gameObject == _target.gameObject)
            {
                //Debug.Log("Hit target enemy!");

                Destroy(gameObject);
                return;
            }

            // Remove one from pierce
            _pierce -= 1;
            if (_pierce <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

}
