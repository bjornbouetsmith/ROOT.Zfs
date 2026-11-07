using Xunit.Sdk;
using Xunit.v3;

// MSTest ran tests sequentially by default; keep the same execution model under xUnit.
[assembly: Parallelization(Mode = ParallelMode.None)]
