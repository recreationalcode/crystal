using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShadowManager : NetworkBehaviour
{
    public float timeBetweenSpawns;

    private float maxTimeBetweenSpawns;
    private float minTimeBetweenSpawns = 1f;

    [SerializeField] private NetworkPrefabRef _shadowShipPrefab;
    [SerializeField] private NetworkPrefabRef _playerAIPrefab;

    private IEnumerator spawnCoroutine;

    private void Awake()
    {
        maxTimeBetweenSpawns = timeBetweenSpawns;
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        // Player AIs
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
        
        spawnCoroutine = SpawnShadowShips();

        StartCoroutine(spawnCoroutine);
    }

    public override void FixedUpdateNetwork() {
        timeBetweenSpawns = Mathf.Max(minTimeBetweenSpawns, maxTimeBetweenSpawns -
            ((maxTimeBetweenSpawns / 2f) * (GridManager.crystal.Count / 50f)) -
            ((maxTimeBetweenSpawns / 2f) * (Runner.SessionInfo.PlayerCount / 16f)));
    }

    private Vector3 RandomShadowSpawnPoint() {
        Vector2 axialCoordinates = GridManager.shadowOutline[Random.Range(0, GridManager.shadowOutline.Count - 1)];
        Vector3 spawnPoint = GridManager.GetCellCenter(axialCoordinates);
        
        return spawnPoint + new Vector3(0, Ship.altitude, 0);
    }

    private IEnumerator SpawnShadowShips()
    {         
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            NetworkObject shadowShip = Runner.Spawn(_shadowShipPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
            shadowShip.transform.parent = transform;
        }
    }

    void OnGUI ()
    {
        GUI.Label (new Rect (Screen.width - 100,Screen.height - 50,100,50), "Shadow Spawn: " + timeBetweenSpawns);
    }
}
