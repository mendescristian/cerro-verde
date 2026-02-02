using TUnit.Core.Interfaces;

namespace Controle.Library.Tests.Parallelism;

public class ParallelLimit : IParallelLimit
{
    public int Limit => 5;
}
