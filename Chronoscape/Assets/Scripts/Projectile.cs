using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField, Tooltip("How much damage the projectile inflicts.")]
    private float attackDamage;

    [SerializeField, Tooltip("How long in seconds until the projectile despawns.")]
    private float projectileLifespan;

    private void Start()
    {
        StartCoroutine(ProjectileTimer());
    }

    // Damages an object when the projectile collides with it.
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.DoDamage(attackDamage);
            Debug.Log($"{damageableObject} was damaged!");
        }
    }

    private IEnumerator ProjectileTimer()
    {
        yield return new WaitForSeconds(projectileLifespan);
        Destroy(gameObject);
    }
}
