using RngLib;

namespace DungeonGenerator
{
    public abstract class DungeonWorker<T>
    {
        public IRng RandomSource { get; set; } = new SimpleRNG();
        public DungeonFloor<T> Dungeon { get; protected set; }

        public DungeonWorker(DungeonFloor<T> InDungeon)
        {
            Dungeon = InDungeon;
        }
    }
}
