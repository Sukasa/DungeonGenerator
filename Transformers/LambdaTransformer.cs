using System;
using System.Collections.Generic;
using System.Drawing;
using RngLib;
using System.Threading.Tasks;

namespace DungeonGenerator.Transformers
{
    public class LambdaTransformer<T, U> : DungeonTransformer<T, U>
    {
        public List<Func<DungeonFloor<T>, Point, IRng, TransformResult?>> Lambdas { get; protected set; }

        public struct TransformResult
        {
            public U Value;
            public int Priority;
        }

        public LambdaTransformer(DungeonFloor<T> InDungeon, DungeonFloor<U> OutDungeon) : base(InDungeon, OutDungeon)
        {
            Lambdas = new List<Func<DungeonFloor<T>, Point, IRng, TransformResult?>>();
        }

        public TransformResult CreateTransform(U Value, int Priority)
        {
            TransformResult TR;
            TR.Value = Value;
            TR.Priority = Priority;

            return TR;
        }

        public override void Transform(ulong Seed)
        {
            if (OutputDungeon == null)
                throw new InvalidOperationException("Output dungeon is null");

            if (OutputDungeon.Size != Dungeon.Size)
                throw new InvalidOperationException("Output dungeon dimensions do not match input dungeon dimensions");

            Action<int, int> Iterator = (X, Y) =>
            {
                Point Pos = new Point(X, Y);
                TransformResult? TR = null;
                int Prio = int.MinValue;

                for (int i = 0; i < Lambdas.Count; i++)
                {
                    TransformResult? T = Lambdas[i].Invoke(Dungeon, Pos, RandomSource);
                    if (T.HasValue && T.Value.Priority > Prio)
                    {
                        TR = T;
                        Prio = T.Value.Priority;
                    }
                }

                if (TR.HasValue)
                    OutputDungeon.SetTile(Pos, TR.Value.Value);
            };

            Parallel.For(0, Dungeon.Size.Width, X =>
            {
                for (int Y = 0; Y < Dungeon.Size.Height; Y++)
                {
                    Iterator(X, Y);
                }
            });

        }
    }
}
