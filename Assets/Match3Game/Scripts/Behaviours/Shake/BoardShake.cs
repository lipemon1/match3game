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

        public void Shake(float shakeMultiplier = 1f)
        {
            StartCoroutine(ShakeCo(shakeMultiplier));
        }
        
        private IEnumerator ShakeCo(float shakeMultiplier)
        {
            var originalPos = transform.localPosition;
            var elapsed = 0f;
            while (elapsed < gameConfig.Duration)
            {
                var x = Random.Range(-1, 1f) * (gameConfig.Magnitude + shakeMultiplier);
                var y = Random.Range(-1, 1f) * (gameConfig.Magnitude + shakeMultiplier);
                
                transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}