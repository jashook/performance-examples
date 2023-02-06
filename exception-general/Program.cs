using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<ExceptionComparison>();

[MemoryDiagnoser]
public class ExceptionComparison
{

    public string SimulateExceptionTrapAndLog(Action<int, bool, bool, bool, bool, int> method, bool logException, bool logStackTrace, int breakDepth, int depth, bool throwException, bool catchAtLowestPoint)
    {
        try
        {
            method(breakDepth, throwException, logException, logStackTrace, catchAtLowestPoint, depth);
        }
        catch(Exception e)
        {
            if (logException || logStackTrace || catchAtLowestPoint)
            {
                string message = "";
                if (logException)
                {
                    if (e.InnerException != null) message = e.InnerException.ToString();
                }

                if (logStackTrace)
                {
                    // Should ideally use StringBuilder here. Avoiding so because it is
                    // a common mistake and I have seen this code many times in production
                    // services.
                    message += " " + $"{new StackTrace()}";
                }

                // Returning here; however, most services would write to some form
                // of IO at this point, which is additional overhead.
                if (catchAtLowestPoint)
                {
                    return message;
                }
                else
                {
                    #pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw e;
                    #pragma warning restore CA2200 // Rethrow to preserve stack details
                }
            }
            else
            {
                // Rethrow. We want this to be caught at the top of the stack.
                #pragma warning disable CA2200 // Rethrow to preserve stack details
                throw e;
                #pragma warning restore CA2200 // Rethrow to preserve stack details
            }
        }

        return string.Empty;
    }

    /// <Summary>Recursive method which will simulate a deep stack trace</Summary>
    /// <Notes>
    /// The JIT will attempt to optimize recursive methods into loops. Methods 
    /// which throw are not candidates.
    /// </Notes>
    public void RecurseUntilException(int breakDepth = 1, bool throwException = true, bool logException = false, bool logStackTrace = false, bool catchAtLowestPoint = false, int depth = 0)
    {
        if (depth >= breakDepth)
        {
            if (!throwException) return;

            throw new Exception("Catch me!");
        }

        SimulateExceptionTrapAndLog(method: RecurseUntilException, throwException: throwException, logException: logException, logStackTrace: logStackTrace, breakDepth: breakDepth, depth: ++depth, catchAtLowestPoint: catchAtLowestPoint);
    }

