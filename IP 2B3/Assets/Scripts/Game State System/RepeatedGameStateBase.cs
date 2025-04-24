using System.Collections;
using UnityEngine;

namespace B3.GameStateSystem
{
    public abstract class RepeatedGameStateBase : GameStateBase
    {
        [field:SerializeField] public int RepeatTimes { get; set; }
    }
}