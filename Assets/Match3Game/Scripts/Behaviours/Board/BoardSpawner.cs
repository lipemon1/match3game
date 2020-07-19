using System.Linq;
using Match3Game.Scripts.Behaviours.Slots;
using Match3Game.Scripts.Model;
using UnityEngine;

namespace Match3Game.Scripts.Behaviours.Board
{
    public class BoardSpawner : MonoBehaviour
    {
        public static BoardSpawner Instance { get; set; }

        [Header("UI Elements")] public Sprite[] animalsSprites;
        public RectTransform gameBoard;

        [Header("Prefabs")] [SerializeField] private GameObject _animalPrefab;

        [Header("Board Reference")] private Slot[,] _board;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameBoard);
        }

        public void InstantiateBoard(int width, int height, Slot[,] board)
        {
            _board = board;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Slot slot = getNodeAtPoint(new Point(x, y));

                    var val = slot.animal;
                    if (val <= 0) continue;
                    GameObject p = Instantiate(_animalPrefab, gameBoard);
                    AnimalSlot piece = p.GetComponent<AnimalSlot>();
                    RectTransform rect = p.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    piece.Initialize(val, new Point(x, y), animalsSprites[(int) (val - 1)]);
                    slot.SetSlot(piece);
                }
            }
        }

        Slot getNodeAtPoint(Point p)
        {
            return _board[p.x, p.y];
        }
    }
}