using System.Threading.Tasks;

public static class AsyncBenchmarkHelper
{
    private static async Task<double> ConfigureAwaitAsyncHelper(int timeout, bool configureAwait = false)
    {
        if (configureAwait)
        {
            await Task.Delay(timeout).ConfigureAwait(false);
        }
        else
        {
            await Task.Delay(timeout);
        }

        return 100.0;
    }

    private static ValueTask<double> AwaitHelper(int timeout, bool configureAwait = false)
    {
        if (timeout == 0)
        {
            return new ValueTask<double>(100);
        }
        else
        {
            return new ValueTask<double>(ConfigureAwaitAsyncHelper(timeout, configureAwait));
        }
    }

    public static async ValueTask<double> ValueTaskHelper(int timeout)
    {
        return await AwaitHelper(timeout);
    }

    public static async Task<double> TaskHelper(int timeout)
    {
        await Task.Delay(timeout);
        return 100.0;
    }

    public static async Task<double> ConfigureAwaitFalseNonAsync()
    {
        return await ConfigureAwaitAsyncHelper(0, true);
    }

    public static async Task<double> ConfigureAwaitTrueNonAsync()
    {
        return await ConfigureAwaitAsyncHelper(0);
    }

    public static async Task<double> ConfigureAwaitFalseAsync()
    {
        return await ConfigureAwaitAsyncHelper(1, true);
    }
    
    public static async Task<double> ConfigureAwaitTrueAsync()
    {
        return await ConfigureAwaitAsyncHelper(1);
    }

    public static async ValueTask<double> ValueTaskNonAwait()
    {
        return await ValueTaskHelper(0);
    }

    public static async ValueTask<double> ValueTaskAwait()
    {
        return await ValueTaskHelper(1);
    }

    public static async Task<double> ValueTaskConditionalCompletion()
    {
        for (int index = 0; index < 1024; ++index)
        {
            await ValueTaskHelper(index % 2);
        }

        return 100.0;
    }

    public static async Task<double> TaskConditionalCompletion()
    {
        for (int index = 0; index < 1024; ++index)
        {
            await TaskHelper(index % 2);
        }

        return 100.0;
    }
}