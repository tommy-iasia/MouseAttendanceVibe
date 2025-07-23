using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MouseAttendance;
    
public class AttendanceReporter
{
    private State? _lastStateReported = null;
    private static readonly HttpClient _httpClient = new HttpClient();
        public event EventHandler<AttendanceEventArgs>? StateReported;

        public void Subscribe(AttendanceDetector detector)
        {
            detector.StateChanged += OnDetectorStateChanged;
        }

    public void OnDetectorStateChanged(object? sender, AttendanceEventArgs e)
    {
            // Assume e.Timestamp is in local time
            var timeOfDay = e.Timestamp.TimeOfDay;
            bool allowed = (e.State == State.Working && timeOfDay >= TimeSpan.FromHours(8) && timeOfDay < TimeSpan.FromHours(10))
                        || (e.State == State.Left   && timeOfDay >= TimeSpan.FromHours(17) && timeOfDay < TimeSpan.FromHours(19));
            if (!allowed)
                return;
            if (_lastStateReported == e.State)
                return;
            _lastStateReported = e.State;
            StateReported?.Invoke(this, new AttendanceEventArgs(e.State, e.Timestamp));
            Logger.Log($"Reporter: State reported {e.State} at {e.Timestamp:yyyy-MM-dd HH:mm:ss}");
            // Fire-and-forget HTTP reporting
            _ = SendReportAsync(e.State, e.Timestamp);
    }
        private async Task SendReportAsync(State state, DateTime timestamp)
        {
            try
            {
                // Map internal state to HTTP API types
                var type = state switch
                {
                    State.Working => "arrive",
                    State.Left => "leave",
                    _ => state.ToString().ToLowerInvariant()
                };
                // send in UTC
                var time = timestamp.ToUniversalTime().ToString("o");
                var uri = new Uri($"https://us-central1-home-dashboard-7b463.cloudfunctions.net/attendanceAdd?type={type}&time={time}");
                await _httpClient.GetAsync(uri).ConfigureAwait(false);
                Logger.Log($"Reporter: Sent HTTP report for {state} at {timestamp:yyyy-MM-dd HH:mm:ss}");
            }
            catch
            {
                // ignore failures
            }
        }
}
