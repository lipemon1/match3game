using Match3Game.Scripts.Behaviours.Slots;

namespace Match3Game.Scripts.Model
{
    [System.Serializable]
    public class Slot
    {
        public AnimalType animal;
        public Point index;
        private AnimalSlot _animalSlot;

        public Slot(int animal, Point point)
        {
            this.animal = (AnimalType) animal;
            index = point;
        }

        /// <summary>
        /// Set animal on this slot
        /// </summary>
        /// <param name="animalSlot"></param>
        public void SetSlot(AnimalSlot animalSlot)
        {
            _animalSlot = animalSlot;
            animal = _animalSlot == null ? AnimalType.Empty : _animalSlot.animal;
            if (_animalSlot == null) return;
            _animalSlot.SetIndex(index);
        }

        /// <summary>
        /// Return the animal slot inside this slot
        /// </summary>
        /// <returns></returns>
        public AnimalSlot GetAnimalSlot()
        {
            return _animalSlot;
        }
    }

    public enum AnimalType
    {
        Undefined = -1,
        Empty = 0,
        Cat = 1,
        Dog = 2,
        Pig = 3,
        Panda = 4,
        Frog = 5
    }
}