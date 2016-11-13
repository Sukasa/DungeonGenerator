using RngLib;

namespace Dungeon_Generator
{
    public abstract class DungeonGenerator<T>
    {
        public IRng RandomSource { get; set; } = new SimpleRNG();
        public DungeonFloor<T> Dungeon { get; private set; }

        public T DefaultOpenTile { get; set; } = default(T);
        public T DefaultBlockedTile { get; set; } = default(T);
        public T DefaultFeatureTile { get; set; } = default(T);

        public DungeonGenerator(DungeonFloor<T> Dungeon)
        {
            this.Dungeon = Dungeon;
            RandomSource.UseDefaultSeed();
        }

        public void Generate()
        {
            Generate(RandomSource.NextULong());
        }

        public abstract void Generate(ulong Seed);
    }
}
