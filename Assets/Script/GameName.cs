using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class GameName : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
    {
        #region Private Fields

        private Text _nickName;

        #endregion
        
        #region Public Fields

        public NetworkManager networkManager;

        #endregion
        
        #region MonoBehaviour CallBacks

        private void Awake()
        {
            _nickName = GetComponent<Text>();
            _nickName.text = PlayerPrefs.GetString("PlayerName");
            networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        }

        #endregion

        #region IPunObservable CallBacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_nickName.text);
            }
            else
            {
                _nickName.text = (string)stream.ReceiveNext();
            }
        }

        #endregion

        #region IPunInstantiateMagicCallback

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Debug.Log($"{_nickName} 생성됨");
            if (photonView.IsMine)
                transform.parent = networkManager.namePos1;
            else
                transform.parent = networkManager.namePos2;
            gameObject.transform.localPosition = Vector3.zero;
        }

        #endregion
    }
}
