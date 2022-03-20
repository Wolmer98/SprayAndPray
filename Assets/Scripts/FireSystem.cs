using System;
using System.Reflection;
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
        [HideInInspector] public float Cooldown; // This wil be transfered back to the fireChain after being modified by the FireModifiers.
        public float Size;
        public float Speed;
        public int PierceTimes;
        public int ProjectilesOnDestroy;
        public float Lifetime;

        public List<Component> OnHitStatusEffects;

        public Func<Vector3, Vector3> GetTargetPosition;
    }

    [System.Serializable]
    public class FireChain
    {
        public FireRequest OriginalFireRequest; // Unmodified fire request, set upon main weapon change.

        [HideInInspector] public float CurrentCooldown;
        public float OriginalCooldown = 5.0f;
        [HideInInspector] public float TimeAtLastFire;
        [HideInInspector] public FireRequest CurrentFireRequest; // Fire request modified by FireModifiers, copied from OriginalFireRequest
        public List<Func<FireRequest, FireRequest>> FireModifiers = new List<Func<FireRequest, FireRequest>>();
    }

    public List<FireChain> FireChains = new List<FireChain>();

    void Update()
    {
        foreach (var fireChain in FireChains)
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
            fireChain.CurrentFireRequest.OnHitStatusEffects = new List<Component>();

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
        var spawnRotation = GetProjectileSpawnRotation(fireRequest, transform.position);

        // Instantiate and set stats.
        var projectile = Instantiate(fireRequest.ProjectilePrefab, fireRequest.SpawnPosition, spawnRotation);
        projectile.FireRequest = fireRequest;
        
        projectile.transform.localScale = Vector3.one * fireRequest.Size;

        var projectileMovement = projectile.GetComponent<ProjectileMovement>();
        projectileMovement.MovementSpeed *= fireRequest.Speed;
    }

    public static Quaternion GetProjectileSpawnRotation(FireRequest fireRequest, Vector3 point)
    {
        fireRequest.SpawnPosition = point;
        var targetPosition = fireRequest.GetTargetPosition(fireRequest.SpawnPosition);
        var spawnDirection = (targetPosition - fireRequest.SpawnPosition).normalized;
        float zRot = Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;
        var spawnRotation = Quaternion.Euler(0.0f, 0.0f, zRot - 90);

        return spawnRotation;
    }
}

public class FireModifiers
{
    public static FireSystem.FireRequest DamageIncrase(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Damage *= 2;
        return fireRequest;
    }

    public static FireSystem.FireRequest SizeIncrease(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Size *= 1.3f;
        return fireRequest;
    }

    public static FireSystem.FireRequest CooldownReduction(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Cooldown *= 0.5f;
        return fireRequest;
    }

    public static FireSystem.FireRequest HeavyProjectile(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Speed *= 0.6f;
        fireRequest.Damage *= 2.0f;
        return fireRequest;
    }

    public static FireSystem.FireRequest LightProjectile(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Speed *= 1.3f;
        fireRequest.Damage *= 0.7f;
        return fireRequest;
    }

    public static FireSystem.FireRequest ShortRangeCooldownReduction(FireSystem.FireRequest fireRequest)
    {
        if (GameManager.Instance.Player == null || fireRequest.GetTargetPosition == null)
            return fireRequest;

        var playerPosition = GameManager.Instance.Player.transform.position;
        var targetPosition = TargetMethods.ClosestEnemy(playerPosition);
        fireRequest.Cooldown *= Mathf.Lerp(0.4f, 1.0f, Vector3.Distance(playerPosition, targetPosition) / 7.0f);
        return fireRequest;
    }

    public static FireSystem.FireRequest DamagePerSize(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Damage *= Mathf.Lerp(1.0f, 5.0f, (fireRequest.Size - 1) / 2.0f);
        return fireRequest;
    }

    public static FireSystem.FireRequest PiercingProjectile(FireSystem.FireRequest fireRequest)
    {
        fireRequest.PierceTimes++;
        return fireRequest;
    }

    public static FireSystem.FireRequest ProjectilesOnDestroy(FireSystem.FireRequest fireRequest)
    {
        fireRequest.ProjectilesOnDestroy++;
        return fireRequest;
    }

    public static FireSystem.FireRequest LifetimeIncrease(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Lifetime *= 1.5f;
        return fireRequest;
    }

    public static FireSystem.FireRequest DamagePerLifetime(FireSystem.FireRequest fireRequest)
    {
        fireRequest.Lifetime *= Mathf.Lerp(1.0f, 5.0f, (fireRequest.Lifetime - 2.0f) / 6.0f);
        return fireRequest;
    }

    public static FireSystem.FireRequest FrostStatusEffect(FireSystem.FireRequest fireRequest)
    {
        fireRequest.OnHitStatusEffects.Add(new FrostStatusEffect());
        return fireRequest;
    }

    public static FireSystem.FireRequest FireStatusEffect(FireSystem.FireRequest fireRequest)
    {
        fireRequest.OnHitStatusEffects.Add(new FireStatusEffect());
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
                {
                    closestDist = dist;
                    closestEnemy = collider.transform.position;
                }
            }
        }

        return closestEnemy;
    }

    public static Vector3 PeriodicCircle(Vector3 point)
    {
        const float radius = 20.0f;

        var dir = Vector3.right;
        float degrees = (Time.time * 0.2f % 1.0f) * -360.0f;
        dir = Quaternion.Euler(0.0f, 0.0f, degrees) * dir;

        return point + (dir * radius);
    }

    public static Vector3 RandomCardinalDirection(Vector3 point)
    {
        const float radius = 20.0f;

        var dir = Vector3.zero;
        int rng = UnityEngine.Random.Range(0, 4);
        switch (rng)
        {
            case 0: dir = new Vector3(1.0f, 0.0f); break;
            case 1: dir = new Vector3(0.0f, 1.0f); break;
            case 2: dir = new Vector3(-1.0f, 0.0f); break;
            case 3: dir = new Vector3(0.0f, -1.0f); break;
        }
        return point + (dir * radius);
    }
}
