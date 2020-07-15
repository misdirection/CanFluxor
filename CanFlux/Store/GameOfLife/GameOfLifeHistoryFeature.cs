using Fluxor;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeHistoryFeature : Feature<GameOfLifeHistoryState>
    {
        public override string GetName() => "Game of Life History";

        protected override GameOfLifeHistoryState GetInitialState() => new GameOfLifeHistoryState(new GameOfLifeState(600, 10));
    }
}
