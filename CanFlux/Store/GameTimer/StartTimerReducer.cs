using Fluxor;

namespace CanFlux.Store.GameTimer
{
    public class StartTimerReducer : Reducer<TimerState, StartTimerAction>
    {
        public override TimerState Reduce(TimerState state, StartTimerAction action)
        {
            state.Tick.Start();
            return state;
        }
    }
}
