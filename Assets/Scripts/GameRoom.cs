using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Scenes
{
    public class GameRoom : MonoBehaviour
    {
        [SerializeField] Transform chatPanel;
        [SerializeField] Transform backButtonMenu;

       public void OnLeaveRoom()
        { 
            Networking.NetworkClient.mInstance.LeaveRoom();
        }


        public void ShowChatPanel()
        {
            chatPanel.gameObject.SetActive(true);
        }

        public void SendChat(TMP_Text _text)
        {
            Networking.NetworkClient.mInstance.SendChatMessage(_text.text);
            chatPanel.gameObject.SetActive(false);
        }

        public void ShowBackButtonMenu()
        {
            backButtonMenu.gameObject.SetActive(true);
        }

        public void CloseBackButtonMenu()
        {
            backButtonMenu.gameObject.SetActive(false);
        }

        public void ExitRoom()
        {
            Networking.NetworkClient.mInstance.LeaveRoom();
        }
          
    }
}
