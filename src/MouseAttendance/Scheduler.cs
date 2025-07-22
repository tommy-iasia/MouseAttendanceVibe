using System;
using System.Threading;
using System.Windows.Forms;

namespace MouseAttendance
{
    public class Scheduler : IDisposable
    {
        private readonly AttendanceDetector _detector;
    private readonly System.Threading.Timer _timer;

        public Scheduler(AttendanceDetector detector, AttendanceReporter reporter)
        {
            _detector = detector;
            reporter.Subscribe(detector);
            // Trigger immediately, then every minute
            _timer = new System.Threading.Timer(TickCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private void TickCallback(object? state)
        {
            var now = DateTime.Now;
            var pos = Cursor.Position;
            _detector.Tick(now, pos);
        }

        public void Dispose() => _timer.Dispose();
    }
}
