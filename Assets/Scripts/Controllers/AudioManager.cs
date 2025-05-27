using UnityEngine;

namespace Controllers
{
    public class AudioManager
    {
        private static AudioClip _on5EnemiesKilled;

        public AudioManager(AudioClip on5EnemiesKilled)
        {
            _on5EnemiesKilled = on5EnemiesKilled;
        }

        public static void PlayClip(Vector3 pos)
        {
            AudioSource.PlayClipAtPoint(_on5EnemiesKilled, pos);
        }
    }
}