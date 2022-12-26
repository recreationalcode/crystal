using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShadowManager : NetworkBehaviour
{
    public float timeBetweenSpawns;
    private float timeBetweenAISpawns;
    private float maxTimeBetweenSpawns;
    private float minTimeBetweenSpawns = 1f;

    public static float elapsedTime = 0f;
    private static float elapsedTimeOffset;

    [SerializeField] private NetworkPrefabRef _shadowShipPrefab;
    [SerializeField] private NetworkPrefabRef _playerAIPrefab;

    public static HashSet<NetworkObject> shadowShips = new HashSet<NetworkObject>();
    public static HashSet<NetworkObject> playerAIShips = new HashSet<NetworkObject>();

    private IEnumerator spawnCoroutine;

    private void Awake()
    {
        maxTimeBetweenSpawns = timeBetweenSpawns;
        timeBetweenAISpawns = 5 * timeBetweenSpawns;
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        elapsedTimeOffset = Time.realtimeSinceStartup;

        spawnCoroutine = SpawnShadowShips();

        StartCoroutine(spawnCoroutine);

        // TODO Remove when AI is no longer required for demo purposes
        StartCoroutine(SpawnPlayerAIShips());
    }

    public override void FixedUpdateNetwork() {
        elapsedTime = Time.realtimeSinceStartup - elapsedTimeOffset;

        timeBetweenSpawns = Mathf.Max(minTimeBetweenSpawns, maxTimeBetweenSpawns -
            ((maxTimeBetweenSpawns / 2f) * (GridManager.highestCrystalSize / 30)) -
            ((maxTimeBetweenSpawns / 2f) * (Runner.SessionInfo.PlayerCount + playerAIShips.Count / 10f)));
    }

    public static float GetShadowHealthFactor()
    {
        return 1 + (ShadowManager.elapsedTime / 60 / 2);
    }

    public static float GetShadowDamageFactor()
    {
        return 1 + (ShadowManager.elapsedTime / 60 / 2);
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
            shadowShips.Add(shadowShip);
        }
    }

    private IEnumerator SpawnPlayerAIShips()
    {         
        while (true)
        {
            // if (playerAIShips.Count == 10) yield break;

            NetworkObject playerAIShip = Runner.Spawn(_playerAIPrefab, RandomShadowSpawnPoint(), Quaternion.identity);
            playerAIShips.Add(playerAIShip);

            yield return new WaitForSeconds(timeBetweenAISpawns);
        }
    }

    void OnGUI ()
    {
        GUI.Label (new Rect (Screen.width - 100,Screen.height - 50,100,50), "Shadow Spawn: " + timeBetweenSpawns);
    }
}
