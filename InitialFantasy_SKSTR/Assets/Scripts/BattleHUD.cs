using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // needed for TextMeshProUGUI to work

// i used the tutorial from https://www.youtube.com/watch?v=_1pz_ohupPs
// to write this code - Toby

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    public TextMeshProUGUI hpText; // also want hp displayed
    public Slider hpSlider;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.Name;
        levelText.text = "Level " + unit.Level;
        hpSlider.maxValue = unit.MaxHealth;
        hpSlider.value = unit.Health;
        hpText.text = "HP " + unit.Health; // display hp
    } 

    public void SetHp(int hp) {
        hpSlider.value = hp;
        hpText.text = "HP " + hp; // update HP text
    }
    
}
