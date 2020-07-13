using CanFlux.Pages;
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
                case RestartAction a:
                    state.Past.Add(state.Present);
                    return new GameOfLifeHistoryState(new List<GameOfLifeState>(), new GameOfLifeState(1000, 10) , new List<GameOfLifeState>());
                default:
                    return new GameOfLifeHistoryState(state.Past, state.Present, state.Future);
            }
        }

        public GameOfLifeState ReducePopulateAction(GameOfLifeState state, PopulateAction action)
        {
            var newGeneration = new Color[state.ArraySize, state.ArraySize];
            for (var x = 0; x < state.ArraySize; x++)
            {
                for (var y = 0; y < state.ArraySize; y++)
                {
                    var red = 0;
                    var green = 0;
                    var blue = 0;
                    var count = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            bool outOfBounds = x + i < 0 || x + i > state.ArraySize - 1 || y + j < 0 || y + j > state.ArraySize - 1 || (i == 0 && j == 0);
                            if (!outOfBounds && state.Cells[x + i, y + j] != Color.White)
                            {
                                count++;
                                red += state.Cells[x + i, y + j].R;
                                green += state.Cells[x + i, y + j].G;
                                blue += state.Cells[x + i, y + j].B;
                            }
                        }
                    }

                    var isAlive = state.Cells[x, y] != Color.White;
                    if (isAlive && (count == 3 || count == 2))
                    {
                        newGeneration[x, y] = state.Cells[x, y];
                    }
                    else if (!isAlive && count == 3)
                    {
                        newGeneration[x, y] = Color.FromArgb(255, red / count, green / count, blue / count);
                    }
                    else
                    {
                        newGeneration[x, y] = Color.White;
                    }
                }
            }

            return new GameOfLifeState(newGeneration, state.BoardSize, state.SquareSize, state.GenerationCount + 1);
        }
    }
}
