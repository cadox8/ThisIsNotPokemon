using System;
using System.Collections;
using System.Security.Cryptography;
using Data;
using DG.Tweening;
using pokemon;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Battle {

    public enum BattleState {
        Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver
    }

    public enum BattleAction {
        Move, Switch, Use, Run
    }
    
    public class BattleSystem : MonoBehaviour {

        [SerializeField] public BattleUnit playerUnit;
        [SerializeField] public BattleUnit enemyUnit;

        [SerializeField] public BattleDialogBox dialogBox;
        [SerializeField] public PartyScreen partyScreen;

        [SerializeField] private GameObject pokeballSprite;

        public event Action<bool> OnBattleOver;
        
        private BattleState state;
        private BattleState? prevState;

        private int currentAction;
        private int currentMove;
        private int currentMember;

        private PokemonParty playerParty;
        private Pokemon wildPokemon;
        
        public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
        {
            this.playerParty = playerParty;
            this.wildPokemon = wildPokemon;
            StartCoroutine(this.SetupBattle());
        }

        private IEnumerator SetupBattle()
        {
            this.playerUnit.Setup(playerParty.GetHealthyPokemon());
            this.enemyUnit.Setup(wildPokemon);
            
            partyScreen.Init();

            this.dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            
            yield return dialogBox.TypeDialog($"Un {this.enemyUnit.Pokemon.Base.name} ha aparecido");
            
            ActionSelection();
        }

        private void BattleOver(bool won)
        {
            state = BattleState.BattleOver;
            playerParty.pokemons.ForEach(pokemon => pokemon.OnBattleOver());
            OnBattleOver(won);
        }

        private void ActionSelection()
        {
            this.state = BattleState.ActionSelection;
            dialogBox.SetDialog("Elige una acción");
            this.dialogBox.EnableActionsSelector(true);
        }

        private void OpenPartyScreen()
        {
            state = BattleState.PartyScreen;
            partyScreen.SetPartyData(playerParty.pokemons);
            partyScreen.gameObject.SetActive(true);
        }
        
        private void MoveSelection()
        {
            this.state = BattleState.MoveSelection;
            this.dialogBox.EnableActionsSelector(false);
            this.dialogBox.EnableDialogText(false);
            this.dialogBox.EnableMoveSelector(true);
        }

        private IEnumerator RunTurns(BattleAction playerAction)
        {
            state = BattleState.RunningTurn;

            if (playerAction == BattleAction.Move)
            {
                playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
                enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
                
                // Primer ataque
                bool playerGoesFirst = true;

                int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
                int enemyMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;

                if (enemyMovePriority > playerMovePriority) 
                    playerGoesFirst = false;
                else if (enemyMovePriority == playerMovePriority)
                    playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
                

                BattleUnit firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
                BattleUnit secondUnit = playerGoesFirst ? enemyUnit : playerUnit;

                Pokemon secondPokemon = secondUnit.Pokemon;

                yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(firstUnit);
                if (state == BattleState.BattleOver) yield break;

                if (secondPokemon.HP > 0)
                {
                    yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                    yield return RunAfterTurn(secondUnit);
                    if (state == BattleState.BattleOver) yield break;
                }
            }
            else
            {
                if (playerAction == BattleAction.Switch)
                {
                    Pokemon selectedPokemon = playerParty.pokemons[currentMember];
                    state = BattleState.Busy;
                    yield return SwitchPokemon(selectedPokemon);
                } else if (playerAction == BattleAction.Use)
                {
                    dialogBox.EnableActionsSelector(false);
                    yield return ThrowPokeball();
                }
                
                
                Move enemyMove = enemyUnit.Pokemon.GetRandomMove();
                yield return RunMove(enemyUnit, playerUnit, enemyMove);
                yield return RunAfterTurn(enemyUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            
            if (state != BattleState.BattleOver) ActionSelection();
        }

        private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
        {
            if (!sourceUnit.Pokemon.OnBeforeTurn())
            {
                yield return ShowStatusChanges(sourceUnit.Pokemon);
                yield return sourceUnit.hud.UpdateHP();
                yield break;
            }
            yield return ShowStatusChanges(sourceUnit.Pokemon);

            move.PP--;
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} usó {move.Base.Name}");

            if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
            {
                sourceUnit.PlayAttackAnimation();
                yield return new WaitForSeconds(1);
            
                targetUnit.PlayHitAnimation();

                if (move.Base.Category == MoveCat.Estado)
                {
                    yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
                }
                else
                {
                    DamageDetails damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                    yield return targetUnit.hud.UpdateHP();
                    yield return ShowDamageDetails(damageDetails);
                }

                if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
                {
                    foreach (var s in move.Base.SecondaryEffects)
                    {
                        if (Random.Range(1, 101) <= s.Chance)
                            yield return RunMoveEffects(s, sourceUnit.Pokemon, targetUnit.Pokemon, s.Target);
                    }
                }
            
                if (targetUnit.Pokemon.HP <= 0)
                {
                    yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} murió");
                    targetUnit.PlayFaintAnimation();
                    yield return new WaitForSeconds(2);
                
                    CheckForBattleOver(targetUnit);
                }
            }
            else
            {
                yield return dialogBox.TypeDialog($"El ataque ha fallado");
            }
        }

        private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
        {
            if (effects.Boosts != null)
            {
                if (moveTarget == MoveTarget.Self)
                    source.ApplyBoost(effects.Boosts);
                else
                    target.ApplyBoost(effects.Boosts);
            }

            if (effects.Status != ConditionID.none) target.SetStatus(effects.Status);
            if (effects.VolatileStatus != ConditionID.none) target.SetVolatileStatus(effects.VolatileStatus);

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }

        private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
        {
            if (state == BattleState.BattleOver) yield break;

            yield return new WaitUntil(() => state == BattleState.RunningTurn);
            
            sourceUnit.Pokemon.OnAfterTurn();
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.hud.UpdateHP();
            
            if (sourceUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} murió");
                sourceUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2);
                
                CheckForBattleOver(sourceUnit);
            }
        }

        private bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
        {
            float moveAccuracy = move.Base.Accuracy;

            if (moveAccuracy == -1) return true;

            int accuracy = source.StatsBoosts[Stat.Precision];
            int evasion = source.StatsBoosts[Stat.Evasion];
            
            float[] boostValues = { 1, 4f / 3f, 3f / 3f, 2, 7f/ 3f, 8f / 3f, 3 };

            if (accuracy > 0)
                moveAccuracy *= boostValues[accuracy];
            else if (accuracy < 0)
                moveAccuracy /= boostValues[-accuracy];
            
            if (evasion > 0)
                moveAccuracy /= boostValues[evasion];
            else if (evasion < 0)
                moveAccuracy *= boostValues[-evasion];

            return Random.Range(1, 101) <= moveAccuracy;
        }

        private IEnumerator ShowStatusChanges(Pokemon pokemon)
        {
            while (pokemon.StatusChanges.Count > 0) yield return dialogBox.TypeDialog(pokemon.StatusChanges.Dequeue());
        }

        private void CheckForBattleOver(BattleUnit faintedUnit)
        {
            if (faintedUnit.isPlayerUnit)
            {
                Pokemon nextPokemon = playerParty.GetHealthyPokemon();
                if (nextPokemon != null) 
                    OpenPartyScreen();
                else
                    BattleOver(false);
            }
            else
            {
                BattleOver(true);
            }
        }

        private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
        {
            if (damageDetails.Critical > 1f) yield return dialogBox.TypeDialog("¡Golpe crítico!");
            if (damageDetails.TypeEffectiveness > 1f)
            {
                yield return dialogBox.TypeDialog("¡Es super efectivo!");
            } else if (damageDetails.TypeEffectiveness < 1) yield return dialogBox.TypeDialog("¡No es muy efectivo!");
        }
        
        public void HandleUpdate()
        {

            switch (state)
            {
                case BattleState.ActionSelection:
                    HandleActionSelector();
                    break;
                case BattleState.MoveSelection:
                    HandleMoveSelector();
                    break;
                case BattleState.PartyScreen:
                    HandlePartyScreenSelector();
                    break;
            }
        }

        private void HandleActionSelector()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                ++currentAction;
            } else if (Input.GetKeyDown(KeyCode.A))
            {
                --currentAction;
            } else if (Input.GetKeyDown(KeyCode.S))
            {
                currentAction += 2;
            } else if (Input.GetKeyDown(KeyCode.W))
            {
                currentAction -= 2;
            }

            currentAction = Mathf.Clamp(currentAction, 0, 3);
            
            dialogBox.UpdateActionSelector(currentAction);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (currentAction)
                {
                    case 0:
                        MoveSelection();
                        break;
                    case 1:
                        StartCoroutine(RunTurns(BattleAction.Use));
                        break;
                    case 2:
                        prevState = state;
                        OpenPartyScreen();
                        break;
                }
            }
        }

        private void HandleMoveSelector()
        {
            if (Input.GetKeyDown(KeyCode.D))
            { 
                ++currentMove;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                --currentMove;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currentMove += 2;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                currentMove -= 2;
            }

            currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);
            
            dialogBox.UpdateMoveSelector(currentMove, playerUnit.Pokemon.Moves[currentMove]);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Move move = playerUnit.Pokemon.Moves[currentMove];
                if (move.PP == 0) return;

                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(RunTurns(BattleAction.Move));
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                ActionSelection();
            }
        }

        private void HandlePartyScreenSelector()
        {
            if (Input.GetKeyDown(KeyCode.D))
                ++currentMember;
            else if (Input.GetKeyDown(KeyCode.A))
                --currentMember;
            else if (Input.GetKeyDown(KeyCode.S))
                currentMember += 2;
            else if (Input.GetKeyDown(KeyCode.W))
                currentMember -= 2;
            
            currentMember = Mathf.Clamp(currentMember, 0, playerParty.pokemons.Count - 1);
            partyScreen.UpdateMemberSelection(currentMember);
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Pokemon selectedMember = playerParty.pokemons[currentMember];

                if (selectedMember.HP <= 0)
                {
                    partyScreen.SetMessageText("No puedes elegir un pokemon muerto!");
                    return;
                }

                if (selectedMember == playerUnit.Pokemon)
                {
                    partyScreen.SetMessageText("Este pokemon ya está combatiendo!");
                    return;
                }
                partyScreen.gameObject.SetActive(false);

                if (prevState == BattleState.ActionSelection)
                {
                    prevState = null;
                    StartCoroutine(RunTurns(BattleAction.Switch));
                }
                else
                {
                    state = BattleState.Busy;
                    StartCoroutine(SwitchPokemon(selectedMember));
                }
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                partyScreen.gameObject.SetActive(false);
                ActionSelection();
            }
        }

        private IEnumerator SwitchPokemon(Pokemon newPokemon)
        {
            if (playerUnit.Pokemon.HP > 0)
            {
                yield return dialogBox.TypeDialog($"Vuelve {playerUnit.Pokemon.Base.Name}");
                playerUnit.PlayExitAnimation();
                yield return new WaitForSeconds(2);
            }

            playerUnit.Setup(newPokemon);
            dialogBox.SetMoveNames(newPokemon.Moves);
            yield return dialogBox.TypeDialog($"Adelante {newPokemon.Base.name}");

            state = BattleState.RunningTurn;
        }

        private IEnumerator ThrowPokeball()
        {
            state = BattleState.Busy;
            yield return dialogBox.TypeDialog("Has usado una POKEBALL");
            
            var pokeballObject = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
            var pokeball = pokeballObject.GetComponent<SpriteRenderer>();
            
            // Animaciones pa
            yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2, 1, 1)
                .WaitForCompletion();
            yield return enemyUnit.PlayCatchAnimation();
            yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, .5f).WaitForCompletion();

            int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);
            
            for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
            {
                yield return new WaitForSeconds(.5f);
                yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10), .8f).WaitForCompletion();
            }

            if (shakeCount == 4)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} ha sido capturado");
                yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
                
                playerParty.AddPokemon(enemyUnit.Pokemon);
                yield return dialogBox.TypeDialog("El pokemon ha sido añadido a tu equipo");
                
                Destroy(pokeball);
                BattleOver(true);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                pokeball.DOFade(0, .2f);
                yield return enemyUnit.PlayBreakAnimation();

                yield return dialogBox.TypeDialog("Casi lo consigues, cruck");
                
                Destroy(pokeball);
                state = BattleState.RunningTurn;
            }
        }

        // El algoritmo no es el original de los juegos de pokemon ya que es mucho lio, en su lugar
        // es la formula original de la 3a genración pero editada por mi
        private int TryToCatchPokemon(Pokemon pokemon)
        {
            float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * 150 * 1.5f / (3 * pokemon.MaxHp);
            if (a >= 255) return 4;

            float b = 1048560 / Mathf.Sqrt((Mathf.Sqrt(1671168 / a)));

            int shakeCount = 0;
            while (shakeCount < 4)
            {
                if (Random.Range(0, 65535) >= b) break;
                ++shakeCount;
            }

            return shakeCount;
        }
    }
}