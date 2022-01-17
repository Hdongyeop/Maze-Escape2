using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    public string roomName = "";
    public int playerCount = 0;
    public int maxPlayer = 0;
    public bool isLock = false;
    
    public Text roomNameText;
    public Text playerCountText;
    public GameObject lockSprite;

    public void UpdateInfo()
    {
        lockSprite.SetActive(isLock);
        roomNameText.text = roomName;
        playerCountText.text = $"{playerCount} / {maxPlayer}";
    }
}
