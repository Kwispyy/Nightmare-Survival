public class Killer_FSM
{
    private Action<Vector2, Vector2, GameTime> activeState; // Delegate on active state machine

    public Killer_FSM()
    {
    }

    public void SetState(Action<Vector2, Vector2, GameTime> state)
    {
        activeState = state;
    }

    public void Update(Vector2 playerPos, Vector2 killerPos, GameTime gameTime)
    {
        if (activeState != null)
        {
            activeState.Invoke(playerPos, killerPos, gameTime);
        }
    }
}