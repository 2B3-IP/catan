using UnityEngine;

namespace B3.BoardSystem
{
    [System.Serializable]
    internal sealed class BoardLine
    {
        [field: SerializeField] public Transform SpawnPosition { get; private set; }
        [field:SerializeField] public Transform[] EndPositions { get; private set; }
    }
}