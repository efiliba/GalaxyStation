using System.Linq;

namespace GalaxyStation
{
    public enum SideType { LeftClosed, RightClosed, TopClosed, BottomClosed, LeftOpened, RightOpened, TopOpened, BottomOpened }
    public enum ExamineType { Current, Bottom, Left, Top, Right }

    public class TileLookup
    {
        protected System.Collections.Generic.Dictionary<SideType, int[]> keys;
        protected System.Collections.Generic.Dictionary<ExamineType, int[]> examineKeys;
        protected int firstGid;

        public TileLookup(int firstGid)
        {
            this.firstGid = firstGid;       
        }

        public int this[SideType sideType, int gid]
        {
            get { return keys[sideType][gid - firstGid] + firstGid; }
        }

        public int this[ExamineType examineType, int gid]
        {
            get { return examineKeys[examineType][gid - firstGid] + firstGid; }
        }
    }

    public class RoofTileLookup : TileLookup
    {
        public RoofTileLookup(int firstGid) : base(firstGid)
        {
            keys = new System.Collections.Generic.Dictionary<SideType, int[]>(8);
            keys[SideType.LeftClosed] = new int[] { 14, 5, 2, 3, 3, 5, 6, 7, 8, 7, 8, 11, 6, 2, 14, 11 };
            keys[SideType.RightClosed] = new int[] { 0, 1, 2, 3, 4, 5, 5, 14, 3, 0, 4, 2, 1, 13, 14, 13 };
            keys[SideType.TopClosed] = new int[] { 0, 1, 5, 14, 0, 5, 6, 7, 7, 9, 9, 6, 12, 1, 14, 12, 16 };
            keys[SideType.BottomClosed] = new int[] { 0, 0, 3, 3, 4, 14, 7, 7, 8, 9, 10, 8, 9, 4, 14, 15 };
            keys[SideType.LeftOpened] = new int[] { 0, 1, 13, 4, 4, 1, 12, 9, 10, 9, 10, -1, 12, 13, 0, 16 };
            keys[SideType.RightOpened] = new int[] { 9, 12, 11, 8, 10, 6, 6, 7, 8, 9, 10, 11, 12, -1, 7, 16 };
            keys[SideType.TopOpened] = new int[] { 4, 13, 2, 3, 4, 2, 11, 8, 8, 10, 10, 11, -1, 13, 3, 16 };
            keys[SideType.BottomOpened] = new int[] { 1, 1, 2, 2, 13, 5, 6, 6, 11, 12, -1, 11, 12, 13, 5, 16 };
        }
    }

    public class WallTileLookup : TileLookup
    {
        public WallTileLookup(int firstGid) : base(firstGid)
        {
            keys = new System.Collections.Generic.Dictionary<SideType, int[]>(4);
            keys[SideType.LeftClosed] = new int[] { 0, 0, 0, 3, 3, 7, -1, 6, 6 };
            keys[SideType.RightClosed] = new int[] { 6, 2, 2, 7, 2, 5, 0, 6, 2 };
            keys[SideType.LeftOpened] = new int[] { 1, 1, 2, 2 };
            keys[SideType.RightOpened] = new int[] { 0, 1, 1, 0 };
        }
    }

    public class ModifyWallTileLookup : TileLookup
    {
        public ModifyWallTileLookup(int firstGid) : base(firstGid)
        {
            examineKeys = new System.Collections.Generic.Dictionary<ExamineType, int[]>(2);
            examineKeys[ExamineType.Left] = new int[] { 2, 2, 2, 5, 5, 5, 8, 8, 8 };
            examineKeys[ExamineType.Right] = new int[] { 6, 2, 8, 6, 7, 8, 6, 7, 8 };
        }
    }

    public class BuildRoofTileLookup : TileLookup
    {
        public BuildRoofTileLookup(int firstGid) : base(firstGid)
        {
            examineKeys = new System.Collections.Generic.Dictionary<ExamineType, int[]>(8);
            examineKeys[ExamineType.Current] = new int[] { 14, 3, 0, 4, 5, 2, 1, 13, 7, 8, 9, 10, 6, 11, 12, 15 };
            examineKeys[ExamineType.Bottom] = new int[] { 1, 1, 2, 2, 13, 5, 6, 6, 11, 12, 15, 11, 12, 13, 5, 15 };
            examineKeys[ExamineType.Right] = new int[] { 9, 12, 11, 8, 10, 6, 6, 7, 8, 9, 10, 11, 12, 15, 7, 15 };
            examineKeys[ExamineType.Top] = new int[] { 4, 13, 2, 3, 4, 2, 11, 8, 8, 10, 10, 11, 15, 13, 3, 15 };
            examineKeys[ExamineType.Left] = new int[] { 0, 1, 13, 4, 4, 1, 12, 9, 10, 9, 10, 15, 12, 13, 0, 15 };
        }
    }
}