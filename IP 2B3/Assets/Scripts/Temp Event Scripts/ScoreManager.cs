using System;
using Score_System.UI;
using UnityEngine;

namespace Score_System
{
    internal sealed class ScoreManager : MonoBehaviour
    {
        public static event Action<int> OnUpdateScore;
        private int score = 0;
        
        [SerializeField] private UIScoreText a;
        
        [ContextMenu("a")]
        public void Increase()
        {
            score++;
            OnUpdateScore?.Invoke(score);
        }
    }
}