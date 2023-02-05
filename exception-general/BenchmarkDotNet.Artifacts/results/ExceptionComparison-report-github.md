``` ini

BenchmarkDotNet=v0.13.4, OS=macOS Monterey 12.6.2 (21G320) [Darwin 21.6.0]
Apple M1 Max, 1 CPU, 10 logical and 10 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 7.0.0 (7.0.22.51805), Arm64 RyuJIT AdvSIMD


```
|                              Method |           Mean |       Error |      StdDev |
|------------------------------------ |---------------:|------------:|------------:|
|                 BaselineNoException |       158.7 ns |     0.21 ns |     0.19 ns |
|     BaselineNoExceptionHundredDepth |       158.7 ns |     0.28 ns |     0.26 ns |
|    BaselineNoExceptionThousandDepth |       158.7 ns |     0.33 ns |     0.29 ns |
| BaselineNoExceptionTenThousandDepth |       160.3 ns |     0.23 ns |     0.20 ns |
|                   ExceptionOneDepth | 1,365,137.6 ns | 2,294.77 ns | 2,034.25 ns |
|               ExceptionHundredDepth | 1,372,675.8 ns | 1,797.36 ns | 1,681.25 ns |
|              ExceptionThousandDepth | 1,363,758.6 ns | 1,181.93 ns |   986.96 ns |
|           ExceptionTenThousandDepth | 1,367,513.6 ns | 2,045.97 ns | 1,813.70 ns |
