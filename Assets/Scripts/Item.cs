using System;
using UnityEngine;

public class Item : MonoBehaviour, IHit
{
    [SerializeField] ItemSO itemSO;
    public float currentHealth;

    void Start()
    {
        OnInit();
    }

    public void OnInit()
    {
        currentHealth = itemSO.maxHealth;
    }
    public int GetCurrentLevel()
    {
        return itemSO.levelRequired;
    }

    public void OnHit(float damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            OnDie();
        }

    }

    private void OnDie()
    {
        //TODO: disable healthbar, rayfire

        Debug.Log($"rayfire");
    }
}