using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class PlayerMovementScript : MonoBehaviour {

        [Header("Velocidad")]
        public float velocity;

        [Header("Capa de sólidos")] public LayerMask propsLayer;

        // --- Internal ---

        private PlayerDirection _direction;
        
        private bool _isSprinting;

        private Animator _animator;

        private bool _canWalk = true;
        
        void Start()
        {
            this._animator = GetComponent<Animator>();

            this._direction = PlayerDirection.None;
            this._isSprinting = false;
        }

        void Update()
        {
            this._direction = PlayerDirection.None;
            if (Input.GetKey(KeyCode.W) && this._direction == PlayerDirection.None) this._direction = PlayerDirection.Up;
            if (Input.GetKey(KeyCode.A) && this._direction == PlayerDirection.None) this._direction = PlayerDirection.Left;
            if (Input.GetKey(KeyCode.S) && this._direction == PlayerDirection.None) this._direction = PlayerDirection.Down;
            if (Input.GetKey(KeyCode.D) && this._direction == PlayerDirection.None) this._direction = PlayerDirection.Right;

            if (Input.GetKeyUp(KeyCode.LeftShift)) this._isSprinting = !this._isSprinting;
            
            this.MovePlayer();
        }

        private void MovePlayer()
        {
            Vector3 direction;

            switch (this._direction)
            {
                case PlayerDirection.Up:
                    direction = Vector3.up;
                    break;
                case PlayerDirection.Left:
                    direction = Vector3.left;
                    break;
                case PlayerDirection.Down:
                    direction = Vector3.down;
                    break;
                case PlayerDirection.Right:
                    direction = Vector3.right;
                    break;
                default:
                    direction = Vector3.zero;
                    break;
            }

            Vector3 movement = direction * (this._isSprinting ? this.velocity * 2f : this.velocity) * Time.deltaTime;

            if (this.IsWalkable(movement + transform.position) && this._canWalk) transform.Translate(movement);

            if (this._direction != PlayerDirection.None)
            {
                this._animator.SetBool("isSprinting", this._isSprinting);
                this._animator.SetFloat("moveX", direction.x);
                this._animator.SetFloat("moveY", direction.y);
                this._animator.SetBool("isMoving", true);
            }
            else
            {
                this._animator.SetBool("isMoving", false);
            }
        }
        
        // --- Collision ---
        private bool IsWalkable(Vector3 targetPos)
        {
            return Physics2D.OverlapCircle(targetPos - new Vector3(0, -0.5f, 0), 0.2f, this.propsLayer) == null;
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("NPC"))
            {
                Debug.Log("hit!");
            }
        }
    }
}