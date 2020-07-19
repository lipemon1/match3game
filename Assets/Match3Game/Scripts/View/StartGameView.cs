using System;
using Match3Game.Scripts.Loop;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.View
{
    public class StartGameView : MonoBehaviour
    {
        [SerializeField] private Animator viewAnim;
        [SerializeField] private Button playButton;
        
        //value for animator
        private static readonly int Playing = Animator.StringToHash("playing");

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        public void ShowStart()
        {
            viewAnim.SetBool(Playing, false);
        }

        public void HideStart()
        {
            viewAnim.SetBool(Playing, true);
        }

        private void OnPlayButtonClicked()
        {
            GameLoop.Instance.StartGame();
        }
    }
}
