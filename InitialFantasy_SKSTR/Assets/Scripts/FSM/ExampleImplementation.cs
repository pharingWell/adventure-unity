using UnityEngine;

namespace FSM
{
    public class TestImplementation : MonoBehaviour
    {
        StateMachine _stateMachine = new StateMachine();
   
        void Start()
        {
           _stateMachine.ChangeState(new ExampleState(this));
        }
 
        void Update()
        {
            _stateMachine.Update();
        }
    }

}