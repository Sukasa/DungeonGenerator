using RngLib;
using System.Drawing;

namespace DungeonGenerator.Decorators
{
    public interface IFeaturePlacer<T>
    {
        T FillValue { get; set; }
        T ReplaceValue { get; set; }
        IRng RandomSource { get; set; }
        int Keepout { get; }
        
        void Create(DungeonFloor<T> Dungeon, Point Origin, Point Direction);
    }
}
