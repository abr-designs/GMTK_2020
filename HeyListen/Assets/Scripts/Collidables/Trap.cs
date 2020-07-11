using UnityEngine;

public class Trap : Collidable
{
    // Start is called before the first frame update
    protected override string collisionTag => "Player";

    [SerializeField]
    private float damage;
    
    //================================================================================================================//
    
    protected override void Collided(Collider2D other)
    { 
        other.gameObject.GetComponent<IHealth>().ChangeHealth(-damage);

        //If we collide we want to disable the object from interacting with the character again
        collider.enabled = false;
    }
}
