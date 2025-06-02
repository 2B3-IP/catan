using System;
using System.Collections;
using B3.UI;
using UnityEngine;

namespace B3.GameStateSystem
{
    [Serializable]
    internal sealed class PlayerEndGameState : GameStateBase
    {
        public static event Action OnPlayerEnd;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip victoryClip;
        public AudioSource AudioSource => audioSource;
        public AudioClip VictoryClip => victoryClip;
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("PlayerEndGameState OnEnter");
            OnPlayerEnd?.Invoke();
            var currentPlayer = stateMachine.CurrentPlayer;
            if (currentPlayer.VictoryPoints >= 10)
            {
                NotificationManager.Instance.AddNotification(
                    $"{currentPlayer.colorTag}{currentPlayer.playerName}</color> WON.");
                
                if (AudioSource && VictoryClip)
                    AudioSource.PlayOneShot(VictoryClip);
                
                yield return new WaitForSeconds(2f); // Așteaptă ca notificarea să se vadă

                Application.Quit(); // În build închide aplicația
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; // În editor oprește play mode
                #endif

                yield break;
            }

            AI.SendMove("end round");

            stateMachine.StartMachineWithOtherPlayer();
            yield break;
        }
    }
}