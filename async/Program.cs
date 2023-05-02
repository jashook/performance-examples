using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<AsyncBenchmark>();

[MemoryDiagnoser]
public class AsyncBenchmark
{
    [Benchmark]
    public async Task ImmediatelyCompleteConfigureAwaitNonAsync()
    {
        await AsyncBenchmarkHelper.ConfigureAwaitFalseNonAsync().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task ImmediatelyCompleteNonConfigureAwaitNonAsync()
    {
        await AsyncBenchmarkHelper.ConfigureAwaitTrueNonAsync();
    }

    [Benchmark]
    public async Task ImmediatelyCompleteValueTaskNonAsync()
    {
        await AsyncBenchmarkHelper.ValueTaskNonAwait();
    }

    [Benchmark]
    public async Task TaskAwaitConfigureAwaitAsync()
    {
        await AsyncBenchmarkHelper.ConfigureAwaitFalseAsync().ConfigureAwait(false);
    }

    [Benchmark]
    public async Task TaskAwaitNonConfigureAwaitAsync()
    {
        await AsyncBenchmarkHelper.ConfigureAwaitTrueAsync();
    }

    [Benchmark]
    public async Task ValueTaskAsync()
    {
        await AsyncBenchmarkHelper.ValueTaskAwait();
    }

    [Benchmark]
    public async Task TaskConditionalImmediateCompletion()
    {
        await AsyncBenchmarkHelper.TaskConditionalCompletion();
    }

    [Benchmark]
    public async Task ValueTaskConditionalImmediateCompletion()
    {
        await AsyncBenchmarkHelper.ValueTaskConditionalCompletion();
    }
}