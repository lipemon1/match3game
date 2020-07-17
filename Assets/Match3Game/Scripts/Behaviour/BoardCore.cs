using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Match3Game.Scripts.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Game.Scripts.Behaviour
{
    public class BoardCore : MonoBehaviour
    {
        [Header("Board Configuration")] 
        [SerializeField] private ArrayLayout boardLayout;
        [SerializeField] private BoardPiece[] boardPieces;
        private const int Width = 9;
        private const int Height = 14;
        private BoardSlot[,] _boardSlots;
        private static readonly int[] Seeds = {0,1,2,3,4};
        private System.Random _random;

        public RectTransform boardHolder;
        public GameObject animalPrefab;

        // Start is called before the first frame update
        private void Start()
        {
            PrepareSeed();
        }

        /// <summary>
        /// Initialize _board using const values
        /// </summary>
        private void PrepareSeed()
        {
            var seed = GetSeed();
            _random = new System.Random(seed.GetHashCode());

            CreateBoard();
//            AvoidInitialMatches();
            ShowBoard();
        }

        /// <summary>
        /// Creates the game board
        /// </summary>
        private void CreateBoard()
        {
            _boardSlots = new BoardSlot[Width,Height];

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    _boardSlots[x, y] = new BoardSlot(GetSlotOption(y, x), new BoardPoint(x,y));
                }
            }
        }

        /// <summary>
        /// Avoid our board start with already matches made
        /// </summary>
        private void AvoidInitialMatches()
        {
            var removeList = new List<BoardSlotOption>();
            
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    var boardPoint = new BoardPoint(i, j);
                    var curAnimal = GetBoardPointAnimal(boardPoint);

                    removeList = new List<BoardSlotOption>();
                    while (Connected(boardPoint, true).Count > 0)
                    {
                        curAnimal = GetBoardPointAnimal(boardPoint);
                        if(!removeList.Contains(curAnimal))
                            removeList.Add(curAnimal);
                        SetBoardPointAnimal(boardPoint, NewValue(ref removeList));
                    }
                }
            }
        }

        private void ShowBoard()
        {
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    var curAnimal = _boardSlots[i, j].curValue;
                    
                    if(curAnimal == BoardSlotOption.Undefined || curAnimal == BoardSlotOption.Empty) continue;

                    var boardPointCreated = Instantiate(animalPrefab, boardHolder);
                    var rect = boardPointCreated.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(32 + (64 * i), -32 - (64 * j));
                }
            }
        }

        /// <summary>
        /// Here the magics happens, all the verification to see who is connectec to who
        /// </summary>
        /// <param name="boardPoint"></param>
        /// <param name="main"></param>
        /// <returns></returns>
        private List<BoardPoint> Connected(BoardPoint boardPoint, bool main)
        {
            var connected = new List<BoardPoint>();
            var curAnimal = GetBoardPointAnimal(boardPoint);
            BoardPoint[] directions =
            {
                BoardPoint.Up,
                BoardPoint.Right,
                BoardPoint.Down,
                BoardPoint.Left
            };

            //Checking if there is 2 or more animals on thoses directions
            foreach (var dir in directions)
            {
                var line = new List<BoardPoint>();

                var same = 0;
                for (var i = 1; i < 3; i++)
                {
                    var check = BoardPoint.Sum(boardPoint, BoardPoint.Mult(dir, i));
                    if (GetBoardPointAnimal(check) == curAnimal)
                    {
                        line.Add(check);
                        same++;
                    }
                }

                //if there are more than 1 animal of the same time in this direction, then we have a match
                if (same > 1)
                    AddBoardPoints(ref connected, line); //adding this board points to connected list
            }

            //Checking if we are in the middle of two similar animals
            for (var i = 0; i < 2; i++)
            {
                var line = new List<BoardPoint>();

                var same = 0;
                BoardPoint[] check =
                {
                    BoardPoint.Sum(boardPoint, directions[i]), 
                    BoardPoint.Sum(boardPoint, directions[i + 2])
                };
                
                //Here checking if both sides of the slot are the same, if so, add then to the list
                foreach (var point in check)
                {
                    if (GetBoardPointAnimal(point) == curAnimal)
                    {
                        line.Add(point);
                        same++;
                    }
                }

                if (same > 1)
                    AddBoardPoints(ref connected, line);
            }

            //Check for square 2x2
            for (var i = 0; i < 4; i++)
            {
                var square = new List<BoardPoint>();

                var same = 0;
                var next = i + 1;
                if (next >= 4)
                    next -= 4;

                BoardPoint[] check =
                {
                    BoardPoint.Sum(boardPoint, directions[i]), BoardPoint.Sum(boardPoint, directions[next]),
                    BoardPoint.Sum(boardPoint, BoardPoint.Sum(directions[i], directions[next]))
                };

                //Checking here all sides of that slot, if they have the same animal, add all to list
                foreach (var point in check)
                {
                    if (GetBoardPointAnimal(point) == curAnimal)
                    {
                        square.Add(point);
                        same++;
                    }
                }
                
                if(same > 2)
                    AddBoardPoints(ref connected, square);
            }

            //Checking for other matches after the current match
            if (main)
                for (var i = 0; i < connected.Count; i++)
                    AddBoardPoints(ref connected, Connected(connected[i], false));
            
            if(connected.Count > 0)
                connected.Add(boardPoint);

            return connected;
        }

        private void AddBoardPoints(ref List<BoardPoint> boardPoints, List<BoardPoint> add)
        {
            foreach (var boardPoint in add)
            {
                var doAdd = true;
                for (var i = 0; i < boardPoints.Count; i++)
                {
                    if (boardPoints[i].Equals(boardPoint))
                    {
                        doAdd = false;
                        break;
                    }
                }
                
                if(doAdd)
                    boardPoints.Add(boardPoint);
            }
        }

        /// <summary>
        /// Return the current value(Animal) over some slot on board
        /// </summary>
        /// <param name="boardPoint"></param>
        /// <returns></returns>
        private BoardSlotOption GetBoardPointAnimal(BoardPoint boardPoint)
        {
            if (boardPoint.X < 0 || boardPoint.X >= Width || boardPoint.Y < 0 || boardPoint.Y >= Height)
                return BoardSlotOption.Undefined;
            
            return _boardSlots[boardPoint.X, boardPoint.Y].curValue;
        }

        /// <summary>
        /// Set a new animal to the current board slot passed by boardPoint
        /// </summary>
        /// <param name="boardPoint"></param>
        /// <param name="animal"></param>
        private void SetBoardPointAnimal(BoardPoint boardPoint, BoardSlotOption animal)
        {
            _boardSlots[boardPoint.X, boardPoint.Y].curValue = animal;
        }

        private BoardSlotOption NewValue(ref List<BoardSlotOption> remove)
        {
            var available = new List<BoardSlotOption>();
            for (var i = 0; i < boardPieces.Length; i++)
                available.Add((BoardSlotOption)i + 1);

            foreach (var slotOption in available)
                available.Remove(slotOption);

            if (available.Count <= 0) return (BoardSlotOption)0;
            return available[_random.Next(0, available.Count)];
        }

        /// <summary>
        /// Return the option choosed randomic for that slot or undefined if layout is -1
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private BoardSlotOption GetSlotOption(int y, int x)
        {
            return boardLayout.rows[y].row[x] ? BoardSlotOption.Undefined : (BoardSlotOption) (Random.Range(0,
                boardPieces.Length - 1));
        }

        /// <summary>
        /// Returns a random seed based on seeds const array
        /// </summary>
        /// <returns></returns>
        private static string GetSeed()
        {
            return Random.Range(0, Seeds.Length).ToString();
        }
    }
}
