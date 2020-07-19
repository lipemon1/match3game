using Match3Game.Scripts.Behaviours.Swap;
using Match3Game.Scripts.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3Game.Scripts.Behaviours.Slots
{
    public class AnimalSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public AnimalType animal;
        public Point index;

        [HideInInspector] public Vector2 pos;
        [SerializeField] public RectTransform rect;
        [SerializeField] private Image img;

        bool updating; //if we are updating this animal slot

        public void Initialize(AnimalType animal, Point point, Sprite animalSprite)
        {
            this.animal = animal;
            SetIndex(point);
            img.sprite = animalSprite;
        }

        public void SetIndex(Point point)
        {
            index = point;
            ResetPosition();
            UpdateGameObjectName();
        }

        public void ResetPosition()
        {
            pos = new Vector2(32 + (64 * index.x), -32 - (64 * index.y));
        }

        public void MovePositionTo(Vector2 move)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
        }

        public bool UpdatePiece()
        {
            if (Vector3.Distance(rect.anchoredPosition, pos) > 1)
            {
                MovePositionTo(pos);
                updating = true;
                return true;
            }
            else
            {
                rect.anchoredPosition = pos;
                updating = false;
                return false;
            }
        }

        /// <summary>
        /// Change the object name for better visualization on editor
        /// </summary>
        private void UpdateGameObjectName()
        {
            transform.name = "Animal Slot [" + index.x + ", " + index.y + "]";
        }

        #region Pointer Events

        public void OnPointerDown(PointerEventData eventData)
        {
            if (updating) return;
            AnimalSwap.instance.OnAnimalClickDown(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AnimalSwap.instance.OnAnimalClickUp();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AnimalSwap.instance.OnAnimalClick(this);
        }

        #endregion
    }
}