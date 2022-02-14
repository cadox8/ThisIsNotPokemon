using System;
using Battle;
using Data;
using Player;
using pokemon;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay {

    public enum GameState {
        FreeRoam, Battle, Dialog
    }
    
    public class GameController : MonoBehaviour {
        
        [FormerlySerializedAs("playerMovement")] [SerializeField] public PlayerController playerController;
        [SerializeField] public BattleSystem battleSystem;
        [SerializeField] public Camera worldCamera;
        
        private GameState state;

        private void Awake()
        {
            ConditionsDB.Init();
        }

        private void Start()
        {
            playerController.OnEncounter += StartBattle;
            battleSystem.OnBattleOver += EndBattle;

            DialogManager.Instance.OnShowDialog += () =>
            {
                state = GameState.Dialog;
            };
            DialogManager.Instance.OnCloseDialog += () =>
            {
                if (state == GameState.Dialog)
                    state = GameState.FreeRoam;
            };
        }

        private void StartBattle()
        {
            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            Pokemon wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
            Pokemon wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
            
            battleSystem.StartBattle(playerController.GetComponent<PokemonParty>(), wildPokemonCopy);
        }

        private void EndBattle(bool won)
        {
            state = GameState.FreeRoam;
            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
        }

        private void Update()
        {
            switch (state)
            {
                case GameState.FreeRoam:
                    playerController.HandleUpdate();
                    break;
                case GameState.Battle:
                    battleSystem.HandleUpdate();
                    break;
                case GameState.Dialog:
                    DialogManager.Instance.HandleUpdate();
                    break;
            }
        }
    }
}