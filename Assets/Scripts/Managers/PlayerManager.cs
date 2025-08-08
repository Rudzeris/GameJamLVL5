using System;
using UnityEngine;

[AddComponentMenu("Managers/Player")]
public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    public int MaxHealth { get; private set; }
    public int Health { get; private set; }

    public void UpdateData(int health, int maxHealth)
    {
        Health = health;
        MaxHealth = maxHealth;
    }

    public void ChangeHealth(int value)
    {
        Health += value;
        if (Health <= 0)
            Health = 0;
        else if (Health > MaxHealth)
            Health = MaxHealth;
    }
    public void Startup()
    {
        Debug.Log("Player Manager starting...");
        Status = ManagerStatus.Started;
    }
}
