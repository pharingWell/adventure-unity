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
    // prefabs
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    // battle stations are the hexagons the enemy and player sit on 
    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    // two instances of Unit
    Unit playerUnit;
    Unit enemyUnit;

    public IPanel DialoguePanel;
    public TextMeshProUGUI dialogueText; // use TextMeshProUGUI for hud
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public GameObject actions;
    public Animator playerAnimator;
    public Animator enemyAnimator;
    
    public BattleState state;
    private static readonly int PlayerAttackHash = Animator.StringToHash("PlayerAttack");
    private static readonly int EnemyAttackHash = Animator.StringToHash("EnemyAttackHash");
    private static readonly int EnemyDead = Animator.StringToHash("enemyDead");
    private static readonly int PlayerDead = Animator.StringToHash("playerDead");

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        SaveSerializer.GameDataSaved += GameSaved;
        SaveSerializer.GameDataLoaded += GameLoaded;
    }
    
    void GameSaved()
    {
        playerUnit.Health = 1;
        Destroy(enemyGo);
        Destroy(enemyUnit);
        Destroy(playerGo);
        Destroy(playerUnit);
        SaveSystem.Load();
    }
    
    void GameLoaded()
    {
        Setup();
    }

    void Setup()
    {
        actions.SetActive(false);
        // instantiate a player game object using the player prefab
        // spawn it on top of the player battle station
        GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGo.GetComponent<Unit>();

        // instantiate an enemy game object using the enemy prefab
        // spawn it on top of the enemy battle station
        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();

        playerAnimator = playerGo.GetComponent<Animator>();
        enemyAnimator = enemyGo.GetComponent<Animator>();

        // changes the dialogue text to include the enemy's name
        dialogueText.text = "You encountered a " + enemyUnit.Name + "!";
        
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        
        yield return new WaitForSeconds(2f); // need coroutines for this line

        state = BattleState.PLAYERTURN; // now that the battle is set up, let the player have their turn
        // if we want the enemy to go first, the above line should be state = BattleState.PLAYERTURN;
        // and then below, EnemyTurn();
        PlayerTurn();
    }

    IEnumerator EnemyTurn() 
    {
        dialogueText.text = enemyUnit.Name + " attacks!";
        enemyAnimator.SetTrigger(EnemyAttackHash);
        playerAnimator.SetTrigger(EnemyAttackHash);

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.AttackDamage); // is player dead after taking damage?
        
        playerHUD.SetHp(playerUnit.Health); // player's hp bar reflects new hp; also updates hp text
        yield return new WaitForSeconds(1f);

        enemyAnimator.ResetTrigger(EnemyAttackHash);
        playerAnimator.ResetTrigger(EnemyAttackHash);
        
        if (isDead) // player is dead
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        } else // player survived
        {
            state = BattleState.PLAYERTURN;
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
        playerAnimator.SetTrigger(PlayerAttackHash);
        enemyAnimator.SetTrigger(PlayerAttackHash); 
        // damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.AttackDamage);

        enemyHUD.SetHp(enemyUnit.Health);
        dialogueText.text = "The attack is successful!";
        yield return new WaitForSeconds(1f); // could be more
        
        dialogueText.text = "You dealt " + playerUnit.AttackDamage + " damage!";

        playerAnimator.ResetTrigger(PlayerAttackHash);
        enemyAnimator.ResetTrigger(PlayerAttackHash);
        
        // check if enemy is dead
        if(isDead)
        {            
            // end the battle
            state = BattleState.WON;
            yield return new WaitForSeconds(2f);
            
            StartCoroutine(EndBattle());
        } else 
        {
            // enemy turn
            state = BattleState.ENEMYTURN; 
            yield return new WaitForSeconds(2f);

            StartCoroutine(EnemyTurn());
        }
        // change state based on what happened
    }

    IEnumerator EndBattle() // should probably turn this into a coroutine later
    {
        // only updating dialogue text at the moment, so coroutine isn't necessary... yet
        if (state == BattleState.WON)
        {
            //playerAnimator.SetTrigger(enemyDead);
            playerAnimator.SetTrigger(EnemyDead);
            enemyAnimator.SetTrigger(EnemyDead);

            dialogueText.text = "You won the battle!";
            yield return new WaitForSeconds(1f);

            

            // LOAD OUT OF BATTLE HERE

            /*
            // below was an xp system that would be implemented in a further stage of development
            
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
                                    + ", and your PlayerAttack is " + playerUnit.AttackDamage + ".";
            } else 
            {
                yield return new WaitForSeconds(1f);
                dialogueText.text = "[End Battle]";
            }
            */

        } else if (state == BattleState.LOST)
        {
            dialogueText.text = "You lost.";
            playerAnimator.SetTrigger(PlayerDead);
            enemyAnimator.SetTrigger(PlayerDead);

            // would probably load out of battle 

            // LOAD OUT OF BATTLE HERE
        }
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
        actions.SetActive(false);
        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton() // player clicks heal button
    {
        if (state != BattleState.PLAYERTURN) 
            return;
        actions.SetActive(false);
        StartCoroutine(PlayerHeal());
    }

}
