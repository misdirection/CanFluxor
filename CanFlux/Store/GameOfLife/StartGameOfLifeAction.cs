namespace CanFlux.Store.GameOfLife
{
    public class StartGameOfLifeAction : IGameOfLifeAction
    {
        public bool GameStarted { get; } = true;
    }
}
