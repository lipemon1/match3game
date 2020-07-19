using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Match3Game.Scripts;
using Match3Game.Scripts.Behaviours;
using Match3Game.Scripts.Behaviours.Audio;
using Match3Game.Scripts.Behaviours.Slots;
using Match3Game.Scripts.Model;
using Match3Game.Scripts.Scriptables;
using UnityEngine;

public class BoardCore : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("Game Configuration")] 
    [SerializeField] private GameConfig gameConfig;

    [Header("UI Elements")]
    public RectTransform gameBoard;
    public RectTransform killedBoard;

    [Header("Prefabs")]
    public GameObject animalPrefab;
    public GameObject animalKilledPrefab;

    private int[] _fills;
    private Slot[,] _board;

    private List<AnimalSlot> _update;
    private List<FlippedPieces> _flipped;
    private List<AnimalSlot> _dead;
    private List<KilledAnimal> _killed;

    private System.Random _random;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        var finishedUpdating = _update.Where(animalSlot => !animalSlot.UpdatePiece()).ToList();

        foreach (var aSlot in finishedUpdating)
        {
            var flip = GetFlipped(aSlot);
            AnimalSlot flippedPiece = null;

            var x = aSlot.index.x;
            _fills[x] = Mathf.Clamp(_fills[x] - 1, 0, gameConfig.Width);

            var connected = ConnectedPoints(aSlot.index, true);
            var wasFlipped = (flip != null);

            //checking if the slot was flipped
            if (wasFlipped)
            {
                flippedPiece = flip.GetOtherPiece(aSlot);
                AddMatchedPoints(ref connected, ConnectedPoints(flippedPiece.index, true));
            }

            //check if we have no connecteds, no matches at all
            if (connected.Count == 0)
            {
                //if it was flipped, flip back then
                if (wasFlipped)
                    FlipPieces(aSlot.index, flippedPiece.index, false);
            }
            else //If we have some match
            {
                SfxPlayer.Instance.PlayPop();
                
                //remove every animal slot connected on that match
                foreach (var pnt in connected)
                {
                    KillPiece(pnt);
                    
                    var slot = GetSlotAtPoint(pnt);
                    var animalSlot = slot.GetAnimalSlot();
                    if (animalSlot != null)
                    {
                        animalSlot.gameObject.SetActive(false);
                        _dead.Add(animalSlot);
                    }
                    slot.SetSlot(null);
                }

                ApplyGravityToBoard();
            }

            //Removing flip
            _flipped.Remove(flip);
            _update.Remove(aSlot);
        }
    }

    /// <summary>
    /// Faking a gravity system
    /// </summary>
    private void ApplyGravityToBoard()
    {
        for (var x = 0; x < gameConfig.Width; x++)
        {
            for (var y = (gameConfig.Height - 1); y >= 0; y--) //Start at the bottom and grab the next
            {
                var point = new Point(x, y);
                var slot = GetSlotAtPoint(point);
                var animal = GetAnimalAtPoint(point);
                if (animal != 0) continue;
                for (var ny = (y - 1); ny >= -1; ny--)
                {
                    var nextPoint = new Point(x, ny);
                    var nextAnimal = GetAnimalAtPoint(nextPoint);
                    if (nextAnimal == 0)
                        continue;
                    if (nextAnimal != AnimalType.Undefined)
                    {
                        var gotten = GetSlotAtPoint(nextPoint);
                        var animalSlot = gotten.GetAnimalSlot();

                        
                        slot.SetSlot(animalSlot);
                        _update.Add(animalSlot);

                        
                        gotten.SetSlot(null);
                    }
                    else
                    {
                        var newVal = GenerateAnimal();
                        AnimalSlot aSlot;
                        var fallPnt = new Point(x, (-1 - _fills[x]));
                        if(_dead.Count > 0)
                        {
                            var revivedAnimal = _dead[0];
                            revivedAnimal.gameObject.SetActive(true);
                            aSlot = revivedAnimal;

                            _dead.RemoveAt(0);
                        }
                        else
                        {
                            var obj = Instantiate(animalPrefab, gameBoard);
                            var animalSlot = obj.GetComponent<AnimalSlot>();
                            aSlot = animalSlot;
                        }

                        aSlot.Initialize(newVal, point, gameConfig.Animals[(int) (newVal - 1)]);
                        aSlot.rect.anchoredPosition = GetPositionFromPoint(fallPnt);

                        var hole = GetSlotAtPoint(point);
                        hole.SetSlot(aSlot);
                        ResetPiece(aSlot);
                        _fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    private FlippedPieces GetFlipped(AnimalSlot animalSlot)
    {
        return _flipped.FirstOrDefault(t => t.GetOtherPiece(animalSlot) != null);
    }

    /// <summary>
    /// Initialize Game Core
    /// </summary>
    private void Init()
    {
        _fills = new int[gameConfig.Width];
        var seed = Seed.GenerateSeed();
        _random = new System.Random(seed.GetHashCode());
        _update = new List<AnimalSlot>();
        _flipped = new List<FlippedPieces>();
        _dead = new List<AnimalSlot>();
        _killed = new List<KilledAnimal>();

        CreateBoard();
        AvoidInitialMatches();
        InstantiateBoard();
    }

    /// <summary>
    /// Initialize BOARD with every slot
    /// </summary>
    private void CreateBoard()
    {
        _board = new Slot[gameConfig.Width, gameConfig.Height];
        for(var y = 0; y < gameConfig.Height; y++)
        {
            for(var x = 0; x < gameConfig.Width; x++)
            {
                _board[x, y] = new Slot((boardLayout.rows[y].row[x]) ? - 1 : (int) GenerateAnimal(), new Point(x, y));
            }
        }
    }

    /// <summary>
    /// This method avoid our board to start with matches
    /// </summary>
    private void AvoidInitialMatches()
    {
        List<AnimalType> remove;
        for (var x = 0; x < gameConfig.Width; x++)
        {
            for (var y = 0; y < gameConfig.Height; y++)
            {
                var point = new Point(x, y);
                var animal = GetAnimalAtPoint(point);
                if (animal <= 0) continue;

                remove = new List<AnimalType>();
                while (ConnectedPoints(point, true).Count > 0)
                {
                    animal = GetAnimalAtPoint(point);
                    if (!remove.Contains(animal))
                        remove.Add(animal);
                    SetAnimalAtPoint(point, NewValue(ref remove));
                }
            }
        }
    }

    /// <summary>
    /// Creates the board in UI game
    /// </summary>
    private void InstantiateBoard()
    {
        for (var x = 0; x < gameConfig.Width; x++)
        {
            for (var y = 0; y < gameConfig.Height; y++)
            {
                var slot = GetSlotAtPoint(new Point(x, y));

                //Checking if we have a animal
                var animal = slot.animal;
                if (animal <= 0) continue;
                
                var slotGameObject = Instantiate(animalPrefab, gameBoard);
                var animalSlot = slotGameObject.GetComponent<AnimalSlot>();
                
                var rectTransform = slotGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                
                animalSlot.Initialize(animal, new Point(x, y), gameConfig.Animals[(int) (animal - 1)]);
                slot.SetSlot(animalSlot);
            }
        }
    }
     
    public void ResetPiece(AnimalSlot piece)
    {
        piece.ResetPosition();
        _update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (GetAnimalAtPoint(one) < 0) return;

        var slotOne = GetSlotAtPoint(one);
        var animalOne = slotOne.GetAnimalSlot();
        if (GetAnimalAtPoint(two) > 0)
        {
            var slotTwo = GetSlotAtPoint(two);
            var pieceTwo = slotTwo.GetAnimalSlot();
            slotOne.SetSlot(pieceTwo);
            slotTwo.SetSlot(animalOne);

            if(main)
                _flipped.Add(new FlippedPieces(animalOne, pieceTwo));

            _update.Add(animalOne);
            _update.Add(pieceTwo);
        }
        else
            ResetPiece(animalOne);
    }

    private void KillPiece(Point p)
    {
        var available = new List<KilledAnimal>();
        foreach (var t in _killed)
            if (!t.isFalling) available.Add(t);

        KilledAnimal set = null;
        if (available.Count > 0)
            set = available[0];
        else
        {
            var kill = GameObject.Instantiate(animalKilledPrefab, killedBoard);
            var kPiece = kill.GetComponent<KilledAnimal>();
            set = kPiece;
            _killed.Add(kPiece);
        }

        var animal = (int)(GetAnimalAtPoint(p) - 1);
        if (set != null && animal >= 0 && animal < gameConfig.Animals.Length)
            set.Initialize(gameConfig.Animals[animal], GetPositionFromPoint(p));
    }

    #region Matches

        /// <summary>
    /// Return a list of matched points relative to 'point'
    /// </summary>
    /// <param name="point"></param>
    /// <param name="main"></param>
    /// <returns></returns>
    private List<Point> ConnectedPoints(Point point, bool main)
    {
        var connected = new List<Point>();
        var animal = GetAnimalAtPoint(point);
        Point[] directions =
        {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left
        };
        
        //First check to see if we have 2 or more animals on every direction (horizontal and vertical)
        foreach(var dir in directions)
        {
            var line = new List<Point>();

            var same = 0;
            for(var i = 1; i < 3; i++)
            {
                var check = Point.Sum(point, Point.Mult(dir, i));
                if (GetAnimalAtPoint(check) != animal) continue;
                
                line.Add(check);
                same++;
            }

            //when we have more than 1 animal repeated in the same direction then it's a match
            if (same > 1) 
                AddMatchedPoints(ref connected, line);
        }

        //second check in case we are in the middle of two animal similar to us
        for(var i = 0; i < 2; i++) 
        {
            var line = new List<Point>();

            var same = 0;
            Point[] check = { Point.Sum(point, directions[i]), Point.Sum(point, directions[i + 2]) }; // i + 2 to check both sides
            foreach (var next in check) 
            {
                if (GetAnimalAtPoint(next) != animal) continue;
                line.Add(next);
                same++;
            }

            if (same > 1)
                AddMatchedPoints(ref connected, line);
        }

        
        //third check we gonna look for boxes shapes like 2x2
        for(var i = 0; i < 4; i++)
                 {
            var square = new List<Point>();

            var same = 0;
            var next = i + 1;
            if (next >= 4)
                next -= 4;

            Point[] check = { Point.Sum(point, directions[i]), Point.Sum(point, directions[next]), Point.Sum(point, Point.Sum(directions[i], directions[next])) };
            foreach (var pnt in check)
            {
                if (GetAnimalAtPoint(pnt) != animal) continue;
                square.Add(pnt);
                same++;
            }

            if (same > 2)
                AddMatchedPoints(ref connected, square);
        }

        if (!main) return connected;
        {
            for (var i = 0; i < connected.Count; i++)
                AddMatchedPoints(ref connected, ConnectedPoints(connected[i], false));
        }

        return connected;
    }

    /// <summary>
    /// Add points to a reference list of points to save our current match
    /// </summary>
    /// <param name="points"></param>
    /// <param name="add"></param>
    private static void AddMatchedPoints(ref List<Point> points, IEnumerable<Point> add)
    {
        foreach(var p in add)
        {
            var doAdd = true;
            var i = 0;
            for(; i < points.Count; i++)
            {
                if (!points[i].Equals(p)) continue;
                doAdd = false;
                break;
            }

            if (doAdd) points.Add(p);
        }
    }

    #endregion

    #region Slots and Animals

    /// <summary>
    /// Return a random animal
    /// </summary>
    /// <returns></returns>
    private AnimalType GenerateAnimal()
    {
        var animal = AnimalType.Cat;
        animal = (AnimalType) ((_random.Next(0, 100) / (100 / gameConfig.Animals.Length)) + 1);
        return animal;
    }

    /// <summary>
    /// Return the current animal
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private AnimalType GetAnimalAtPoint(Point point)
    {
        if (point.x < 0 || point.x >= gameConfig.Width || point.y < 0 || point.y >= gameConfig.Height) return AnimalType.Undefined;
        return _board[point.x, point.y].animal;
    }

    /// <summary>
    /// Set a animal inside a point on the board
    /// </summary>
    /// <param name="point"></param>
    /// <param name="animal"></param>
    private void SetAnimalAtPoint(Point point, AnimalType animal)
    {
        _board[point.x, point.y].animal = animal;
    }

    /// <summary>
    /// Return a slot on some board position
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private Slot GetSlotAtPoint(Point point)
    {
        return _board[point.x, point.y];
    }
    
    /// <summary>
    /// Calculate new Animal value
    /// </summary>
    /// <param name="remove"></param>
    /// <returns></returns>
    private AnimalType NewValue(ref List<AnimalType> remove)
    {
        var available = new List<AnimalType>();
        for (var i = 0; i < gameConfig.Animals.Length; i++)
            available.Add((AnimalType) (i + 1));
        foreach (int i in remove)
            available.Remove((AnimalType) i);

        return available.Count <= 0 ? AnimalType.Empty : available[_random.Next(0, available.Count)];
    }

    #endregion

    public static Vector2 GetPositionFromPoint(Point point)
    {
        return new Vector2(32 + (64 * point.x), -32 - (64 * point.y));
    }
}
