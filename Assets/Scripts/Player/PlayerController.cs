using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Boxfriend.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float _jumpForce;
        [SerializeField][Range(-10f,10f)] private float _maxVelocity, _minVelocity;
        [SerializeField][Range(500,1000)] private int _jumpDelay;

        [Header("Components")]
        [SerializeField] private Rigidbody2D _rb2d;
        [SerializeField] private InputActionAsset _inputAction;

        private InputAction _jump;
        private bool _canJump = true;


        public static event Action OnPlayerDeath;
        public static event Action OnPlayerJump;
        public static event Action OnGetPoints;
        public static event Action OnPlayerPause;
        
        public bool IsAlive { get; private set; }

        private void OnEnable ()
        {
            _jump = _inputAction.FindAction("Jump");
            _jump.performed += Jump;
            _inputAction.FindAction("Pause").performed += _ => OnPlayerPause?.Invoke();
            IsAlive = true;
            _rb2d.isKinematic = true;
        }

        private void OnDisable () => _jump.performed -= Jump;
        
        private void Jump (InputAction.CallbackContext value)
        {
            if (!_canJump || !IsAlive) return;
            
            _rb2d.velocity = Vector2.zero;
            _rb2d.angularVelocity = 0;
            _rb2d.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _rb2d.AddTorque(Random.Range(-1,1),ForceMode2D.Impulse);
            var cooldown = JumpCooldown();
            OnPlayerJump?.Invoke();

            _rb2d.isKinematic = false;
        }

        private async Task JumpCooldown ()
        {
            _canJump = false;
            await Task.Delay(_jumpDelay);
            _canJump = true;
        }

        private void FixedUpdate ()
        {
            var yVelocity = _rb2d.velocity.y;
            yVelocity = Mathf.Clamp(yVelocity, _minVelocity, _maxVelocity);
            _rb2d.velocity = new Vector2(0, yVelocity);
        }

        private void OnCollisionEnter2D (Collision2D other)
        {
            if (!other.collider.CompareTag("Obstacle")) return;
            
            IsAlive = false;
            OnPlayerDeath?.Invoke();
            _rb2d.velocity = Vector2.zero;
            _rb2d.isKinematic = true;
        }

        private void OnTriggerEnter2D (Collider2D other)
        {
            if (other.CompareTag("Point"))
                OnGetPoints?.Invoke();
        }
    }
}
