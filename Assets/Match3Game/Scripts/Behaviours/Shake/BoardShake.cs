using System;
using System.Collections;
using Match3Game.Scripts.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Game.Scripts.Behaviours.Shake
{
    public class BoardShake : MonoBehaviour
    {
        public static BoardShake Instance { get; set; }

        [Header("Game Configuration")] 
        [SerializeField] private GameConfig gameConfig;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        public void Shake()
        {
            StartCoroutine(ShakeCo());
        }
        
        private IEnumerator ShakeCo()
        {
            var originalPos = transform.localPosition;
            var elapsed = 0f;
            while (elapsed < gameConfig.Duration)
            {
                var x = Random.Range(-1, 1f) * gameConfig.Magnitude;
                var y = Random.Range(-1, 1f) * gameConfig.Magnitude;
                
                transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}