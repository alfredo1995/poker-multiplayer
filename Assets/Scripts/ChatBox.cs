using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Project.Gameplay
{
    public class ChatBox : MonoBehaviour
    {
        [SerializeField] TMP_Text chatMessageDisplay;


        public void SetMessage(string _msg)
        {
            chatMessageDisplay.text = _msg;
            Destroy(gameObject, 5f);
        }
    }
}
