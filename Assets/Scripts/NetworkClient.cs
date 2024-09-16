using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;
using System;

namespace Project.Networking
{
    public class NetworkClient
    {
        public static NetworkClient mInstance;
        public static string gameid = "card-game-lwj5t5oeksljthpqfm51g";


        private Client Client;

        public Action<Message> onPlayerSpawned,onPlayerLeaveRoom,onRoundStarted,onTurnChanged,onTransaction
            ,onRoundOver,onActionShow,onStatusUpdate,onChatMessageReceived;
        public Action onActionPack,onActionShowFailed;
        public Action<LeaderboardEntry[]> onLeaderboardRefresh;

        private List<Connection> allConnections = new List<Connection>();

        private Connection Connection { get { return allConnections[allConnections.Count-1]; } }


        private string username;

        public static NetworkClient getInstance
        {
            get
            {
                if(mInstance == null)
                {
                    mInstance = new NetworkClient();
                }
               
                return mInstance;
            }

         
        }

        public void RegisterUser(string username,string password)
        {
            PlayerIO.Authenticate(gameid, "public", new Dictionary<string, string>
            {
                {"register","true"},
                {"username",username},
                {"password",password}
            }, null, delegate (Client client)
              {
                  Debug.Log("Registration successfull");
              }, delegate (PlayerIOError error)
             {
                 Debug.Log("Registration failed :"+error.Message);
             });
        }


        public void LogUser(string username, string password)
        {
            this.username = username;
            
            PlayerIO.Authenticate(gameid, "public", new Dictionary<string, string>
            {
                {"username",username },
                {"password",password}
            }, 
            null,
            delegate (Client _client)
              {
                  Client = _client;
                 JoinLobbyRoom();
                 Debug.Log("Login Successfull! Congratulations");
              }, 
            delegate (PlayerIOError error)
             {
                 Debug.Log("Login Failed! "+error.Message);
             });
        }

        private void JoinLobbyRoom()
        {
            //Client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

            Client.Multiplayer.CreateJoinRoom("$service-room$", "Lobby", true, null, null,
                (Connection connection) =>
                {
                    allConnections.Add(connection);
                    Debug.Log("Successfully joined lobby");
                    LevelManager.mInstance.OnAuthenticationComplete();

                    /// logic to check weather user has joined for the first time or not
                     
                    Client.BigDB.LoadMyPlayerObject((DatabaseObject databaseObj) =>
                    {
                        try
                        {
                            string _username = databaseObj.GetString(NetworkConstant.DATABASE_USERNAME);
                        }
                        catch(Exception E)
                        {
                            Debug.Log("Error Occured : " + E.Message);
                            Connection.Send(Message.Create(NetworkConstant.FIRST_TIME_LOGIN, username));
                        } 
                    });


                }, (PlayerIOError error) =>
                 {
                     Debug.Log("Error : "+error.Message);
                 });
        }

        public void JoinGameRoom()
        {
            string roomID = Client.ConnectUserId + "" + DateTime.Now.ToString();

            Client.Multiplayer.ListRooms("GameRoom", null, 5, 0,
                (RoomInfo[] rooms) =>
            {
                if(rooms.Length > 0)
                {
                    Client.Multiplayer.JoinRoom(rooms[0].Id, null, 
                        (Connection _connection)=>
                    {
                        allConnections.Add(_connection);
                        Debug.Log("Successfully joined game room");
                        _connection.OnMessage += GameRoomMessageHandler;
                        _connection.OnDisconnect += DisconnectFromGameRoom;
                        LevelManager.mInstance.OnJoinGameRoom();
                    });
                }
                else
                {
                    Client.Multiplayer.CreateJoinRoom(roomID, "GameRoom", true, null, null,
                     (Connection connection) =>
                     {
                         allConnections.Add(connection);
                         Debug.Log("Successfully joined game room");
                         connection.OnMessage += GameRoomMessageHandler;
                         connection.OnDisconnect += DisconnectFromGameRoom;
                         LevelManager.mInstance.OnJoinGameRoom();
                     });
                }

            },(PlayerIOError error)=>
            {
                Client.Multiplayer.CreateJoinRoom(roomID, "GameRoom", true, null, null,
              (Connection connection) =>
              {
                  allConnections.Add(connection);
                  Debug.Log("Successfully joined game room");
                  connection.OnMessage += GameRoomMessageHandler;
                  connection.OnDisconnect += DisconnectFromGameRoom;
                  LevelManager.mInstance.OnJoinGameRoom();
              }); 
            });
              
        }

        private void DisconnectFromGameRoom(object sender, string message)
        {
            Debug.Log("Disconnected");
            allConnections.RemoveAt(allConnections.Count - 1);
            LevelManager.mInstance.OnLeaveGameRoom();
        }



        private void GameRoomMessageHandler(object sender, Message e)
        { 
            switch (e.Type)
            {
                case NetworkConstant.SPAWN_LOCAL_PLAYER:
                    onPlayerSpawned(e);
                    break;
                case NetworkConstant.SPAWN_FOREIGN_PLAYER:
                    onPlayerSpawned(e);
                    break;
                case NetworkConstant.PLAYER_LEFT:
                    onPlayerLeaveRoom(e);
                    break;
                case NetworkConstant.ROUND_STARTED:
                    onRoundStarted(e);
                    break;
                case NetworkConstant.CHANGE_TURN:
                    onTurnChanged(e);
                    break;
                case NetworkConstant.TRANSACTION:
                    onTransaction(e);
                    break;
                case NetworkConstant.ROUND_OVER:
                    onRoundOver(e);
                    break;
                case NetworkConstant.ACTION_PACK:
                    onActionPack();
                    break;
                case NetworkConstant.ACTION_SHOW:
                    onActionShow(e);
                    break; 
                case NetworkConstant.ACTION_SHOW_FAILED:
                    Debug.Log("Action Show event failed");
                    break;
                case NetworkConstant.STATUS_UPDATE:
                    onStatusUpdate(e);
                    break;
                case NetworkConstant.CHAT_MESSAGE:
                    onChatMessageReceived(e);
                    break;
                   

            }
        }

         
        public void LeaveRoom()
        {
            Connection.Disconnect(); 

        }

        public void DisconnectAll()
        {
             
            foreach(Connection conn in allConnections)
            {
                conn.Disconnect();
            }
        }

        
        public string GetID { get { return Client.ConnectUserId; } }
        
        
        public void ActionChaal()
        {
            Connection.Send(Message.Create(NetworkConstant.ACTION_CHAAL));
        }

        public void ActionPack()
        {
            Connection.Send(Message.Create(NetworkConstant.ACTION_PACK));
        }
         
        public void ActionShow()
        {
            Connection.Send(Message.Create(NetworkConstant.ACTION_SHOW));
        }


        public void SendChatMessage(string _msg)
        {
            Connection.Send(Message.Create(NetworkConstant.CHAT_MESSAGE, _msg));
        }
        

        public void GetLeaderboardEntries()
        {
            Client.Leaderboards.GetTop("chips", null, 0, 10, null, (LeaderboardEntry[] entries) =>
                 {
                     onLeaderboardRefresh(entries);
                 });
        }

    }
}
