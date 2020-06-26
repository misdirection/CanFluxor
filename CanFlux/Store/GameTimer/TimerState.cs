using System.Timers;

namespace CanFlux.Store.GameTimer
{
    public class TimerState
    {
        public TimerState(double interval) => Tick = new Timer(interval);

        public Timer Tick { get;}
    }
}
