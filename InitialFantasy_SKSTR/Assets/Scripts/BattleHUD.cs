using System.Collections;
using System.Collections.Generic;
using IFSKSTR.SaveSystem;
using IFSKSTR.SaveSystem.GDB.SaveSerializer;
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
    private Unit _unit;
    public void SetUnit(Unit unit)
    {
        _unit = unit;
        SaveSerializer.GameDataLoaded += SetHUD;
        SetHUD();
    }

    public void SetHUD()
    {
        nameText.text = _unit.Name;
        levelText.text = "Level " + _unit.Level;
        healthSlider.maxValue = _unit.MaxHealth;
        healthSlider.value = _unit.Health;
        healthText.text = _unit.Health.ToString();
        print(_unit.Level);

    } 

    public void SetHp(int health)
    {
        healthText.text = health.ToString();
        healthSlider.value = health;
    }
}
