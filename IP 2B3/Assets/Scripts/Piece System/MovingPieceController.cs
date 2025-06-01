using System.Collections;
using UnityEngine;

namespace B3.PieceSystem
{
    public abstract class MovingPieceController : MonoBehaviour
    {
        [SerializeField] private float spawnDuration = 1f;
    }
}