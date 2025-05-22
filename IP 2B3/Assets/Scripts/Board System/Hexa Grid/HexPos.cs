using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace B3.BoardSystem
{
    public readonly struct HexPosition
    {
        public int X { get; }
        public int Y { get; }
        
        public HexPosition Top => new(X, Y + 1);
        public HexPosition TopRight => new(X + 1, Y);
        public HexPosition BottomRight => new(X + 1, Y - 1);
        public HexPosition Bottom => new(X, Y - 1);
        public HexPosition BottomLeft => new(X - 1, Y);
        public HexPosition TopLeft => new(X - 1, Y + 1);
    
        public HexPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public IEnumerable<HexPosition> GetNeighbours()
        {
            yield return Top;
            yield return TopRight;
            yield return BottomRight;
            yield return Bottom;
            yield return BottomLeft;
            yield return TopLeft;
        }

        public HexPosition GetNeighbour(HexEdgeDir dir) =>
            dir switch
            {
                HexEdgeDir.Top => Top,
                HexEdgeDir.TopRight => TopRight,
                HexEdgeDir.BottomRight => BottomRight,
                HexEdgeDir.Bottom => Bottom,
                HexEdgeDir.BottomLeft => BottomLeft,
                HexEdgeDir.TopLeft => TopLeft,
                _ => throw new InvalidEnumArgumentException()
            };
    }

}