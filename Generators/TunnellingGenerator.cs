using Sukasa.ExtensionFunctions;
using System;
using System.Drawing;

namespace Dungeon_Generator.Generators
{
    public class CityRoadsGenerator<T> : DungeonGenerator<T>
    {
        public CityRoadsGenerator(DungeonFloor<T> Dungeon) : base(Dungeon)
        {
        }

        public int Roads { get; private set; } = 0;

        public override void Generate(ulong Seed)
        {
            Dungeon.Clear(DefaultBlockedTile);
            RandomSource.SetSeed(Seed);

            // Start somewhere in the center of the map
            Point Start = Dungeon.GetRandomTile(RandomSource, Dungeon.Size.Width >> 1);

            // Pick a random direction to go in
            Point Direction = new Point(1, 0).Turn(90 * RandomSource.Next(0, 4));

            Roads = 0;
            Road(Start, RandomSource.Next(4, 6), RandomSource.Next(Dungeon.Size.Height >> 1, Dungeon.Size.Width - (Dungeon.Size.Width >> 2)), Direction);
        }

        private void Road(Point Start, int Width, int Length, Point Direction, int Depth = 1)
        {
            // Return early without counting as a road
            if (Length < 0 || Width <= 0 || Depth > 15)
                return;

            Roads++;

            // Basic variables used
            Point Position = Start;
            bool Vertical = Direction.Y == 0;
            int LDepth = 0;
            int CheckSkip = 4;
            int TooClosePenalty = 12;

            for (; Length > 0; Length--, LDepth++, CheckSkip--)
            {
                if (CheckSkip <= 0 && !Equals(Dungeon.GetTile(Position), DefaultBlockedTile))
                    return;

                // Mark road tiles
                if (Vertical)
                {
                    for (int i = 0; i < Width; i++)
                        Dungeon.SetTile(Position.X, Position.Y + i, DefaultOpenTile);
                }
                else
                {
                    for (int i = 0; i < Width; i++)
                        Dungeon.SetTile(Position.X + i, Position.Y, DefaultOpenTile);
                }

                // Reduce the too-close penalty as we go
                TooClosePenalty = Math.Max(TooClosePenalty - 1, 0);

                // Determine if we should add a 'feature' to the road here (Turn, intersection, branch)
                if (TooClosePenalty == 0 && RandomSource.NextDouble() - (LDepth * 0.005f) <= 0.324f)
                {
                    TooClosePenalty = 5;
                    CheckSkip = Width + 1;

                    if (RandomSource.NextDouble() < 0.1f)
                    {
                        // Turn
                        int Turn = 90 + 180 * RandomSource.Next(0, 2);
                        Point NewDirection = Direction.Turn(Turn);
                        Point Back = NewDirection.Turn(180);

                        if (NewDirection.X == -1 || NewDirection.Y == -1)
                        {
                            for (int j = 0; j < Width - 1; j++)
                            {
                                Position.Offset(Back);
                            }
                        }

                        Direction = NewDirection;
                        Vertical = !Vertical;
                        Length += Width;
                        continue;
                    }
                    else if (RandomSource.NextDouble() < 0.13f)
                    {
                        // Intersection
                        Point NewDirection = Direction.Turn(90);
                        Road(Position, Width - (Depth & 1), RandomSource.Next(Length - (Depth / 2), Length), NewDirection, Depth + 1);
                        NewDirection = Direction.Turn(-90);
                        Road(Position, Width - (Depth & 1), RandomSource.Next(Length - (Depth / 2), Length), NewDirection, Depth + 1);
                    }
                    else if (RandomSource.NextDouble() < 0.2f)
                    {
                        // Branch
                        bool OffsDir = RandomSource.NextDouble() >= 0.5;
                        Point NewDirection = Direction.Turn(OffsDir ? -90 : 90);
                        Road(Position, Width - (Depth % 3 == 0 ? 1 : 0), RandomSource.Next(Length - Depth, Length), NewDirection, Depth + 1);
                    }
                    else
                    {
                        int Turn = 90 + 180 * RandomSource.Next(0, 2);
                        Point NewDirection = Direction.Turn(Turn);
                        Point DecoStart = Position;
                        while (!Equals(Dungeon.GetTile(DecoStart), DefaultBlockedTile))
                            DecoStart.Offset(NewDirection);

                        Decorators.RoomPlacer<T> Placer = new Decorators.RoomPlacer<T>();
                        Placer.RandomSource = RandomSource;
                        Placer.FillValue = DefaultFeatureTile;
                        Placer.ReplaceValue = DefaultBlockedTile;
                        Placer.Create(Dungeon, DecoStart, NewDirection);
                        TooClosePenalty = Math.Max(TooClosePenalty, Placer.Keepout);
                        Length = Math.Max(Length, (Placer.Keepout >> 1) + 2);
                    }
                }

                // And progress in the road direction
                Position.Offset(Direction);
            }
        }
    }
}
