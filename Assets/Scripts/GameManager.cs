using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;
using PlayerIOClient;
using Project.Gameplay.Player;

namespace Project.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        private NetworkClient Client;

        [SerializeField] NetworkIdentity[] playerPrefabs;

        private List<NetworkIdentity> allPlayers;


        void Start()
        {
            Client = NetworkClient.getInstance;

            Client.onPlayerSpawned += onPlayerSpawned;
            Client.onPlayerLeaveRoom += onPlayerLeave;
            Client.onRoundStarted += onRoundStarted;
            Client.onTurnChanged += onTurnChanged;
            Client.onTransaction += onTransaction;
            Client.onRoundOver += onRoundOver;
            Client.onActionPack += onActionPack;
            Client.onActionShow += onActionShow;
            Client.onStatusUpdate += onStatusUpdate;
            Client.onChatMessageReceived += onChatMessageReceived;

            allPlayers = new List<NetworkIdentity>(); 
        }
         
        
        private void onPlayerSpawned(Message message)
        {
            if (message.Type.Equals(NetworkConstant.SPAWN_LOCAL_PLAYER))
            {
                NetworkIdentity identity = playerPrefabs[0];
                identity.ID = message.GetString(0);

                identity.gameObject.SetActive(true); 
                allPlayers.Add(identity); 
            }
            else
            {
                NetworkIdentity identity = playerPrefabs[allPlayers.Count];
                identity.ID = message.GetString(0);
                identity.GetComponent<PlayerManager>().SetUsername(message.GetString(1));
                identity.gameObject.SetActive(true);
                allPlayers.Add(identity);
            }
        }


        private void onPlayerLeave(Message message)
        {
            string playerID = message.GetString(0);

            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(playerID))
                {
                    nI.gameObject.SetActive(false);
                }
            }
        }


        private void onRoundStarted(Message message)
        {
            Debug.Log("Round Started");
            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(Client.GetID))
                {
                    nI.GetComponent<PlayerManager>().AcceptCard(message);
                }
            }
        }

        private void onTurnChanged(Message message)
        {
            string _playerid = message.GetString(0);
            Debug.Log("Turn Changed : "+_playerid);

            foreach (NetworkIdentity nI in allPlayers)
            {
                nI.GetComponent<PlayerManager>().IsMyTurn(nI.ID.Equals(_playerid));
            }
        }

        private void onTransaction(Message message)
        {
            int _amt = message.GetInt(0);

            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(Client.GetID))
                {
                    nI.GetComponent<PlayerManager>().SetChips(_amt);
                }
            }
        }

        private void onRoundOver(Message message)
        {
            Debug.Log("Round Over");
            foreach (NetworkIdentity nI in allPlayers)
            {
                nI.GetComponent<PlayerManager>().onRoundOver();
            }
        }

       
        private void onActionShow(Message message)
        {
            /// 0 -> playerid
            /// 1,2,3 cards for the player
            string _playerID = message.GetString(0);
            List<string> _cards = new List<string>();

            for(uint i = 1; i < message.Count; i++)
            {
                _cards.Add(message.GetString(i));
            }

            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(_playerID))
                {
                    nI.GetComponent<PlayerManager>().ActionCardShowEvent(_cards);
                }
            }
        }
         
        private void onActionPack()
        {
            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(Client.GetID))
                {
                    nI.GetComponent<PlayerManager>().onPackCard();
                }
            }
        }

        private void onStatusUpdate(Message message)
        {
            string _playerID = message.GetString(0);
            string _playerStatus = message.GetString(1);

            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(_playerID))
                {
                    nI.GetComponent<PlayerManager>().SetStatus(_playerStatus);
                }
            }
        }

        private void onChatMessageReceived(Message message)
        {
            string _playerID = message.GetString(0);
            string _message = message.GetString(1);

            foreach(NetworkIdentity nI in allPlayers)
            {
                if (nI.ID.Equals(_playerID))
                {
                    nI.GetComponent<PlayerManager>().ShowChatMessage(_message);
                }
            }
        }
        
        private void OnDestroy()
        {
            Client.onPlayerSpawned -= onPlayerSpawned;
            Client.onPlayerLeaveRoom -= onPlayerLeave;
            Client.onRoundStarted -= onRoundStarted;
            Client.onTurnChanged -= onTurnChanged;
            Client.onTransaction -= onTransaction;
            Client.onRoundOver -= onRoundOver;
            Client.onActionShow -= onActionShow;
            Client.onStatusUpdate -= onStatusUpdate;
            Client.onChatMessageReceived -= onChatMessageReceived;
        }

        private void OnApplicationQuit()
        {
            Client.DisconnectAll();
        }
    }
}
