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
    
    public class Maze : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        [SerializeField] private bool[] _onOffData;

        #endregion
        
        #region Public Fields
        
        public bool[] onOffData
        {
            get => _onOffData;
        }
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
            _onOffData = new bool[60];
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
                _onOffData[i] = bars[i].GetComponent<Bar>().check;
            }
        }

        #endregion
    }

}
