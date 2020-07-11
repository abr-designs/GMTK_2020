using UnityEngine;

public class Treasure : Collidable
{
    protected override string collisionTag => "Player";

    [SerializeField, Range(0f,100f)]
    private float logicImprovement; 
    
    protected override void Collided(Collider2D other)
    {
        other.GetComponent<Character>().AdjustLogicLevel(logicImprovement / 100f);
    }
}
