namespace FSM
{
    public class ExampleState : IState
    {
        TestImplementation owner;
 
        public ExampleState(TestImplementation owner) { this.owner = owner; }
        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}