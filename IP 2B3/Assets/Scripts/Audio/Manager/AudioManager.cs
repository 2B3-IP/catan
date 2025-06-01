using UnityEngine;
using TheBlindEye.ObjectPoolSystem;
using TheBlindEye.Utility;

namespace TheBlindEye.Managers
{
    internal sealed class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioObjectPoolData poolData;

        private void Awake() => Audio.SetAudioManager(PlayAudio);

        private void PlayAudio(AudioClip audioClip, Vector3 position, float volume, bool isSpatial)
        {
            var poolObject = poolData.Get(position, Quaternion.identity);
            poolObject.PlayAudio(audioClip, volume, isSpatial);
            
            poolObject.Register(ReleasePoolObject);
        }

        private void ReleasePoolObject(SoundEmitter poolObject) => poolData.Release(poolObject);
    }
}