using System;
namespace Logic.Models
{
    interface IItemAnalyzed
    {
        int Id { get; }
        string Name { get; }
        double XCoord { get; }
        double YCoord { get; }
    }
}
