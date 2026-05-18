using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public int CalculateHandValue()
    {
        int totalValue = 0;

        int aceCount = 0;

        foreach (Card card in cards)
        {
            totalValue += card.value;

            if (card.rank == "A")
            {
                aceCount++;
            }
        }

        while (totalValue > 21 && aceCount > 0)
        {
            totalValue -= 10;

            aceCount--;
        }

        return totalValue;
    }
}