using System.Collections.Generic;
using UnityEngine;

namespace B3.BoardSystem
{
    public class HexGrid<T> where T : class
    {
        private readonly T[,] _matrix;
        private readonly int _width;
        private readonly int _height;

        public float DistanceFromCenter { get;private set; }

        private int CenterX => _width / 2;
        private int CenterY => _height / 2;

        public HexGrid(int width, int height, float distanceFromCenter = 4f)
        {
            _width = width * 2 + 1;
            _height = height * 2 + 1;
            
            _matrix = new T[_width, _height];

            DistanceFromCenter = distanceFromCenter;
        }

        public T this[HexPosition position] 
        {
            get
            {
                int x = position.X + CenterX;
                int y = position.Y - CenterY;
                
                if (x < 0 || y < 0 || x >= _width || y >= _height) 
                    return null;
                
                return _matrix[x, y];
            }
            
            set
            {
                int x = position.X + CenterX;
                int y = position.Y - CenterY;
                
                if (x < 0 || y < 0 || x >= _width || y >= _height) 
                    return;

                _matrix[x, y] = value;
            }
        }
        
        public Vector2 ToWorldPosition(HexPosition position)
        {
            float x = DistanceFromCenter * Mathf.Sqrt(3) * (position.X + position.Y / 2f);
            float y = DistanceFromCenter * 1.5f * position.Y;
            return new Vector2(x, -y);
        }
        
        public HexPosition FromWorldPosition(Vector2 worldPosition)
        {
            float q = (Mathf.Sqrt(3f) / 3f * worldPosition.x - 1f / 3f * worldPosition.y) / DistanceFromCenter;
            float r = (2f / 3f * worldPosition.y) / DistanceFromCenter;

            // Convert to axial coordinates
            float x = q;
            float y = -q - r;
            float z = r;

            // Rounding to nearest hex
            int rx = Mathf.RoundToInt(x);
            int ry = Mathf.RoundToInt(y);
            int rz = Mathf.RoundToInt(z);

            float xDiff = Mathf.Abs(rx - x);
            float yDiff = Mathf.Abs(ry - y);
            float zDiff = Mathf.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            // Convert axial to offset coordinates
            int hexX = rx;
            int hexY = rz;

            return new HexPosition(hexX, hexY);
        }
        //metoda care returneaza cele 6 pozitii vecine ale hexagonului
        public List<HexPosition> GetNeighbors(HexPosition position)
        {
            return new List<HexPosition>
            {
                position.Top,
                position.TopRight,
                position.BottomRight,
                position.Bottom,
                position.BottomLeft,
                position.TopLeft
            };
        }
        //metoda care iti returneaza continutul vecinilor
        public List<T> GetNeighborValues(HexPosition position)
        {
            List<T> neighbors = new();
            foreach (var neighborPos in GetNeighbors(position))
            {
                var value = this[neighborPos];
                if (value != null)
                    neighbors.Add(value);
            }
            return neighbors;
        }
        
        
    }
}