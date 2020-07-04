using System.Collections.Generic;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeHistoryState
    {
        public GameOfLifeHistoryState(GameOfLifeState present)
        {
            Past = new List<GameOfLifeState>();
            Present = present;
            Future = new List<GameOfLifeState>();
        }

        public GameOfLifeHistoryState(List<GameOfLifeState> past, GameOfLifeState present, List<GameOfLifeState> future)
        {
            Past = past;
            Present = present;
            Future = future;
        }

        public List<GameOfLifeState> Past { get; }
        public GameOfLifeState Present { get; }
        public List<GameOfLifeState> Future { get; }
    }
}
