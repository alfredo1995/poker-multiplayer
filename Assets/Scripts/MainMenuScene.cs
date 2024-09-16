using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;

namespace Project.Scenes
{
    public class MainMenuScene : MonoBehaviour
    {
        [SerializeField] Transform leaderboardPanel;
         
        public void JoinGameRoom()
        {
            NetworkClient.mInstance.JoinGameRoom();
        }

        public void ShowLeaderboardPanel()
        {
            leaderboardPanel.gameObject.SetActive(true);
            NetworkClient.mInstance.GetLeaderboardEntries();
        }

        public void CloseLeaderboardPanel()
        {
            leaderboardPanel.gameObject.SetActive(false);
        }
    }
}
