namespace DungeonGenerator.Generators
{
    public abstract class DungeonGenerator<T> : DungeonWorker<T>
    {
        public T DefaultOpenTile { get; set; } = default(T);
        public T DefaultBlockedTile { get; set; } = default(T);
        public T DefaultFeatureTile { get; set; } = default(T);

        public DungeonGenerator(DungeonFloor<T> InDungeon) : base(InDungeon)
        {
            RandomSource.UseDefaultSeed();
        }

        public void Generate()
        {
            Generate(RandomSource.NextULong());
        }

        public abstract void Generate(ulong Seed);
    }
}
