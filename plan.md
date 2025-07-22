# Mouse Attendance

I would like to create an auto attendance recorder.

It takes attendance using mouse position.

And the implementation are divided into phases.

## Phase 1

In this phase, we setup the development environment.

This project should be a light .NET program for Windows.

Install dotnet and create an empty project.

Build the empty project.

## Phase 2

In order to facilitate development, we first do not use mouse position.

Also, in order to make it more testable. We would develop a testable class first.

### Logic

Create a class which has public functions which will, in future, be called by outer class.

```cs
public void Tick(DateTime time, Point mouse)
```

`time` provides the current time. `mouse` provides the current mouse position.

Therefore, the class will response the `Mouse` and see if it is in `Working` or `Left` state. A `Working` state means mouse moved. `Left` state means mouse idle for 10 minutes.

Remember that the timestamp for `Working` is when mouse moved. But the timestamp for `Left` is when mouse starts idle.

## Phase 3

Write test cases for Phase 2. Verify that the class works well.

## Phase 4

In phase 2, the detect logic is implemented. Now, implement the report logic.

### Logic

We should only report on a certain time range every day. 

We only report `Working` when it is 08:00 - 10:00. On the other hand, we only report `Left` when it is 17:00 - 19:00.

And repeated states should not be fired twice, unless the state change.

## Phase 5

Write test cases for Phase 4.

## Phase 6

Now, we implement the outer class. The outer class should call inner class's `Time` and `Mouse` every minutes.

If your class is simple enough, no need to write test.

## Phase 7

Finally, we report by CURLing http://myservice.abc/attend?type=working&time=2025-07-22T09:00:00Z.

Implement the final part.

If your final piece of code is simple enough, no need to write test.