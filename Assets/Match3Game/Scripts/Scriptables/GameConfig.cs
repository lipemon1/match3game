using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "Game Configuration", menuName = "ScriptableObjects/Create Game Configuration",
        order = 1)]
    public class GameConfig : ScriptableObject
    {
        [Header("Board Size")] [SerializeField]
        private int width = 8;

        [SerializeField] private int height = 8;

        [Header("Animals Sprites")] [SerializeField]
        private Sprite[] animalsSprites;

        [Header("Swap Configuration")] [SerializeField]
        private float swapThreshold = 10;

        [Header("Animals Death Parameters")] [SerializeField]
        private float speed = 16f;

        [SerializeField] private float gravity = 32f;
        [SerializeField] private float killedSpread = 1f;

        [Header("Pop SFX")] 
        [SerializeField] private List<AudioClip> popsSfx = new List<AudioClip>();

        [Header("Camera Shake")] 
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeMagnitude;

        #region Properties

        public float Magnitude
        {
            get => shakeMagnitude;
            set => shakeMagnitude = value;
        }

        public float Duration
        {
            get => shakeDuration;
            set => shakeDuration = value;
        }

        public List<AudioClip> PopsSfx => popsSfx;

        public float KilledSpread => killedSpread;

        public float Gravity => gravity;

        public float Speed => speed;

        public float SwapThreshold => swapThreshold;

        public int Height => height;

        public int Width => width;

        public Sprite[] Animals => animalsSprites;

        #endregion
    }
}