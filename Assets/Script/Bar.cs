using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace Com.Redsea.MazeEscape
{
    public class Bar : MonoBehaviour
    {
        #region Private Fields
        
        private Color _changeColor = Color.black;
        private Collider _collider;
        private SpriteRenderer _spriteRenderer;
        private Color _oldColor;
        private int[] _wall = new[] {0, 1, 2, 3, 4, 5, 10, 16, 21, 27, 32, 38, 43, 49, 54, 55, 56, 57, 58, 59};

        #endregion
        
        #region Public Fields

        public int index;
        public bool check;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            index = int.Parse(gameObject.name);
            _collider = GetComponent<Collider>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _oldColor = _spriteRenderer.color;
        }

        private void Update()
        {
            
        }

        private void OnMouseEnter()
        {
            if (!check)
            {
                _spriteRenderer.color = _changeColor;
            }
        }

        private void OnMouseExit()
        {
            if (!check)
            {
                _spriteRenderer.color = _oldColor;
            }
        }

        private void OnMouseDown()
        {
            if (!check)
            {
                check = true;
                _spriteRenderer.color = _changeColor;    
            }
            else
            {
                if (_wall.Contains(index))
                {
                    // 벽이면 리턴
                    return;
                }
                check = false;
                _spriteRenderer.color = _oldColor;
            }
            
        }

        #endregion

        #region Custom Methods

        public void BarColorUpdate()
        {
            if (check)
            {
                _spriteRenderer.color = _changeColor;
            }
            else
            {
                _spriteRenderer.color = _oldColor;
            }
        }

        #endregion

    }

}
