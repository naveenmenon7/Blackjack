using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();

    string[] suits = { "hearts", "diamonds", "clubs", "spades" };

    string[] ranks =
    {
        "A", "02", "03", "04", "05", "06",
        "07", "08", "09", "10", "J", "Q", "K"
    };

    public void CreateDeck()
    {
        deck.Clear();

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                int value = GetCardValue(rank);

                string spriteName = "card_" + suit + "_" + rank;

                Sprite cardSprite = Resources.Load<Sprite>("Cards/" + spriteName);

                if(cardSprite == null)
                {
                    Debug.LogError("SPRITE NOT FOUND: " + spriteName);
                }

                Card newCard = new Card(
                    suit,
                    rank,
                    value,
                    cardSprite
                );

                deck.Add(newCard);

                Debug.Log("Created Card: " + spriteName);
            }
        }
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];

            int randomIndex = Random.Range(i, deck.Count);

            deck[i] = deck[randomIndex];

            deck[randomIndex] = temp;
        }
    }

    public Card DrawCard()
    {
        Card drawnCard = deck[0];

        deck.RemoveAt(0);

        return drawnCard;
    }

    int GetCardValue(string rank)
    {
        if (rank == "A")
            return 11;

        if (rank == "J" || rank == "Q" || rank == "K")
            return 10;

        return int.Parse(rank);
    }
}