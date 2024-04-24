using System;
using System.Collections.Generic;
using IFSKSTR.SaveSystem;
using IFSKSTR.SaveSystem.GDB.SaveSerializer;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour, ISavable
{
    [SerializeField] private string unitName;
    public string Name { get => unitName; set => unitName = value; }
    [SerializeField] private int level;
    [Author]
    public int Level { get => level; set => level = value; }
    [Author]
    private int _health;
    public int Health { get => _health; set => _health = value; }
    [SerializeField] private int maxHealth;
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    [SerializeField] private int attackDamage;
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }
    private void Start()
    {
        SaveSystem.Register(gameObject, new List<TypeConduitPair>{
                new(typeof(string), () => Name, o => Name = (string)o),
                new (typeof(int), () => Level, o => Level = (int)o),
                new (typeof(int), () => _health, o => _health = (int)o),
                new (typeof(int), () => MaxHealth, o => MaxHealth = (int)o),
                new (typeof(int), () => AttackDamage, o => AttackDamage = (int)o)
            }, this
        );
    }

    public void OnLoad()
    {
        
    }

    public void OnSave()
    {
        
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
