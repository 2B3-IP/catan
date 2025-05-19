using B3.PlayerBuffSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;

namespace B3.PortSystem
{
    public sealed class RandomPortController : PortController
    {
        public override ResourceType? ResourceType => null;

        public override void AddPlayerBuff(PlayerBase player)
        {
            var ownerBuffs = player.GetComponent<PlayerBuffs>();
            if (ownerBuffs == null)
                return;
            Debug.Log("add port ? buff");
            for (int i = 0; i < 5; i++)
            {
                var resourceType = (ResourceType)i;
                ownerBuffs.AddBuff(resourceType, PlayerBuff.Trade3_1);
            }
        }
    }
}