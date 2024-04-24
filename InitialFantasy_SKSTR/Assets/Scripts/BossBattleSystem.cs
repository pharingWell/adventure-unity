using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // needed for TextMeshProUGUI to work

// this is essentially the same as BattleSystem.cs
// however, it is customized for boss battle
//      because the battle's behavior and main mechanics are different.
// i've kept the "normal" battle system lines (when relevant) to show what would
//      have been used instead to make edits and understanding the 
//      code a bit easier, at least in my opinion.
// that doesn't apply to areas where the code is just too different to the
//      point that it doesn't make sense to have the originals.
// also, because this is based on BattleSystem.cs, i cite that i
//      used this tutorial to make the system: 
//      https://www.youtube.com/watch?v=_1pz_ohupPs

// - Toby (now sadly just "Mimi" on SIS)

public enum BossBattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BossBattleSystem : MonoBehaviour
{
    // prefabs
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    // battle stations are the ritual circles the enemy and player sit on 
    public Transform playerBossBattleStation;
    public Transform enemyBossBattleStation;

    // two instances of Unit
    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI dialogueText; // use TextMeshProUGUI for hud
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public GameObject actions;
    
    public BossBattleState state;

    // Start is called before the first frame update
    void Start()
    {
        state = BossBattleState.START;
        StartCoroutine(SetupBattle()); 
    }

    IEnumerator SetupBattle()  // the coroutine (glorified funtion that handles everything in a state)
    {
        actions.SetActive(false);
        // instantiate a player game object using the player prefab
        // spawn it on top of the player battle station
        GameObject playerGO = Instantiate(playerPrefab, playerBossBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        // instantiate an enemy game object using the enemy prefab
        // spawn it on top of the enemy battle station
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBossBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        // changes the dialogue text to include the enemy's name
        // dialogueText.text = "You encountered a " + enemyUnit.Name + "!";
        dialogueText.text = "You encountered aaaaaa ERR0R_n0_n@m#? ?";
        
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f); // need coroutines for this line

        state = BossBattleState.PLAYERTURN; // now that the battle is set up, let the player have their turn
        // if we want the enemy to go first, the above line should be state = BattleState.PLAYERTURN;
        // and then below, EnemyTurn();
        PlayerTurn();
    }

    IEnumerator EnemyTurn() 
    {
        // dialogueText.text = enemyUnit.Name + " attacks!";
        dialogueText.text = "";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.AttackDamage); // is player dead after taking damage?

        playerHUD.SetHp(playerUnit.Health); // player's hp bar reflects new hp; also updates hp text

        yield return new WaitForSeconds(1f);

        if (isDead) // player is dead
        {
            state = BossBattleState.LOST;
            EndBattle();
        } else // player survived
        {
            state = BossBattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

    void PlayerTurn() 
    {
        dialogueText.text = "Choose an action:";
        actions.SetActive(true);
    }

     IEnumerator PlayerAttack()
    {
        // damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.AttackDamage);

        enemyHUD.SetHp(enemyUnit.Health);
        dialogueText.text = "The attack is successful!";
        yield return new WaitForSeconds(1f); // could be more
        dialogueText.text = "You dealt " + playerUnit.AttackDamage + " damage!";

        // check if enemy is dead
        if(isDead)
        {
            // end the battle
            state = BossBattleState.WON;
            yield return new WaitForSeconds(2f);
            StartCoroutine(EndBattle());
        } else 
        {
            // enemy turn
            state = BossBattleState.ENEMYTURN; 
            yield return new WaitForSeconds(2f);
            StartCoroutine(EnemyTurn());
        }
        // change state based on what happened
    }

    IEnumerator EndBattle() // should probably turn this into a coroutine later
    {
        // only updating dialogue text at the moment, so coroutine isn't necessary... yet
        if (state == BossBattleState.WON)
        {
            dialogueText.text = "You won the battle!";
            yield return new WaitForSeconds(2f);
            // maybe the player gains xp? obviously they don't really get to "keep" their levels
            // but the illusion of getting xp and levels
            int currLevel = playerUnit.Level;
            playerUnit.GainXP(enemyUnit.XP); // player gains as much xp as the enemy has
            dialogueText.text = "You gained " + enemyUnit.XP + " XP.";
            if (currLevel < playerUnit.Level) 
            {   
                yield return new WaitForSeconds(1f);
                dialogueText.text = "Wow! You leveled up to " + playerUnit.Level + ".";
                yield return new WaitForSeconds(2f);
                dialogueText.text = "Your Max HP is now " + playerUnit.MaxHealth 
                                    + ", and your Attack is " + playerUnit.AttackDamage + ".";
            } else 
            {
                yield return new WaitForSeconds(1f);
                dialogueText.text = "[End BossBattle]";
            }
        } else if (state == BattleState.LOST)
        {
            dialogueText.text = "You lost.";
            // would probably load out of battle 
        }
    }

    IEnumerator PlayerHeal() 
    {
        playerUnit.Heal(5); // arbitrary number
        // would probably be dependent on items in future
        playerHUD.SetHp(playerUnit.Health);
        dialogueText.text = "You feel renewed strength!";

        state = BossBattleState.ENEMYTURN; // should prevent player from infinite healing

        yield return new WaitForSeconds(2f);
        // player used their turn by healing
        // we could choose for them to be able to use items without turn ending
        //state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton() // player clicks attack button
    {
        if (state != BossBattleState.PLAYERTURN)
            return;
        actions.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton() // player clicks heal button
    {
        if (state != BossBattleState.PLAYERTURN) 
            return;
        actions.SetActive(false);
        StartCoroutine(PlayerHeal());
    }

}
