using System;
using Photon.Pun;
using UnityEngine;

namespace Com.Redsea.MazeEscape
{
    public class MazeData
    {
        public string presetName;
        public int presetIndex;
        
        public bool[] onOffData;
        public int startIndex;
        public int endIndex;
    }
    
    public class Maze : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        #region Private Fields

        private NetworkManager _netWorkManager;

        #endregion
        #region Public Fields

        [HideInInspector]
        public bool[] defaultSetting = new[]
        {
            true,true,true,true,true,
            true,false,false,false,false,true,
            false,false,false,false,false,
            true,false,false,false,false,true,
            false,false,false,false,false,
            true,false,false,false,false,true,
            false,false,false,false,false,
            true,false,false,false,false,true,
            false,false,false,false,false,
            true,false,false,false,false,true,
            true,true,true,true,true
        };

        public bool[] onOffData;
        public GameObject[] bars;
        public Transform[] positions;
        public int startIndex;
        public int endIndex;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            startIndex = -1;
            endIndex = -1;
            onOffData = new bool[60];
            if(GameObject.FindGameObjectWithTag("NetworkManager") != null)
                _netWorkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        }

        private void Update()
        {
            BarCheck();
        }

        #endregion

        #region MonoBehaviourPunCallbacks CallBacks

        

        #endregion

        #region Custom Methods

        private void BarCheck()
        {
            for (int i = 0; i < bars.Length; i++)
            {
                onOffData[i] = bars[i].GetComponent<Bar>().check;
            }
        }

        
        #endregion

        #region IPunInstantiateMagicCallback

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (photonView.IsMine)
            {
                transform.parent = _netWorkManager.mazePos1;
            }
            else
            {
                transform.parent = _netWorkManager.mazePos2;
            }
            
            gameObject.transform.localPosition = Vector3.zero;
        }

        #endregion
    }

}
