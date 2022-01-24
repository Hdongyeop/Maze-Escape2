using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class StartButton : MonoBehaviour
    {
        #region Public Fields

        [Tooltip("안내 텍스트")]
        public Text infoText;
        [Tooltip("닉네임 텍스트")]
        public Text nickName;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            
        }

        #endregion
        
        #region Custom Methods

        public void OnClicked()
        {
            if (string.IsNullOrEmpty(nickName.text))
            {
                infoText.text = "닉네임을 입력해주세요";
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }

        #endregion
        
    }

}