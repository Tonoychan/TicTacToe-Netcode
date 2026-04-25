using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefab;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform lineCompletePrefab;
    
    private const float GRID_SIZE = 3.1f;
    
    private List<GameObject> visualGameObjects;

    private void Awake()
    {
        visualGameObjects =  new List<GameObject>();
    }

    private void Start()
    {
        GameManager.instance.OnGridPositionClicked += GameManager_OnClickedGridPosition;
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
        GameManager.instance.OnRematch += GameManager_OnRematch;
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        foreach (GameObject go in visualGameObjects)
        {
            Destroy(go);
        }
        visualGameObjects.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        float eulerZ = 0f;
        switch (e.line.orientation)
        {
            case GameManager.Orientation.Horizontal:
                eulerZ = 0f;
                break;
            case GameManager.Orientation.Vertical:
                eulerZ = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                eulerZ = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                eulerZ = -45f;
                break;
        }
        
        Transform lineCompleteTransform = 
            Instantiate(lineCompletePrefab,GetWorldGridPosition(e.line.centerGridPosition.x,e.line.centerGridPosition.y),
                Quaternion.Euler(0f,0f,eulerZ));
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjects.Add(lineCompleteTransform.gameObject);
    }

    private void GameManager_OnClickedGridPosition(object sender, GameManager.OnClickedGridPositionEventArgs e)
    {
        SpawnObjectRpc(e.x,e.y,e.playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }
        Transform spawned = Instantiate(prefab,GetWorldGridPosition(x, y), Quaternion.identity);
        spawned.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjects.Add(spawned.gameObject);
    }

    private Vector2 GetWorldGridPosition(int x, int y)
    {
        Vector2 gridPosition = new Vector2(-GRID_SIZE + x * GRID_SIZE,-GRID_SIZE + y * GRID_SIZE);
        return gridPosition;
    }
}
