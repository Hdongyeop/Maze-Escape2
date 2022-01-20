using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Redsea.MazeEscape
{
    public class RightArrow : MonoBehaviour
    {
        #region MonoBehaviour CallBacks

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// 오른쪽 화살표를 클릭했을 때
        /// </summary>
        public void OnClickRightArrow()
        {
            Debug.Log($"선택한 프리셋 인덱스 : {PlayerPrefs.GetInt("PRESET_NUMBER")}");
            SceneManager.LoadScene(2);
        }

        #endregion
    }
    
}
