using UnityEngine;

namespace Match3Game.Scripts.Model
{
    [System.Serializable]
    public class BoardSlot
    {

        public BoardSlotOption curValue;
        public BoardPoint point;

        public BoardSlot(BoardSlotOption curValue, BoardPoint point)
        {
            this.curValue = curValue;
            this.point = point;
        }
    }
    
    public enum BoardSlotOption
    {
        Undefined = -1,
        Empty,
        Cat,
        Dog,
        Duck,
        Pig,
        Panda
    }
}
