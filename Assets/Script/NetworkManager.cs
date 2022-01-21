using System;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace Com.Redsea.MazeEscape
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        // 두 명이 모두 레디를 했을 때 flag
        private bool _ready;
        
        #endregion
        
        #region Public Fields

        [Tooltip("True이면 내 턴")]
        public bool turn;
        [Tooltip("하단 인포 텍스트 배경")]
        public GameObject textArea;
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
        [Tooltip("플레이어(초록색)")]
        public GameObject playerPrefab;
        [Tooltip("적(빨간색)")]
        public GameObject enemy;

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
                turn = true;
            
            _ready = false;
            
            InstantiatePrefabs();
        }

        private void Update()
        {
            if(!_ready)
                ReadyCheck();
            else
            {
                InputProcess();
            }
        }

        #endregion

        #region MenoBehaviourPunCallbacks CallBacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"{otherPlayer.NickName}이 나갔습니다.");
            
            // ready false
            _ready = false;
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
            if (turn)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                    EndTurn();
            }
        }

        /// <summary>
        /// 두 플레이어 모두 레디를 했는지 검사하는 함수
        /// </summary>
        public void ReadyCheck()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount != 2)
            {
                textArea.SetActive(false);
                return;
            }
            
            var readyButtons = GameObject.FindGameObjectsWithTag("ReadyButton");
            foreach (var readyButton in readyButtons)
            {
                if (!readyButton.GetComponent<ReadyButton>().ready || readyButtons.Length != 2)
                    return;
            }

            // 두 플레이어 모두 레디를 했을 때
            _ready = true;

            // 플레이어 소환
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);

            // 텍스트 구역을 불러옴
            textArea.SetActive(true);
            
            // 마스터 클라이언트면 턴 정하기
            if(PhotonNetwork.IsMasterClient)
                DecideFirstTurn();
        }

        private void DecideFirstTurn()
        {
            Random ran = new Random();
            if (ran.Next() % 2 == 1)
            {
                // 방장 선공
                turn = true;
                infoText.text = "당신의 턴 입니다.";
                photonView.RPC("SetInfoText", RpcTarget.Others, "상대의 턴 입니다.");
            }
            else
            {
                // 도전자 선공
                turn = false;
                infoText.text = "상대의 턴 입니다.";
                photonView.RPC("SetInfoText", RpcTarget.Others, "당신의 턴 입니다.");
            }
        }

        /// <summary>
        /// 자신의 턴을 종료하고 상대로 넘긴다.
        /// </summary>
        private void EndTurn()
        {
            turn = false;
            infoText.text = "상대의 턴 입니다.";
            photonView.RPC("YieldTurn", RpcTarget.Others);
            photonView.RPC("SetInfoText", RpcTarget.Others, "당신의 턴 입니다.");
        }
        
        #endregion

        #region PUN RPCs
        
        /// <summary>
        /// 선택한 위치로 갈 수 있는지, 갈 수 있다면 이동
        /// </summary>
        [PunRPC]
        public void AbleDirection(int fromIndex, int toIndex)
        {
            // from -> to
            
        }

        /// <summary>
        /// 턴 넘기기에 필요한 RPC
        /// </summary>
        [PunRPC]
        public void YieldTurn()
        {
            turn = true;
        }

        [PunRPC]
        public void SetInfoText(string str)
        {
            infoText.text = str;
        }
        
        #endregion
    }
}

