using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boxfriend
{
    public class Pipe : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        
        [SerializeField] private Rigidbody2D _rb2d;
        
        private void FixedUpdate ()
        {
            _rb2d.velocity = Vector2.left * _moveSpeed;
        }
    }
}
