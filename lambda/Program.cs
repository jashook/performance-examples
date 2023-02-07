using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<LambdaBenchmarking>();

public static class MemoryExtension
{
    public delegate bool Callback<in T>(T value);

    public static ref T Find<T> (this Memory<T> memory, Callback<T> callback, out bool found)
    {
        for (int index = 0; index < memory.Length; ++index)
        {
            if (callback(memory.Span[index]))
            {
                found = true;
                return ref memory.Span[index];
            }
        }
        
        found = false;
        return ref memory.Span[0];
    }
}

struct GuidHolder
{
    public GuidHolder() { }

    public Guid guid1 = Guid.NewGuid();
    public Guid guid2 = Guid.NewGuid();
    public Guid guid3 = Guid.NewGuid();
    public Guid guid4 = Guid.NewGuid();
    public Guid guid5 = Guid.NewGuid();
    public Guid guid6 = Guid.NewGuid();
    public Guid guid7 = Guid.NewGuid();
    public Guid guid8 = Guid.NewGuid();
    public Guid guid9 = Guid.NewGuid();
    public Guid guid10 = Guid.NewGuid();
    public Guid guid11 = Guid.NewGuid();
    public Guid guid12 = Guid.NewGuid();
    public Guid guid13 = Guid.NewGuid();
    public Guid guid14 = Guid.NewGuid();
    public Guid guid15 = Guid.NewGuid();
    public Guid guid16 = Guid.NewGuid();
    public Guid guid17 = Guid.NewGuid();
    public Guid guid18 = Guid.NewGuid();
    public Guid guid19 = Guid.NewGuid();
    public Guid guid20 = Guid.NewGuid();
    public Guid guid21 = Guid.NewGuid();
    public Guid guid22 = Guid.NewGuid();
    public Guid guid23 = Guid.NewGuid();
    public Guid guid24 = Guid.NewGuid();
    public Guid guid25 = Guid.NewGuid();
    public Guid guid26 = Guid.NewGuid();
    public Guid guid27 = Guid.NewGuid();
    public Guid guid28 = Guid.NewGuid();
    public Guid guid29 = Guid.NewGuid();
    public Guid guid30 = Guid.NewGuid();
    public Guid guid31 = Guid.NewGuid();
    public Guid guid32 = Guid.NewGuid();
    public Guid guid33 = Guid.NewGuid();
    public Guid guid34 = Guid.NewGuid();
    public Guid guid35 = Guid.NewGuid();
    public Guid guid36 = Guid.NewGuid();
    public Guid guid37 = Guid.NewGuid();
    public Guid guid38 = Guid.NewGuid();
    public Guid guid39 = Guid.NewGuid();
    public Guid guid40 = Guid.NewGuid();
    public Guid guid41 = Guid.NewGuid();
    public Guid guid42 = Guid.NewGuid();
    public Guid guid43 = Guid.NewGuid();
    public Guid guid44 = Guid.Empty;
}

[MemoryDiagnoser]
public class LambdaBenchmarking
{
    // Avoid allocations?
    Memory<Guid> guids = new Guid[1024];

    static readonly GuidHolder Holder = new();

    [Benchmark]
    /// <Summary>Simple example of how lambdas can accidentaly cause many allocations</Summary>
    public void PreCSharpNineLambda()
    {
        for (int index = 0; index < guids.Length; ++index)
        {
            guids.Span[index] = Guid.NewGuid();
        }

        guids.Span[guids.Length - 1] = Guid.Empty;
        GuidHolder holder = new();

        bool found;
        
        // Non static lambdas will capture the local variables and this
        Guid valueFound = MemoryExtension.Find(guids, item => item == holder.guid44, out found);
    }

    [Benchmark]
    /// <Summary>Simple example of how lambdas can accidentaly cause many allocations</Summary>
    public void StaticLambda()
    {
        for (int index = 0; index < guids.Length; ++index)
        {
            guids.Span[index] = Guid.NewGuid();
        }

        guids.Span[guids.Length - 1] = Guid.Empty;

        bool found;
        // This is not allowed.
        // Guid valueFound = MemoryExtension.Find(guids, item => item == holder.guid44, out found);

        Guid valueFound = MemoryExtension.Find(guids, static item => item == LambdaBenchmarking.Holder.guid44, out found);
    }
}