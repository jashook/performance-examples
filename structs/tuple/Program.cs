using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ValueTupleBenchmark>();

[MemoryDiagnoser]
public class ValueTupleBenchmark
{
    [Benchmark]
    public void TupleExample()
    {
        Tuple<int, int> tup = new Tuple<int, int>(1, 2);
    }

    [Benchmark]
    public void ValueTupleExample()
    {
        ValueTuple<int, int> tup = (3, 4);
    }

    [Benchmark]
    public void TupleCreatedInLoop()
    {
        for (int index = 0; index < 1000000; ++index)
        {
            Tuple<int, int> tup = new Tuple<int, int>(1, 2);
        }
    }

    [Benchmark]
    public void ValueTupleCreatedInLoop()
    {
        for (int index = 0; index < 1000000; ++index)
        {
            ValueTuple<int, int> tup = (3, 4);
        }
    }

    [Benchmark]
    public void LargeTupleCreatedInLoop()
    {
        for (int index = 0; index < 1000000; ++index)
        {
            Tuple<long, long, long, long, long, long> tup = new Tuple<long, long, long, long, long, long>(1, 4, 5, 6, 7, 8);
        }
    }

    [Benchmark]
    public void LargeValueTupleCreatedInLoop()
    {
        for (int index = 0; index < 1000000; ++index)
        {
            ValueTuple<long, long, long, long, long, long> tup = (3, 4, 5, 6, 7, 8);
        }
    }
}