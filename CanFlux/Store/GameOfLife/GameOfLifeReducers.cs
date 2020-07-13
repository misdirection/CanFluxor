using Fluxor;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeReducers
    {

        [ReducerMethod]
        public GameOfLifeHistoryState GameOfLifeReducer(GameOfLifeHistoryState state, IGameOfLifeAction action)
        {
            switch (action)
            {
                case UndoAction a:
                    var previous = state.Past.LastOrDefault();
                    state.Past.RemoveAt(state.Past.Count - 1);
                    state.Future.Insert(0, state.Present);
                    return new GameOfLifeHistoryState(state.Past, previous, state.Future);
                case RedoAction a:
                    var next = state.Future.FirstOrDefault();
                    state.Future.RemoveAt(0);
                    state.Past.Add(state.Present);
                    return new GameOfLifeHistoryState(state.Past, next, state.Future);
                case PopulateAction a:
                    state.Past.Add(state.Present);
                    var present = ReducePopulateAction(state.Present, a);
                    return new GameOfLifeHistoryState(state.Past, present, new List<GameOfLifeState>());
                default:
                    return new GameOfLifeHistoryState(state.Past, state.Present, state.Future);
            }
        }

        public GameOfLifeState ReducePopulateAction(GameOfLifeState state, PopulateAction action)
        {
            var newGeneration = new string[state.ArraySize, state.ArraySize];
            for (var x = 0; x < state.ArraySize; x++)
            {
                for (var y = 0; y < state.ArraySize; y++)
                {
                    int red = IsNeighbourAlive(x - 1, y - 1, state, "Red")
                        + IsNeighbourAlive(x - 1, y, state, "Red")
                        + IsNeighbourAlive(x - 1, y + 1, state, "Red")
                        + IsNeighbourAlive(x, y - 1, state, "Red")
                        + IsNeighbourAlive(x, y + 1, state, "Red")
                        + IsNeighbourAlive(x + 1, y - 1, state, "Red")
                        + IsNeighbourAlive(x + 1, y, state, "Red")
                        + IsNeighbourAlive(x + 1, y + 1, state, "Red");

                    int green = IsNeighbourAlive(x - 1, y - 1, state, "Green")
                        + IsNeighbourAlive(x - 1, y, state, "Green")
                        + IsNeighbourAlive(x - 1, y + 1, state, "Green")
                        + IsNeighbourAlive(x, y - 1, state, "Green")
                        + IsNeighbourAlive(x, y + 1, state, "Green")
                        + IsNeighbourAlive(x + 1, y - 1, state, "Green")
                        + IsNeighbourAlive(x + 1, y, state, "Green")
                        + IsNeighbourAlive(x + 1, y + 1, state, "Green");

                    var shouldLive = false;
                    var isAlive = state.Cells[x, y] != null;
                    if (isAlive && (red + green == 3 || red + green == 2))
                    {
                        shouldLive = true;
                    }
                    else if (!isAlive && red + green == 3)
                    {
                        shouldLive = true;
                    }
                    if (shouldLive)
                    {
                        newGeneration[x, y] = red > green ? "Red" : "Green";
                    }
                    else
                    {
                        newGeneration[x, y] = null;
                    }
                }
            }

            return new GameOfLifeState(newGeneration, state.BoardSize, state.SquareSize, state.GenerationCount + 1);
        }
        private int IsNeighbourAlive(int x, int y, GameOfLifeState state, string color)
        {
            var result = 0;

            bool outOfBounds = x < 0 || x > state.ArraySize - 1 || y < 0 || y > state.ArraySize - 1;
            if (!outOfBounds)
            {
                result = state.Cells[x, y] == color ? 1 : 0;
            }
            return result;
        }

    }
}
