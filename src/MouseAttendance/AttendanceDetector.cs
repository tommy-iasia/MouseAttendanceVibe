using System;
using System.Drawing;

namespace MouseAttendance
{
    public enum State { Working, Left }

    public class AttendanceEventArgs : EventArgs
    {
        public State State { get; }
        public DateTime Timestamp { get; }

        public AttendanceEventArgs(State state, DateTime timestamp)
        {
            State = state;
            Timestamp = timestamp;
        }
    }

    public class AttendanceDetector
    {
        private State _currentState;
        private Point _lastPos;
        private DateTime _lastMoveTime;

        public event EventHandler<AttendanceEventArgs>? StateChanged;

        public AttendanceDetector()
        {
            _currentState = State.Left;
            _lastPos = new Point(int.MinValue, int.MinValue);
            _lastMoveTime = DateTime.MinValue;
        }

        public void Tick(DateTime time, Point mouse)
        {
            if (mouse != _lastPos)
            {
                _lastPos = mouse;
                _lastMoveTime = time;
                if (_currentState != State.Working)
                {
                    _currentState = State.Working;
                    Logger.Log($"Detector: State changed to {State.Working} at {time:yyyy-MM-dd HH:mm:ss}");
                    StateChanged?.Invoke(this, new AttendanceEventArgs(State.Working, time));
                }
            }
            else if (time - _lastMoveTime >= TimeSpan.FromMinutes(10))
            {
                if (_currentState != State.Left)
                {
                    var idleTimestamp = _lastMoveTime.AddMinutes(10);
                    _currentState = State.Left;
                    Logger.Log($"Detector: State changed to {State.Left} at {idleTimestamp:yyyy-MM-dd HH:mm:ss}");
                    StateChanged?.Invoke(this, new AttendanceEventArgs(State.Left, idleTimestamp));
                }
            }
        }
    }
}
