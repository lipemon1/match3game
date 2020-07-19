using Match3Game.Scripts.Behaviours.Slots;

namespace Match3Game.Scripts.Model
{
    [System.Serializable]
    public class FlippedPieces
    {
        public AnimalSlot one;
        public AnimalSlot two;
 
        public FlippedPieces(AnimalSlot o, AnimalSlot t)
        {
            one = o; two = t;
        }
 
        public AnimalSlot GetOtherPiece(AnimalSlot p)
        {
            if (p == one)
                return two;
            else if (p == two)
                return one;
            else
                return null;
        }
    }
}
