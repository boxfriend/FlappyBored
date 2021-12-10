using System.Threading.Tasks;
using Boxfriend.Player;
using TMPro;
using UnityEngine;

namespace Boxfriend
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _pointsText, _highScoreText;
        [SerializeField] private GameObject _pausePanel; 

        private int _pointCount, _highScore;

        private void OnEnable ()
        {
            PlayerController.OnGetPoints += UpdatePoints;
            PlayerController.OnPlayerPause += Pause;
        }
        private void OnDisable ()
        {
            PlayerController.OnGetPoints -= UpdatePoints;
            PlayerController.OnPlayerPause -= Pause;

        }

        private void Start ()
        {
            _highScore = GameManager.Instance.HighScore;
            _highScoreText.text = $"High Score: {_highScore}";
        }

        private void UpdatePoints ()
        {
            _pointCount++;
            _pointsText.text = $"Points: {_pointCount}";
            
            if(_pointCount > _highScore)
                _highScoreText.text = $"High Score: {_pointCount}";
        }

        private void Pause ()
        {
            var task = PauseTask();
            
            async Task PauseTask ()
            {
                _pausePanel.SetActive(true);
                await Task.Delay(1000);
                _pausePanel.SetActive(false);
            }
        }
    }
}
