using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization; // needed for TextMeshProUGUI to work

// i used the tutorial from https://www.youtube.com/watch?v=_1pz_ohupPs
// to write this code - Toby

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    public Slider healthSlider;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.Name;
        levelText.text = "Level " + unit.Level;
        healthSlider.maxValue = unit.MaxHealth;
        healthSlider.value = unit.Health;
        healthText.text = unit.Health.ToString();

    } 

    public void SetHp(int health)
    {
        healthText.text = health.ToString();
        healthSlider.value = health;
    }
}
