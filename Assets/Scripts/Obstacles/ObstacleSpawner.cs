using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boxfriend.Player;
using UnityEngine;
using UnityEngine.Pool;

namespace Boxfriend
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private float _minY, _maxY, _minX;
        [SerializeField] private int _spawnDelay;
        
        [SerializeField] private Pipe _pipePrefab;
        private IObjectPool<Pipe> _pipePool;

        private readonly List<Pipe> _activePipes = new List<Pipe>();

        private bool isPlayerDead;

        private Pipe CreatePipePoolObject ()
        {
            return Instantiate(_pipePrefab);
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            _pipePool = new ObjectPool<Pipe>(CreatePipePoolObject,TakeFromPool,ReturnToPool,
                DestroyedFromPool,true,15,30);
        }

        private void OnEnable ()
        {
            PlayerController.OnPlayerDeath += OnPlayerDeath;
            PlayerController.OnPlayerJump += BeginSpawning;
        }
        private void OnDisable ()
        {
            PlayerController.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath ()
        {
            isPlayerDead = true;

            for (var i = _activePipes.Count - 1; i >= 0; i--)
            {
                _pipePool.Release(_activePipes[i]);
            }
        }

        private void BeginSpawning ()
        {
            PlayerController.OnPlayerJump -= BeginSpawning;
            var spawning = SpawnPipeTask();
        }

        private async Task SpawnPipeTask ()
        {
            while (!isPlayerDead)
            {
                var pipe = _pipePool.Get();
                await Task.Delay(_spawnDelay);
            }
        }

        private void TakeFromPool (Pipe pipe)
        {
            _activePipes.Add(pipe);
            var spawnPosition = transform.position;
            spawnPosition.y = UnityEngine.Random.Range(_minY, _maxY);
            pipe.transform.position = spawnPosition;
            pipe.gameObject.SetActive(true);
        }

        private void ReturnToPool (Pipe pipe)
        {
            pipe.gameObject.SetActive(false);
            pipe.transform.position = transform.position;
            _activePipes.Remove(pipe);
        }

        private void DestroyedFromPool (Pipe pipe)
        {
            if (pipe == null) return;
            
            if(_activePipes.Contains(pipe))
                _activePipes.Remove(pipe);
            
            Destroy(pipe.gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            var toRemove = _activePipes.Where(pipe => pipe.transform.position.x < _minX).ToList();
            foreach (var pipe in toRemove)
                _pipePool.Release(pipe);
            
            toRemove.Clear();
        }

        private void OnDestroy ()
        {
            _pipePool.Clear();
        }
    }
}
