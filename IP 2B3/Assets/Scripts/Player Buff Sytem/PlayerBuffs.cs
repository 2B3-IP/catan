using System.Collections.Generic;
using B3.ResourcesSystem;
using UnityEngine;

namespace B3.PlayerBuffSystem
{
    public sealed class PlayerBuffs : MonoBehaviour
    {
        private readonly Dictionary<ResourceType, PlayerBuff> _buffs = new();

        private void Awake()
        {
            for (int i = 0; i < 5; i++)
            {
                var resourceType = (ResourceType)i;
                _buffs.Add(resourceType, PlayerBuff.Trade4_1);
            }
        }

        public void inialize()
        {
            Awake();
        }
        public void AddBuff(ResourceType resourceType, PlayerBuff buff)
        {
            
            var currentBuffIndex = _buffs[resourceType];
            if (buff < currentBuffIndex)
                return;
            
            _buffs[resourceType] = buff;
        }

        public int GetResourceAmount(ResourceType resourceType)
        {
            return _buffs[resourceType] switch
            {
                PlayerBuff.Trade4_1 => 4,
                PlayerBuff.Trade3_1 => 3,
                PlayerBuff.Trade2_1 => 2,
                _ => 0
            };
        }
    }
}