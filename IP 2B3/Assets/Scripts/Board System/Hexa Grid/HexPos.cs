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
    };

}