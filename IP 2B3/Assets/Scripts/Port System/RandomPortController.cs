using B3.GameStateSystem;
using B3.PlayerBuffSystem;
using B3.ResourcesSystem;

namespace B3.PortSystem
{
    public sealed class RandomPortController : PortController
    {
        public override void AddPlayerBuff()
        {
            var ownerBuffs = OwnerBuffs;
            if (ownerBuffs == null)
                return;

            for (int i = 0; i < 5; i++)
            {
                var resourceType = (ResourceType)i;
                ownerBuffs.AddBuff(resourceType, PlayerBuff.Trade3_1);
            }
        }
    }
}