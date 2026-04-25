using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;
    [SerializeField] private GameObject crossYouTextGameObject;
    [SerializeField] private GameObject circleYouTextGameObject;
    [SerializeField] private TextMeshProUGUI playerCrossScore_Text;
    [SerializeField] private TextMeshProUGUI playerCircleScore_Text;

    private void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossYouTextGameObject.SetActive(false);
        circleYouTextGameObject.SetActive(false);
        
        playerCrossScore_Text.SetText("");
        playerCircleScore_Text.SetText("");
    }

    private void Start()
    {
        GameManager.instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.instance.OnTurnChanged += GameManager_OnTurnChanged;
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
        GameManager.instance.OnScoreChanged += GameManager_OnScoreChanged;
    }

    private void GameManager_OnScoreChanged(object sender, EventArgs e)
    {
        GameManager.instance.GetScore(out int playerCrossScore, out int playerCircleScore);
        playerCrossScore_Text.SetText(playerCrossScore.ToString());
        playerCircleScore_Text.SetText(playerCircleScore.ToString());
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
       
    }

    private void GameManager_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        if (GameManager.instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            crossYouTextGameObject.SetActive(true);
        }
        else
        {
            circleYouTextGameObject.SetActive(true);
        }

        playerCrossScore_Text.SetText("0");
        playerCircleScore_Text.SetText("0");
        
        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.instance.GetCurrentPlayer() == GameManager.PlayerType.Cross)
        {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false);
        }
        else
        {
            circleArrowGameObject.SetActive(true);
            crossArrowGameObject.SetActive(false);
        }
    }
}
