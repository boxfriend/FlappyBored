using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Boxfriend.Data;
using Boxfriend.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Boxfriend
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int _score, _highScore;
        private SaveData _loadedData;
        private event Action OnFinishLoad;
        
        public int HighScore => _highScore;

        private void Awake ()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
            
            DontDestroyOnLoad(this);

            PlayerController.OnGetPoints += () => _score++;
            PlayerController.OnPlayerDeath += GameOverEvent;
            OnFinishLoad += () => SceneManager.LoadSceneAsync("GameScene");

            var loadingData = LoadSave();
            
        }

        private async Task LoadSave ()
        {
            var path = $@"{Application.persistentDataPath}\Saves";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var file = path + @"\SaveData.box";

            using var reader = new StreamReader(file);
            var data = await reader.ReadToEndAsync();

            _loadedData = JsonUtility.FromJson<SaveData>(data);
            _highScore = (_loadedData.HighScore > _highScore) ? _loadedData.HighScore : _highScore;
            
            OnFinishLoad?.Invoke();
        }
        
        private void GameOverEvent()
        {
            var save = SaveGame();
            SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
            _score = 0;
        }

        private async Task SaveGame ()
        {
            _highScore = (_score > HighScore) ? _score : HighScore;
            var saveData = new SaveData(_highScore);
            
            var file = $@"{Application.persistentDataPath}\Saves\SaveData.box";
            await using var writer = File.CreateText(file);
            await writer.WriteAsync(saveData.Json);
            
            Debug.Log($"Game Saved");
        }
    }
}
