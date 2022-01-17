using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields
        
        

        #endregion
        
        #region Public Fields

        public static NetworkManager Instance;

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            Instance = this;
        }

        #endregion

        #region MenoBehaviourPunCallbacks CallBacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.IsMasterClient)
            {
                Debug.Log("방장이 나갔습니다.");
            }
            else
            {
                Debug.Log("도전자가 나갔습니다.");
            }
        }

        #endregion
        
        #region Custom Methods

        public void OnClickLeaveRoomButton()
        {
            Debug.Log("항복하기");
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(1);
        }

        #endregion
    }
}

