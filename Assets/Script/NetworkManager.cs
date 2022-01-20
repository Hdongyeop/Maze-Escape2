using System;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
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

        [Tooltip("True이면 내 턴")]
        public bool turn;
        [Tooltip("하단 인포 텍스트")]
        public Text infoText;
        [Tooltip("플레이어 닉네임 프리팹")]
        public GameObject playerNamePrefab;
        [Tooltip("자신의 닉네임 위치")]
        public Transform namePos1;
        [Tooltip("상대방 닉네임 위치")] 
        public Transform namePos2;
        [Tooltip("미궁 프리팹")]
        public GameObject mazePrefab;
        [Tooltip("자신의 미궁 위치")]
        public Transform mazePos1;
        [Tooltip("상대방 미궁 위치")]
        public Transform mazePos2;
        [Tooltip("레디 버튼 프리팹")]
        public GameObject readyButtonPrefab;
        [Tooltip("자신의 Area")]
        public GameObject area1;
        [Tooltip("상대의 Area")]
        public GameObject area2;

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            InstantiatePrefabs();
        }

        private void Update()
        {
            
        }

        #endregion

        #region MenoBehaviourPunCallbacks CallBacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"{otherPlayer.NickName}이 나갔습니다.");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"{newPlayer.NickName}이 입장했습니다.");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log($"{newMasterClient.NickName}님이 마스터 클라이언트가 되었습니다.");
        }

        #endregion
        
        #region Custom Methods

        public void OnClickLeaveRoomButton()
        {
            Debug.Log("나가기(패배)");
            //패배 처리
            
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(2);
        }

        private void InstantiatePrefabs()
        {
            // 닉네임
            PhotonNetwork.Instantiate(playerNamePrefab.name, Vector3.zero, Quaternion.identity);
            // 미궁
            PhotonNetwork.Instantiate(mazePrefab.name, Vector3.zero, Quaternion.identity);
            // 레디 버튼
            PhotonNetwork.Instantiate(readyButtonPrefab.name, Vector3.zero, Quaternion.identity);
        }

        private void InputProcess()
        {
            
        }
        
        #endregion

        #region PUN RPCs

        /// <summary>
        /// 선택한 위치로 갈 수 있는지, 갈 수 있다면 이동
        /// </summary>
        [PunRPC]
        public void ableDirection()
        {
            
        }

        #endregion
    }
}

