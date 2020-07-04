namespace CanFlux.Store.GameOfLife
{
    public class StopGameOfLifeAction : IGameOfLifeAction
    {
        public bool GameStarted { get; } = false;
    }
}
