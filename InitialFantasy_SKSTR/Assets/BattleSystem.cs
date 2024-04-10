using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // needed for TextMeshProUGUI to work

// i used the tutorial from https://www.youtube.com/watch?v=_1pz_ohupPs
// to write this code - Toby

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI dialogueText; // use TextMeshProUGUI for hud

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle() 
    {
        // instantiate a player game object using the player prefab
        // spawn it on top of the player battle station
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        // instantiate an enemy game object using the enemy prefab
        // spawn it on top of the enemy battle station
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        // changes the dialogue text to include the enemy's name
        dialogueText.text = "You encountered a " + enemyUnit.unitName + "!";
        
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
    }

}
