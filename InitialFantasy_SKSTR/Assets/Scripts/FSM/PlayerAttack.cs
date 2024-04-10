using UnityEngine;
using UnityEngine.UI;

namespace FSM
{
    public class PlayerAttack : IState
    {
        TestImplementation owner;
 
        public PlayerAttack(TestImplementation owner) { this.owner = owner; }

        GameObject player;
        GameObject enemy;
        
        public Text attackText;
        
        public int damage = 10; // damage dealt by player. should be taken from a player object, not hardcoded
        public int enemyHP;

        public void Enter()
        {
            Debug.Log("entering PlayerAttack state");
            // get enemy information and player information
            // assuming that we have one enemy tagged as "Enemy"
            // assuming that we have one player tagged as "Player"
            enemy = GameObject.FindWithTag("Enemy");
            player = GameObject.FindWithTag("Player");
        }

        public void Execute()
        {
            Debug.Log("updating PlayerAttack state");
            // calculate damage dealt to monster
            // get enemy hp
            enemyHP = enemy.GetComponent<EnemyBehavior>().currentHP; // only has the hp for now
            enemyHP -= damage;
            // let the player know what happened
            attackText.text = "You attacked the monster and dealt " + damage + " damage.";
            // debugging statements
            Debug.Log("enemyHP value: " + enemyHP);
            Debug.Log("currentHP component value: " + enemy.GetComponent<EnemyBehavior>().currentHP);
        }

        public void Exit()
        {
            Debug.Log("exiting PlayerAttack state");
        }
    }
}