using System;
using System.Collections;
using B3.DiceSystem;
using B3.ResourcesSystem;
using UnityEngine;
using B3.PieceSystem;
using System.Collections.Generic;
using System.Linq;
using B3.BankSystem;
using B3.BoardSystem;
using B3.PlayerSystem;
using Random = UnityEngine.Random;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class DiscardGameState : GameStateBase
    {
        public static event Action OnDiscardStateEnd;
        [SerializeField] private float timeout;

        [SerializeField] private BankController bankController;
         public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("discard state");

            var allPlayers = stateMachine.PlayersManager.players;
            List<Coroutine> coroutines = new List<Coroutine>();
            int discardPlayers = 0;
            
            foreach (var player in allPlayers)
            {
                int total = player.TotalResources();
                if (player is not AIPlayer && total > 7) //temp
                {
                    Coroutine coroutine = stateMachine.StartCoroutine(player.DiscardResourcesCoroutine(timeout));
                    coroutines.Add(coroutine);
                    discardPlayers++;
                }
            }

            float elapsed = 0f;
         

            while (elapsed < timeout )
            {
                elapsed += Time.deltaTime;
                int count = discardPlayers;

                foreach (var player in allPlayers)
                {
                    if (player.DiscardResources != null)
                    {
                        count--;

                    }
                }

                if (count == 0)
                    break;

                yield return null;
            }

            
            foreach (var coroutine in coroutines)
            {
                if (coroutine != null)
                    stateMachine.StopCoroutine(coroutine);
            }

            OnDiscardStateEnd?.Invoke();
            
            foreach (var player in allPlayers)
            {
                if(player.DiscardResources == null)
                    RemoveRandomResources(player);
                else
                {
                    for (int i = 0; i < player.Resources.Length; i++)
                    {
                        player.RemoveResource((ResourceType)i, player.DiscardResources[i]);
                        bankController.GiveResources((ResourceType)i, player.DiscardResources[i]);
                    }
                }
                
            }
            
            stateMachine.ChangeState<ThiefGameState>();
            yield break;
        }


        private void RemoveRandomResources(PlayerBase player)
        {
            var totalResources = player.TotalResources();
            if (totalResources > 7)
            {
                int toDiscard = totalResources / 2;

                for (int i = 0; i < toDiscard; i++)
                {
                    int index = 0;
                    do
                    { index = Random.Range(0, player.Resources.Length);
                    } while (player.Resources[index] <= 0);
            
                    bankController.GiveResources((ResourceType)index, 1);
                    player.RemoveResource((ResourceType)index, 1);
                
                }
            }
        }
    }
}