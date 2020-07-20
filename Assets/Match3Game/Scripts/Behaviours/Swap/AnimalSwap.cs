using System;
using System.Collections.Generic;
using Match3Game.Scripts.Behaviours.Board;
using Match3Game.Scripts.Behaviours.Slots;
using Match3Game.Scripts.Model;
using Match3Game.Scripts.Scriptables;
using UnityEngine;

namespace Match3Game.Scripts.Behaviours.Swap
{
    public class AnimalSwap : MonoBehaviour
    {
        private enum SwapType
        {
            Drag,
            Click
        }

        [System.Serializable]
        public class AnimalSlotRandomDirection
        {
            public Point animalPoint;
            public List<int> directionAvailable = new List<int>();
            public List<int> allDirectionAvailable = new List<int>();
            public int nextDir;
        }

        public static AnimalSwap instance;

        [Header("References Needed")] [SerializeField]
        private GameConfig gameConfig;

        [SerializeField] private BoardCore gameBoard;

        [Header("Swap Mode")] private SwapType _curSwapMode;

        [Header("Variables for Swap Change")] //this variables will make our swap works
        private AnimalSlot _animalSlotDragMoving;

        private Vector2 _mouseStart;

        [Header("Variables for Click Change")] 
        private AnimalSlot _animalSlotClickMoving;

        [Header("Possible stats")] //those one will handle our click or possible swap
        private AnimalSlot _possibleAnimalSlotMoving;

        private Vector2 _possibleMouseStart;
        private Point _newIndex;
        private bool _isSwaping;

        [Header("Random Direction Variables")] [SerializeField]
        private AnimalSlotRandomDirection curAnimalDirections;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (_possibleAnimalSlotMoving != null)
            {
                var possibleDir = ((Vector2) Input.mousePosition - _possibleMouseStart);
                var dirMagnitude = possibleDir.magnitude;
                if (dirMagnitude > gameConfig.SwapThreshold)
                {
                    EnableSwap(_possibleAnimalSlotMoving, _possibleMouseStart);
                    _possibleAnimalSlotMoving = null;
                    _possibleMouseStart = Vector2.zero;
                    return;
                }
            }

            if (_curSwapMode == SwapType.Drag)
                SwapAnimalByDrag();
        }

        #region Click Events

        /// <summary>
        /// Event called when user click on some animal, can be a drag or click until this moment
        /// </summary>
        /// <param name="animalSlot"></param>
        public void OnAnimalClickDown(AnimalSlot animalSlot)
        {
            _curSwapMode = SwapType.Click;

            UpdateCurAnimalDirections(animalSlot);

            if (_possibleAnimalSlotMoving != null) return;
            _possibleAnimalSlotMoving = animalSlot;
            _possibleMouseStart = Input.mousePosition;
        }

        /// <summary>
        /// Drop our animal for swap, if it was a click swap, it will be called the same way
        /// </summary>
        public void OnAnimalClickUp()
        {
            if (_curSwapMode == SwapType.Drag)
            {
                //checking if we have a animal to move using swap
                if (_animalSlotDragMoving == null) return;

                if (!_newIndex.Equals(_animalSlotDragMoving.index))
                    gameBoard.FlipAnimal(_animalSlotDragMoving.index, _newIndex, true);
                else
                    gameBoard.ResetAnimalSlot(_animalSlotDragMoving);
                _animalSlotDragMoving = null;
            }
            else
            {
                //checking if we have a animal to move using click
                if (_animalSlotClickMoving == null) return;

                if (!_newIndex.Equals(_animalSlotClickMoving.index))
                    gameBoard.FlipAnimal(_animalSlotClickMoving.index, _newIndex, true);
                else
                    gameBoard.ResetAnimalSlot(_animalSlotClickMoving);
                _animalSlotClickMoving = null;
            }
        }

        /// <summary>
        /// Called when we do a full click, so it is called also when we do a OnPointerUp event
        /// </summary>
        /// <param name="animalSlot"></param>
        public void OnAnimalClick(AnimalSlot animalSlot)
        {
            //Cleaning possible values because we want to clear our mouse
            if (_possibleAnimalSlotMoving != null)
                _possibleAnimalSlotMoving = null;
            _possibleMouseStart = Vector2.zero;

            if (_possibleAnimalSlotMoving == null && _animalSlotDragMoving == null)
            {
                _animalSlotClickMoving = animalSlot;
                _curSwapMode = SwapType.Click;
                SwapAnimalByClick();
            }
        }

        #endregion

        #region Swaps Methods

