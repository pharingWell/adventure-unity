using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    
    public string unitName;
    public int unitLevel;

    public int damage;
    public int maxHP;
    public int currentHP;

    public bool TakeDamage(int damage)
    {
        // subtract the damage from the current hp
        currentHP -= damage;
        // if the damage taken results in 0 or lower hp,
        // then unit is dead
        if(currentHP <= 0)
            return true;
        else
            return false;
    }

    public void Heal(int amount) 
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP; // don't go over max health
    }
}
