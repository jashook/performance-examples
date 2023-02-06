using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<TestInheritance>();

public class Base
{
    public readonly int value = 10;

    public virtual int GetValue()
    {
        return value;
    }
}

public class First : Base
{
    public readonly int firstValue = 11;

    public override int GetValue()
    {
        return value + base.GetValue();
    }
}

public class Second : Base
{
    public readonly int secondValue = 12;

    public override int GetValue()
    {
        return value + base.GetValue();
    }
}

public interface IBase
{
    public abstract int DoSomething();
}

public class Doer : IBase
{
    public int DoSomething()
    {
        return 15;
    }
}

public class DoerNoOverhead
{
    public int DoSomething()
    {
        return 16;
    }
}

public class BaseDirect
{
    public readonly int value = 100;

    public int GetValue()
    {
        return value;
    }
}

public class FirstDirect
{
    public readonly int value = 101;
    public BaseDirect encapsulatedValue = new();

    public int GetValue()
    {
        return value + encapsulatedValue.GetValue();
    }
}

public class SecondDirect
{
    public readonly int value = 102;
    public BaseDirect encapsulatedValue = new();

    public int GetValue()
    {
        return value + encapsulatedValue.GetValue();
    }
}

public class TestInheritance
{
    private readonly int reps = 1000;

    [Benchmark]
    public void Baseline()
    {
        FirstDirect encapsulatedObject = new();
        SecondDirect secondObj = new();
        
        for (int index = 0; index < reps; ++index)
        {
            int value = encapsulatedObject.GetValue();
            value = secondObj.GetValue();
        }
    }

    [Benchmark]
    public void InheritanceDevirt()
    {
        Base firstDerivedObj = new First();
        Base secondDerivedObj = new Second();

        for (int index = 0; index < reps; ++index)
        {
            int value = firstDerivedObj.GetValue();
            value = secondDerivedObj.GetValue();
        }
    }

    public void HelperMethod(Base first, Base second)
    {
        int value = first.GetValue();
        value = second.GetValue();
    }

    [Benchmark]
    public void InheritanceNoDevirt()
    {
        Base firstDerivedObj = new First();
        Base secondDerivedObj = new Second();

        for (int index = 0; index < reps; ++index)
        {
            HelperMethod(firstDerivedObj, secondDerivedObj);
        }
    }

    public void DoerHelperMethod(IBase doer)
    {
        doer.DoSomething();
    }

    public void DoerNoOverheadHelperMethod(DoerNoOverhead doer)
    {
        doer.DoSomething();
    }

    [Benchmark]
    public void DoerInterface()
    {
        Doer doer = new();

        for (int index = 0; index < reps; ++index)
        {
            DoerHelperMethod(doer);
        }
    }

    [Benchmark]
    public void DoerNoInterface()
    {
        DoerNoOverhead doer = new();

        for (int index = 0; index < reps; ++index)
        {
            DoerNoOverheadHelperMethod(doer);
        }
    }
}