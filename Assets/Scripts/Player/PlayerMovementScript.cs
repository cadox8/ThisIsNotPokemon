using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour {

    private float _velocity = 1f;
    
    private List<KeyCode> _keys;

    private PlayerDirection _direction;
    
    private bool _isSprinting;

    void Start()
    {
        this._keys = new List<KeyCode>();
        
        this._keys.Add(KeyCode.W);
        this._keys.Add(KeyCode.A);
        this._keys.Add(KeyCode.S);
        this._keys.Add(KeyCode.D);
        
        this._keys.Add(KeyCode.LeftShift);
        this._keys.Add(KeyCode.Space);

        this._direction = PlayerDirection.None;
    }

    void Update()
    {
        foreach (var key in _keys)
        {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    case KeyCode.W:
                        this._direction = PlayerDirection.Up;
                        break;
                    case KeyCode.A:
                        this._direction = PlayerDirection.Left;
                        break;
                    case KeyCode.S:
                        this._direction = PlayerDirection.Down;
                        break;
                    case KeyCode.D:
                        this._direction = PlayerDirection.Right;
                        break;
                }
                this.MovePlayer();
            }
        }
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
        
        transform.Translate(direction * this._velocity * Time.deltaTime);
    }
}
