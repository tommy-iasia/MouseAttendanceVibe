# Mouse Attendance Implementation

## Tech Stack
- .NET 8 Windows Forms application (`dotnet new winforms`)
- System.Windows.Forms.Cursor for capturing mouse position
- MSTest for unit testing (built-in Visual Studio tooling)
- `HttpClient` for HTTP GET reporting

## Project Structure

```
MouseAttendance/
├─ src/
│  └─ MouseAttendance/           # WinForms entry point project
│     ├─ Program.cs              # app bootstrap, scheduler instantiation
│     ├─ AttendanceDetector.cs   # state machine for Working/Left
│     ├─ AttendanceReporter.cs   # time-windowed report logic
│     └─ Scheduler.cs            # timer loop calling Tick + report
└─ tests/
   └─ MouseAttendance.Tests/     # MSTest test project
      ├─ AttendanceDetectorTests.cs
      └─ AttendanceReporterTests.cs
```

## Phase 1: Environment Setup
1. Install .NET 8 SDK
2. Create solution and projects:
   ```powershell
   dotnet new sln -n MouseAttendance
   dotnet new winforms -o src/MouseAttendance
   dotnet new mstest -o tests/MouseAttendance.Tests
   dotnet sln add src/MouseAttendance/MouseAttendance.csproj tests/MouseAttendance.Tests/MouseAttendance.Tests.csproj
   dotnet add tests/MouseAttendance.Tests/MouseAttendance.Tests.csproj reference src/MouseAttendance/MouseAttendance.csproj
   ```
3. Build solution: `dotnet build`

## Phase 2 & 3: Detect Logic + Tests
- **AttendanceDetector**
  - Method: `void Tick(DateTime time, Point mouse)`
  - Maintains:
    - `DateTime _lastMoveTime`
    - `State _currentState` (`Working` | `Left`)
  - On each Tick:
    1. If `mouse != _lastPos`, set `_currentState = Working`, update `_lastMoveTime = time`
    2. Else if `time - _lastMoveTime >= 10 min`, set `_currentState = Left` (timestamp when idle started)
  - Expose an event or property for state changes
- **Testing**
  - Simulate sequences of Tick calls with custom `DateTime` and `Point`
  - Assert correct state transitions and timestamps

## Phase 4 & 5: Reporting Logic + Tests
- **AttendanceReporter**
  - Input: state change events from detector
  - Only emit:
    - `Working` events if `08:00 ≤ localTime < 10:00`
    - `Left` events if `17:00 ≤ localTime < 19:00`
  - Suppress duplicate events until state flips
- **Testing**
  - Feed events at various times, assert only allowed ones pass through

## Phase 6: Scheduler
- **Scheduler**
  - Use `System.Threading.Timer` or `async Task` loop with `Task.Delay(TimeSpan.FromMinutes(1))`
  - On each interval:
    1. `now = DateTime.UtcNow`
    2. `pos = Cursor.Position`
    3. `detector.Tick(now, pos)`
    4. If a state change is published, forward to `AttendanceReporter`

## Phase 7: HTTP Reporting
- In `AttendanceReporter`, on accepted event:
  ```csharp
  using var client = new HttpClient();
  var uri = new Uri($"http://myservice.abc/attend?type={state.ToString().ToLowerInvariant()}&time={time:o}");
  await client.GetAsync(uri);
  ```
- Simple fire‐and‐forget pattern; log failures if needed

---
Short, modular design ensures testability, simple scheduling, and clear separation of concerns.
