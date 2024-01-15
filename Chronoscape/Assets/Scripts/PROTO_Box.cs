using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PROTO_Box : MonoBehaviour, IDamageable
{
    private float health = 10f;

    private void Update()
    {
        if (health <= 0) Destroy(gameObject);
    }

    public void DoDamage(float damage)
    {
        health -= damage;
    }
}
