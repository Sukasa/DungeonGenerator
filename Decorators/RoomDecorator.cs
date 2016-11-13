using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RngLib;
using Sukasa.ExtensionFunctions;

namespace Dungeon_Generator.Decorators
{
    class RoomPlacer<T> : IDecorator<T>
    {
        public T FillValue { get; set; }
        public T ReplaceValue { get; set; }

        public int Keepout
        {
            get
            {
                return RoomKeepout + 3;
            }
        }

        public IRng RandomSource { get; set; }

        private enum Edge : int
        {
            North,
            West,
            South,
            East,
            Total
        }

        private Edge Turn180(Edge In)
        {
            return (Edge)(((int)In + 2) % (int)Edge.Total);
        }

        private int RoomKeepout = 1;

        private int RoomWidth = 1;
        private int RoomHeight = 1;
        private int RoomX = 1;
        private int RoomY = 1;
        private Edge FixedEdge;

        private void Widen(int Amt = 1)
        {
            if ((int)FixedEdge % 2 == 0)
            {
                RoomWidth += Amt;
                RoomX -= (RoomWidth & 1) * Math.Sign(Amt);
                RoomKeepout = RoomWidth;
            }
            else
            {
                RoomHeight += Amt;
                RoomY -= (RoomHeight & 1) * Math.Sign(Amt);
                RoomKeepout = RoomHeight;
            }
        }

        private void Deepen(int Amt = 1)
        {
            switch (FixedEdge)
            {
                case Edge.North:
                    RoomHeight += Amt;
                    break;
                case Edge.South:
                    RoomY -= Amt;
                    RoomHeight += Amt;
                    break;
                case Edge.West:
                    RoomWidth += Amt;
                    break;
                case Edge.East:
                    RoomX -= Amt;
                    RoomWidth += Amt;
                    break;
            }
        }

        public void Create(DungeonFloor<T> Dungeon, Point Origin, Point Direction)
        {
            Origin.Offset(Direction);

            if (Direction.X == -1)
            {
                FixedEdge = Edge.East;
            }
            else if (Direction.X == 1)
            {
                FixedEdge = Edge.West;
            }
            else
            {
                if (Direction.Y == 1)
                {
                    FixedEdge = Edge.North;

                }
                else
                {
                    FixedEdge = Edge.South;
                }
            }


            RoomX = Origin.X;
            RoomY = Origin.Y;

            int MaxWidth = RandomSource.Next(6, 12);
            int MaxHeight = RandomSource.Next(6, 12);

            bool Deeper = true;
            bool Wider = true;

            // Try to increase Width by 1 and then Depth by 1 twice, up to a max of 7/7
            do
            {
                if (Wider)
                {
                    Widen();
                    if (!RoomClear(Dungeon))
                    {
                        Wider = false;
                        Widen(-1);
                        Widen(-1);
                    }
                }
                if (Deeper)
                {
                    Deepen();
                    if (!RoomClear(Dungeon))
                    {
                        Deeper = false;
                        Deepen(-2);
                    }
                }
            } while ((Wider || Deeper) && RoomWidth < MaxWidth && RoomHeight < MaxHeight);

            //Now fill the room
            for (int X = RoomX; X < RoomX + RoomWidth; X++)
                for (int Y = RoomY; Y < RoomY + RoomHeight; Y++)
                    Dungeon.SetTile(X, Y, FillValue);

            Origin.Offset(Direction.Turn(180));

            if (RoomWidth <= 0 || RoomHeight <= 0)
                return;

            // Now draw a door to the room
            if ((int)FixedEdge % 2 == 0)
            {
                Dungeon.SetTile(Origin.X + (RoomWidth >> 2), Origin.Y, FillValue);
            }
            else
            {
                Dungeon.SetTile(Origin.X, Origin.Y + (RoomHeight >> 2), FillValue);
            }
        }

        private bool RoomClear(DungeonFloor<T> Dungeon)
        {
            for (int X = RoomX; X < RoomX + RoomWidth; X++)
                for (int Y = RoomY; Y < RoomY + RoomHeight; Y++)
                    if (!Equals(Dungeon.GetTile(X, Y), ReplaceValue))
                        return false;
            return true;
        }
    }
}
