using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Combat;
using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Levels
{
    public class PalaceFloor : MonoBehaviour
    {
        //List of enemies to spawn from
        
        //List of spawned enemies

        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask obstacleLayerMask;
        
        private bool _spawned;

        public GameObject portalPrefab;
        public List<GameObject> enemyPrefabs;

        private List<GameObject> _spawnedEnemies = new List<GameObject>();

        private void Update()
        {
            if (_spawned)
            {
                bool allKilled = _spawnedEnemies.Count == _spawnedEnemies.Count(enemy => enemy == null);
                if (allKilled) SpawnLevelEnd();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (playerMask.HasLayer(other.gameObject.layer))
            {
               if (!_spawned) SpawnEnemies();   
            }
        }

        private void SpawnEnemies()
        {
            _spawned = true;
            int enemiesToSpawn = Mathf.RoundToInt(Mathf.Ceil(LevelGenerator.Instance.currentLevel / 10.0f) * 3);
            int enemiesSpawned = 0;
            int c = 0;

            while (enemiesSpawned < enemiesToSpawn && c < 1000)
            {
                Vector2 posToSpawn = Random.insideUnitCircle * 75;
                Vector3 actualPos = new Vector3(posToSpawn.x, transform.position.y + 0.5f, posToSpawn.y);

                if (Physics.OverlapSphere(posToSpawn, 3, obstacleLayerMask).Length == 0)
                {
                    GameObject spawnedEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], actualPos, Quaternion.identity);
                    _spawnedEnemies.Add(spawnedEnemy);
                    enemiesSpawned++;
                }
                
                c++;
            }
        }

        private void SpawnLevelEnd()
        {
            Instantiate(portalPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform.parent);
        }
    }
}
