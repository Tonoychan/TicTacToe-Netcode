using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform placeSoundPrefab;
    [SerializeField] private Transform winSoundPrefab;
    [SerializeField] private Transform lostSoundPrefab;

    private void Start()
    {
        GameManager.instance.OnObjectPlaced += GameManager_OnObjectPlaced;
        GameManager.instance.OnGameWin += GameManager_OnGameWin;
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (GameManager.instance.GetLocalPlayerType() == e.winPlayerType)
        {
            Transform Sound = Instantiate(winSoundPrefab);
            Destroy(Sound.gameObject,5f);
        }
        else
        {
            Transform Sound = Instantiate(lostSoundPrefab);
            Destroy(Sound.gameObject,5f);
        }
    }

    private void GameManager_OnObjectPlaced(object sender, EventArgs e)
    {
        Transform Sound = Instantiate(placeSoundPrefab);
        Destroy(Sound.gameObject,5f);
    }
}
