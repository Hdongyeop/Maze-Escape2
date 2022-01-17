using System;
using Photon.Pun;
using UnityEngine;

namespace Com.Redsea.MazeEscape
{
    public class Maze : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        [SerializeField] private bool[] onOffData; 

        #endregion
        
        #region Public Fields

        public GameObject[] bars;
        public Transform[] positions;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            onOffData = new bool[60];
        }

        #endregion

        #region MonoBehaviourPunCallbacks CallBacks

        

        #endregion
    }

}
