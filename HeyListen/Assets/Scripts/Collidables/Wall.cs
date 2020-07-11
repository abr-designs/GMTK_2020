using UnityEngine;

public class Wall : Collidable
{
    protected override string collisionTag => "Player";
    protected override void Collided(Collider2D other)
    {
        other.GetComponent<Character>().HitWall();
    }
}
