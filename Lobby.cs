using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;
 
 
[RoomType("Lobby")]
public class LobbyRoom : Game<Player> {


    public override void UserJoined(Player player)
    {
        Console.WriteLine("User joined : "+player.ConnectUserId);
    }


    public override void UserLeft(Player player)
    {
        Console.WriteLine("User Left : " + player.ConnectUserId);
    }

    public override void GotMessage(Player player, Message message)
    {
        switch (message.Type)
        {
            case NetworkConstant.FIRST_TIME_LOGIN:
                /// saving the username and depositing it into its account aka playerobject

                PlayerIO.BigDB.LoadOrCreate("PlayerObjects", player.ConnectUserId,
                    (DatabaseObject databaseObj) =>
                {
                    databaseObj.Set(NetworkConstant.DATABASE_USERNAME, message.GetString(0));
                    databaseObj.Set(NetworkConstant.DATABASE_CHIPS, 100000);

                    databaseObj.Save();
                });

                player.Leaderboards.Set("chips", null, player.CHIPS, (LeaderboardEntry LeaderboardEntry) =>
                   {
                       Console.WriteLine("Success");
                   });
                break;
        }
    }
} 