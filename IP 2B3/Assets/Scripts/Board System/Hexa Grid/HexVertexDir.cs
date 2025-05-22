using System.ComponentModel;

namespace B3.BoardSystem
{
    // pozitia este pusa in functie de centrul hex-ului luat
    public enum HexVertexDir
    {
        TopRight = 0,
        Right      = 1,
        BottomRight = 2,
        BottomLeft = 3,
        Left      = 4,
        TopLeft = 5,
    }
    public static class HexVertexDirExt {
        public static HexVertexDir Opposite(this HexVertexDir dir) =>
            dir switch
            {
                HexVertexDir.TopLeft => HexVertexDir.BottomRight,
                HexVertexDir.TopRight => HexVertexDir.BottomLeft,
                HexVertexDir.Right => HexVertexDir.Left,
                HexVertexDir.BottomRight => HexVertexDir.TopRight,
                HexVertexDir.BottomLeft => HexVertexDir.TopRight,
                HexVertexDir.Left => HexVertexDir.Right,
                _ => throw new InvalidEnumArgumentException()
            };
    }
}