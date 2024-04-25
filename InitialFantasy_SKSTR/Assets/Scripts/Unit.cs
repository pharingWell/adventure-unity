using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    [SerializeField] private int xp;
    [SerializeField] private int levelUpAmount;
    [SerializeField] private int attackDamage;
    [SerializeField] private int currentHp;
    [SerializeField] private int maxHp;
        
    public Unit(string unitName, int unitLevel, int xp, int levelUpAmount, int startingHp, int maxHp, int attackDamage) {
        Name = unitName;
        Level = unitLevel;
        XP = xp;
        LevelUpAmount = levelUpAmount;
        Health = startingHp;
        MaxHealth = maxHp;
        AttackDamage = attackDamage;
    }

    public string Name
    {
        get => unitName;
        private set => unitName = value;
    }
    public int Level
    {
        get => unitLevel;
        private set => unitLevel = value;
    }
    public int XP
    {
        get => xp;
        private set => xp = value;
    }
    public int LevelUpAmount
    {
        get => levelUpAmount;
        private set => levelUpAmount = value;
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

    /*
    // below are functions that would be used with a level up feature,
    // one that is beyond the development of this beta
    // other vestiges have been left in place as they don't take up much,
    // but the relevant functions have been removed

    public void GainXP(int amount)
    {
        if ((xp + amount) >= levelUpAmount) // if the player has enough xp to level up
        {   
            Debug.Log("should level up");
            LevelUp(amount); // call LevelUp function
        } else 
        {
            xp += amount; // add gained xp to xp total
        }
    }

    public void LevelUp(int amount) {
        // first, update the amount of xp the player has now
        // i'm going by the system of current xp + xp --> level up! 
        // then current xp resets to 0 + whatever xp wasn't used to level up
        xp += amount;
        xp -= levelUpAmount;
        unitLevel += 1;
        levelUpAmount = levelUpAmount * 500; // new threshold for leveling up

        // now boost stats
        maxHp += 3; // each level gives an additional 3 maxHp
        currentHp = maxHp; // heal player after battle?
        attackDamage += 2; // each level gives an additional 2 attackDamage
    }
    */

}
