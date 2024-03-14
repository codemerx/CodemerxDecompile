using System;
using System.Threading;

namespace CodemerxDecompile;

public class Debouncer
{
    private readonly TimeSpan time;
    private PeriodicTimer? lastPeriodicTimer;

    public Debouncer(TimeSpan time)
    {
        this.time = time;
    }
    
    public async void Debounce(Action action)
    {
        lastPeriodicTimer?.Dispose();

        lastPeriodicTimer = new PeriodicTimer(time);
        if (await lastPeriodicTimer.WaitForNextTickAsync())
        {
            action();
        }
    }
}