    /// <Summary>Recursive method demonstrating throw vs non throw performance</Summary>
    /// <Notes>
    /// The JIT will attempt to optimize recursive methods into loops. This method
    /// will be optimized to a loop.
    /// </Notes>
    public void RecurseUntilExceptionNoThrow(int breakDepth = 1, int depth = 0)
    {
        if (depth >= breakDepth)
        {
            return;
        }

        RecurseUntilExceptionNoThrow(breakDepth, ++depth);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoEh()
    {
        RecurseUntilExceptionNoThrow(1);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoEhFiveDepth()
    {
        RecurseUntilExceptionNoThrow(5);
    }

    [Benchmark]
    /// <Summary> Baseline method no EH in method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoEhTenDepth()
    {
        RecurseUntilExceptionNoThrow(10);
    }

    [Benchmark]
    /// <Summary> Baseline method no EH in method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoEhTwentyFiveDepth()
    {
        RecurseUntilExceptionNoThrow(25);
    }

    [Benchmark]
    /// <Summary> Baseline method no EH in method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoEhHundredDepth()
    {
        RecurseUntilExceptionNoThrow(100);
    }

    [Benchmark]
    /// <Summary> Baseline method no EH in method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoEhThousandDepth()
    {
        RecurseUntilExceptionNoThrow(1000);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoException()
    {
        RecurseUntilException(1, false);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoExceptionFiveDepth()
    {
        RecurseUntilException(5, false);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoExceptionTenDepth()
    {
        RecurseUntilException(10, false);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoExceptionTwentyFiveDepth()
    {
        RecurseUntilException(25, false);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoExceptionHundredDepth()
    {
        RecurseUntilException(100, false);
    }

    [Benchmark]
    /// <Summary> Baseline method</Summary>
    /// <Notes>
    ///     Baseline will run the same method, which can throw without any,
    ///     exception handling. Theory is runtime will increase with stack
    ///     depth.
    /// </Notes>
    public void BaselineNoExceptionThousandDepth()
    {
        RecurseUntilException(1000, false);
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the top of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     exponentially with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneDepth()
    {
        try
        {
            RecurseUntilException(1);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the top of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     exponentially with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionFiveDepth()
    {
        try
        {
            RecurseUntilException(5);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the top of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     exponentially with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTenDepth()
    {
        try
        {
            RecurseUntilException(10);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the top of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     exponentially with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTwentyFiveDepth()
    {
        try
        {
            RecurseUntilException(25);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the top of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     exponentially with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionHundredDepth()
    {
        try
        {
            RecurseUntilException(100);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the top of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     exponentially with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionThousandDepth()
    {
        try
        {
            RecurseUntilException(1000);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneDepthHandleLowestStack()
    {
        try
        {
            RecurseUntilException(1, catchAtLowestPoint: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionFiveDepthHandleLowestStack()
    {
        try
        {
            RecurseUntilException(5, catchAtLowestPoint: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTenDepthHandleLowestStack()
    {
        try
        {
            RecurseUntilException(10, catchAtLowestPoint: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTwentyFiveDepthHandleLowestStack()
    {
        try
        {
            RecurseUntilException(25, catchAtLowestPoint: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneHundredDepthHandleLowestStack()
    {
        try
        {
            RecurseUntilException(100, catchAtLowestPoint: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneThousandDepthHandleLowestStack()
    {
        try
        {
            RecurseUntilException(1000, catchAtLowestPoint: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneDepthHandleLowestStackLog()
    {
        try
        {
            RecurseUntilException(1, catchAtLowestPoint: true, logException: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionFiveDepthHandleLowestStackLog()
    {
        try
        {
            RecurseUntilException(5, catchAtLowestPoint: true, logException: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTenDepthHandleLowestStackLog()
    {
        try
        {
            RecurseUntilException(10, catchAtLowestPoint: true, logException: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTwentyFiveDepthHandleLowestStackLog()
    {
        try
        {
            RecurseUntilException(25, catchAtLowestPoint: true, logException: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionHundrenDepthHandleLowestStackLog()
    {
        try
        {
            RecurseUntilException(100, catchAtLowestPoint: true, logException: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionThousandDepthHandleLowestStackLog()
    {
        try
        {
            RecurseUntilException(1000, catchAtLowestPoint: true, logException: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneDepthHandleLowestStackLogStackTraceHandleLowestStackLogStackTrace()
    {
        try
        {
            RecurseUntilException(1, catchAtLowestPoint: true, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionFiveDepthHandleLowestStackLogStackTrace()
    {
        try
        {
            RecurseUntilException(5, catchAtLowestPoint: true, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTenDepthHandleLowestStackLogStackTrace()
    {
        try
        {
            RecurseUntilException(10, catchAtLowestPoint: true, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTwentyFiveDepthHandleLowestStackLogStackTrace()
    {
        try
        {
            RecurseUntilException(25, catchAtLowestPoint: true, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionHundreDepthHandleLowestStackLogStackTrace()
    {
        try
        {
            RecurseUntilException(100, catchAtLowestPoint: true, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the lowest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~1000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionThousandDepthHandleLowestStackLogStackTrace()
    {
        try
        {
            RecurseUntilException(1000, catchAtLowestPoint: true, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the highest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionOneDepthLogStackTraceLogStackTrace()
    {
        try
        {
            RecurseUntilException(1, catchAtLowestPoint: false, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the highest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionFiveDepthLogStackTrace()
    {
        try
        {
            RecurseUntilException(5, catchAtLowestPoint: false, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the highest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTenDepthLogStackTrace()
    {
        try
        {
            RecurseUntilException(10, catchAtLowestPoint: false, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the highest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionTwentyFiveDepthLogStackTrace()
    {
        try
        {
            RecurseUntilException(25, catchAtLowestPoint: false, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the highest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionHundredDepthLogStackTrace()
    {
        try
        {
            RecurseUntilException(100, catchAtLowestPoint: false, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }

    [Benchmark]
    /// <Summary> Exception will be thrown and caught at the highest level of the stack and logging and stack trace will be done.</Summary>
    /// <Notes>
    ///     Hypothesis is that overhead of catching an exception will increase
    ///     linearly with stack depth. Run time will be ~5000x slower than 
    ///     the baselines.
    /// </Notes>
    public void ExceptionThousandDepthLogStackTrace()
    {
        try
        {
            RecurseUntilException(1000, catchAtLowestPoint: false, logException: true, logStackTrace: true);
        }
        catch (Exception)
        {

        }
    }
}