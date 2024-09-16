using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Project.Gameplay.Player
{
    public class CardPlaceholder : MonoBehaviour
    {

        [SerializeField] List<Image> cardPlaceholders;
        [SerializeField] Sprite defaultSprite;

        public void SetCards(List<string> _cards)
        {
            for(int i = 0; i < cardPlaceholders.Count; i++)
            {
                cardPlaceholders[i].sprite = Resources.Load<Sprite>("Cards/" + _cards[i]);
            }
        }

        public void RemoveCardPlaceholder()
        {
            for(int i = 0; i < cardPlaceholders.Count; i++)
            {
                cardPlaceholders[i].sprite = defaultSprite;
            }
        }
    }
}