        /// <summary>
        /// Called to afirm that we are dealing with a drag swap
        /// </summary>
        /// <param name="animalSlot"></param>
        /// <param name="startedPosition"></param>
        private void EnableSwap(AnimalSlot animalSlot, Vector2 startedPosition)
        {
            _curSwapMode = SwapType.Drag;

            if (_animalSlotDragMoving != null) return;
            _animalSlotDragMoving = animalSlot;
            _mouseStart = startedPosition;
        }

        private void SwapAnimalByClick()
        {
            var dir = GetDirectionToSwap(_curSwapMode);
            var directionMagnitude = (gameConfig.SwapThreshold * gameConfig.SwapThreshold);

            var add = Point.Zero;
            _newIndex = GetPointToSwapWith(dir, directionMagnitude, out add);

            OnAnimalClickUp();
        }

        private void SwapAnimalByDrag()
        {
            if (_animalSlotDragMoving == null) return;

            var dir = GetDirectionToSwap(_curSwapMode);
            var directionMagnitude = dir.magnitude;

            var add = Point.Zero;
            _newIndex = GetPointToSwapWith(dir, directionMagnitude, out add);

            _animalSlotDragMoving.MovePositionTo(GetPositionToGo(add));
        }

        #endregion

        #region Helpers For Swap

        /// <summary>
        /// Return the Vector2 position our animal has to go right now
        /// </summary>
        /// <param name="referencePoint"></param>
        /// <returns></returns>
        private Vector2 GetPositionToGo(Point referencePoint)
        {
            var animalSlot = _curSwapMode == SwapType.Click ? _animalSlotClickMoving : _animalSlotDragMoving;

            var pos = BoardCore.GetPositionFromPoint(animalSlot.index);
            if (!_newIndex.Equals(animalSlot.index))
                pos += Point.Mult(new Point(referencePoint.x, -referencePoint.y), 16).ToVector();

            return pos;
        }

        /// <summary>
        /// Return the vector direction that will be used for swap animals
        /// </summary>
        /// <param name="swapType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Vector2 GetDirectionToSwap(SwapType swapType)
        {
            switch (swapType)
            {
                case SwapType.Drag:
                    return ((Vector2) Input.mousePosition - _mouseStart);
                case SwapType.Click:
                    return GetRandomDirection();
                default:
                    throw new ArgumentOutOfRangeException(nameof(swapType), swapType, null);
            }
        }

        /// <summary>
        /// Return the point that we want to swap right now
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="directionMagnitude"></param>
        /// <param name="addPoint"></param>
        /// <returns></returns>
        private Point GetPointToSwapWith(Vector2 dir, float directionMagnitude, out Point addPoint)
        {
            var nDir = dir.normalized;
            var aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            var pointToSwapWith = Point.CloneOf(_curSwapMode == SwapType.Drag
                ? _animalSlotDragMoving.index
                : _animalSlotClickMoving.index);
            addPoint = Point.Zero;

            if (directionMagnitude > 32)
            {
                if (aDir.x > aDir.y)
                    addPoint = (new Point((nDir.x > 0) ? 1 : -1, 0));
                else if (aDir.y > aDir.x)
                    addPoint = (new Point(0, (nDir.y > 0) ? -1 : 1));
            }

            pointToSwapWith.Sum(addPoint);

            return pointToSwapWith;
        }

        #endregion

        #region Click Direction

        /// <summary>
        /// Return the next direction used by the mouse
        /// </summary>
        /// <returns></returns>
        private Vector2 GetRandomDirection()
        {
            var randomDir = curAnimalDirections.nextDir;
            curAnimalDirections.nextDir--;

            if (curAnimalDirections.nextDir == -1)
                ResetDirectionsToUse();

            switch (randomDir)
            {
                case 0:
                    return Vector2.left;
                case 1:
                    return Vector2.up;
                case 2:
                    return Vector2.right;
                case 3:
                    return Vector2.down;
                default:
                    return Vector2.right;
            }
        }

        /// <summary>
        /// Update the curAnimalDirection everytime its changes
        /// </summary>
        /// <param name="animalSlot"></param>
        private void UpdateCurAnimalDirections(AnimalSlot animalSlot)
        {
            if (curAnimalDirections.animalPoint.Equals(animalSlot.index)) return;

            curAnimalDirections.animalPoint = animalSlot.index;
            ResetDirectionsToUse();
        }

        /// <summary>
        /// Reset the directions available to use
        /// </summary>
        private void ResetDirectionsToUse()
        {
            curAnimalDirections.nextDir = 3;

            curAnimalDirections.directionAvailable.Clear();
            curAnimalDirections.directionAvailable = curAnimalDirections.allDirectionAvailable;

            // if(curAnimalDirections.animalPoint.x != 0 && curAnimalDirections.animalPoint.y != (gameConfig.Width - 1))
            // {
            //     curAnimalDirections.directionAvailable = curAnimalDirections.allDirectionAvailable;
            // }
        }

        #endregion
    }
}