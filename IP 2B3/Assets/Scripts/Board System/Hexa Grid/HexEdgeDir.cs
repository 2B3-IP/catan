using System;
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
        
        public static HexVertexDir GetVertexDirBasedOnStartDir(this HexVertexDir startDir, HexPosition startPos, HexPosition endPos)
        {
            return startDir switch
            {
                HexVertexDir.TopLeft => startPos.X == endPos.X ? HexVertexDir.BottomLeft : HexVertexDir.Right,
                HexVertexDir.TopRight => startPos.Y == endPos.Y ? HexVertexDir.Left : HexVertexDir.BottomRight,
                HexVertexDir.Right => startPos.Y == endPos.Y ? HexVertexDir.BottomLeft : HexVertexDir.TopLeft,
                HexVertexDir.BottomRight => startPos.X == endPos.X ? HexVertexDir.TopRight : HexVertexDir.Left,
                HexVertexDir.BottomLeft => startPos.X == endPos.X ? HexVertexDir.TopLeft : HexVertexDir.Right,
                HexVertexDir.Left => startPos.Y == endPos.Y ? HexVertexDir.TopRight : HexVertexDir.BottomRight,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static HexEdgeDir GetHexDir(HexVertexDir vertex1, HexVertexDir vertex2)
        {
            return (vertex1, vertex2) switch
            {
                (HexVertexDir.TopLeft, HexVertexDir.TopRight) => HexEdgeDir.Top,
                (HexVertexDir.TopRight, HexVertexDir.Right) => HexEdgeDir.TopRight,
                (HexVertexDir.Right, HexVertexDir.BottomRight) => HexEdgeDir.BottomRight,
                (HexVertexDir.BottomRight, HexVertexDir.BottomLeft) => HexEdgeDir.Bottom,
                (HexVertexDir.BottomLeft, HexVertexDir.Left) => HexEdgeDir.BottomLeft,
                (HexVertexDir.Left, HexVertexDir.TopLeft) => HexEdgeDir.TopLeft,
                
                (HexVertexDir.TopRight, HexVertexDir.TopLeft) => HexEdgeDir.Top,
                (HexVertexDir.Right, HexVertexDir.TopRight) => HexEdgeDir.TopRight,
                (HexVertexDir.BottomRight, HexVertexDir.Right) => HexEdgeDir.BottomRight,
                (HexVertexDir.BottomLeft, HexVertexDir.BottomRight) => HexEdgeDir.Bottom,
                (HexVertexDir.Left, HexVertexDir.BottomLeft) => HexEdgeDir.BottomLeft,
                (HexVertexDir.TopLeft, HexVertexDir.Left) => HexEdgeDir.TopLeft,
                
                _ => throw new ArgumentException($"Invalid vertex directions: {vertex1}, {vertex2}"), 
            };
        }
        
        public static (HexVertexDir, HexVertexDir) GetVertexDirs(this HexEdgeDir edgeDir)
        {
            return edgeDir switch
            {
                HexEdgeDir.Top => (HexVertexDir.TopLeft, HexVertexDir.TopRight),
                HexEdgeDir.TopRight => (HexVertexDir.TopRight, HexVertexDir.Right),
                HexEdgeDir.BottomRight => (HexVertexDir.Right, HexVertexDir.BottomRight),
                HexEdgeDir.Bottom => (HexVertexDir.BottomRight, HexVertexDir.BottomLeft),
                HexEdgeDir.BottomLeft => (HexVertexDir.BottomLeft, HexVertexDir.Left),
                HexEdgeDir.TopLeft => (HexVertexDir.Left, HexVertexDir.TopLeft),
                _ => throw new ArgumentOutOfRangeException(nameof(edgeDir), edgeDir, null)
            };
        }
        
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