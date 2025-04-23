using UnityEngine;
using B3.GameStateSystem;
using B3.PlayerBuffSystem;
using B3.ResourcesSystem;

namespace B3.PortSystem
{
    public sealed class ResourcePortController : PortController
    {
        [SerializeField] private ResourceType resourceType;
        
        public override void AddPlayerBuff()
        {
            var ownerBuffs = OwnerBuffs;
            
            if (ownerBuffs != null)
                ownerBuffs.AddBuff(resourceType, PlayerBuff.Trade2_1);
        }
    }
}