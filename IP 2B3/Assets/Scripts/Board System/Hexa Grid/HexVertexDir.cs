using System.ComponentModel;
using UnityEngine;

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
                HexVertexDir.TopLeft => HexVertexDir.BottomLeft,
                HexVertexDir.TopRight => HexVertexDir.BottomRight,
                HexVertexDir.Right => HexVertexDir.BottomLeft,
                HexVertexDir.BottomRight => HexVertexDir.TopRight,
                HexVertexDir.BottomLeft => HexVertexDir.TopLeft,
                HexVertexDir.Left => HexVertexDir.BottomRight,
                _ => throw new InvalidEnumArgumentException()
            };
        
        public static Vector2 OffsetFromCenter(this HexVertexDir dir)
        {
            float h = Mathf.Sqrt(3) / 2;

            return dir switch
            {
                HexVertexDir.TopRight => new Vector2(0.5f, h),
                HexVertexDir.Right => new Vector2(1f, 0),
                HexVertexDir.BottomRight => new Vector2(0.5f, -h),
                HexVertexDir.BottomLeft => new Vector2(-0.5f, -h),
                HexVertexDir.Left => new Vector2(-1f, 0),
                HexVertexDir.TopLeft => new Vector2(-0.5f, h),
                _ => throw new InvalidEnumArgumentException()
            };
        }
    }
}