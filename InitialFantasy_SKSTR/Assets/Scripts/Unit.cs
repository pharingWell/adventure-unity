using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISavable
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int AttackDamage { get; set; }
    public Unit(string name, int level, int startingHp, int maxHp, int attackDamage) {
        Name = name;
        Level = level;
        Health = startingHp;
        MaxHealth = maxHp;
        AttackDamage = attackDamage;
        SaveManager.Register(GetInstanceID(),    
            new List<TypeConduitPair>{
                new(typeof(string), o => Name = (string)o, () => Name),
                new (typeof(int), o => Level = (int)o, () => Level),
                new (typeof(int), o => Health = (int)o, () => Health),
                new (typeof(int), o => MaxHealth = (int)o, () => MaxHealth),
                new (typeof(int), o => AttackDamage = (int)o, () => AttackDamage)
            }
        );
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
