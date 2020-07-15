using CanFlux.Pages;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CanFlux.Store.GameOfLife
{
    public class GameOfLifeReducers
    {
        private Random random = new Random();

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
                    return new GameOfLifeHistoryState(new List<GameOfLifeState>(), new GameOfLifeState(600, 10), new List<GameOfLifeState>());
                default:
                    return new GameOfLifeHistoryState(state.Past, state.Present, state.Future);
            }
        }

        private struct RGB
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
        }

        public GameOfLifeState ReducePopulateAction(GameOfLifeState state, PopulateAction action)
        {
            var newGeneration = new Color[state.ArraySize, state.ArraySize];
            for (var x = 0; x < state.ArraySize; x++)
            {
                for (var y = 0; y < state.ArraySize; y++)
                {
                    var rgb = new RGB();
                    var count = 0;
                    for (int i = Math.Max(0, x - 1); i <= Math.Min(state.ArraySize - 1, x + 1); i++)
                    {
                        for (int j = Math.Max(0, y - 1); j <= Math.Min(state.ArraySize - 1, y + 1); j++)
                        {
                            if (!(i == x && j == y) && state.Cells[i, j] != Color.White)
                            {
                                count++;
                                rgb.R += state.Cells[i, j].R;
                                rgb.G += state.Cells[i, j].G;
                                rgb.B += state.Cells[i, j].B;
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
                        if (random.Next(100) == 0)
                        {
                            newGeneration[x, y] = Color.FromArgb(255, random.Next(256), random.Next(256), random.Next(256));
                        }
                        else
                        {
                            newGeneration[x, y] = Color.FromArgb(255, rgb.R / 3, rgb.G / 3, rgb.B / 3);
                        }
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
