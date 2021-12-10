using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Boxfriend
{
    public class GameOverManager : MonoBehaviour
    {
        public void Restart ()
        {
            SceneManager.UnloadSceneAsync("GameScene");
            Debug.Log("Unloaded Game Scene, restarting now");
            SceneManager.LoadSceneAsync("GameScene");
        }

        public void Quit ()
        {
            Application.Quit(420);
        }
    }
}
