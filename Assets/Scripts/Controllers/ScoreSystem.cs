using System;

namespace Controllers
{
    public class ScoreSystem
    {
        public static int Score
        {
            get
            {
                OnScoreUpdate?.Invoke(_score);
                return _score;
            }
            set => _score = value;
        }

        private static int _score;

        public static event  Action<int> OnScoreUpdate;
    }
}