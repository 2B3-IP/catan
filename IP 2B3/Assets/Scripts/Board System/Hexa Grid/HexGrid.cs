using UnityEngine;

namespace B3.BoardSystem
{
    public class HexGrid<T> where T : class
    {
        private readonly T[,] _matrix;
        private readonly int _width;
        private readonly int _height;
        
        private readonly float _distanceFromCenterToCorner;

        private int CenterX => _width / 2;
        private int CenterY => _height / 2;

        public HexGrid(int width, int height, float distanceFromCenterToCorner = 4f)
        {
            _width = width * 2 + 1;
            _height = height * 2 + 1;
            
            _matrix = new T[_width, _height];

            _distanceFromCenterToCorner = distanceFromCenterToCorner;
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
            float x = _distanceFromCenterToCorner * Mathf.Sqrt(3) * (position.X + position.Y / 2f);
            float y = _distanceFromCenterToCorner * 1.5f * position.Y;
            return new Vector2(x, -y);
        }
    }
}