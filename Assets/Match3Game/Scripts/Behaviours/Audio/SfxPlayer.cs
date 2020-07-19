using Match3Game.Scripts.Scriptables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Game.Scripts.Behaviours.Audio
{
    public class SfxPlayer : MonoBehaviour
    {
        public static SfxPlayer Instance { get; set; }

        [Header("Game Configuration")] [SerializeField]
        private GameConfig gameConfig;

        [Header("Audio")] [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        public void PlayPop(float volumeModifier = 1)
        {
            audioSource.PlayOneShot(GetRandomPop(), volumeModifier);
        }

        private AudioClip GetRandomPop()
        {
            var randomPopIndex = Random.Range(0, gameConfig.PopsSfx.Count);
            return gameConfig.PopsSfx[randomPopIndex];
        }
    }
}