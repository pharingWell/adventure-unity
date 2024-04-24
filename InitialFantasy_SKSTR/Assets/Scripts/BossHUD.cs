using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // needed for TextMeshProUGUI to work

// this is essentially the same as BattleHUD.cs
// however, it is customized for the boss in the boss battle
//      because the boss doesn't have normal hp, level text, etc.
//      and may use special characters when those things are not normally
//      acceptable or use variable types that do not line up with the
//      ones in the normal BattleHUD.cs.
// the layout of this code is roughly the same as BattleHUD.cs
// also, because this is based on BattleHUD.cs, i cite that i
//      used this tutorial to make the system: 
//      https://www.youtube.com/watch?v=_1pz_ohupPs

// - Toby (now sadly just "Mimi" on SIS)

public class BossHUD : MonoBehaviour
{
     public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;

    public TextMeshProUGUI hpText; // also want hp displayed
    public Slider hpSlider;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.Name;
        levelText.text = "01001001 00100000 01100001 01101101 00100000 01010100 01101000 01100001 01110100 00100000 01110111 01101000 01101001 01100011 01101000 00100000 01000011 01100001 01101110 01001110 01001111 01010100 00100000 01100010 01100101 00100000 01001011 01101110 01101111 01110111 01101110 00101110";
        // the above translates to: "I am That which CanNOT be Known."

        hpSlider.maxValue = unit.MaxHealth;
        hpSlider.value = unit.Health;

        // idea: convey hp info not in normal numbers but rather in words
        // hp starts off at a ridiculously high number
        hpText.text = "giv3 uuupp 'hero' ";
    } 

    public void SetHp(int hp, Unit unit) { // a unit is included to get the health values
        hpSlider.value = hp;
        
        // idea: convey hp info not in normal numbers but rather in words
        if (unit.Health <= 500 && unit.Health > 100) 
        {   
            // the player has actually dealt some damage
            hpText.text = "p3rh@ps u cann ?"; 
        } else if (unit.Health <= 100) 
        {
            // the player is close to success
            hpText.text = "c c3@SE 111mmeDIEat3l yy!!";
        } else 
        {
            // the player has not dealt enough damage yet to be a concern
            // to the horror
            hpText.text = "giv3 uuupp 'hero' ";
        }
    }
}
