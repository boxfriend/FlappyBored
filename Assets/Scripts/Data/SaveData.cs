using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boxfriend.Data
{
    [System.Serializable]
    public struct SaveData
    {
        public string Name;
        public int HighScore;
        public float Volume;

        public SaveData (int highScore, string name = "Box", float volume = 0.75f)
        {
            HighScore = highScore;
            Volume = Mathf.Clamp(volume, 0.01f, 1f);
            Name = name;
        }

        public string Json => JsonUtility.ToJson(this,true);

    }
}
