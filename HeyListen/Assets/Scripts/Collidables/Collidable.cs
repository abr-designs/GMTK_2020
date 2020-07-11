using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Collidable : MonoBehaviour
{
    protected abstract string collisionTag { get; }

    protected new SpriteRenderer renderer
    {
        get
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();
            return _renderer;
        }
    }
    private SpriteRenderer _renderer;

    protected new Collider2D collider
    {
        get
        {
            if (_collider == null)
                _collider = GetComponent<Collider2D>();
            return _collider;
        }
    }
    private Collider2D _collider;

    protected new Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = gameObject.transform;
            return _transform;
        }
    }
    private Transform _transform;

    //================================================================================================================//
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(collisionTag))
            return;

        Collided(other);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag(collisionTag))
            return;

        Collided(other.collider);
    }
    
    //================================================================================================================//

    protected abstract void Collided(Collider2D other);
}
