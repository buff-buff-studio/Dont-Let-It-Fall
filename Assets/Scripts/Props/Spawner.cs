using System;
using System.Collections.Generic;
using System.Linq;
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
        public Transform spawnParent;
        
        public Transform[] topSpawnPoints;
        public Transform[] sideSpawnPoints;
        
        public const float SIZE = 0.5F;

        public LootTable lootTable;
        public bool spawnOnStart = true;

        private void Start() 
        {
            if(spawnOnStart)
                Spawn();
        }

        /*
        private void OnDrawGizmos() 
        {
            foreach (var top in topSpawnPoints)
            {
                Matrix4x4 preMatrix = Gizmos.matrix;
                Gizmos.color = Color.blue;
                Gizmos.matrix = top.worldToLocalMatrix;
                Gizmos.DrawWireCube(top.position, Vector3.one);
                Gizmos.matrix = preMatrix;
            }
            foreach (var side in sideSpawnPoints)
            {
                Matrix4x4 preMatrix = Gizmos.matrix;
                Gizmos.color = Color.green;
                Gizmos.matrix = side.worldToLocalMatrix;
                Gizmos.DrawWireCube(side.position, Vector3.one);
                Gizmos.matrix = preMatrix;
            }
        }
        */

        public void Spawn(bool spawnOnTop = true)
        {
            List<Vector3> buffer = new List<Vector3>();
            Transform[] spawnPoints = spawnOnTop ? topSpawnPoints : sideSpawnPoints;

            foreach(LootTable.LootTableEntry entry in lootTable.entries)
            {
                int count = UnityEngine.Random.Range(entry.minCount, entry.maxCount + 1);

                for(int i = 0; i < count; i ++)
                {
                    Vector3 position = Vector3.zero;

                    bool keep = true;

                    while(keep)
                    {
                        position = GetRandomPointInBounds(spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].localPosition);

                        keep = buffer.Any(v => Vector3.Distance(v, position) < 1f);
                    }
                    
                    GameObject go = GameObject.Instantiate(entry.prefab, position, transform.rotation, spawnParent);
                    buffer.Add(position);
                    
                    if(!spawnOnTop) go.GetComponent<Rigidbody>().AddForce(new Vector3(-1, UnityEngine.Random.Range(0,1), 0), ForceMode.Impulse);
                }
            }
        }
        
        public Vector3 GetRandomPointInBounds(Vector3 pos) 
        {      
            Vector3 point = pos;
            point += new Vector3(
                UnityEngine.Random.Range(-SIZE, SIZE),
                0,
                UnityEngine.Random.Range(-SIZE, SIZE)
            );
        
            return transform.TransformPoint(point);
        }
    }
}