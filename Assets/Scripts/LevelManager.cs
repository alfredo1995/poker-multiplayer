using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager mInstance;


    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(mInstance);
        }
    }


    public void OnAuthenticationComplete()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void OnJoinGameRoom()
    {
        SceneManager.LoadScene("Game Room");
    }

    public void OnLeaveGameRoom()
    {
        SceneManager.LoadScene("Main Menu");
    }
     
}
