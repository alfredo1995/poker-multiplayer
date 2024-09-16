using PlayerIOClient;
using Project.Networking;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Project.Gameplay.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] Transform turnIndicator, turnPanel;
        [SerializeField] TMP_Text bottomTextView,topTextView;
        [SerializeField] CardPlaceholder cardPlaceholder;
        [SerializeField] ChatBox chatBubble;

        private NetworkClient Client;

        private void Start()
        {
            Client = NetworkClient.mInstance;
        }

        public void AcceptCard(Message message)
        {
            GetComponent<CardManager>().SpawnCards(message);
        }


        public void IsMyTurn(bool _value)
        {
            if (_value)
            {
                /// its my turn
                turnIndicator.gameObject.SetActive(true);
                if (turnPanel != null)
                {
                    /// local player for the game instance
                    turnPanel.gameObject.SetActive(true);
                }
                else
                {
                    /// foreign player for the game instance
                }
            }
            else
            {
                /// its not my turn
                turnIndicator.gameObject.SetActive(false);
                if (turnPanel != null)
                    turnPanel.gameObject.SetActive(false);
            }
        }

        public void SetChips(int amount)
        {
            topTextView.text = amount.ToString();
        }

        public void SetUsername(string _username)
        {
            topTextView.text = _username;
        }

        public void SetStatus(string _status)
        {
            bottomTextView.text = _status;
        }


        public void onRoundOver()
        {
            IsMyTurn(false);
            GetComponent<CardManager>().DestroyCards();

            if (cardPlaceholder != null)
            {
                cardPlaceholder.RemoveCardPlaceholder();
            }
        }

        /// <summary>
        /// gets triggered when we pack the card
        /// </summary>
        public void onPackCard()
        {
            GetComponent<CardManager>().DestroyCards();
        }

        public void Chaal()
        {
            Client.ActionChaal();
        }

        public void Pack()
        {
            Client.ActionPack();
        }

        public void Show()
        {
            Client.ActionShow();
        }


        public void ActionCardShowEvent(List<string> _cards)
        {
            cardPlaceholder.SetCards(_cards);
        }
         
        public void ShowChatMessage(string _msg)
        {
            ChatBox chatMsg = Instantiate(chatBubble, transform);
            chatMsg.SetMessage(_msg);
        }
    }
}
