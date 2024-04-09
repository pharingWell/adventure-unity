namespace FSM
{
    public class TestState : IState
    {
        TestImplementation owner;
 
        public TestState(TestImplementation owner) { this.owner = owner; }
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