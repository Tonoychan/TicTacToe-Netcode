using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button host_Button;
    [SerializeField] private Button join_Button;

    private void Awake()
    {
        host_Button.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartHost();
            HideUI();
        }));
        
        join_Button.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartClient();
            HideUI();
        }));
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }
}
