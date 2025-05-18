using UnityEngine;
using B3.GameStateSystem;
using B3.PlayerBuffSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;

namespace B3.PortSystem
{
    public sealed class ResourcePortController : PortController
    {
        [SerializeField] private ResourceType resourceType;

        public override ResourceType? ResourceType => resourceType;

        public override void AddPlayerBuff(PlayerBase player)
        {
            var ownerBuffs = player.GetComponent<PlayerBuffs>();
            
            Debug.Log("add " + resourceType + " port buff");
            if (ownerBuffs != null)
                ownerBuffs.AddBuff(resourceType, PlayerBuff.Trade2_1);
        }
    }
}