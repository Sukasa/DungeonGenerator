using RngLib;
using Sukasa.ExtensionFunctions;
using System.Drawing;

namespace DungeonGenerator
{
    /// <summary>
    ///     Dungeon floor class.  Contains the floor array, and eventually post-processing / SerDes / various utility functionality
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DungeonFloor<T>
    {
        public T[] Tiles { get; private set; }
        public Size Size { get; set; }
        public int Stride { get; private set; }

        public DungeonFloor(int Dimensions) : this(Dimensions, Dimensions)
        {
        }

        public DungeonFloor(Size Dimensions) : this(Dimensions.Width, Dimensions.Height)
        {
        }

        public DungeonFloor(int Width, int Height)
        {
            Tiles = new T[Width * Height];
            Stride = Width;
            Size = new Size(Width, Height);
        }

        public void SetTile(int X, int Y, T Value)
        {
            if (X < 0 || X >= Size.Width)
                return;

            if (Y < 0 || Y >= Size.Height)
                return;

            Tiles[X + (Y * Stride)] = Value;
        }

        public T GetTile(Point Position)
        {
            return GetTile(Position.X, Position.Y);
        }

        public T GetTile(int X, int Y)
        {
            if (X < 0 || X >= Size.Width)
                return default(T);

            if (Y < 0 || Y >= Size.Height)
                return default(T);

            return Tiles[X + (Y * Stride)];
        }

        public void SetTile(Point Position, T Value)
        {
            SetTile(Position.X, Position.Y, Value);
        }

        public void Clear(T Value)
        {
            for (int x = 0; x < Tiles.Length; x++)
                Tiles[x] = Value;
        }

        public virtual void DoPostCreationBake()
        {
            // Nothing in the base class
        }
        // TODO stuff like save/load, etc

        public Point GetRandomEdgeTile(IRng RandomSource)
        {
            int Side = RandomSource.NextByte() & 0x3;
            int Offset;
            if ((Side & 1) == 1)
                Offset = (int)(Size.Height * RandomSource.NextDouble());
            else
                Offset = (int)(Size.Width * RandomSource.NextDouble());

            switch (Side)
            {
                case 0:
                    return new Point(Offset, 0);
                case 1:
                    return new Point(Size.Width - 1, Offset);
                case 2:
                    return new Point(Offset, Size.Height - 1);
                default:
                    return new Point(0, Offset);
            }
        }

        public Point GetRandomTile(IRng RandomSource, int EdgeAvoid = 0)
        {
            return new Point(RandomSource.Next(EdgeAvoid, Size.Width - EdgeAvoid), RandomSource.Next(EdgeAvoid, Size.Height - EdgeAvoid));
        }

        public bool Adjacent4(Point Position, T Value)
        {
            return Adjacent4(Position.X, Position.Y, Value);
        }

        public bool Adjacent4(int X, int Y, T Value)
        {
            if (Equals(GetTile(X - 1, Y), Value))
                return true;

            if (Equals(GetTile(X + 1, Y), Value))
                return true;

            if (Equals(GetTile(X, Y - 1), Value))
                return true;

            if (Equals(GetTile(X, Y + 1), Value))
                return true;

            return false;
        }

        public bool Adjacent8(Point Position, T Value)
        {
            return Adjacent8(Position.X, Position.Y, Value);
        }

        public bool Adjacent8(int X, int Y, T Value)
        {
            for (int dX = -1; dX < 2; dX++)
                for (int dY = -1; dY < 2; dY++)
                    if ((dX | dY) != 0 && Equals(GetTile(X + dX, Y + dY), Value))
                        return true;

            return false;
        }

        public int NumWithin(Point P, int Radius, params T[] Values)
        {
            return NumWithin(P.X, P.Y, Radius, Values);
        }

        public int NumWithin(int X, int Y, int Radius, params T[] Values)
        {
            int Count = 0;
            for (int dX = - Radius; dX <= Radius; dX++)
                for (int dY = - Radius; dY <= Radius; dY++)
                    if (Values.Contains(GetTile(X + dX, Y + dY)))
                        Count++;

            return Count;
        }

        public bool EdgePierced(T SolidValue)
        {
            for (int Y = 0; Y < Size.Height; Y++)
            {
                if (!Equals(GetTile(0, Y), SolidValue))
                    return true;

                if (!Equals(GetTile(Size.Width - 1, Y), SolidValue))
                    return true;
            }

            for (int X = 0; X < Size.Width; X++)
            {
                if (!Equals(GetTile(X, 0), SolidValue))
                    return true;

                if (!Equals(GetTile(X, Size.Height - 1), SolidValue))
                    return true;
            }

            return false;
        }
    }
}
