using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;
using PlayerIOClient;

namespace Project.Leaderboard
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] Transform contentTransform;
        [SerializeField] LeaderboardData LeaderboardData;

        List<LeaderboardData> allEntries = new List<LeaderboardData>();

        private void Start()
        {
            NetworkClient.mInstance.onLeaderboardRefresh += onLeaderboardRefresh;  
        }

        private void onLeaderboardRefresh(LeaderboardEntry[] entries)
        { 
            foreach(LeaderboardEntry entry in entries)
            {
                LeaderboardData leaderboardData = Instantiate(LeaderboardData, contentTransform);
                leaderboardData.ShowData(entry.ConnectUserId,entry.Score.ToString());
                allEntries.Add(leaderboardData);
            }
        }

    }
}
