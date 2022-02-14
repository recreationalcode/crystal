using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _shipPrefab;
    [SerializeField] private NetworkPrefabRef _shadowManagerPrefab;
    [SerializeField] private NetworkPrefabRef _gridManagerPrefab;

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            // TODO Replace static cell center with something dynamic for multiplayer
            Vector3 spawnPosition = GridManager.GetCellCenter(new Vector2(0, 1));

            NetworkObject networkPlayerObject = runner.Spawn(_shipPrefab, spawnPosition, Quaternion.identity, player);

            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);  
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
      // Find and remove the players avatar
      if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
      {
        runner.Despawn(networkObject);
        _spawnedCharacters.Remove(player);
      }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
      var data = new NetworkInputData();

      data.direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      data.isFiring = Input.GetKeyDown("space") || Input.GetKey("space");

      input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }


    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });


        // Spawn Managers
        if (mode == GameMode.Host)
        {
            _runner.Spawn(_shadowManagerPrefab, Vector3.zero, Quaternion.identity);
            _runner.Spawn(_gridManagerPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void OnGUI()
    {
      if (_runner == null)
      {
        if (GUI.Button(new Rect(0,0,200,40), "Host"))
        {
            StartGame(GameMode.Host);
        }
        if (GUI.Button(new Rect(0,40,200,40), "Join"))
        {
            StartGame(GameMode.Client);
        }
      }
    }
}