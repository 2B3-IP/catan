using System;
using UnityEngine;

namespace TheBlindEye.Utility
{
    public static class Audio
    {
        private static Action<AudioClip, Vector3, float, bool> _onPlaySound;
        
        public static void SetAudioManager(Action<AudioClip, Vector3, float, bool> onPlaySound) =>
            _onPlaySound = onPlaySound;

        public static void Play(AudioClip audioClip, Vector3 position, float volume = 1f, bool isSpatial = true) =>
            _onPlaySound?.Invoke(audioClip, position, volume, isSpatial);
    }
}