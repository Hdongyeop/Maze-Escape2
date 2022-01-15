using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInput : MonoBehaviour
    {
        #region Private Constants

        private const string PlayerNamePrefKey = "PlayerName";

        #endregion

        #region Public Fields

        public InputField nameInputField;

        #endregion
        
        #region MonoBehaviour CallBacks
        
        private void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(PlayerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(PlayerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
            
            nameInputField.onValueChanged.AddListener(SetPlayerName);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the name of player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // # Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }

            PhotonNetwork.NickName = value;
            
            PlayerPrefs.SetString(PlayerNamePrefKey, value);
        }

        #endregion
    }

}