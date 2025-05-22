using System.ComponentModel;
using UnityEngine;

namespace B3.BoardSystem
{
    //Aceeasi idee ca la HexVertex dir, pozitia fata de centrul hexului
    public enum HexEdgeDir
    {
        Top     = 0,
        TopRight = 1,
        BottomRight = 2,
        Bottom     = 3,
        BottomLeft = 4,
        TopLeft = 5 
    }
    
    public static class HexEdgeDirExt {
        public static HexEdgeDir Opposite(this HexEdgeDir dir) =>
            dir switch 
            {
                HexEdgeDir.Top => HexEdgeDir.Bottom,
                HexEdgeDir.TopRight => HexEdgeDir.BottomLeft,
                HexEdgeDir.BottomRight => HexEdgeDir.TopLeft,
                HexEdgeDir.Bottom => HexEdgeDir.Top,
                HexEdgeDir.BottomLeft => HexEdgeDir.TopRight,
                HexEdgeDir.TopLeft => HexEdgeDir.BottomRight,
                _ => throw new InvalidEnumArgumentException()
            };
        
        public static Vector2 OffsetFromCenterOfHex(this HexEdgeDir dir)
        {
            float h = Mathf.Sqrt(3) / 2;

            return dir switch
            {
                HexEdgeDir.Top         => new (0, h),
                HexEdgeDir.TopRight    => new (0.75f, h / 2),
                HexEdgeDir.BottomRight => new (0.75f, -h / 2),
                HexEdgeDir.Bottom      => new (0, -h),
                HexEdgeDir.BottomLeft  => new (-0.75f, -h / 2),
                HexEdgeDir.TopLeft     => new (-0.75f, h / 2),
                _ => throw new InvalidEnumArgumentException()
            };
        }
    }
}