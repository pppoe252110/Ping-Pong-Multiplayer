using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UI;

public class PingPongManager : NetworkBehaviour
{
    private bool meReady = false;
    private bool otherPlayerReady = false;

    private NetworkObject spawnedBall;
    public NetworkPrefabRef ballPrefab;

    public Text ClientPoints;
    public Text ServerPoints;

    public GameObject otherPlayerNotReady;
    public GameObject localPlayerNotReady;

    private static PingPongManager instance;

    public int serverPoints = 0;
    public int clientPoints = 0;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddPoints(TargtPlayer target)
    {
        switch (target)
        {
            case TargtPlayer.Server:
                serverPoints++;
                break;
            case TargtPlayer.Client:
                clientPoints++;
                break;
        }
        UpdateScore();
    }

    public void AddServerPoints()
    {
        RPC_AddPoints(TargtPlayer.Server);
    }

    public void AddClientPoints()
    {
        RPC_AddPoints(TargtPlayer.Client);
    }

    public void UpdateScore()
    {
        ServerPoints.gameObject.SetActive(true);
        ClientPoints.gameObject.SetActive(true);

        if (GameManager.GetInstance()._runner.IsServer)
        {
            ServerPoints.text = serverPoints.ToString();
            ClientPoints.text = clientPoints.ToString();
        }
        else
        {
            ClientPoints.text = serverPoints.ToString();
            ServerPoints.text = clientPoints.ToString();
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public override void Spawned()
    {
        if (!GameManager.GetInstance()._runner.IsServer)
        {
            Camera.allCameras[0].transform.rotation = Quaternion.Euler(0, 0, 180);
            RPC_SceneLoaded();
        }
    }

    public static PingPongManager GetInstance()
    {
        if (!instance)
            instance = FindObjectOfType<PingPongManager>();
        return instance;
    }

    public void Start()
    {
        localPlayerNotReady.SetActive(false);
        otherPlayerNotReady.SetActive(false);
    }

    public void SetReady()
    {
        RPC_SetReady();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SceneLoaded(RpcInfo info = default)
    {
        ShowReadyUI();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetReady(RpcInfo info = default)
    {
        if (info.IsInvokeLocal)
        {
            meReady = true;
            localPlayerNotReady.SetActive(false);
        }
        else
        {
            otherPlayerReady = true;
            otherPlayerNotReady.SetActive(false);
        }

        if (meReady&&otherPlayerReady)
        {
            if (Runner.IsServer)
            {
                spawnedBall = Runner.Spawn(ballPrefab, Vector3.zero, Quaternion.identity);
            }
        }
    }

    public void ShowReadyUI()
    {
        localPlayerNotReady.SetActive(true);
        otherPlayerNotReady.SetActive(true);

    }
}
public enum TargtPlayer
{
    Server, Client
}