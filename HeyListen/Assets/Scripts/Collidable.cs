using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Collidable : MonoBehaviour
{
    protected abstract string collisionTag { get; }

    protected new SpriteRenderer renderer;
    protected new Collider2D collider;
    protected Transform transform;
    
    // Start is called before the first frame update
    private void Start()
    {
        transform = gameObject.transform;
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag(collisionTag))
            return;

        Collided(other);
    }

    protected abstract void Collided(Collision2D other);
}
