using UnityEngine;

namespace Match3Game.Scripts.Model
{
    [System.Serializable]
    public class Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToVector()
        {
            return new Vector2(x, y);
        }

        public bool Equals(Point point)
        {
            return (x == point.x && y == point.y);
        }

        public static Point CloneOf(Point point)
        {
            return new Point(point.x, point.y);
        }

        #region Operations

        public static Point Mult(Point point, int multiplier)
        {
            return new Point(point.x * multiplier, point.y * multiplier);
        }

        public void Sum(Point first)
        {
            x += first.x;
            y += first.y;
        }

        public static Point Sum(Point first, Point second)
        {
            return new Point(first.x + second.x, first.y + second.y);
        }

        #endregion

        #region Directions

        public static Point Zero => new Point(0, 0);

        public static Point Up => new Point(0, 1);

        public static Point Down => new Point(0, -1);

        public static Point Right => new Point(1, 0);

        public static Point Left => new Point(-1, 0);

        #endregion
    }
}