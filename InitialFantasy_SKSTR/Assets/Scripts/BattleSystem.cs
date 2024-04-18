using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IFSKSTR.SaveSystem;
using IFSKSTR.SaveSystem.GDB.SaveSerializer;
using UnityEngine.UIElements; // needed for TextMeshProUGUI to work

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

    public IPanel DialoguePanel;
    public TextMeshProUGUI dialogueText; // use TextMeshProUGUI for hud

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        SaveSerializer.GameDataSaved += GameSaved;
       // SaveSerializer.GameDataLoaded += GameLoaded;
    }

    void GameSaved()
    {
        playerUnit.Health = 1;
        SaveSystem.Load();
    }
    
    void GameLoaded(object o)
    {
        
    }
    
    IEnumerator SetupBattle()  // the coroutine (glorified function that handles everything in a state)
    {
        // instantiate a player game object using the player prefab
        // spawn it on top of the player battle station
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        playerUnit.Reset();

        // instantiate an enemy game object using the enemy prefab
        // spawn it on top of the enemy battle station
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyUnit.Reset();
        
        // changes the dialogue text to include the enemy's name
        dialogueText.text = "You encountered a " + enemyUnit.Name + "!";
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit); 
        yield return new WaitForSeconds(1f); // need coroutines for this line
        SaveSystem.Save();
        state = BattleState.PLAYERTURN; // now that the battle is set up, let the player have their turn
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        // damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.AttackDamage);

        enemyHUD.SetHp(enemyUnit.Health);
        dialogueText.text = "The attack is successful!";

        //yield return new WaitForSeconds(2f); // need to prevent player from infinite attacking

        // check if enemy is dead
        if(isDead)
        {
            // end the battle
            state = BattleState.WON;
            yield return new WaitForSeconds(2f);
            EndBattle();
        } else 
        {
            // enemy turn
            state = BattleState.ENEMYTURN; 
            yield return new WaitForSeconds(2f);
            StartCoroutine(EnemyTurn());
        }
        // change state based on what happened
    }

    IEnumerator EnemyTurn() 
    {
        dialogueText.text = enemyUnit.Name + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.AttackDamage); // is player dead after taking damage?

        playerHUD.SetHp(playerUnit.Health);

        yield return new WaitForSeconds(1f);

        if (isDead) // player is dead
        {
            state = BattleState.LOST;
            EndBattle();
        } else // player survived
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

    void EndBattle() // should probably turn this into a coroutine later
    {
        // only updating dialogue text at the moment, so coroutine isn't necessary... yet
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        } else if (state == BattleState.LOST)
        {
            dialogueText.text = "You lost.";
            // would probably load out of battle 
        }
    }


    void PlayerTurn() 
    {
        dialogueText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal() 
    {
        playerUnit.Heal(5); // arbitrary number
        // would probably be dependent on items in future
        playerHUD.SetHp(playerUnit.Health);
        dialogueText.text = "You feel renewed strength!";

        state = BattleState.ENEMYTURN; // should prevent player from infinite healing

        yield return new WaitForSeconds(2f);
        // player used their turn by healing
        // we could choose for them to be able to use items without turn ending
        //state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton() // player clicks attack button
    {
        if (state != BattleState.PLAYERTURN) 
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton() // player clicks heal button
    {
        if (state != BattleState.PLAYERTURN) 
            return;

        StartCoroutine(PlayerHeal());
    }

}
