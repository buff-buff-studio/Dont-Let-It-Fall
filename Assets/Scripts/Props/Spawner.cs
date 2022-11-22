using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLIFR.Props
{   
    [Serializable]
    public class LootTable
    {
        [Serializable]
        public class LootTableEntry
        {
            public GameObject prefab;
            [Range(0, 20)]
            public int minCount;
            [Range(0, 20)]
            public int maxCount;
        }

        public LootTableEntry[] entries;
    }

    public class Spawner : MonoBehaviour
    {
        public const float SIZE = 0.5F;

        public LootTable lootTable;
        public bool spawnOnStart = true;

        private void Start() 
        {
            if(spawnOnStart)
                Spawn();
        }

        private void OnDrawGizmos() 
        {
            Matrix4x4 preMatrix = Gizmos.matrix;
            Gizmos.color = Color.blue;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = preMatrix;
        }

        public void Spawn()
        {
            List<Vector3> buffer = new List<Vector3>();

            foreach(LootTable.LootTableEntry entry in lootTable.entries)
            {
                int count = UnityEngine.Random.Range(entry.minCount, entry.maxCount + 1);

                for(int i = 0; i < count; i ++)
                {
                    Vector3 position = Vector3.zero;

                    bool keep = true;

                    while(keep)
                    {
                        position = GetRandomPointInBounds();
                        keep = false;

                        foreach(Vector3 v in buffer)
                        {
                            if(Vector3.Distance(v, position) < 1f)
                            {
                                keep = true;
                                break;
                            }
                        }
                    }
                    
                    GameObject go = GameObject.Instantiate(entry.prefab, position, transform.rotation);
                    buffer.Add(position);
                }
            }
        }
        
        public Vector3 GetRandomPointInBounds() 
        {      
            Vector3 point = new Vector3(
                UnityEngine.Random.Range(-SIZE, SIZE),
                UnityEngine.Random.Range(-SIZE, SIZE),
                UnityEngine.Random.Range(-SIZE, SIZE)
            );
        
            return transform.TransformPoint(point);
        }
    }
}