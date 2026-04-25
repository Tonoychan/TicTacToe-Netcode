using System;
using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    
    private void OnMouseDown()
    {
        GameManager.instance.ClickedOnGridPositionRpc(x, y, GameManager.instance.GetLocalPlayerType());
    }
}
