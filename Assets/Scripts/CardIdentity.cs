using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay
{
    public class CardIdentity : MonoBehaviour
    {
         public string CardFace { get; set; }

         

        public void SetCardFace(string _cardFace)
        {
            CardFace = _cardFace;

            GetComponent<Image>().sprite = Resources.Load<Sprite>("Cards/" + _cardFace);
        }


    }
}
