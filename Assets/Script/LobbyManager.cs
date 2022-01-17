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
        private List<RoomInfo> _totalRoomList;

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
        [Tooltip("비밀번호 물어보는 UI")]
        public GameObject askPasswordUI;
        [Tooltip("비밀번호 물어보는 UI 중 Enter Btn")]
        public GameObject askPasswordUIEnterBtn;
        
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
            _totalRoomList = new List<RoomInfo>();
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
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("룸에 입장했습니다.");
                PhotonNetwork.LoadLevel(2);
            }
            else
            {
                if (PhotonNetwork.InRoom)
                {
                    var room = PhotonNetwork.CurrentRoom;
                    if (room == null)
                    {
                        Debug.Log("룸이 없습니다.");
                        return;
                    }

                    // 비밀번호 체크
                    Hashtable cp = room.CustomProperties;
                    if (askPasswordUI.GetComponentInChildren<InputField>().text == (string)cp["P"])
                    {
                        Debug.Log("룸에 입장했습니다.");
                        PhotonNetwork.LoadLevel(2);
                    }
                    else
                    {
                        Debug.Log("비밀번호가 틀렸습니다.");
                        PhotonNetwork.LeaveRoom();
                        //TODO 비밀번호 틀렸을 때 효과
                    }
                }
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("로비에 접속하였습니다.");
            // 방 리스트 2초마다 갱신
            //InvokeRepeating("OnRoomListUpdate", 0f, 2f);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("방 입장에 실패했습니다. MSG : " + message);
        }

        /// <summary>
        /// 이 함수가 실행되는 경우는 다음과 같다
        /// https://gimchicchige-mukgoshipda-1223.tistory.com/33
        /// 따라서 전체 roomList를 선언해서 따로 관리해야 한다.
        /// </summary>
        /// <param name="roomList"></param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log($"룸 갯수 : {roomList.Count}");
            
            foreach (var obj in GameObject.FindGameObjectsWithTag("ROOM"))
            {
                Destroy(obj);
            }

            foreach (var roomInfo in roomList)
            {
                // TODO 성능저하 가능성?
                // 중복된 room은 total에 넣지 않는다.
                if(!_totalRoomList.Contains(roomInfo))
                    _totalRoomList.Add(roomInfo);

                if (roomInfo.PlayerCount == 0 || roomInfo.MaxPlayers == 0)
                {
                    Debug.Log($"{roomInfo.Name}의 플레이어가 0명이기 때문에 패스");
                    _totalRoomList.Remove(roomInfo);
                }
                
            }
            
            foreach (var roomInfo in _totalRoomList)
            {
                Debug.Log($"룸 이름 : {roomInfo.Name}");
                
                GameObject room = Instantiate(roomPrefab);
                room.transform.parent = roomPanelUI.transform;

                RoomData roomData = room.GetComponent<RoomData>();
                roomData.roomName = roomInfo.Name;
                roomData.maxPlayer = roomInfo.MaxPlayers;
                roomData.playerCount = roomInfo.PlayerCount;
                roomData.isLock = (string)roomInfo.CustomProperties["P"] == "" ? false : true;
                roomData.UpdateInfo();
                roomData.GetComponent<Button>().onClick.AddListener
                (
                    delegate
                    {
                        OnClickRoom(roomData.roomName, roomData.isLock);
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
                winRate.text = (win / (win + lose)) * 100 + " %";
            }
        }

        /// <summary>
        /// 방 만들기에서 확인 버튼 method
        /// </summary>
        public void MakeRoom()
        {
            if (_isConnceted)
            {
                var roomName = makeRoomName.text;
                var password = makeRoomPassword.text;

                if (roomName == "")
                {
                    Debug.Log("방 제목은 필수로 입력해야 합니다.");
                    //TODO 방 제목 입력안했을 때 처리
                    return;
                }
                
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
        
        /// <summary>
        /// 로비에 있는 방을 클릭할 때 호출되는 Method
        /// </summary>
        /// <param name="roomName">룸 이름에 따라 접속한다</param>
        /// <param name="isLock">비밀번호가 걸려있는지 아닌지</param>
        void OnClickRoom(string roomName, bool isLock)
        {
            Debug.Log("방을 클릭했습니다.");
            if (isLock)
            {
                // 비밀번호 물어보는 UI 띄우기
                askPasswordUI.SetActive(true);
                askPasswordUI.GetComponentInChildren<InputField>().text = "";
                askPasswordUIEnterBtn.GetComponent<Button>().onClick.AddListener(() => { PhotonNetwork.JoinRoom(roomName, null); });
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName, null);
            }
        }
        
        #endregion
    }
}

