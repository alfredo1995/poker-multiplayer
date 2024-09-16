using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

public class DeckGenerator
{

    public string[] suits = new string[] { "S", "H", "D", "C" };
    public string[] cardValues = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };

    private List<string> generatedDeck;

    public DeckGenerator()
    {
        GenerateDeck();
    }

    private void GenerateDeck()
    {
        generatedDeck = new List<string>();

        foreach(string suit in suits)
        {
            foreach(string value in cardValues)
            {
                generatedDeck.Add(suit + "," + value);
            }
        }

        Shuffle(generatedDeck);
    }

    
    void Shuffle<T>(List<T> list)
    {

        System.Random random = new System.Random();
        int n = list.Count;

        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    public Message GetCards()
    {
        Message message = Message.Create(NetworkConstant.ROUND_STARTED);

        for (int i = 0; i < 3; i++)
        {
            message.Add(generatedDeck[0]);
            generatedDeck.RemoveAt(0);
        }

        return message;
    }

  
    /// <summary>
    /// returns true if playerOnCards > playerTwoCards
    /// returns false if playerOneCards < playerTwoCards
    /// </summary>
    /// <param name="playerOne"></param>
    /// <param name="playerTwo"></param>
    /// <returns></returns>
    public bool CompareCards(List<string> playerOneCards,List<string> playerTwoCards)
    {
        int sumCardsA = 0;
        int sumCardsB = 0;

        for(int i = 0; i < playerOneCards.Count; i++)
        {
            sumCardsA += int.Parse(playerOneCards[i].Split(',')[1]);
            sumCardsB += int.Parse(playerTwoCards[i].Split(',')[1]);
        }

        if(sumCardsA > sumCardsB)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
     
}
