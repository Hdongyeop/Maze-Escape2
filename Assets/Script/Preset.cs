using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class Preset : MonoBehaviour
    {
        #region Private Fields

        private bool _isSelected;

        #endregion
        
        #region Public Fields

        public int index;
        public ReadyManager readyManager;
        public GameObject frameImage;
        public Text indexText;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            _isSelected = false;
            indexText.text = "[" + index + "]";
        }

        private void Update()
        {
            
        }

        private void OnMouseEnter()
        {
            // 프레임 나타내기
            frameImage.gameObject.SetActive(true);
        }

        private void OnMouseExit()
        {
            // 프레임 끄기
            if(!_isSelected)
                frameImage.gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            // ReadyManager의 curPresetIndex 변경
            readyManager.curPresetIndex = index;

            // 모든 프리셋 프레임 없애기
            var frames = GameObject.FindGameObjectsWithTag("PRESET");
            foreach (var frame in frames)
            {
                frame.GetComponent<Preset>().frameImage.SetActive(false);
                frame.GetComponent<Preset>()._isSelected = false;
            }

            // 프레임 나타내기
            frameImage.gameObject.SetActive(true);
            _isSelected = true;
        }

        #endregion
    }
    
}
