using System.Collections;
using System.Collections.Generic;
using B3.BuildingSystem;
using B3.PieceSystem;
using B3.PlayerBuffSystem;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.PlayerSystem
{
    public abstract class PlayerBase : MonoBehaviour
    {
        protected const float MIN_DICE_THROW_FORCE = 1f;
        protected const float MAX_DICE_THROW_FORCE = 2f;
        
        /// <summary>
        /// Mapat cu ResourceType (Resources[0] = Ore, Resources[1] = Wheat, Resources[2] = Wood, Resources[3] = Brick, Resources[4] = Sheep)
        /// </summary>
        /// 
        public int[] Resources { get; private set; }  = new int[5];

        public int VictoryPoints { get; private set; }
        public float DiceThrowForce { get; protected set; }
        public SettlementController SelectedHouse { get; protected set; }
        
        public PathController SelectedPath { get; protected set; }
        public bool IsTurnEnded { get; set; }
        
        public PlayerBuffs PlayerBuffs { get; private set; }
        
        public List<SettlementController> Settlements { get; private set; } = new();
        public List<PathController> Paths { get; private set; } = new();
        
        public PieceController SelectedThiefPiece { get; protected set; }
        public SettlementController SelectedSettlement { get; protected set; }
        
        protected virtual void Awake() =>
            PlayerBuffs = GetComponent<PlayerBuffs>();

        public abstract IEnumerator DiceThrowForceCoroutine();
        public abstract IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController);
        public abstract void OnTradeAndBuildUpdate();

        public abstract IEnumerator BuildHouseCoroutine();
        
        public abstract IEnumerator BuildRoadCoroutine();
        public abstract IEnumerator UpgradeToCityCoroutine();
        
        public IEnumerator EndTurnCoroutine()
        {
            while (!IsTurnEnded)
            {
                OnTradeAndBuildUpdate();
                yield return null;
            }
        }
        
        public void AddResource(ResourceType resource, int amount)//De modificat pe UI staturile corespunzatoare
        {
            int resourceIndex = (int)resource;
            Resources[resourceIndex] += amount;
        }
        
        public void RemoveResource(ResourceType resource, int amount)
        {
            int resourceIndex = (int)resource;
            if (Resources[resourceIndex] < 0)
                return;
            
            Resources[resourceIndex] -= amount;
        }
        
        public void AddVictoryPoints(int amount)
        {
            VictoryPoints += amount;
        }
        
        public void RemoveVictoryPoints(int amount)
        {
            if (VictoryPoints < 0)
                return;
            
            VictoryPoints -= amount;
        }
        
        public int GetHousesCount()
        {
            int count = 0;
            foreach (var settlement in Settlements)
            {
                if (!settlement.IsCity)
                    count++;
            }

            return count;
        }
        
        public int GetCitiesCount()
        {
            int housesCount = GetHousesCount();
            return Settlements.Count - housesCount;
        }
    }
}