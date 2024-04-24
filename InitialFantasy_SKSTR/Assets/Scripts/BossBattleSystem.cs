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
// used this tutorial to understand how to adjust a sprite's color from a script:
//      https://stuartspixelgames.com/2019/02/19/how-to-change-sprites-colour-or-transparency-unity-c/
//      along with the man page on Color for understanding how color values work in
//      script vs. in editor:
//      https://docs.unity3d.com/ScriptReference/Color.html
// this man page explains random number generation:
//      https://docs.unity3d.com/ScriptReference/Random.Range.html

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

    public SpriteRenderer enemyBBStationSprite; // needed to adjust color of enemy's battle station

    // two instances of Unit
    Unit playerUnit;
    Unit enemyUnit;

    // colors for battle: one is the player input, one is the boss's answer
    string playerColor;
    string enemyColor;

    float healFactor; // amount by which player heals

    public TextMeshProUGUI dialogueText; // use TextMeshProUGUI for hud
    public BattleHUD playerHUD;
    public BossHUD enemyHUD;
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
        // PlayerTurn();
        // in this case, the enemy should go first because it will change its color
        // after attacking
        EnemyTurn();
    }

    IEnumerator EnemyTurn() 
    {
        // dialogueText.text = enemyUnit.Name + " attacks!";
        dialogueText.text = "n n@m3!? attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.AttackDamage); // is player dead after taking damage?

        playerHUD.SetHp(playerUnit.Health); // player's hp bar reflects new hp; also updates hp text

        yield return new WaitForSeconds(1f);

        // set the enemy's color, which the player will have to correctly guess
        // quick note: unlike in editor, Color(r, g, b, a) only takes values from
        // 0 to 1, not 0 to 255. you can use decimals, though.
        int colorSelected = Random.Range(1, 5); // int version of Random.Range --> upper bound is EXCLUSIVE
        switch(colorSelected) 
        {
            case 1: // red
                enemyColor = "RED";
                enemyBBStationSprite.color = new Color(1, 0, 0, 1); 
                break;
            case 2: // green
                enemyColor = "GREEN";
                enemyBBStationSprite.color = new Color(0, 1, 0, 1);
                // code block
                break;
            case 3: // blue
                enemyColor = "BLUE";
                enemyBBStationSprite.color = new Color(0, 0, 1, 1);
                break;
            case 4: // yellow
                enemyColor = "YELLOW";
                enemyBBStationSprite.color = new Color(1, 0.92f, 0.016f, 1);
                break;
            default: // red 
                enemyColor = "RED";
                enemyBBStationSprite.color = new Color(1, 0, 0, 1);
                // code block
                break;
        }

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
        // the "actions" are really just the colors
        actions.SetActive(true);
    }

     IEnumerator PlayerAttack()
    {
        // does the "attack" match the correct color?
        if (string.Equals(playerColor, enemyColor)) 
        {
            // damage the enemy
            bool isDead = enemyUnit.TakeDamage(playerUnit.AttackDamage);
            // enemyHUD.SetHP(enemyUnit.Health);
            enemyHUD.SetHp(enemyUnit.Health, enemyUnit); // change with BossHUD.cs --> must include unit
            // dialogueText.text = "The attack is successful!";
            dialogueText.text = "You did... something, but does it matter...?";
            // yield return new WaitForSeconds(1f); // could be more
            // dialogueText.text = "You dealt " + playerUnit.AttackDamage + " damage!";
            // check if enemy is dead
            // change state based on what happened
            if(isDead)
            {
                // end the battle
                state = BossBattleState.WON;
                yield return new WaitForSeconds(2f);
                StartCoroutine(EndBattle());
            } 
            // i think the player should also be able to recover the damage they took
            StartCoroutine(PlayerHeal()); // the switch to ENEMYTURN and EnemyTurn() is within PlayerHeal()
        } else 
        {
            dialogueText.text = "The being seems unaffected.";
            // enemy turn
            state = BossBattleState.ENEMYTURN; 
            yield return new WaitForSeconds(2f);
            StartCoroutine(EnemyTurn());
        }
        
    }

    IEnumerator EndBattle() 
    {
        // only updating dialogue text at the moment, so coroutine isn't necessary... yet
        if (state == BossBattleState.WON)
        {
            dialogueText.text = "yy0U W1ll r3GRET tthiiissss";
            yield return new WaitForSeconds(1f);
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
            } 
            // LOAD OUT OF BATTLE HERE

        } else if (state == BossBattleState.LOST)
        {
            dialogueText.text = "You lost.";
            // would probably load out of battle 

            // LOAD OUT OF BATTLE HERE
        }
    }

    IEnumerator PlayerHeal() 
    {
        // playerUnit.Heal(5); // arbitrary number
        playerUnit.Heal(enemyUnit.AttackDamage + 2); // recover the damage the enemy did plus some
        
        playerHUD.SetHp(playerUnit.Health);
        // dialogueText.text = "You feel renewed strength!";
        dialogueText.text = "You feel strength flowing back to you...";

        state = BossBattleState.ENEMYTURN; // should prevent player from infinite healing

        yield return new WaitForSeconds(2f);
        // player used their turn by healing
        // we could choose for them to be able to use items without turn ending
        //state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnRedButton() // player clicks red
    {
        if (state != BossBattleState.PLAYERTURN)
            return;
        actions.SetActive(false);
        playerColor = "RED";
        StartCoroutine(PlayerAttack());
    }

    public void OnGreenButton() // player clicks green
    {
        if (state != BossBattleState.PLAYERTURN)
            return;
        actions.SetActive(false);
        playerColor = "GREEN";
        StartCoroutine(PlayerAttack());
    }

    public void OnBlueButton() // player clicks blue
    {
        if (state != BossBattleState.PLAYERTURN)
            return;
        actions.SetActive(false);
        playerColor = "BLUE";
        StartCoroutine(PlayerAttack());
    }

    public void OnYellowButton() // player clicks yellow
    {
        if (state != BossBattleState.PLAYERTURN)
            return;
        actions.SetActive(false);
        playerColor = "YELLOW";
        StartCoroutine(PlayerAttack());
    }

/* *****************OLD BUTTON FUNCTIONS************************
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
*/

}
