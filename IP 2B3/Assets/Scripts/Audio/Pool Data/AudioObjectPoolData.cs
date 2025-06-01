using UnityEngine;

namespace TheBlindEye.ObjectPoolSystem
{
    [CreateAssetMenu(menuName = "Object Pools/Managers/Audio", fileName = "Audio ObjectPoolData")]
    public sealed class AudioObjectPoolData : ObjectPoolData<SoundEmitter>
    { }
}