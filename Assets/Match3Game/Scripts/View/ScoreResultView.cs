using Match3Game.Scripts.Behaviours.Score;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.Scripts.View
{
    public class ScoreResultView : MonoBehaviour
    {
        [SerializeField] private Text catAmount;
        [SerializeField] private Text dogAmount;
        [SerializeField] private Text frogAmount;
        [SerializeField] private Text pandaAmount;
        [SerializeField] private Text pigAmount;

        /// <summary>
        /// Update all result on interface
        /// </summary>
        /// <param name="scorePayload"></param>
        public void ShowResults(ScoreManager.ScorePayload scorePayload)
        {
            catAmount.text = scorePayload.catResult.amount.ToString();
            dogAmount.text = scorePayload.dogResult.amount.ToString();
            frogAmount.text = scorePayload.frogResult.amount.ToString();
            pandaAmount.text = scorePayload.pandaResult.amount.ToString();
            pigAmount.text = scorePayload.pigResult.amount.ToString();
        }
    }
}
