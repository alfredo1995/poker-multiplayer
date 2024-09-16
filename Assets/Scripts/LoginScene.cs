using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;
using TMPro;

namespace Project.Scenes
{
    public class LoginScene : MonoBehaviour
    {
        [SerializeField] TMP_InputField username, password;
        
        public void RegisterUser()
        {
            NetworkClient.getInstance.RegisterUser(username.text, password.text);
        }

        public void LoginUser()
        {
            NetworkClient.getInstance.LogUser(username.text, password.text);
        }
    }
}
