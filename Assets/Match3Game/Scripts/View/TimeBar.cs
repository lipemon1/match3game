using System;
using Match3Game.Scripts.Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.View
{
    public class TimeBar : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private Slider timeSlider;
        
        private bool _canRunTime;

        private Action _onTimeover;

        // Update is called once per frame
        private void Update()
        {
            if(_canRunTime)
                RunTime();
        }

        /// <summary>
        /// Enable the game time to run
        /// </summary>
        /// <param name="onTimeover"></param>
        public void StartCanRun(Action onTimeover)
        {
            timeSlider.maxValue = gameConfig.GameTime;
            timeSlider.minValue = 0f;
            timeSlider.value = 0f;
            _canRunTime = true;
            _onTimeover = onTimeover;
        }

        /// <summary>
        /// Stops the gametime from run
        /// </summary>
        public void StopCanRun()
        {
            _canRunTime = false;
        }
        
        /// <summary>
        /// Increment time to game(progressbar)
        /// </summary>
        private void RunTime()
        {
            timeSlider.value += Time.deltaTime;
            
            if(timeSlider.value >= timeSlider.maxValue)
                _onTimeover?.Invoke();
        }
    }
}
