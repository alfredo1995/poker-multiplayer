using System.Collections.Generic;
using PlayerIO.GameLibrary;


public enum PlayerState
{
    WAITING,
    PLAYING,
    CHAAL,
    PACKED,
    SHOW
}
 
public class Player : BasePlayer
{
    private int Chips;
    private List<string> Cards = new List<string>();
    
    public PlayerState STATE { get; set; }

    public int CHIPS { get { return Chips; } }
     
    public List<string> GetCards { get { return Cards; } }
     
    public void SetCards(Message message)
    {
        Cards.Clear();

        for(uint i = 0; i < message.Count; i++)
        {
            Cards.Add(message.GetString(i));
        }
    }

    public void CreditChips(int amount)
    {
        Chips += amount;
        Send(Message.Create(NetworkConstant.TRANSACTION, Chips));
    }

    public void DebitChips(int amount)
    {
        Chips -= amount;
        Send(Message.Create(NetworkConstant.TRANSACTION, Chips));
    }

}
