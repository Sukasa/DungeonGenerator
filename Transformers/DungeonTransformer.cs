using System;

namespace DungeonGenerator.Transformers
{
    public abstract class DungeonTransformer<T, U> : DungeonWorker<T>
    {
        public DungeonFloor<U> OutputDungeon { get; set; }

        public DungeonTransformer(DungeonFloor<T> InDungeon, DungeonFloor<U> OutDungeon) : base(InDungeon)
        {
            OutputDungeon = OutDungeon;
            
            if (OutputDungeon != null && OutputDungeon.Size != Dungeon.Size)
                throw new InvalidOperationException("Output dungeon dimensions do not match input dungeon dimensions");

            RandomSource.UseDefaultSeed();
        }

        public void Transform()
        {
            Transform(RandomSource.NextULong());
        }

        public abstract void Transform(ulong Seed);
    }
}
