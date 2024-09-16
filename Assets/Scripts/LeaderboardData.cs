using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Leaderboard
{
    public class LeaderboardData : MonoBehaviour
    {
        [SerializeField] TMP_Text nameText, chipsText;


        public void ShowData(string name,string chips)
        {
            nameText.text = name;
            chipsText.text = chips;
        }
    }
}
