using System;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Com.Redsea.MazeEscape
{
    public class GreenPlayer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
    {
        #region Private Fields

        private bool _onceFlag = true;
        private NetworkManager _networkManager;
        private MazeData _mazeData;
        private Maze _curMaze;
        private GreenPlayer _otherPlayer;

        #endregion

        #region Public Fields

        [Tooltip("플레이어의 현재 위치 인덱스")]
        public int curPlayerIndex = -55;

        #endregion
        
        #region MonoBehaviour CallBacks

        private void Awake()
        {
            Debug.Log("GreenPlayer : Awake");
            _networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
            
            if (photonView.IsMine)
            {
                // 미궁 프리셋 불러오기
                string json = File.ReadAllText(Application.dataPath + $"/Json/Preset_{PlayerPrefs.GetInt("PRESET_NUMBER")}.json");
                _mazeData = JsonUtility.FromJson<MazeData>(json);
                var mazes = GameObject.FindGameObjectsWithTag("Maze");
                foreach (var maze in mazes)
                {
                    if (maze.GetPhotonView().IsMine)
                    {
                        _curMaze = maze.GetComponent<Maze>(); // 그냥 깡통
                    }
                }
            }
            else
            {
                _mazeData = new MazeData();
            }
        }

        private void Start()
        {
            Debug.Log("GreenPlayer : Start");
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (_onceFlag)
                {
                    _otherPlayer = FindOtherPlayer();
                    MoveToStart();
                }
                InputProcess();

                if (_otherPlayer != null)
                {
                    var tmp = _curMaze.positions[curPlayerIndex].transform.position;
                    tmp.x += 317f;
                    gameObject.transform.localPosition = tmp;
                }
            }
        }

        #endregion

        #region Custom Methods

        private void InputProcess()
        {
            
        }

        private GreenPlayer FindOtherPlayer()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            Debug.LogWarning($"players length : {players.Length}");
            foreach (var player in players)
            {
                if (!player.GetPhotonView().IsMine)
                {
                    // 한 번만 하도록
                    _onceFlag = false;
                    return player.GetComponent<GreenPlayer>();
                }
            }
            return null;
        }

        private void MoveToStart()
        {
            if (_otherPlayer != null)
            {
                // 현재 플레이어 인덱스를 다른 플레이어의 미궁 시작 인덱스로
                curPlayerIndex = _otherPlayer._mazeData.startIndex;
            }
        }
        
        #endregion

        #region MonoBehaviourPunCallbacks CallBacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // 누군가 나가면 자신을 파괴한다.
            PhotonNetwork.Destroy(gameObject);
        }

        #endregion

        #region IPunInstantiateMagicCallback

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (photonView.IsMine)
            {
                transform.parent = _networkManager.area1.transform;
                gameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.parent = _networkManager.area2.transform;
                gameObject.transform.localPosition = Vector3.zero;
            }
        }

        #endregion

        #region IPunObservable

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //Debug.Log("GreenPlayer : OnPhotonSerializeView");
            if (stream.IsWriting)
            {
                stream.SendNext(_mazeData.onOffData);
                stream.SendNext(_mazeData.startIndex);
                stream.SendNext(_mazeData.endIndex);
                stream.SendNext(curPlayerIndex);
            }
            else
            {
                _mazeData.onOffData = (bool[])stream.ReceiveNext();
                _mazeData.startIndex = (int)stream.ReceiveNext();
                _mazeData.endIndex = (int)stream.ReceiveNext();
                curPlayerIndex = (int) stream.ReceiveNext();
            }
        }

        #endregion
    }
    
}
