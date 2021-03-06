﻿using Match3Game.Scripts.Behaviours.Score;
using Match3Game.Scripts.Model;
using Match3Game.Scripts.Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.Behaviours.Slots
{
    public class KilledAnimal : MonoBehaviour
    {
        public bool isFalling;
        private Vector2 _moveDir;

        [Header("References")] [SerializeField]
        private RectTransform rect;

        [SerializeField] private Image img;
        [SerializeField] private GameConfig gameConfig;

        public void Initialize(Sprite animalSprite, Vector2 start, AnimalType animalType)
        {
            isFalling = true;

            _moveDir = Vector2.up;
            _moveDir.x = Random.Range(-gameConfig.KilledSpread, gameConfig.KilledSpread);
            _moveDir *= gameConfig.Speed / 2;

            img.sprite = animalSprite;
            rect.anchoredPosition = start;
            
            //Saving the match on ScoreManager
            ScoreManager.Instance.ReceiveMatch(animalType, 1);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isFalling) return;
            _moveDir.y -= Time.deltaTime * gameConfig.Gravity;
            _moveDir.x = Mathf.Lerp(_moveDir.x, 0, Time.deltaTime);
            rect.anchoredPosition += _moveDir * (Time.deltaTime * gameConfig.Speed);
            if (rect.position.x < -1024f || rect.position.x > Screen.width + 1024f || rect.position.y < -1024f ||
                rect.position.y > Screen.height + 1024f)
                isFalling = false;
        }
    }
}