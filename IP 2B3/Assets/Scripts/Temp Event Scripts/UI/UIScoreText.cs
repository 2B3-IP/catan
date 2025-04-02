using TMPro;
using UnityEngine;

namespace Score_System.UI
{
    internal sealed class UIScoreText : MonoBehaviour
    {
        [SerializeField] private TMP_Text a;

        private void OnEnable() =>
            ScoreManager.OnUpdateScore += SetScore;
        
        private void OnDisable() =>
            ScoreManager.OnUpdateScore -= SetScore;

        private void SetScore(int score) =>
            a.SetText(score.ToString());
    }
}