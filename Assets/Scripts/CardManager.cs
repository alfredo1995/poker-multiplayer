using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;

namespace Project.Gameplay.Player
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] Transform cardSpawnPosition;
        [SerializeField] CardIdentity cardPrefab;

        private List<CardIdentity> CardPrefabs;

        private void Start()
        {
            CardPrefabs = new List<CardIdentity>();
        }

        public void SpawnCards(Message message)
        {
            int difference = -50;

             for(uint i = 0; i < message.Count; i++)
            {
                string _card = message.GetString(i);

                CardIdentity identity = Instantiate(cardPrefab, cardSpawnPosition);
                identity.gameObject.name = _card;
                identity.SetCardFace(_card);
                CardPrefabs.Add(identity);

                Vector3 position = identity.GetComponent<RectTransform>().localPosition;
                identity.GetComponent<RectTransform>().localPosition = new Vector3(position.x + difference + 50 * i, position.y, position.z); 
            }
        } 

        public void DestroyCards()
        {
            for(int i = 0; i < CardPrefabs.Count; i++)
            {
                Destroy(CardPrefabs[i].gameObject);
            }
            CardPrefabs.Clear();
        }
    }

}