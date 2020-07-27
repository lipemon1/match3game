using Match3Game.Scripts.Behaviours.Score;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.View
{
    public class ScoreResultView : MonoBehaviour
    {
        [SerializeField] private Text[] catAmount;
        [SerializeField] private Text[] dogAmount;
        [SerializeField] private Text[] frogAmount;
        [SerializeField] private Text[] pandaAmount;
        [SerializeField] private Text[] pigAmount;

        private ScoreManager.ScorePayload curScore;

        /// <summary>
        /// Update all result on interface
        /// </summary>
        /// <param name="scorePayload"></param>
        public void ShowResults(ScoreManager.ScorePayload scorePayload)
        {
            curScore = scorePayload;
            
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            foreach (var text in catAmount)
                text.text = curScore.catResult.amount.ToString();
            
            foreach (var text in dogAmount)
                text.text = curScore.dogResult.amount.ToString();
            
            foreach (var text in frogAmount)
                text.text = curScore.frogResult.amount.ToString();
            
            foreach (var text in pandaAmount)
                text.text = curScore.pandaResult.amount.ToString();
            
            foreach (var text in pigAmount)
                text.text = curScore.pigResult.amount.ToString();
        }
    }
}
