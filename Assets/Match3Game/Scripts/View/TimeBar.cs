using System;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.View
{
    public class TimeBar : MonoBehaviour
    {
        [SerializeField] private float maxTime;
        [SerializeField] private Slider timeSlider;
        
        private bool _canRunTime;

        private Action _onTimeover;

        // Update is called once per frame
        private void Update()
        {
            if(_canRunTime)
                RunTime();
        }

        public void StartCanRun(Action onTimeover)
        {
            timeSlider.maxValue = maxTime;
            timeSlider.minValue = 0f;
            timeSlider.value = 0f;
            _canRunTime = true;
            _onTimeover = onTimeover;
        }

        public void StopCanRun()
        {
            _canRunTime = false;
        }
        
        private void RunTime()
        {
            timeSlider.value += Time.deltaTime;
            
            if(timeSlider.value >= timeSlider.maxValue)
                _onTimeover?.Invoke();
        }
    }
}
