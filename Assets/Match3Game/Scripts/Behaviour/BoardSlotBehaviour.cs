using Match3Game.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.Behaviour
{
    public class BoardSlotBehaviour : MonoBehaviour
    {
        [SerializeField] private Image _slotImage;
        
        [Header("Run Time")]
        private BoardSlot _boardSlot;

        public void InitializeSlot(Sprite image)
        {
            _slotImage.sprite = image;
        }
    }
}
