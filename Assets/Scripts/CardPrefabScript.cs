using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPrefabScript : MonoBehaviour
{
    [SerializeField] Sprite defaultCardSprite;
    [SerializeField] List<Image> cardsList;


    public void SetCards(List<string> _cards)
    {
        for(int i=0;i<cardsList.Count;i++)
        {
            cardsList[i].sprite = Resources.Load<Sprite>("Cards/" + _cards[i]);
        }
    }

    public void RemoveCards()
    {
        for (int i = 0; i < cardsList.Count; i++)
        {
            cardsList[i].sprite = defaultCardSprite;
        }
    }

}
