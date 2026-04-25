using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }

    private PlayerType[,] playerTypesArray;

    public event EventHandler<OnClickedGridPositionEventArgs> OnGridPositionClicked;

    public class OnClickedGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }
    
    public event EventHandler OnGameStarted;
    public event EventHandler OnTurnChanged;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnRematch;
    public event EventHandler OnGameTied;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnObjectPlaced;

    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
    
    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,
    }
    
    public struct Line
    {
        public List<Vector2Int> gridVector2IntList;
        public Vector2Int centerGridPosition;
        public Orientation orientation;
    }
    
    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayer =  new NetworkVariable<PlayerType>(PlayerType.None,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    private List<Line> lineList;
    private NetworkVariable<int> playerCrossScore =  new NetworkVariable<int>();
    private NetworkVariable<int> playerCircleScore = new NetworkVariable<int>();
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        playerTypesArray = new PlayerType[3, 3];
        lineList = new List<Line>
        {
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 1),
                },
                centerGridPosition = new Vector2Int(1, 0),
                orientation = Orientation.Horizontal
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1),
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Horizontal
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2),
                },
                centerGridPosition = new Vector2Int(1, 2),
                orientation = Orientation.Horizontal
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                },
                centerGridPosition = new Vector2Int(0, 1),
                orientation = Orientation.Vertical
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 2),
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Vertical
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(2, 0),
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 2),
                },
                centerGridPosition = new Vector2Int(2, 1),
                orientation = Orientation.Vertical
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 2),
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalA
            },
            new Line
            {
                gridVector2IntList = new List<Vector2Int>
                {
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 0),
                },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalB
            }
        };
    }
    
    
    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        currentPlayer.OnValueChanged += (value, newValue) =>
        {
            OnTurnChanged?.Invoke(this, EventArgs.Empty);
        };

        playerCrossScore.OnValueChanged += (value, newValue) =>
        {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
        };
        playerCircleScore.OnValueChanged += (value, newValue) =>
        {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayer.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        if (playerType != currentPlayer.Value)
        {
            return;
        }

        if (playerTypesArray[x, y] != PlayerType.None)
        {
            return;
        }
        playerTypesArray[x, y] = playerType;
        TriggerOnObjectPlaceRpc();

        OnGridPositionClicked?.Invoke(this, new OnClickedGridPositionEventArgs
        {
            x = x,
            y = y,
            playerType = currentPlayer.Value
        });

        switch (currentPlayer.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayer.Value = PlayerType.Circle;
            break;
            case PlayerType.Circle:
                currentPlayer.Value = PlayerType.Cross;
            break;
        }
        TestWinner();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnObjectPlaceRpc()
    {
        OnObjectPlaced?.Invoke(this, EventArgs.Empty);
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return aPlayerType!=PlayerType.None &&
               aPlayerType == bPlayerType &&
               bPlayerType == cPlayerType;
    }
    
    private bool TestWinnerLine(Line line)
    {
       return TestWinnerLine(
            playerTypesArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y],
            playerTypesArray[line.gridVector2IntList[1].x, line.gridVector2IntList[1].y],
            playerTypesArray[line.gridVector2IntList[2].x, line.gridVector2IntList[2].y]);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winningPlayerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line =  line,
            winPlayerType =  winningPlayerType
        });
    }

    private void TestWinner()
    {
        for(int i=0;i<lineList.Count;i++)
        {
            Line line = lineList[i];
            if (TestWinnerLine(line))
            {
                Debug.Log("Player Wins!");
                currentPlayer.Value = PlayerType.None;
                PlayerType winner = playerTypesArray[line.gridVector2IntList[0].x, line.gridVector2IntList[0].y];
                switch (winner)
                {
                    case PlayerType.Cross:
                        playerCrossScore.Value++;
                        break;
                    case PlayerType.Circle:
                        playerCircleScore.Value++;
                        break;
                }
                TriggerOnGameWinRpc(i,winner);
                return;
            }
        }

        bool isTied = true;
        for (int x = 0; x < playerTypesArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypesArray.GetLength(1); y++)
            {
                if (playerTypesArray[x, y] == PlayerType.None)
                {
                    isTied = false;
                    break;
                }
            }
        }

        if (isTied)
        {
            TriggerOnGameTiedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameTiedRpc()
    {
        OnGameTied?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int x = 0; x < playerTypesArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypesArray.GetLength(1); y++)
            {
                playerTypesArray[x, y] = PlayerType.None;
            }
        }
        currentPlayer.Value = PlayerType.Cross;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayer()
    {
        return currentPlayer.Value;
    }

    public void GetScore(out int playerCrossScore, out int playerCircleScore)
    {
        playerCrossScore = this.playerCrossScore.Value;
        playerCircleScore = this.playerCircleScore.Value;
    }


}
