using UnityEngine;

namespace Match3Game.Scripts.Model
{
    [System.Serializable]
    public class BoardPoint
    {
        private int _x;
        private int _y;

        #region Constructor

        public BoardPoint(int x, int y)
        {
            _x = x;
            _y = y;
        }

        #endregion

        #region Properties

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public int X
        {
            get => _x;
            set => _x = value;
        }

        #endregion

        #region Public Methods

        #region Vectors

        /// <summary>
        /// Create a new Board Point using a Vector2
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static BoardPoint FromVector(Vector2 vector2)
        {
            return new BoardPoint((int)vector2.x, (int)vector2.y);
        }

        /// <summary>
        /// Create a new Board Point using a Vector3
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static BoardPoint FromVector(Vector3 vector3)
        {
            return new BoardPoint((int)vector3.x, (int)vector3.y);
        }

        /// <summary>
        /// Returns a Vector2 bases on this BoardPoint
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2()
        {
            return new Vector2(_x, _y);
        }

        #endregion

        #region Operations

        #region Non Static Operations

        /// <summary>
        /// Multiply this board point by an multiplier
        /// </summary>
        /// <param name="multiplier"></param>
        public void Multiply(int multiplier)
        {
            _x *= multiplier;
            _y *= multiplier;
        }

        /// <summary>
        /// Sum value from BoardPoint to this one
        /// </summary>
        /// <param name="boardPoint"></param>
        public void Sum(BoardPoint boardPoint)
        {
            _x += boardPoint._x;
            _y += boardPoint._y;
        }

        #endregion

        #region Statis Operations

        /// <summary>
        /// Return a new Board Point multiplied by an multiplier
        /// </summary>
        /// <param name="boardPoint"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static BoardPoint Mult(BoardPoint boardPoint, int multiplier)
        {
            return new BoardPoint(boardPoint._x * multiplier, boardPoint._y * multiplier);
        }

        /// <summary>
        /// Return a new Board Point with the Sum from First and Second
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static BoardPoint Sum(BoardPoint first, BoardPoint second)
        {
            return new BoardPoint(first._x += second._x, first._y += second._y);
        }

        #endregion

        #endregion

        #region Directions

        public static BoardPoint Zero => new BoardPoint(0,0);
        public static BoardPoint One => new BoardPoint(1,1);
        public static BoardPoint Up => new BoardPoint(0,1);
        public static BoardPoint Down => new BoardPoint(0,-1);
        public static BoardPoint Right => new BoardPoint(1,0);
        public static BoardPoint Left => new BoardPoint(-1,0);

        #endregion

        #region Other Methods

        /// <summary>
        /// Return a new instance(clone) from PointToClone
        /// </summary>
        /// <param name="pointToClone"></param>
        /// <returns></returns>
        public static BoardPoint ClonePoint(BoardPoint pointToClone)
        {
            return new BoardPoint(pointToClone._x, pointToClone._y);
        }

        /// <summary>
        /// Return if BoardPoint is equals to this one
        /// </summary>
        /// <param name="boardPoint"></param>
        /// <returns></returns>
        public bool Equals(BoardPoint boardPoint)
        {
            return _x == boardPoint._x && _y == boardPoint._y;
        }

        #endregion

        #endregion
    }
}
