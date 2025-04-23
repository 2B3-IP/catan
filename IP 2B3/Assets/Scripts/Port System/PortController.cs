using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PlayerBuffSystem;
using UnityEngine;

namespace B3.PortSystem
{
    public abstract class PortController : MonoBehaviour
    {
        [SerializeField] private HousePivot[] portTransform;
        
        protected PlayerBuffs OwnerBuffs
        {
            get
            {
                foreach (var housePivot in portTransform)
                {
                    var owner = housePivot.Owner;
                    
                    if (owner != null)
                        return owner.GetComponent<PlayerBuffs>();
                }

                return null;
            }
        }
        
        public abstract void AddPlayerBuff();
    }
}