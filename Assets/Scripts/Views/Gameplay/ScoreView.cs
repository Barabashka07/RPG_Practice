using TMPro;
using UnityEngine;

namespace Views.Gameplay
{
    public class ScoreView : View
    {
        [SerializeField] private TextMeshProUGUI _score;

        public void UpdateScore(int score)
        {
            _score.text = score.ToString();
        }
    }
}