using System;
using System.Collections;
using NPC;
using SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Player {
    public class PlayerController : MonoBehaviour {

        [Header("Velocidad")]
        public float velocity;

        [Header("Capa de sólidos")] public LayerMask propsLayer;
        [Header("Capa de Grass")] public LayerMask grassLayer;
        [Header("Capa de Interactuables")] public LayerMask interactableLayer;
        [Header("Capa de Interactuables")] public LayerMask portal;


        // --- Internal ---

        public event Action OnEncounter;

        private Vector2 input;
        
        private bool _isMoving;

        private Animator _animator;
        
        void Start()
        {
            this._animator = GetComponent<Animator>();
        }

        public void HandleUpdate()
        {
            if (!_isMoving)
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");

                if (input.x != 0) input.y = 0;
                
                if (input != Vector2.zero)
                {
                    _animator.SetFloat("moveX", input.x);
                    _animator.SetFloat("moveY", input.y);
                    
                    Vector2 targetPos = transform.position;
                    targetPos.x += input.x;
                    targetPos.y += input.y;

                    if (IsWalkable(targetPos)) StartCoroutine(Move(targetPos));
                }
            }
            this._animator.SetBool("isMoving", _isMoving);
            
            if (Input.GetKeyDown(KeyCode.Z)) Interact();
        }
        
        private IEnumerator Move(Vector3 targetPos)
        {
            _isMoving = true;
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, velocity * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            OnMoveOver();
            _isMoving = false;
            
            CheckForPokemon();
        }

        // --- Interact ---
        private void Interact()
        {
            Vector3 facingDir = new Vector3(this._animator.GetFloat("moveX"), this._animator.GetFloat("moveY"));
            Vector3 interactPos = transform.position + facingDir;
            
            Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, this.interactableLayer);

            if (collider != null)
            {
                collider.GetComponent<Interactable>()?.Interact();
            }
        }

        private void OnMoveOver()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            if (currentScene == 1)
            {
                Debug.Log(transform.position);
                if (transform.position.x < 13 && transform.position.x > 11 && transform.position.y > 0 &&
                    transform.position.y < 1)
                {
                    Debug.Log("dadsad");
                    SceneManager.LoadScene(2);
                }
            } else if (currentScene == 2)
            {
                
            }
        }
        
        // --- Collision ---
        private bool IsWalkable(Vector3 targetPos)
        {
            return Physics2D.OverlapCircle(targetPos, 0.2f, this.propsLayer | this.interactableLayer) == null;
        }

        private void CheckForPokemon()
        {
            if (Physics2D.OverlapCircle(transform.position, 0.2f, this.grassLayer) != null)
            {
                if (Random.Range(1, 101) <= 10)
                {
                    _animator.SetBool("isMoving", false);
                    OnEncounter();
                }
            }
        }
    }
}