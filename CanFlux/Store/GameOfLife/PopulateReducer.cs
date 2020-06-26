using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanFlux.Store.GameOfLife
{
    public class PopulateReducer : Reducer<GameOfLifeState, PopulateAction>
    {
        private GameOfLifeState _state;
        public override GameOfLifeState Reduce(GameOfLifeState state, PopulateAction action)
        {
            _state = state;
            var newGeneration = new bool[state.ArraySize, state.ArraySize];
            for (var x = 0; x < state.ArraySize; x++)
            {
                for (var y = 0; y < state.ArraySize; y++)
                {
                    int count = IsNeighbourAlive(x - 1, y - 1)
                        + IsNeighbourAlive(x - 1, y)
                        + IsNeighbourAlive(x - 1, y + 1)
                        + IsNeighbourAlive(x, y - 1)
                        + IsNeighbourAlive(x, y + 1)
                        + IsNeighbourAlive(x + 1, y - 1)
                        + IsNeighbourAlive(x + 1, y)
                        + IsNeighbourAlive(x + 1, y + 1);

                    var shouldLive = false;
                    var isAlive = state.OldGeneration[x, y];
                    if (isAlive && (count == 3 || count == 2))
                    {
                        shouldLive = true;
                    }
                    else if (!isAlive && count == 3)
                    {
                        shouldLive = true;
                    }
                    newGeneration[x, y] = shouldLive;
                }
            }
            return new GameOfLifeState(newGeneration, state.BoardSize, state.SquareSize, state.GenerationCount+1); ;
        }
        private int IsNeighbourAlive(int x, int y)
        {
            var result = 0;

            bool outOfBounds = x < 0 || x > _state.ArraySize - 1 || y < 0 || y > _state.ArraySize - 1;
            if (!outOfBounds)
            {
                result = _state.OldGeneration[x, y] ? 1 : 0;
            }
            return result;
        }
    }
}
