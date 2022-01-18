using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class ReadyManager : MonoBehaviour
    {
        #region Private Fields

        private bool _checkFixed;
        private bool[] onOffData = new bool[60];
        private MazeData[] _presetData;

        #endregion
        
        #region Public Fields

        public Maze maze;
        public GameObject presetPanel;
        public GameObject presetPrefab;
        public InputField inputField;
        public Text infoText;
        public GameObject belowUI;
        public Sign start;
        public Sign end;
        public int curPresetIndex;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            SetInfoText("입구와 출구를 원하는 곳에 드래그 하세요");
            
            curPresetIndex = -1;
            _presetData = new MazeData[5];
            // Json파일에서 preset 불러오기
        }

        private void Update()
        {
            CheckIsFixed();
        }

        #endregion
        
        #region Custom Methods

        private void CheckIsFixed()
        {
            // 둘 다 Fixed 되었을 때
            if (!_checkFixed && start.isFixed && end.isFixed)
            {
                belowUI.SetActive(true);
                _checkFixed = true;
                SetInfoText("미로를 완성하고 저장하세요");
            }
            

            // 둘 중에 하나가 Fixed되지 않았을 때
            if (!start.isFixed || !end.isFixed)
            {
                belowUI.SetActive(false);
                _checkFixed = false;
                SetInfoText("입구와 출구를 원하는 곳에 드래그 하세요");
            }
        }
        
        public void OnClickSaveButton()
        {
            SaveToJson();
            LoadFromJson();
        }

        public void SaveToJson()
        {
            if (curPresetIndex == -1)
            {
                Debug.Log("프리셋을 선택하지 않았습니다");
                return;
            }

            var startIndex = maze.startIndex;
            var endIndex = maze.endIndex;
            var onOffData = maze.onOffData;

            MazeData mazeData = new MazeData();
            mazeData.presetName = inputField.text;
            mazeData.presetIndex = curPresetIndex;
            mazeData.onOffData = onOffData;
            mazeData.startIndex = startIndex;
            mazeData.endIndex = endIndex;

            string mazeJson = JsonUtility.ToJson(mazeData);
            Debug.Log($"mazeJson : {mazeJson}");
        }

        public void LoadFromJson()
        {
            //TODO _presetData배열에 불러오기 => Preset.cs에서 이 배열을 토대로 불러오기
        }

        public void SetInfoText(string str)
        {
            infoText.text = str;
        }
        
        #endregion
    }
    
}
