using System.Collections;
using UnityEngine;
using TheBlindEye.Utility;

namespace TheBlindEye.ObjectPoolSystem
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("The BlindEye/Object Pools/Managers/Sound Emitter")]
    public sealed class SoundEmitter : PoolObject<SoundEmitter>
    {
        private AudioSource _audio;

        private void Awake() => _audio = GetComponent<AudioSource>();

        public void PlayAudio(AudioClip audioClip, float volume, bool isSpatial)
        {
            _audio.volume = volume; _audio.spatialBlend = isSpatial ? 1f : 0f;
            _audio.PlayOneShot(audioClip);
            
            StartCoroutine(FinishedPlayingCoroutine(audioClip.length));
        }

        private IEnumerator FinishedPlayingCoroutine(float clipLength)
        {
            yield return null;
            //yield return clipLength.GetWait();
            Return(this);
        }
    }
}