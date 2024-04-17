using System;
using UnityEngine;

public class Unit : MonoBehaviour, ISavable
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    [SerializeField] private int attackDamage;
    [SerializeField] private int currentHp;
    [SerializeField] private int maxHp;
    public Unit(string unitName, int unitLevel, int startingHp, int maxHp, int attackDamage) {
        Name = unitName;
        Level = unitLevel;
        Health = startingHp;
        MaxHealth = maxHp;
        AttackDamage = attackDamage;
        Conduit<string> t = new Conduit<string>(s => Name = s, () => Name);
    }
    
    
    
    public string Name
    {
        get => unitName; 
        set => unitName = value;
    }
    public int Level
    {
        get => unitLevel;
        private set => unitLevel = value;
    }
    public int Health
    {
        get => currentHp;
        private set => currentHp = value;
    }
    public int MaxHealth
    {
        get => maxHp;
        private set => maxHp = value;
    }
    public int AttackDamage
    {
        get => attackDamage;
        private set => attackDamage = value;
    }
    
    public bool TakeDamage(int amount)
    {
        if (amount > 0) currentHp -= Math.Min(amount, currentHp); // don't go under 0
       
        if (currentHp < 0) currentHp = 0;
        return (currentHp == 0);  // if the health is 0 or less, then unit is dead
    }

    public void Heal(int amount) 
    {
        currentHp += Math.Min(amount, maxHp-currentHp); // don't go over max health 
    }

}
