using System;
using UnityEngine;
using System.Collections.Generic;

namespace Controllers.SaveLoad.Saveables
{
    [Serializable]
    public class PlayerData
    {
        public float Health;
        public Vector3 Position;
        public List<EnemyData> Enemies; 
    }

    [Serializable]

    public class EnemyData
    {
        public string Id;
        public float Health;
        public Vector3 Position;
        public int PrefabIndex;
        public bool IsBoss;
    }
    
    
    // [Serializable]
    //
    // public class LvlData
    // {
    //     public string Id;
    //     public string LvlName;
    // }
    
}