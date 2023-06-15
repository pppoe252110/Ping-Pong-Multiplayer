using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public int gameScene = 1;
    public int menuScene = 0;

    public NetworkPrefabRef playerPrefab;
    [HideInInspector]public Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();

    private static GameManager instance;
    
    public static GameManager GetInstance()
    {
        if (!instance)
            instance = FindObjectOfType<GameManager>();
        return instance;
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.position = Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition).x;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPoint = new Vector3(0, (player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 6 - 3, 0);
            var networkObj = runner.Spawn(playerPrefab, spawnPoint, Quaternion.identity, player);
            players.Add(player, networkObj);
        }

        Debug.Log("Player Joined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }


    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public NetworkRunner _runner;

    public async void StartGame(GameMode gameMode)
    {
        if (_runner)
            return;
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = "Test",
            Scene = gameScene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2
        });
    }

    public void StartAsHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartAsClient()
    {
        StartGame(GameMode.Client);
    }
}
