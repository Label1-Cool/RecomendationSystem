using System;
namespace Logic.Models
{
    interface IItemAnalyzed
    {
        string Name { get; }
        double XCoord { get; }
        double YCoord { get; }
    }
}
