using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ExceptionComparison>();

public class ExceptionComparison
{
    private readonly int reps = 100; 
    private readonly int avoidCompilerOptimizations = 2;

    public void RecurseUntilException(int breakDepth = 1, bool throwException = true, int depth = 0)
    {
        if (depth >= breakDepth)
        {
            if (!throwException) return;

            throw new Exception("Catch me!");
        }

        RecurseUntilException(++depth, throwException, breakDepth);
    }

    [Benchmark]
    /// <Summary> Baseline method. int is returned to avoid it being optimized away.</Summary>
    public int BaselineNoException()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            if (index * avoidCompilerOptimizations % 2 == 0)
            {
                RecurseUntilException(1, false);
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    /// <Summary> Baseline method. int is returned to avoid it being optimized away.</Summary>
    public int BaselineNoExceptionHundredDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            if (index * avoidCompilerOptimizations % 2 == 0)
            {
                RecurseUntilException(100, false);
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    /// <Summary> Baseline method. int is returned to avoid it being optimized away.</Summary>
    public int BaselineNoExceptionThousandDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            if (index * avoidCompilerOptimizations % 2 == 0)
            {
                RecurseUntilException(1000, false);
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    /// <Summary> Baseline method. int is returned to avoid it being optimized away.</Summary>
    public int BaselineNoExceptionTenThousandDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            if (index * avoidCompilerOptimizations % 2 == 0)
            {
                RecurseUntilException(10000, false);
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    public int ExceptionOneDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            try
            {
                RecurseUntilException(1);
            }
            catch (Exception ex)
            {

            }
        }

        return count;
    }

    [Benchmark]
    public int ExceptionHundredDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            try
            {
                RecurseUntilException(100);
            }
            catch (Exception ex)
            {

            }
        }

        return count;
    }

    [Benchmark]
    public int ExceptionThousandDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            try
            {
                RecurseUntilException(1000);
            }
            catch (Exception ex)
            {

            }
        }

        return count;
    }

    [Benchmark]
    public int ExceptionTenThousandDepth()
    {
        int count = 0;
        for (int index = 0; index < reps; ++index)
        {
            try
            {
                RecurseUntilException(10000);
            }
            catch (Exception ex)
            {

            }
        }

        return count;
    }
}