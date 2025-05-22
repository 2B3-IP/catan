using System.ComponentModel;

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
    }
}