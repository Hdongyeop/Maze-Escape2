using UnityEngine;
using UnityEngine.UI;

namespace Com.Redsea.MazeEscape
{
    public class TurnManager : MonoBehaviour
    {
        #region Public Fields

        public NetworkManager networkManager;
        public Text infoText;
        public GreenPlayer myPlayer;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            
        }

        #endregion

    }

}
