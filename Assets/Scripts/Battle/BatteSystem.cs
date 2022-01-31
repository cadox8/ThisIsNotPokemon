using System;
using UnityEngine;

namespace Battle {
    public class BatteSystem : MonoBehaviour {

        [SerializeField] public BattleUnit PlayerUnit;
        [SerializeField] public BattleHud PlayerHud;
        
        [SerializeField] public BattleUnit EnemyUnit;
        [SerializeField] public BattleHud EnemyHud;

        [SerializeField] public BattleDialogBox DialogBox;
        
        private void Start()
        {
            this.SetupBattle();
        }

        private void SetupBattle()
        {
            this.PlayerUnit.Setup();
            this.EnemyUnit.Setup();
            this.PlayerHud.SetData(this.PlayerUnit.Pokemon);
            this.EnemyHud.SetData(this.EnemyUnit.Pokemon);
            
            this.DialogBox.SetDialog($"Un {this.PlayerUnit.Pokemon.Base.name} ha aparecido");
        }
     }
}