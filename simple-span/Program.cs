using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

var summary = BenchmarkRunner.Run<StringCopyComparison>();

public class HelloWorld
{
    public string? Hello { get; set; }
    public string? World { get; set; }
}

[MemoryDiagnoser]
public class StringCopyComparison
{
    public string jsonStored = @"{
        ""Hello"": ""Hello"",
        ""World"": ""world""
    }";

    [Benchmark]
    public void SplitPath()
    {
        string path = "hello/world";

        var splitString = path.Split('/');
    }

    [Benchmark]
    public void SplitPathNoAlloc()
    {
        string path = "hello/world";
        int pathIndex = path.IndexOf('/');
        
        ReadOnlySpan<char> value = path.AsSpan().Slice(0, pathIndex);
        ReadOnlySpan<char> secondValue = path.AsSpan().Slice(pathIndex + 1);
    }

    [Benchmark]
    public void NewtonSoftDeserialize()
    {
        string json = @"{
            'Hello': 'Hello',
            'World': 'world'
        }";

        HelloWorld? value = JsonConvert.DeserializeObject<HelloWorld>(json);
    }

    [Benchmark]
    public void TextJsonDeserialize()
    {
        string json = @"{
            ""Hello"": ""Hello"",
            ""World"": ""world""
        }";

        HelloWorld? value = System.Text.Json.JsonSerializer.Deserialize<HelloWorld>(json);
    }

    [Benchmark]
    public void TextJsonDeserializeFromSpan()
    {
        ReadOnlySpan<char> span = this.jsonStored.AsSpan();
        HelloWorld? value = System.Text.Json.JsonSerializer.Deserialize<HelloWorld>(span);
    }
}