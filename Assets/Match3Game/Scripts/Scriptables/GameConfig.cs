using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "Game Configuration", menuName = "ScriptableObjects/Create Game Configuration",
        order = 1)]
    public class GameConfig : ScriptableObject
    {
        [Header("Board")] 
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private float boardSpeed = 0.2f;

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
        [SerializeField] private int boardCreationPop = 3;
        [SerializeField] private float animalPopMultiplier = 0.25f;

        [Header("Camera Shake")] 
        [SerializeField] private float shakeDuration = 0.25f;
        [SerializeField] private float shakeMagnitude = 4f;
        [SerializeField] private float animalShakeMultiplier = 0.3f;

        #region Properties

        public float AnimalShakeMultiplier => animalShakeMultiplier;

        public float AnimalPopMultiplier => animalPopMultiplier;

        public int BoardCreationPop => boardCreationPop;

        public float BoardSpeed => boardSpeed;

        public float Magnitude => shakeMagnitude;

        public float Duration => shakeDuration;

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