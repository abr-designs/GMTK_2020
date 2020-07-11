using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Collidable
{
    protected override string collisionTag => "Player";

    [SerializeField]
    private float healthAmount;
    
    protected override void Collided(Collider2D other)
    {
        other.gameObject.GetComponent<IHealth>().ChangeHealth(healthAmount);
    }
}
