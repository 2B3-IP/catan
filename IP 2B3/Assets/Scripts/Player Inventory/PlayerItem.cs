using B3.DevelopmentCardSystem;

namespace B3.PlayerInventorySystem
{
    public struct PlayerItem
    {
        public DevelopmentCardType CardType { get; }
        public bool CanBeUsed { get; set; }

        public PlayerItem(DevelopmentCardType cardType)
        {
            CardType = cardType;
            CanBeUsed = false;
        }
    }
}