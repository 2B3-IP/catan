using System;
using System.Collections;
using System.Collections.Generic;
using B3.BuildingSystem;
using B3.PieceSystem;
using B3.PlayerBuffSystem;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using B3.ThiefSystem;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace B3.PlayerSystem
{
    public abstract class PlayerBase : MonoBehaviour
    {
        protected const float MIN_DICE_THROW_FORCE = 1f;
        protected const float MAX_DICE_THROW_FORCE = 2f;

        public string playerName;
        public string colorTag = "<color=red>";
        
        
        /// <summary>
        /// Mapat cu ResourceType (Resources[0] = Wood, Resources[1] = Brick, Resources[2] = Wheat, Resources[3] = Sheep, Resources[4] = Ore)
        /// </summary>
        /// 
        public int[] Resources { get; private set; } = { 99, 99, 99, 99, 99 };//new int[5];

        public int VictoryPoints { get; private set; }
        public int DiceSum { get; protected set; }
        public int UsedKnightCards { get; private set; }
        public int LongestRoad { get; private set; }
        
        [Foldout("Events")] public UnityEvent onResourcesChanged = new();
        [Foldout("Events")] public UnityEvent onVPChanged = new();
        [Foldout("Events")] public UnityEvent onUsedKnightsChanged = new();
        [Foldout("Events")] public UnityEvent onLongestRoadChanged = new();
        
        public SettlementController SelectedHouse { get; protected set; }
        
        public int[] DiscardResources { get; protected set; }

        public PathController SelectedPath { get; protected set; }
        public bool IsTurnEnded { get; set; }
        
        public PlayerBuffs PlayerBuffs { get; private set; }
        
        public List<SettlementController> Settlements { get; private set; } = new();
        public List<PathController> Paths { get; private set; } = new();
        
        public PieceController SelectedThiefPiece { get; protected set; }
        public SettlementController SelectedSettlement { get; protected set; }
        
        protected virtual void Awake() =>
            PlayerBuffs = GetComponent<PlayerBuffs>();

        public abstract IEnumerator ThrowDiceCoroutine();
        public abstract IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController);
        public abstract void OnTradeAndBuildUpdate();

        public abstract IEnumerator BuildHouseCoroutine();
        
        public abstract IEnumerator BuildRoadCoroutine();
        public abstract IEnumerator UpgradeToCityCoroutine();
        
        public abstract IEnumerator DiscardResourcesCoroutine(float timeout);
        
        public IEnumerator EndTurnCoroutine()
        {
            while (!IsTurnEnded)
            {
                OnTradeAndBuildUpdate();
                yield return null;
            }
            Debug.Log("end turn");
        }

        public void SetLongestRoad(int value)
        {
            LongestRoad = value;
            onLongestRoadChanged.Invoke();
        }
        
        public void AddResource(ResourceType resource, int amount)
        {
            int resourceIndex = (int)resource;
            Resources[resourceIndex] += amount;
            onResourcesChanged.Invoke();
        }
        
        public void RemoveResource(ResourceType resource, int amount)
        {
            int resourceIndex = (int)resource;
            if (Resources[resourceIndex] < 0)
                return;
            
            Resources[resourceIndex] -= amount;
            onResourcesChanged.Invoke();
        }

        public int TotalResources()
        {
            int total = 0;
            for(int i = 0; i < Resources.Length; i++)
                total += Resources[i];
            return total;
        }

       
        public void AddVictoryPoints(int amount)
        {
            VictoryPoints += amount;
            onVPChanged.Invoke();
        }
        
        public void RemoveVictoryPoints(int amount)
        {
            if (VictoryPoints < 0)
                return;
            
            VictoryPoints -= amount;
            onVPChanged.Invoke();
        }

        public void AddUsedKnight()
        {
            UsedKnightCards++;
            onUsedKnightsChanged.Invoke();
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