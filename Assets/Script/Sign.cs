using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Redsea.MazeEscape
{
    public class Sign : MonoBehaviour
    {
        #region Private Fields

        // 얼만큼 가까워야 자리에 달라붙는지
        private float _thresholdLen = 30f;
        // true : Start, false : End
        private bool _isStart;
        // 예외처리 시 돌아올 처음 위치
        private Vector3 _originPosition;
        private Transform[] _positions;

        #endregion
        
        #region Public Fields

        public string signName;
        public Maze maze;
        public bool isFixed;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            signName = gameObject.name;
            _originPosition = transform.position;
            _positions = maze.positions;
            _isStart = (signName == "Start") ? true : false;
        }

        private void OnMouseDrag()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }

        private void OnMouseUp()
        {
            float minLen = int.MaxValue;
            Transform minPos = null;

            // 저장해둔 인덱스 삭제
            if (_isStart)
            {
                maze.startIndex = -1;
            }
            else
            {
                maze.endIndex = -1;
            }
            
            foreach (var position in _positions)
            {
                var len = Vector3.Distance(transform.position, position.position);
                if (minLen > len)
                {
                    minLen = len;
                    minPos = position;
                }
            }

            // 예외처리 : 거리가 임계값보다 멀 때
            if (minLen > _thresholdLen)
            {
                Debug.Log("포인트로부터 거리가 너무 멉니다.");
                isFixed = false;
                // 원래 위치로 되돌리기
                transform.position = _originPosition;
                return;
            }
            
            // 일반적으로 자리를 옮길 때
            if (minPos != null)
            {
                isFixed = true;
                transform.position = minPos.position;
                
                if (_isStart)
                {
                    maze.startIndex = int.Parse(minPos.name);
                    Debug.Log($"Start 자리를 {maze.startIndex}로 옮겼습니다.");
                }
                else
                {
                    maze.endIndex = int.Parse(minPos.name);
                    Debug.Log($"End 자리를 {maze.endIndex}로 옮겼습니다.");
                }
                
                // 예외처리 : 자리를 옮겼는데 그 위치에 이미 Sign이 있는 경우 원래 자리로
                if (maze.startIndex == maze.endIndex)
                {
                    Debug.Log("중복되는 위치입니다.");
                    // 저장해둔 인덱스 삭제
                    if (_isStart)
                    {
                        maze.startIndex = -1;
                    }
                    else
                    {
                        maze.endIndex = -1;
                    }

                    isFixed = false;
                    transform.position = _originPosition;
                }
            }
            
        }

        #endregion
        
    }

}
