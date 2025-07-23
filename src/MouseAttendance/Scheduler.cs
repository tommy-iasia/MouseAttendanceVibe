using System;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MouseAttendance
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT { public int X; public int Y; }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINT lpPoint);
    }

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
            Logger.Log($"Scheduler: Tick at {now:yyyy-MM-dd HH:mm:ss}");
            // Get current cursor position via PInvoke
            NativeMethods.GetCursorPos(out POINT pt);
            var pos = new Point(pt.X, pt.Y);
            Logger.Log($"Scheduler: Mouse position {pos.X},{pos.Y}");
            _detector.Tick(now, pos);
        }

        public void Dispose() => _timer.Dispose();
    }
}
