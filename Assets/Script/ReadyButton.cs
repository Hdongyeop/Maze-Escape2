using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class ReadyButton : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
    {
        #region Private Fields

        private NetworkManager _netWorkManager;

        #endregion
        
        #region Public Fields

        public bool ready;
        public Text readyText;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            ready = false;
            if(GameObject.FindGameObjectWithTag("NetworkManager") != null)
                _netWorkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// 버튼을 눌렀을 때 실행되는 함수
        /// </summary>
        public void OnToggleReadyButton()
        {
            if (photonView.IsMine)
            {
                if (ready == false)
                {
                    ready = true;
                    readyText.text = "준비 완료";
                }
                else
                {
                    ready = false;
                    readyText.text = "준비";
                }
            }
        }

        #endregion
        
        #region IPuncObservable CallBacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(ready);
                stream.SendNext(readyText.text);
            }
            else
            {
                ready = (bool)stream.ReceiveNext();
                readyText.text = (string) stream.ReceiveNext();
            }
        }

        #endregion

        #region IPunInstantiateMagicCallback

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (photonView.IsMine)
            {
                transform.parent = _netWorkManager.area1.transform;
            }
            else
            {
                transform.parent = _netWorkManager.area2.transform;
            }
            
            gameObject.transform.localPosition = new Vector3(0f, -290f, 0f);
        }

        #endregion
    }
    
}
