using System;
using System.Collections;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Redsea.MazeEscape
{
    public class GreenPlayer : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPunObservable
    {
        #region Private Fields

        private bool _finished = false;
        private bool _onceFlag = true;
        private NetworkManager _networkManager;
        private MazeData _mazeData;
        private Maze _curMaze;
        private GreenPlayer _otherPlayer;

        #endregion

        #region Public Fields

        [Tooltip("플레이어의 현재 위치 인덱스")]
        public int curPlayerIndex = -55;

        //public bool[] temp;

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
                
                // 벽 처리를 위한 깡통 미궁 불러오기
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
                Debug.Log($"미궁 데이터 깡통임");
                _mazeData = new MazeData();
                
                // 벽 처리를 위한 깡통 미궁 불러오기
                var mazes = GameObject.FindGameObjectsWithTag("Maze");
                foreach (var maze in mazes)
                {
                    if (!maze.GetPhotonView().IsMine)
                    {
                        _curMaze = maze.GetComponent<Maze>(); // 그냥 깡통
                    }
                }
            }
        }

        private void Start()
        {
            Debug.Log("GreenPlayer : Start");
        }

        private void Update()
        {
            if (photonView.IsMine && _networkManager._ready)
            {
                // 한 번만
                if (_onceFlag)
                {
                    _otherPlayer = FindOtherPlayer();
                    if (_otherPlayer != null)
                    {
                        curPlayerIndex = _otherPlayer._mazeData.startIndex;
                        Debug.Log($"상대 플레이어를 찾았습니다. 상대 플레이어의 시작 지점은 {curPlayerIndex}번 입니다.");
                    
                        // 미궁 bar들 표시
                        for (int i = 0; i < _mazeData.onOffData.Length; i++)
                        {
                            if(_mazeData.onOffData[i])
                                _otherPlayer._curMaze.bars[i].GetComponent<SpriteRenderer>().color = Color.black;
                        }
                    }
                }

                if (_networkManager.turn)
                {
                    InputProcess();
                }

                if (_otherPlayer != null)
                {
                    //Debug.Log($"CurPlayerIndex update : {curPlayerIndex}");
                    var tmp = _curMaze.positions[curPlayerIndex].transform.position;
                    tmp.x += 317f;
                    gameObject.transform.localPosition = tmp;
                }
                
                if(!_finished)
                    CheckFinish();
            }
        }

        #endregion

        #region Custom Methods

        private void CheckFinish()
        {
            if (curPlayerIndex == _otherPlayer._mazeData.endIndex)
            {
                _finished = true;
             
                var WIN = PlayerPrefs.GetInt("WIN");
                var LOSE = PlayerPrefs.GetInt("LOSE");
                
                // 승리, 패배 추가
                _networkManager.infoText.text = "도착지점에 먼저 도달했습니다. 승리!!";
                PlayerPrefs.SetInt("WIN", WIN + 1);
                _networkManager.LappedSetInfoText("상대가 먼저 도착지점에 도달했습니다. 패배");
                photonView.RPC("AddLose", RpcTarget.Others);
                
                // 로비로
                StartCoroutine(WaitSeconds(2f));
            }
        }

        private IEnumerator WaitSeconds(float time)
        {
            yield return new WaitForSeconds(time);
            photonView.RPC("GoToLobby", RpcTarget.Others);
        }
        
        private void InputProcess()
        {
            var y = curPlayerIndex / 5;
            var x = curPlayerIndex % 5;
        
            var upIndex = (y * 11) + x;
            var leftIndex = upIndex + 5;
            var downIndex = leftIndex + 6;
            var rightIndex = downIndex - 5;
            
            // 상,하,좌,우 입력
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!_otherPlayer._mazeData.onOffData[upIndex])
                {
                    curPlayerIndex -= 5;
                    _networkManager.infoText.text = "이동에 성공했습니다.";
                    _networkManager.EndTurn();    
                }
                else
                {
                    _networkManager.infoText.text = "이동에 실패했습니다.";
                    // 부딪혔던 bar는 빨간색으로 색상변경
                    _curMaze.bars[upIndex].GetComponent<SpriteRenderer>().color = Color.red;
                    _networkManager.EndTurn();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (!_otherPlayer._mazeData.onOffData[downIndex])
                {
                    curPlayerIndex += 5;
                    _networkManager.infoText.text = "이동에 성공했습니다.";
                    _networkManager.EndTurn();    
                }
                else
                {
                    _networkManager.infoText.text = "이동에 실패했습니다.";
                    // 부딪혔던 bar는 빨간색으로 색상변경
                    _curMaze.bars[downIndex].GetComponent<SpriteRenderer>().color = Color.red;
                    _networkManager.EndTurn();
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!_otherPlayer._mazeData.onOffData[leftIndex])
                {
                    curPlayerIndex -= 1;
                    _networkManager.infoText.text = "이동에 성공했습니다.";
                    _networkManager.EndTurn();    
                }
                else
                {
                    _networkManager.infoText.text = "이동에 실패했습니다.";
                    // 부딪혔던 bar는 빨간색으로 색상변경
                    _curMaze.bars[leftIndex].GetComponent<SpriteRenderer>().color = Color.red;
                    _networkManager.EndTurn();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!_otherPlayer._mazeData.onOffData[rightIndex])
                {
                    curPlayerIndex += 1;
                    _networkManager.infoText.text = "이동에 성공했습니다.";
                    _networkManager.EndTurn();    
                }
                else
                {
                    _networkManager.infoText.text = "이동에 실패했습니다.";
                    // 부딪혔던 bar는 빨간색으로 색상변경
                    _curMaze.bars[rightIndex].GetComponent<SpriteRenderer>().color = Color.red;
                    _networkManager.EndTurn();
                }
            }
        }

        private GreenPlayer FindOtherPlayer()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            Debug.LogWarning($"players length : {players.Length}");
            foreach (var player in players)
            {
                if (player.GetPhotonView().IsMine == false)
                {
                    // 한 번만 하도록
                    _onceFlag = false;
                    return player.GetComponent<GreenPlayer>();
                }
            }
            return null;
        }
        
        #endregion

        #region MonoBehaviourPunCallbacks CallBacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // 누군가 나가면 자신을 파괴한다.
            if(photonView.IsMine)
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

        #region PUN RPCs

        [PunRPC]
        private void AddWin()
        {
            var WIN = PlayerPrefs.GetInt("WIN");
            PlayerPrefs.SetInt("WIN", WIN + 1);
        }

        [PunRPC]
        private void AddLose()
        {
            var LOSE = PlayerPrefs.GetInt("LOSE");
            PlayerPrefs.SetInt("LOSE", LOSE + 1);
        }

        [PunRPC]
        private void GoToLobby()
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(2);
        }
        
        #endregion
    }
}
