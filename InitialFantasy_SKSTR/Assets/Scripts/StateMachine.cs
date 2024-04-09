public interface IState
{
    public void Enter();
    public void Execute();
    public void Exit();
}
 
public class StateMachine
{
    
    private IState _currentState;
    private IState _previousState;
 
    public void ChangeState(IState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
            _previousState = _currentState;
        }

        _currentState = newState;
        _currentState.Enter();
    }
 
    public void Update()
    {
        if (_currentState != null) _currentState.Execute();
    }

    public void RestoreState()
    {
        if (_previousState != null)
        {
            ChangeState(_previousState);
        }
    }
}
 
