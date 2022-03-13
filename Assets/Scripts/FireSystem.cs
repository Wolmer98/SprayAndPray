using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
    [System.Serializable]
    public struct FireRequest
    {
        public Projectile ProjectilePrefab;
        public Vector3 SpawnPosition;

        public float Damage;
        public float Cooldown; // This wil be transfered back to the fireChain after being modified by the FireModifiers.
        public float Size;
        public float Speed;

        public Func<Vector3, Vector3> GetTargetPosition;
    }

    public class FireChain
    {
        public FireRequest OriginalFireRequest; // Unmodified fire request, set upon main weapon change.

        public float CurrentCooldown;
        public float OriginalCooldown = 5.0f;
        public float TimeAtLastFire;
        public FireRequest CurrentFireRequest; // Fire request modified by FireModifiers, copied from OriginalFireRequest
        public List<Func<FireRequest, FireRequest>> FireModifiers = new List<Func<FireRequest, FireRequest>>();
    }

    public FireRequest TestRequest;
    private List<FireChain> m_fireChains = new List<FireChain>();

    private void Start()
    {
        // Temp.
        TestRequest.GetTargetPosition = TargetMethods.ClosestEnemy;

        var testFireChain = new FireChain();
        testFireChain.OriginalFireRequest = TestRequest;
        testFireChain.FireModifiers.Add(FireModifiers.DamageIncrase);
        testFireChain.FireModifiers.Add(FireModifiers.CooldownReduction);
        testFireChain.FireModifiers.Add(FireModifiers.CooldownReduction);
        testFireChain.FireModifiers.Add(FireModifiers.CooldownReduction);
        m_fireChains.Add(testFireChain);
    }

    void Update()
    {
        foreach (var fireChain in m_fireChains)
        {
            if (TryFireChain(fireChain))
            {
                fireChain.TimeAtLastFire = Time.timeSinceLevelLoad;
            }
        }
    }

    public bool TryFireChain(FireChain fireChain)
    {
        if (Time.timeSinceLevelLoad >= (fireChain.TimeAtLastFire + fireChain.CurrentCooldown))
        {
            fireChain.CurrentFireRequest = fireChain.OriginalFireRequest;
            fireChain.CurrentFireRequest.Cooldown = fireChain.OriginalCooldown;

            // Alter the request through the fire modifiers.
            foreach (var fireModifier in fireChain.FireModifiers)
            {
                fireChain.CurrentFireRequest = fireModifier(fireChain.CurrentFireRequest);
            }

            ExecuteFireRequest(fireChain.CurrentFireRequest);

            fireChain.CurrentCooldown = fireChain.CurrentFireRequest.Cooldown; // Set the current cooldown after modifiers might have altered it.

            return true;
        }

        return false;
    }

    void ExecuteFireRequest(FireRequest fireRequest)
    {
        if (fireRequest.ProjectilePrefab == null)
            return;

        // Get position and rotation.
        fireRequest.SpawnPosition = transform.position;
        var targetPosition = fireRequest.GetTargetPosition(fireRequest.SpawnPosition);
        var spawnDirection = (targetPosition - fireRequest.SpawnPosition).normalized;
        float zRot = Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;
        var spawnRotation = Quaternion.Euler(0.0f, 0.0f, zRot - 90);

        // Instantiate and set stats.
        var projectile = Instantiate(fireRequest.ProjectilePrefab, fireRequest.SpawnPosition, spawnRotation);
        projectile.Damage = fireRequest.Damage;
        projectile.transform.localScale = Vector3.one * fireRequest.Size;

        var projectileMovement = projectile.GetComponent<ProjectileMovement>();
        projectileMovement.MovementSpeed *= fireRequest.Speed;
    }
}

public class FireModifiers
{
    public interface IFireModifier
    {
        FireSystem.FireRequest ProcessFireRequest(FireSystem.FireRequest fireRequest);
    }

    public static FireSystem.FireRequest DamageIncrase(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Damage *= 2;
        return fireRequest;
    }

    public static FireSystem.FireRequest SizeIncrease(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Size *= 2;
        return fireRequest;
    }

    public static FireSystem.FireRequest CooldownReduction(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Cooldown *= 0.75f;
        return fireRequest;
    }

    public static FireSystem.FireRequest HeavyProjectile(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Speed *= 0.7f;
        fireRequest.Damage *= 1.3f;
        return fireRequest;
    }
}

public class TargetMethods
{
    public static Vector3 ClosestEnemy(Vector3 point)
    {
        const float radius = 20.0f;

        var colliders = Physics2D.OverlapCircleAll(point, radius);
        float closestDist = Mathf.Infinity;
        Vector3 closestEnemy = Vector3.zero;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                var dist = Vector2.Distance(point, collider.transform.position);
                if (dist <= closestDist)
                    closestEnemy = collider.transform.position;
            }
        }

        return closestEnemy;
    }
}
