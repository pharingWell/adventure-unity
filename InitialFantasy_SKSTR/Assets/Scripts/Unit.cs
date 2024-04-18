using System;
using System.Collections.Generic;
using IFSKSTR.SaveSystem;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    public string Name { get => unitName; set => unitName = Name; }
    [SerializeField] private int level;
    public int Level { get => level; set => level = Level; }
    private int _health;
    public int Health { get => _health; set => _health = Health; }
    [SerializeField] private int maxHealth;
    public int MaxHealth { get => maxHealth; set => maxHealth = MaxHealth; }
    [SerializeField] private int attackDamage;
    public int AttackDamage { get => attackDamage; set => attackDamage = AttackDamage; }

    private void Start()
    {
        SaveSystem.Register(GetInstanceID(),    
            new List<TypeConduitPair>{
                new(typeof(string), o => Name = (string)o, () => Name),
                new (typeof(int), o => Level = (int)o, () => Level),
                new (typeof(int), o => Health = (int)o, () => Health),
                new (typeof(int), o => MaxHealth = (int)o, () => MaxHealth),
                new (typeof(int), o => AttackDamage = (int)o, () => AttackDamage)
            }
        );
        
    }

    public void Reset()
    {
        Health = MaxHealth;
    }

    public bool TakeDamage(int amount)
    {
        if (amount > 0) Health -= Math.Min(amount, Health); // don't go under 0
       
        if (Health < 0) Health = 0;
        return (Health == 0);  // if the health is 0 or less, then unit is dead
    }

    public void Heal(int amount) 
    {
        Health += Math.Min(amount, MaxHealth-Health); // don't go over max health 
    }

}
