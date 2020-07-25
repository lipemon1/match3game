using System;
using Match3Game.Scripts.Model;
using UnityEngine;

namespace Match3Game.Scripts.Behaviours.Score
{
    public class ScoreManager : MonoBehaviour
    {
        [System.Serializable]
        public class ScorePayload
        {
            public ScoreResult catResult;
            public ScoreResult dogResult;
            public ScoreResult frogResult;
            public ScoreResult pandaResult;
            public ScoreResult pigResult;
        }
        
        public static ScoreManager Instance { get; private set; }

        [SerializeField] private ScorePayload scorePayload;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Set all animals score to zero again
        /// </summary>
        public void ResetScore()
        {
            scorePayload.catResult.amount = 0;
            scorePayload.dogResult.amount = 0;
            scorePayload.frogResult.amount = 0;
            scorePayload.pandaResult.amount = 0;
            scorePayload.pigResult.amount = 0;
        }

        /// <summary>
        /// Receive a amount of animals saved
        /// </summary>
        /// <param name="targetAnimal"></param>
        /// <param name="amount"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ReceiveMatch(AnimalType targetAnimal, int amount)
        {
            switch (targetAnimal)
            {
                case AnimalType.Cat:
                    scorePayload.catResult.amount += amount;
                    break;
                case AnimalType.Dog:
                    scorePayload.dogResult.amount += amount;
                    break;
                case AnimalType.Pig:
                    scorePayload.pigResult.amount += amount;
                    break;
                case AnimalType.Panda:
                    scorePayload.pandaResult.amount += amount;
                    break;
                case AnimalType.Frog:
                    scorePayload.frogResult.amount += amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetAnimal), targetAnimal, null);
            }
        }

        /// <summary>
        /// Returns the current Score Payload Data
        /// </summary>
        /// <returns></returns>
        public ScorePayload GetScorePayload()
        {
            return scorePayload;
        }
    }
}
