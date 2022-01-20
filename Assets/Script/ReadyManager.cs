using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class ReadyManager : MonoBehaviour
    {
        #region Private Fields

        private string jsonPath;
        private bool _checkFixed;
        private MazeData[] _presetData;
        private bool[] _visited;
        private bool _reachAble;

        #endregion
        
        #region Public Fields

        [Tooltip("미궁 제작용")]
        public Maze maze;
        [Tooltip("제목 입력란")]
        public InputField inputField;
        [Tooltip("좌측 상단 설명란")]
        public Text infoText;
        [Tooltip("제목입력란, 세이브버튼을 묶은것")]
        public GameObject belowUI;
        [Tooltip("출발 지점")]
        public Sign start;
        [Tooltip("도착 지점")]
        public Sign end;
        [Tooltip("현재 프리셋 인덱스")]
        public int curPresetIndex;
        [Tooltip("프리셋 배열")]
        public Preset[] presets;
        [Tooltip("다음 씬으로 넘어가는 버튼")]
        public GameObject rightArrow;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            SetInfoText("입구와 출구를 원하는 곳에 드래그 하세요");
            curPresetIndex = -1;
            _presetData = new MazeData[5];
            
            // Json폴더가 없는 경우 생성
            jsonPath = Application.dataPath + "/Json";
            DirectoryInfo di = new DirectoryInfo(jsonPath);
            if(di.Exists == false)
                di.Create();
            
            // Json파일에서 preset 불러오기
            LoadFromJson();
        }

        private void Update()
        {
            CheckIsFixed();
            if(curPresetIndex == -1 || maze.startIndex == -1 || maze.endIndex == -1 || _presetData[curPresetIndex].presetName == "비 어 있 음")
                rightArrow.gameObject.SetActive(false);
            else
                rightArrow.gameObject.SetActive(true);
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
        
        /// <summary>
        /// Save버튼을 클릭했을 때
        /// </summary>
        public void OnClickSaveButton()
        {
            // 방문 배열 초기화
            _visited = new bool[25];
            // 도착 도달 가능 flag 초기화
            _reachAble = false;
            // 도착에 도달할 수 있는지 체크
            FindPath(maze.startIndex);

            if (_reachAble)
            {
                SaveToJson();
                LoadFromJson();
            }
            else
            {
                Debug.Log("도착지점에 도달할 수 없습니다.");
            }
        }

        /// <summary>
        /// 현재 Maze의 상태를 데이터 형태로 변환하고 프리셋에 저장
        /// </summary>
        private void SaveToJson()
        {
            if (curPresetIndex == -1)
            {
                Debug.Log("프리셋을 선택하지 않았습니다");
                return;
            }

            var startIndex = maze.startIndex;
            var endIndex = maze.endIndex;
            var onOffData = maze.onOffData;

            var mazeData = new MazeData();
            mazeData.presetName = inputField.text;
            mazeData.presetIndex = curPresetIndex;
            mazeData.onOffData = onOffData;
            mazeData.startIndex = startIndex;
            mazeData.endIndex = endIndex;

            string mazeJson = JsonUtility.ToJson(mazeData);
            Debug.Log($"mazeJson : {mazeJson}");
            
            File.WriteAllText(jsonPath + $"/Preset_{curPresetIndex}.json", mazeJson);
        }

        /// <summary>
        /// Maze 데이터를 불러오고 프리셋 이름 업데이트
        /// </summary>
        private void LoadFromJson()
        {
            for (int i = 0; i < 5; i++)
            {
                var load = File.ReadAllText(jsonPath + $"/Preset_{i}.json");
                var presetData = JsonUtility.FromJson<MazeData>(load);
                _presetData[i] = presetData;
                presets[i].subject.text = presetData.presetName;
            }
        }

        /// <summary>
        /// 프리셋 삭제
        /// </summary>
        /// <param name="index">프리셋 번호</param>
        public void DeletePreset(int index)
        {
            Debug.Log($"{index}번 프리셋을 삭제했습니다.");
            var load = File.ReadAllText(jsonPath + $"/Preset_{index}.json");
            var presetData = JsonUtility.FromJson<MazeData>(load);

            presetData.presetName = "비 어 있 음";
            presetData.startIndex = -1;
            presetData.endIndex = -1;
            presetData.onOffData = maze.defaultSetting;
            
            File.WriteAllText(jsonPath + $"/Preset_{index}.json", JsonUtility.ToJson(presetData));
            
            LoadFromJson();
        }
        
        /// <summary>
        /// 안내 Text 수정
        /// </summary>
        /// <param name="str">수정할 내용</param>
        private void SetInfoText(string str)
        {
            infoText.text = str;
        }

        /// <summary>
        /// Preset이 클릭되었을 때 Maze의 상태를 업데이트함.
        /// </summary>
        public void MazeUpdate()
        {
            // 데이터 받아오기
            var mazeData = _presetData[curPresetIndex];
            
            // 이미 저장된 프리셋은 도착에 도달할 수 있으므로
            _reachAble = true;
            
            // 예외처리 : 빈 프리셋일 경우
            if (mazeData.startIndex == -1 || mazeData.endIndex == -1)
            {
                Debug.Log("빈 프리셋입니다.");
                // 출발, 도착 원래 위치로
                start.gameObject.transform.position = start.originPosition;
                end.gameObject.transform.position = end.originPosition;

                // 출발, 도착 지점 업데이트
                maze.startIndex = mazeData.startIndex;
                maze.endIndex = mazeData.endIndex;
                
                // 고정 해제
                start.isFixed = false;
                end.isFixed = false;
                
                // InputText 업데이트
                inputField.text = "";
                
                // 벽 원래 대로
                for (int i = 0; i < maze.bars.Length; i++)
                {
                    maze.bars[i].GetComponent<Bar>().check = mazeData.onOffData[i];
                    // UI 업데이트
                    maze.bars[i].GetComponent<Bar>().BarColorUpdate();
                }
                return;
            }
            
            // 출발, 도착 지점 업데이트
            maze.startIndex = mazeData.startIndex;
            maze.endIndex = mazeData.endIndex;
            
            // 출발, 도착 지점 UI 업데이트
            start.MoveTo(mazeData.startIndex);
            start.isFixed = true;
            end.MoveTo(mazeData.endIndex);
            end.isFixed = true;
            
            // 벽 상태 업데이트
            for (int i = 0; i < maze.bars.Length; i++)
            {
                maze.bars[i].GetComponent<Bar>().check = mazeData.onOffData[i];
                // UI 업데이트
                maze.bars[i].GetComponent<Bar>().BarColorUpdate();
            }

            // InputText 업데이트
            inputField.text = mazeData.presetName;
            
            // 현재 프리셋 번호 저장
            PlayerPrefs.SetInt("PRESET_NUMBER", curPresetIndex);
        }

        private void FindPath(int curIndex)
        {
            // 도착지점에 도달했을 때
            if (curIndex == maze.endIndex)
            {
                _reachAble = true;
                return;
            }
            //Debug.Log($"{curIndex}번방 방문");

            // 이미 방문한 지역일 때
            if (_visited[curIndex])
                return;
            
            // 방문 표시
            _visited[curIndex] = true;
            
            // 현재 위치에서 열려있는 문 불러오기
            var bars = maze.GetComponent<Maze>().bars;
            
            var y = curIndex / 5;
            var x = curIndex % 5;
            //Debug.Log($"{y}  {x}");
            
            var upIndex = (y * 11) + x;
            var leftIndex = upIndex + 5;
            var downIndex = leftIndex + 6;
            var rightIndex = downIndex - 5;
            //Debug.Log($"{upIndex}  {leftIndex}  {downIndex}  {rightIndex}");
            
            var ableBar = new bool[]
            {
                bars[upIndex].GetComponent<Bar>().check,
                bars[leftIndex].GetComponent<Bar>().check,
                bars[downIndex].GetComponent<Bar>().check,
                bars[rightIndex].GetComponent<Bar>().check
            };
            //Debug.Log($"{ableBar[0]} {ableBar[1]} {ableBar[2]} {ableBar[3]}");
            
            for (int i = 0; i < ableBar.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        if(!ableBar[0])
                            FindPath(curIndex - 5);
                        break;
                    case 1:
                        if(!ableBar[1])
                            FindPath(curIndex - 1);
                        break;
                    case 2:
                        if(!ableBar[2])
                            FindPath(curIndex + 5);
                        break;
                    case 3:
                        if(!ableBar[3])
                            FindPath(curIndex + 1);
                        break;
                }
            }
        }
        
        #endregion
    }
    
}
