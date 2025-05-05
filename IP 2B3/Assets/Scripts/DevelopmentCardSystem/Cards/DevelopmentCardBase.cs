using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;

namespace B3.DevelopmentCardSystem
{
    public abstract class DevelopmentCardBase
    {
        public abstract IEnumerator UseCard(PlayerBase player);   
    }
}