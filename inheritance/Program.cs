using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<TestInheritance>();

public class Base
{
    public readonly int value = 10;
    public readonly int secondValue = 1000;

    // Choice is to avoid inlining. For the test to be interesting we need
    // to do the call.
    public virtual int GetValue(int choice)
    {
        if (choice % 2 == 0)
        {
            return value;
        }
        else
        {
            return secondValue;
        }
    }
}

public class First : Base
{
    public readonly int firstValue = 11;
    public readonly int notBaseSecondValue = -11;

    // Choice is to avoid inlining. For the test to be interesting we need
    // to do the call.
    public override int GetValue(int choice)
    {
        if (choice % 2 == 0)
        {
            return value + base.GetValue(choice + 1);;
        }
        else
        {
            return notBaseSecondValue + base.GetValue(choice);
        }
    }
}

public class Second : Base
{
    public readonly int notBaseValue = 12;
    public readonly int notBaseSecondValue = -12;

    // Choice is to avoid inlining. For the test to be interesting we need
    // to do the call.
    public override int GetValue(int choice)
    {
        if (choice % 2 == 0)
        {
            return notBaseValue + base.GetValue(choice + 1);;
        }
        else
        {
            return notBaseSecondValue + base.GetValue(choice);
        }
    }
}

public interface IBase
{
    public abstract int DoSomething(int choice);
}

public class Doer : IBase
{
    int value = 15;
    int secondValue = -15;

    public int DoSomething(int choice)
    {
        if (choice % 2 == 0)
        {
            return value;
        }
        else
        {
            return secondValue;
        }
    }
}

public class DoerNoOverhead
{
    int value = 16;
    int secondValue = -16;

    public int DoSomething(int choice)
    {
        if (choice % 2 == 0)
        {
            return value;
        }
        else
        {
            return secondValue;
        }
    }
}

public class BaseDirect
{
    int value = 100;
    int secondValue = -100;

    public int GetValue(int choice)
    {
        if (choice % 2 == 0)
        {
            return value;
        }
        else
        {
            return secondValue;
        }
    }
}

public class FirstDirect
{
    int value = 101;
    int secondValue = -101;
    public BaseDirect encapsulatedValue;

    public FirstDirect()
    {
        encapsulatedValue = new();
    }

    public int GetValue(int choice)
    {
        if (choice % 2 == 0)
        {
            return value + encapsulatedValue.GetValue(choice + 1);;
        }
        else
        {
            return secondValue + encapsulatedValue.GetValue(choice);
        }
    }
}

public class SecondDirect
{
    public readonly int value = 102;
    public readonly int secondValue = -102;
    public BaseDirect encapsulatedValue;

    public SecondDirect()
    {
        encapsulatedValue = new();
    }

    public int GetValue(int choice)
    {
        if (choice % 2 == 0)
        {
            return value + encapsulatedValue.GetValue(choice + 1);;
        }
        else
        {
            return secondValue + encapsulatedValue.GetValue(choice);
        }
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
            int value = encapsulatedObject.GetValue(1);
            value = secondObj.GetValue(1);
        }
    }

    [Benchmark]
    public void InheritanceDevirt()
    {
        Base firstDerivedObj = new First();
        Base secondDerivedObj = new Second();

        for (int index = 0; index < reps; ++index)
        {
            int value = firstDerivedObj.GetValue(1);
            value = secondDerivedObj.GetValue(1);
        }
    }

    public void HelperMethod(Base first, Base second)
    {
        int value = first.GetValue(1);
        value = second.GetValue(0);
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
        doer.DoSomething(1);
    }

    public void DoerNoOverheadHelperMethod(DoerNoOverhead doer)
    {
        doer.DoSomething(1);
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