using UnityEngine;

namespace Match3Game.Scripts.Model
{
    public static class Seed
    {
        private const string AcceptableChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdeghijklmnopqrstuvwxyz1234567890!@#$%^&*()";

        private const int CharsAmount = 15;
        private const string ForceSeed = "";

        public static string GenerateSeed()
        {
            var seed = ForceSeed;

            if (string.IsNullOrEmpty(seed))
                for (var i = 0; i < CharsAmount; i++)
                    seed += AcceptableChars[Random.Range(0, AcceptableChars.Length)];

            return seed;
        }
    }
}