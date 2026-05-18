using UnityEngine;

[System.Serializable]
public class Card
{
    public string suit;
    public string rank;
    public int value;
    public Sprite artwork;

    public Card(string newSuit, string newRank, int newValue, Sprite newArtwork)
    {
        suit = newSuit;
        rank = newRank;
        value = newValue;
        artwork = newArtwork;
    }
}