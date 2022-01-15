using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class TitleManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields
        
        [Tooltip("각 방의 최대 인원")]
        private string gameVersion = "1";
        private bool isConnecting;

        #endregion

        #region Public Fields

        
        
        #endregion

        #region MonoBehavior CallBacks

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        #endregion

        #region MonoBehaviorPunCallBacks CallBacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("TitleManager : OnConnectedToMaster");
        }

        #endregion
        
    }
}
