using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class Damageable : MonoBehaviour
{
    [SerializeField] public int maxHitPoints;
    public int currentHitPoints;
    [SerializeField] private bool isInvulnerable;
    [SerializeField] private bool isHealing;
    [SerializeField] private bool isBoss;
    [SerializeField] private bool isFinalBoss;
    [SerializeField] private bool isDestructibleObject;
    [SerializeField] private bool isLava;
    [SerializeField] private bool isBoulder;
    [SerializeField] private bool isTree;
    [SerializeField] private bool isSpider;
    [SerializeField] private bool isPlant;

    public bool IsHealing => isHealing;
    public bool IsBoss => isBoss;
    public bool IsFinalBoss => isFinalBoss;
    public bool IsDestructibleObject => isDestructibleObject;
    public bool IsLava => isLava;
    public bool IsBoulder => isBoulder;
    public bool IsTree => isTree;
    public bool IsSpider => isSpider;
    public bool IsPlant => isPlant;

    public UnityEvent OnDeath, OnHitWhileInvulnerable, OnReceiveDamage, OnHeal, OnFullyHealed;

    private void Awake()
    {
        ResetDamage();
    }

    public void ApplyDamage(DamageMessage data)
    {
        Debug.Log($"Damage applied: {data.amount}, Spider HP before: {currentHitPoints}");

        if (isInvulnerable)
        {
            OnHitWhileInvulnerable.Invoke();
            return;
        }

        currentHitPoints -= data.amount;

        if (gameObject.GetComponent<SpiderIK>() != null)
        {
            Debug.Log("Spidey Health after damage:" + currentHitPoints);

            if (currentHitPoints <= 0)
            {
                Debug.Log("Spidey is dead.");
                OnSpiderDeath();
                return;
            }
        }

        OnReceiveDamage.Invoke();
    }

    public void Heal(int amount)
    {
        if (currentHitPoints <= 0) return;

        currentHitPoints += amount;
        OnHeal.Invoke();

        if (currentHitPoints >= maxHitPoints)
        {
            currentHitPoints = maxHitPoints;
            OnFullyHealed.Invoke();
        }
    }

    public void ResetDamage()
    {
        currentHitPoints = maxHitPoints;
    }

    public void SetIsInvulnerable(bool isInvulnerable)
    {
        this.isInvulnerable = isInvulnerable;
    }

    public void OnWormDeath(){
        Debug.Log("Worm Died...");
        Destroy(gameObject);
    }

    public void OnSpiderDeath()
    {
        Debug.Log("Destroying spider...");

        SpiderNPCController spiderController = GetComponent<SpiderNPCController>();
        if (spiderController != null)
        {
            spiderController.Die();
        }
        else
        {
            Debug.LogError("SpiderNPCController not found on spider!");
            Destroy(gameObject);
        }
    }
}
