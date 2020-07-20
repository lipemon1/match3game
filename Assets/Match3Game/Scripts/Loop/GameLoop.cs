using Match3Game.Scripts.Behaviours.Board;
using Match3Game.Scripts.Behaviours.Score;
using Match3Game.Scripts.View;
using UnityEngine;

namespace Match3Game.Scripts.Loop
{
    public class GameLoop : MonoBehaviour
    {
        public static GameLoop Instance { get; private set; }

        [SerializeField] private BoardCore boardCore;
        [SerializeField] private TimeBar timeBar;
        [SerializeField] private GameObject avoidInteractionPanel;
        [SerializeField] private StartGameView startGameView;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private ScoreResultView scoreResultView;
        [SerializeField] private GameObject resultPanel;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
            
            CheckObjects();
            resultPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// This method is just to get more life easier on forgetting some object opened
        /// </summary>
        private void CheckObjects()
        {
            startPanel.gameObject.SetActive(true);
            timeBar.gameObject.SetActive(false);
            avoidInteractionPanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called everytime the game start
        /// </summary>
        public void StartGame()
        {
            boardCore.Init();
            timeBar.gameObject.SetActive(true);
            timeBar.StartCanRun(OnGameOver);
            avoidInteractionPanel.gameObject.SetActive(false);
            ScoreManager.Instance.ResetScore();
            startGameView.HideStart();
        }

        /// <summary>
        /// Called everytime the game ends
        /// </summary>
        private void OnGameOver()
        {
            boardCore.StopBoard();
            timeBar.StopCanRun();
            timeBar.gameObject.SetActive(false);
            avoidInteractionPanel.gameObject.SetActive(true);
            scoreResultView.ShowResults(ScoreManager.Instance.GetScorePayload());
            resultPanel.gameObject.SetActive(true);
            startGameView.ShowStart();
        }
    }
}
