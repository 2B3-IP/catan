using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;

namespace B3.DevelopmentCardSystem
{
    [System.Serializable]
    public abstract class DevelopmentCardBase
    {
        public abstract IEnumerator UseCard(PlayerBase player);   
    }
}