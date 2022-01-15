using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace Com.Redsea.MazeEscape
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        private bool _isConnceted;
        [SerializeField] private byte maxPlayersPerRoom = 2;
        
        
        #endregion
        
        #region Public Fields

        [Tooltip("플레이어 닉네임")]
        public Text nickName;
        [Tooltip("승패")]
        public Text winLose;
        [Tooltip("승률")] 
        public Text winRate;
        [Tooltip("방 만들기 UI")]
        public GameObject makeRoomUI;
        [Tooltip("전체 레이아웃")]
        public GameObject layoutUI;
        [Tooltip("서버 커넥트 메세지")]
        public GameObject serverConnectMsg;
        [Tooltip("만들려는 방 이름")]
        public Text makeRoomName;
        [Tooltip("만들려는 방 비밀번호")]
        public InputField makeRoomPassword;
        [Tooltip("룸 프리팹")]
        public GameObject roomPrefab;
        [Tooltip("룸 판넬")]
        public GameObject roomPanelUI;
        
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        #endregion

        #region MonoBehaviourPUNCallbacks CallBacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("서버에 접속 되었습니다.");
            _isConnceted = true;
            serverConnectMsg.SetActive(false);
            InitInfo();

            PhotonNetwork.JoinLobby();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("룸을 생성했습니다.");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("룸에 입장했습니다.");
            PhotonNetwork.LoadLevel(2);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("로비에 접속하였습니다.");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("룸 리스트 업데이트");
            foreach (var roomInfo in roomList)
            {
                GameObject _room = Instantiate(roomPrefab);
                _room.transform.parent = roomPanelUI.transform;
                RoomData roomData = _room.GetComponent<RoomData>();
                roomData.roomName = roomInfo.Name;
                roomData.maxPlayer = roomInfo.MaxPlayers;
                roomData.playerCount = roomInfo.PlayerCount;
                roomData.UpdateInfo();
                roomData.GetComponent<Button>().onClick.AddListener
                (
                    delegate
                    {
                        OnClickRoom(roomData.roomName);
                    }
                );
            }
        }

        #endregion
        
        #region Custom Methods

        /// <summary>
        /// PlayerPrefs에서 닉네임, 승수, 패수, 승률을 가져와서 text에 입력
        /// </summary>
        private void InitInfo()
        {
            // 레이아웃 나타내기
            layoutUI.SetActive(true);
            
            // 닉네임 가져오기
            nickName.text = PlayerPrefs.GetString("PlayerName");
            PhotonNetwork.NickName = nickName.text;

            // 승패 가져오기
            var win = PlayerPrefs.GetInt("WIN");
            var lose = PlayerPrefs.GetInt("LOSE");

            winLose.text = $"{win} / {lose}";
            
            // 승률 가져오기
            if (win == 0 || win + lose == 0)
            {
                winRate.text = "0 %";
            }
            else
            {
                winRate.text = (win / (win + lose)) + " %";
            }
        }

        public void MakeRoom()
        {
            if (_isConnceted)
            {
                var roomName = makeRoomName.text;
                var password = makeRoomPassword.text;
                
                PhotonNetwork.CreateRoom(
                    roomName, 
                    new RoomOptions()
                    {
                        MaxPlayers = maxPlayersPerRoom,
                        IsVisible = true,
                        IsOpen = true,
                        CustomRoomProperties = new Hashtable()
                        {
                            {"P", password}
                        },
                        CustomRoomPropertiesForLobby = new string[]
                        {
                            "P"
                        }
                    },
                    null
                    );
            }
        }
        
        void OnClickRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName, null);
        }
        
        #endregion
    }
}

