using PlayerIO.GameLibrary;
using System;
using System.Collections.Generic;

[RoomType("GameRoom")]
public class GameRoom : Game<Player>
{
    List<Player> allPlayers,currentRoundPlayers;
    DeckGenerator deckGenerator;


    private Player currentPlayer,currentRoundWinner = null;
    private Timer turnTimer;

    private const int ROUND_AMOUNT = 1000;
    private int POT_AMOUNT;
    private bool isRoundRunning = false,isGameStarted = false;

    private const int MINIMUM_CHIPS_AMOUNT = 5000;
    private const int CHAAL_AMOUNT = 2000;
     
    /// <summary>
    /// called when room instance is created
    /// </summary>
    public override void GameStarted()
    {
        allPlayers = new List<Player>();
        currentRoundPlayers = new List<Player>();
        PreloadPlayerObjects = true;
    }

    /// <summary>
    /// called when the user is about to join the room and waiting for permission
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public override bool AllowUserJoin(Player player)
    {
        /// allow only if current players in room is < 3 
        if(PlayerCount < 3 && player.PlayerObject.GetInt(NetworkConstant.DATABASE_CHIPS) > MINIMUM_CHIPS_AMOUNT 
            && !isRoundRunning)
        { 
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// called when user joins the room
    /// </summary>
    /// <param name="player"></param>
    public override void UserJoined(Player player)
    {
        player.Send(Message.Create(NetworkConstant.SPAWN_LOCAL_PLAYER,player.ConnectUserId));

        foreach(Player basePlayer in allPlayers)
        {
            player.Send(Message.Create(NetworkConstant.SPAWN_FOREIGN_PLAYER, basePlayer.ConnectUserId,
                basePlayer.PlayerObject.GetString(NetworkConstant.DATABASE_USERNAME)));
            basePlayer.Send(Message.Create(NetworkConstant.SPAWN_FOREIGN_PLAYER, player.ConnectUserId,
                player.PlayerObject.GetString(NetworkConstant.DATABASE_USERNAME)));
        }

        allPlayers.Add(player); 

        if(allPlayers.Count >= 2 && !isGameStarted)
        {
            ScheduleCallback(StartRound, 3000);
            isGameStarted = true;
        }

        player.CreditChips(player.PlayerObject.GetInt(NetworkConstant.DATABASE_CHIPS));

        BroadcastStatus(player, PlayerState.WAITING);
    }

    /// <summary>
    /// called when user lefts room
    /// </summary>
    /// <param name="player"></param>
    public override void UserLeft(Player player)
    {
        Console.WriteLine("Player Leave");
        Broadcast(Message.Create(NetworkConstant.PLAYER_LEFT, player.ConnectUserId));

        player.GetPlayerObject((DatabaseObject databaseObj) =>
        {
            databaseObj.Set(NetworkConstant.DATABASE_CHIPS, player.CHIPS);
            databaseObj.Save();
        });

        allPlayers.Remove(player);
        if (isRoundRunning)
        {
            /// we would remove the player from currentroundplayer list
            currentRoundPlayers.Remove(player);
            CheckWin();
        }


        /// we would update our leaderboard
        player.Leaderboards.Set("chips", null, player.CHIPS, (LeaderboardEntry LeaderboardEntry) =>
        {
            Console.WriteLine("Success");
        });

    }

    public override void GotMessage(Player player, Message message)
    {
        switch (message.Type)
        {
            case NetworkConstant.ACTION_CHAAL:
                ActionChaal(player,message);
                break;
            case NetworkConstant.ACTION_PACK:
                ActionPack(player, message);
                break;
            case NetworkConstant.ACTION_SHOW:
                ActionShow(player, message);
                break;
            case NetworkConstant.CHAT_MESSAGE:
                Broadcast(Message.Create(NetworkConstant.CHAT_MESSAGE, player.ConnectUserId, message.GetString(0)));
                break;
             
        }
    }

    /// <summary>
    /// called when a round starts
    /// </summary>
    public void StartRound()
    { 

        /// collect round amount from each player
        foreach(Player p in allPlayers)
        {
            if(p.CHIPS > MINIMUM_CHIPS_AMOUNT)
            {
                p.DebitChips(ROUND_AMOUNT);
                POT_AMOUNT += ROUND_AMOUNT;
            }
            else
            {
                p.Disconnect();
            }
        }

        deckGenerator = new DeckGenerator();

        foreach(Player player in allPlayers)
        {
            Message message = deckGenerator.GetCards();
            player.SetCards(message);
            player.Send(message);
            currentRoundPlayers.Add(player);

            BroadcastStatus(player, PlayerState.PLAYING);
        }


        ChangeTurn();
        isRoundRunning = true;
    }
 
    /// <summary>
    /// for changing turns while the round is on
    /// </summary>
    public void ChangeTurn()
    {
        /// round has just started
        if(currentPlayer == null)
        {
            currentPlayer = allPlayers[0];
        }

        Player player = currentPlayer;

        int index = currentRoundPlayers.LastIndexOf(player);
        ++index;

        if(index > currentRoundPlayers.Count - 1)
        {
            player = currentRoundPlayers[0];
        }
        else
        {
            player = currentRoundPlayers[index];
        }

        currentPlayer = player;

        Broadcast(NetworkConstant.CHANGE_TURN, currentPlayer.ConnectUserId);

        /// we would give each player 15 sec to play
        turnTimer =  ScheduleCallback(OnCycleOver, NetworkConstant.TURN_TIME); 
    }

    /// <summary>
    /// called when player does not performs its turn
    /// </summary>
    public void OnCycleOver()
    {
        /// add logic when player fails to make its turn
        currentRoundPlayers.Remove(currentPlayer);

        /// if the round is over then we 
        /// dont want to change the turn
        if (!CheckWin())
        {
            ChangeTurn();
        }
    }

    /// <summary>
    /// check if anyone has won the round or not
    /// </summary>
    /// <returns></returns>
    private bool CheckWin()
    {
        /// if only one player is present in the round then
        /// we will make him as a winner
        if(currentRoundPlayers.Count == 1)
        {
            currentRoundWinner = currentRoundPlayers[0];
            RoundOver();
            return true;
        }

        return false;
    }

    /// <summary>
    /// called when round is over
    /// </summary>
    private void RoundOver()
    {
        /// add all the logic to deposit win amount to 
        /// winner player and start round again

        currentRoundWinner.CreditChips(POT_AMOUNT);
        POT_AMOUNT = 0;

        currentPlayer = currentRoundWinner;
        currentRoundPlayers.Clear();

        ScheduleCallback(() =>
        {
            Broadcast(Message.Create(NetworkConstant.ROUND_OVER, currentRoundWinner.ConnectUserId));
        }, 3000);

        ScheduleCallback(StartRound, NetworkConstant.TIME_BETWEEN_ROUNDS);

        isRoundRunning = false;
    }


    private void ActionChaal(Player player,Message message)
    {
        if(player == currentPlayer && isRoundRunning)
        {
            if(player.CHIPS >= CHAAL_AMOUNT)
            {
                player.DebitChips(CHAAL_AMOUNT);
                POT_AMOUNT += CHAAL_AMOUNT;

                turnTimer.Stop();
                ChangeTurn();

                BroadcastStatus(player, PlayerState.CHAAL);
            }
            else
            {
                /// player does not have enough chips
                player.Disconnect();
            }
        }
    }

    private void ActionPack(Player player, Message message)
    {
        if(player == currentPlayer && isRoundRunning)
        {
            turnTimer.Stop();
            currentRoundPlayers.Remove(player);

            player.Send(NetworkConstant.ACTION_PACK);

            if (!CheckWin())
            {
                ChangeTurn();
            }

            BroadcastStatus(player, PlayerState.PACKED);
        }
    }

   
    private void ActionShow(Player player,Message message)
    {
        if(player == currentPlayer && isRoundRunning)
        {
            if(currentRoundPlayers.Count == 2)
            {
                /// user can implement show event
                turnTimer.Stop();

                currentRoundPlayers[0].Send(Message.Create(NetworkConstant.ACTION_SHOW, currentRoundPlayers[1].ConnectUserId,
                    currentRoundPlayers[1].GetCards[0], currentRoundPlayers[1].GetCards[1], currentRoundPlayers[1].GetCards[2]));

                currentRoundPlayers[1].Send(Message.Create(NetworkConstant.ACTION_SHOW, currentRoundPlayers[0].ConnectUserId,
    currentRoundPlayers[0].GetCards[0], currentRoundPlayers[0].GetCards[1], currentRoundPlayers[0].GetCards[2]));

                if (deckGenerator.CompareCards(currentRoundPlayers[0].GetCards, currentRoundPlayers[1].GetCards))
                {
                    /// player at 0 index have won the round   
                    currentRoundPlayers.RemoveAt(1);
                }
                else
                { 
                    /// player at 1 index have won the round
                    currentRoundPlayers.RemoveAt(0);
                }
                 
                CheckWin();
                BroadcastStatus(player, PlayerState.SHOW);
            }
            else
            {
                /// user cannot implement show action
                player.Send(Message.Create(NetworkConstant.ACTION_SHOW_FAILED));
            }
        }
    }

    private void BroadcastStatus(Player _player,PlayerState _state)
    {
        _player.STATE = _state;
        Broadcast(Message.Create(NetworkConstant.STATUS_UPDATE,_player.ConnectUserId, _state.ToString()));
    }
      
}
