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
        private Random Random { get; set; } = new Random();
        private List<RGB> ColorList { get; set; } = new List<RGB> {
        new RGB{R = 0xFF, G=0xEB, B=0xED },
        new RGB{R = 0xFF, G=0xCD, B=0xD2 },
        new RGB{R = 0xEE, G=0x9A, B=0x9A },
        new RGB{R = 0xE5, G=0x73, B=0x73 },
        new RGB{R = 0xEE, G=0x53, B=0x4F },
        new RGB{R = 0xF4, G=0x42, B=0x36 },
        new RGB{R = 0xE5, G=0x39, B=0x35 },
        new RGB{R = 0xC9, G=0x34, B=0x2D },
        new RGB{R = 0xC6, G=0x28, B=0x27 },
        new RGB{R = 0xB6, G=0x1C, B=0x1C },

        new RGB{R = 0xFB, G=0xE4, B=0xEC },
        new RGB{R = 0xF9, G=0xBB, B=0xD0 },
        new RGB{R = 0xF4, G=0x8F, B=0xB1 },
        new RGB{R = 0xF0, G=0x62, B=0x92 },
        new RGB{R = 0xEC, G=0x40, B=0x7A },
        new RGB{R = 0xEA, G=0x1E, B=0x63 },
        new RGB{R = 0xD8, G=0x1A, B=0x60 },
        new RGB{R = 0xC2, G=0x17, B=0x5B },
        new RGB{R = 0xAD, G=0x14, B=0x57 },
        new RGB{R = 0x89, G=0x0E, B=0x4F },

        new RGB{R = 0xF3, G=0xE5, B=0xF6 },
        new RGB{R = 0xE1, G=0xBE, B=0xE8 },
        new RGB{R = 0xCF, G=0x93, B=0xD9 },
        new RGB{R = 0xB9, G=0x68, B=0xC7 },
        new RGB{R = 0xAA, G=0x47, B=0xBC },
        new RGB{R = 0x9C, G=0x28, B=0xB1 },
        new RGB{R = 0x8E, G=0x24, B=0xAA },
        new RGB{R = 0x7A, G=0x1F, B=0xA2 },
        new RGB{R = 0x6A, G=0x1B, B=0x9A },
        new RGB{R = 0x4A, G=0x14, B=0x8C },

        new RGB{R = 0xEE, G=0xE8, B=0xF6 },
        new RGB{R = 0xD0, G=0xC4, B=0xE8 },
        new RGB{R = 0xB3, G=0x9D, B=0xDB },
        new RGB{R = 0x96, G=0x75, B=0xCE },
        new RGB{R = 0x7E, G=0x57, B=0xC2 },
        new RGB{R = 0x67, G=0x3B, B=0xB7 },
        new RGB{R = 0x5D, G=0x35, B=0xB0 },
        new RGB{R = 0x51, G=0x2D, B=0xA7 },
        new RGB{R = 0x45, G=0x28, B=0x9F },
        new RGB{R = 0x30, G=0x1B, B=0x92 },

        new RGB{R = 0xE8, G=0xEA, B=0xF6 },
        new RGB{R = 0xC5, G=0xCA, B=0xE8 },
        new RGB{R = 0x9E, G=0xA8, B=0xDB },
        new RGB{R = 0x79, G=0x86, B=0xCC },
        new RGB{R = 0x5C, G=0x6B, B=0xC0 },
        new RGB{R = 0x3F, G=0x51, B=0xB5 },
        new RGB{R = 0x39, G=0x49, B=0xAB },
        new RGB{R = 0x30, G=0x3E, B=0x9F },
        new RGB{R = 0x28, G=0x35, B=0x93 },
        new RGB{R = 0x1A, G=0x23, B=0x7E },

        new RGB{R = 0xE4, G=0xF2, B=0xFD },
        new RGB{R = 0xBB, G=0xDE, B=0xFA },
        new RGB{R = 0x90, G=0xCA, B=0xF8 },
        new RGB{R = 0x64, G=0xB5, B=0xF6 },
        new RGB{R = 0x42, G=0xA5, B=0xF6 },
        new RGB{R = 0x21, G=0x96, B=0xF3 },
        new RGB{R = 0x1D, G=0x89, B=0xE4 },
        new RGB{R = 0x19, G=0x76, B=0xD3 },
        new RGB{R = 0x15, G=0x64, B=0xC0 },
        new RGB{R = 0x0E, G=0x47, B=0xA1 },

        new RGB{R = 0xE1, G=0xF5, B=0xFE },
        new RGB{R = 0xB3, G=0xE5, B=0xFC },
        new RGB{R = 0x81, G=0xD5, B=0xFA },
        new RGB{R = 0x4F, G=0xC2, B=0xF8 },
        new RGB{R = 0x28, G=0xB6, B=0xF6 },
        new RGB{R = 0x03, G=0xA9, B=0xF5 },
        new RGB{R = 0x03, G=0x9B, B=0xE6 },
        new RGB{R = 0x02, G=0x88, B=0xD1 },
        new RGB{R = 0x02, G=0x77, B=0xBD },
        new RGB{R = 0x00, G=0x57, B=0x9C },

        new RGB{R = 0xDF, G=0xF7, B=0xF9 },
        new RGB{R = 0xB2, G=0xEB, B=0xF2 },
        new RGB{R = 0x80, G=0xDE, B=0xEA },
        new RGB{R = 0x4D, G=0xD0, B=0xE2 },
        new RGB{R = 0x25, G=0xC6, B=0xDA },
        new RGB{R = 0x00, G=0xBC, B=0xD5 },
        new RGB{R = 0x00, G=0xAC, B=0xC2 },
        new RGB{R = 0x00, G=0x98, B=0xA6 },
        new RGB{R = 0x00, G=0x82, B=0x8F },
        new RGB{R = 0x01, G=0x60, B=0x64 },
        
        new RGB{R = 0xE0, G=0xF2, B=0xF2 },
        new RGB{R = 0xB2, G=0xDF, B=0xDC },
        new RGB{R = 0x80, G=0xCB, B=0xC4 },
        new RGB{R = 0x4C, G=0xB6, B=0xAC },
        new RGB{R = 0x26, G=0xA5, B=0x9A },
        new RGB{R = 0x00, G=0x97, B=0x88 },
        new RGB{R = 0x00, G=0x88, B=0x7A },
        new RGB{R = 0x00, G=0x79, B=0x6A },
        new RGB{R = 0x00, G=0x69, B=0x5B },
        new RGB{R = 0x00, G=0x4C, B=0x3F },
        
        new RGB{R = 0xE8, G=0xF6, B=0xE9 },
        new RGB{R = 0xC8, G=0xE6, B=0xCA },
        new RGB{R = 0xA5, G=0xD6, B=0xA7 },
        new RGB{R = 0x80, G=0xC7, B=0x83 },
        new RGB{R = 0x66, G=0xBB, B=0x6A },
        new RGB{R = 0x4C, G=0xB0, B=0x50 },
        new RGB{R = 0x43, G=0xA0, B=0x47 },
        new RGB{R = 0x39, G=0x8E, B=0x3D },
        new RGB{R = 0x2F, G=0x7D, B=0x32 },
        new RGB{R = 0x1C, G=0x5E, B=0x20 },
        
        new RGB{R = 0xF1, G=0xF7, B=0xE9 },
        new RGB{R = 0xDD, G=0xED, B=0xC8 },
        new RGB{R = 0xC5, G=0xE1, B=0xA6 },
        new RGB{R = 0xAE, G=0xD5, B=0x82 },
        new RGB{R = 0x9C, G=0xCC, B=0x66 },
        new RGB{R = 0x8B, G=0xC2, B=0x4A },
        new RGB{R = 0x7D, G=0xB3, B=0x43 },
        new RGB{R = 0x68, G=0x9F, B=0x39 },
        new RGB{R = 0x54, G=0x8B, B=0x2E },
        new RGB{R = 0x33, G=0x69, B=0x1E },
        
        new RGB{R = 0xF9, G=0xFB, B=0xE6 },
        new RGB{R = 0xF0, G=0xF4, B=0xC2 },
        new RGB{R = 0xE6, G=0xEE, B=0x9B },
        new RGB{R = 0xDD, G=0xE7, B=0x76 },
        new RGB{R = 0xD4, G=0xE0, B=0x56 },
        new RGB{R = 0xCD, G=0xDC, B=0x39 },
        new RGB{R = 0xC0, G=0xCA, B=0x33 },
        new RGB{R = 0xB0, G=0xB4, B=0x2B },
        new RGB{R = 0x9E, G=0x9E, B=0x24 },
        new RGB{R = 0x81, G=0x77, B=0x16 },
        
        new RGB{R = 0xFF, G=0xFD, B=0xE8 },
        new RGB{R = 0xFF, G=0xFA, B=0xC3 },
        new RGB{R = 0xFF, G=0xF5, B=0x9C },
        new RGB{R = 0xFF, G=0xF1, B=0x76 },
        new RGB{R = 0xFF, G=0xEE, B=0x58 },
        new RGB{R = 0xFF, G=0xEB, B=0x3C },
        new RGB{R = 0xFD, G=0xD7, B=0x34 },
        new RGB{R = 0xFA, G=0xC0, B=0x2E },
        new RGB{R = 0xF9, G=0xA8, B=0x25 },
        new RGB{R = 0xF4, G=0x7F, B=0x16 },
        
        new RGB{R = 0xFE, G=0xF8, B=0xE0 },
        new RGB{R = 0xFF, G=0xEC, B=0xB2 },
        new RGB{R = 0xFF, G=0xE0, B=0x83 },
        new RGB{R = 0xFF, G=0xD5, B=0x4F },
        new RGB{R = 0xFF, G=0xC9, B=0x28 },
        new RGB{R = 0xFE, G=0xC1, B=0x07 },
        new RGB{R = 0xFF, G=0xB2, B=0x00 },
        new RGB{R = 0xFF, G=0x9F, B=0x00 },
        new RGB{R = 0xFF, G=0x8E, B=0x01 },
        new RGB{R = 0xFF, G=0x6F, B=0x00 },
        
        new RGB{R = 0xFF, G=0xF2, B=0xDF },
        new RGB{R = 0xFF, G=0xE0, B=0xB2 },
        new RGB{R = 0xFF, G=0xCC, B=0x80 },
        new RGB{R = 0xFF, G=0xB6, B=0x4D },
        new RGB{R = 0xFF, G=0xA8, B=0x27 },
        new RGB{R = 0xFF, G=0x97, B=0x00 },
        new RGB{R = 0xFB, G=0x8C, B=0x00 },
        new RGB{R = 0xF6, G=0x7C, B=0x01 },
        new RGB{R = 0xEF, G=0x6C, B=0x00 },
        new RGB{R = 0xE6, G=0x51, B=0x00 },
        
        new RGB{R = 0xFB, G=0xE9, B=0xE7 },
        new RGB{R = 0xFF, G=0xCC, B=0xBB },
        new RGB{R = 0xFF, G=0xAB, B=0x91 },
        new RGB{R = 0xFF, G=0x8A, B=0x66 },
        new RGB{R = 0xFF, G=0x71, B=0x43 },
        new RGB{R = 0xFE, G=0x57, B=0x22 },
        new RGB{R = 0xF5, G=0x51, B=0x1E },
        new RGB{R = 0xE6, G=0x4A, B=0x19 },
        new RGB{R = 0xD7, G=0x43, B=0x15 },
        new RGB{R = 0xBF, G=0x36, B=0x0C },
        
        new RGB{R = 0xEF, G=0xEB, B=0xEA },
        new RGB{R = 0xD7, G=0xCC, B=0xC8 },
        new RGB{R = 0xBC, G=0xAB, B=0xA4 },
        new RGB{R = 0xA0, G=0x88, B=0x7E },
        new RGB{R = 0x8C, G=0x6E, B=0x63 },
        new RGB{R = 0x7B, G=0x53, B=0x47 },
        new RGB{R = 0x6D, G=0x4D, B=0x42 },
        new RGB{R = 0x5D, G=0x40, B=0x38 },
        new RGB{R = 0x4D, G=0x34, B=0x2F },
        new RGB{R = 0x3E, G=0x26, B=0x22 },
        
        new RGB{R = 0xFA, G=0xFA, B=0xFA },
        new RGB{R = 0xF5, G=0xF5, B=0xF5 },
        new RGB{R = 0xEE, G=0xEE, B=0xEE },
        new RGB{R = 0xE0, G=0xE0, B=0xE0 },
        new RGB{R = 0xBD, G=0xBD, B=0xBD },
        new RGB{R = 0x9E, G=0x9E, B=0x9E },
        new RGB{R = 0x75, G=0x75, B=0x75 },
        new RGB{R = 0x61, G=0x61, B=0x61 },
        new RGB{R = 0x42, G=0x42, B=0x42 },
        new RGB{R = 0x21, G=0x21, B=0x21 },
        
        new RGB{R = 0xEB, G=0xEF, B=0xF2 },
        new RGB{R = 0xCE, G=0xD9, B=0xDD },
        new RGB{R = 0xB0, G=0xBF, B=0xC6 },
        new RGB{R = 0x90, G=0xA4, B=0xAD },
        new RGB{R = 0x79, G=0x8F, B=0x9A },
        new RGB{R = 0x60, G=0x7D, B=0x8B },
        new RGB{R = 0x54, G=0x6F, B=0x7A },
        new RGB{R = 0x46, G=0x5A, B=0x65 },
        new RGB{R = 0x36, G=0x47, B=0x4F },
        new RGB{R = 0x27, G=0x32, B=0x38 }
        
        };

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
                    for (var i = Math.Max(0, x - 1); i <= Math.Min(state.ArraySize - 1, x + 1); i++)
                    {
                        for (var j = Math.Max(0, y - 1); j <= Math.Min(state.ArraySize - 1, y + 1); j++)
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
                        if (false && Random.Next(100) == 0)
                        {
                            var randomColor = ColorList[Random.Next(ColorList.Count)];
                            newGeneration[x, y] = Color.FromArgb(255, randomColor.R, randomColor.G, randomColor.B);
                        }
                        else
                        {
                            var c = Color.FromArgb(255, rgb.R / count, rgb.G / count, rgb.B / count);
                            var nearestColor = ColorList.OrderBy(x => Math.Abs(x.R - c.R) + Math.Abs(x.G - c.G) + Math.Abs(x.B - c.B)).First();
                            newGeneration[x, y] = Color.FromArgb(255, nearestColor.R, nearestColor.G, nearestColor.B);
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
