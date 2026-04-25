using System;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI result_Text;
    [SerializeField] private Color win_color;
    [SerializeField] private Color lose_color;
    [SerializeField] private Color tie_color;
    [SerializeField] private Button rematchButton;

    private void Awake()
    {
        rematchButton.onClick.AddListener(() =>
        {
            GameManager.instance.RematchRpc();
        });
    }


    private void Start()
    {
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
        GameManager.instance.OnRematch += GameManager_OnRematch;
        GameManager.instance.OnGameTied += GameManager_OnGameTied;
        Hide();
    }

    private void GameManager_OnGameTied(object sender, EventArgs e)
    {
        Show();
        result_Text.SetText("TIED!");
        result_Text.color = tie_color;
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.instance.GetLocalPlayerType())
        {
            result_Text.SetText("You Win!");
            result_Text.color = win_color;
        }
        else
        {
            result_Text.SetText("You Lose!");
            result_Text.color = lose_color;
        }
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
