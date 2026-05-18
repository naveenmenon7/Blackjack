using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Card Visuals")]

    public GameObject cardPrefab;

    public Transform playerCardArea;

    public Transform dealerCardArea;

    public enum GameState
    {
        PlayerTurn,
        DealerTurn,
        RoundOver
    }

    public GameState currentState;

    DeckManager deckManager;

    UIManager uiManager;

    Hand playerHand;

    Hand dealerHand;

    void Start()
    {
        deckManager = GetComponent<DeckManager>();

        uiManager = FindAnyObjectByType<UIManager>();

        playerHand = new Hand();

        dealerHand = new Hand();

        StartRound();
    }

    void StartRound()
    {
        deckManager.CreateDeck();

        deckManager.ShuffleDeck();

        // PLAYER CARD 1
        Card playerCard1 = deckManager.DrawCard();

        playerHand.AddCard(playerCard1);

        SpawnCardVisual(playerCard1, playerCardArea);

        // PLAYER CARD 2
        Card playerCard2 = deckManager.DrawCard();

        playerHand.AddCard(playerCard2);

        SpawnCardVisual(playerCard2, playerCardArea);

        // DEALER CARD 1
        Card dealerCard1 = deckManager.DrawCard();

        dealerHand.AddCard(dealerCard1);

        SpawnCardVisual(dealerCard1, dealerCardArea);

        // DEALER CARD 2
        Card dealerCard2 = deckManager.DrawCard();

        dealerHand.AddCard(dealerCard2);

        SpawnCardVisual(dealerCard2, dealerCardArea);

        currentState = GameState.PlayerTurn;

        uiManager.HideResult();

        uiManager.UpdateScores(
            playerHand.CalculateHandValue(),
            dealerHand.CalculateHandValue()
        );

        ShowHands();
    }

    void ShowHands()
    {
        Debug.Log("===== PLAYER HAND =====");

        foreach (Card card in playerHand.cards)
        {
            Debug.Log(card.rank + " of " + card.suit);
        }

        Debug.Log("Player Total: " + playerHand.CalculateHandValue());

        Debug.Log("===== DEALER HAND =====");

        foreach (Card card in dealerHand.cards)
        {
            Debug.Log(card.rank + " of " + card.suit);
        }

        Debug.Log("Dealer Total: " + dealerHand.CalculateHandValue());
    }

    void SpawnCardVisual(Card cardData, Transform parentArea)
    {
        GameObject newCard = Instantiate(cardPrefab, parentArea);

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();

        cardDisplay.SetupCard(cardData);
    }

    public void PlayerHit()
    {
        if (currentState != GameState.PlayerTurn)
            return;

        Card drawnCard = deckManager.DrawCard();

        playerHand.AddCard(drawnCard);

        SpawnCardVisual(drawnCard, playerCardArea);

        Debug.Log("PLAYER HITS!");

        ShowHands();

        int playerTotal = playerHand.CalculateHandValue();

        if (playerTotal > 21)
        {
            Debug.Log("PLAYER BUSTED!");

            uiManager.ShowResult("Player Busted! Dealer Wins!");

            currentState = GameState.RoundOver;
        }

        uiManager.UpdateScores(
            playerHand.CalculateHandValue(),
            dealerHand.CalculateHandValue()
        );
    }

    public void PlayerStand()
    {
        if (currentState != GameState.PlayerTurn)
            return;

        Debug.Log("PLAYER STANDS!");

        currentState = GameState.DealerTurn;

        DealerTurn();
    }

    void DealerTurn()
    {
        Debug.Log("===== DEALER TURN =====");

        while (dealerHand.CalculateHandValue() < 17)
        {
            Card dealerCard = deckManager.DrawCard();

            dealerHand.AddCard(dealerCard);

            SpawnCardVisual(dealerCard, dealerCardArea);

            Debug.Log("Dealer draws a card.");
        }

        ShowHands();

        uiManager.UpdateScores(
            playerHand.CalculateHandValue(),
            dealerHand.CalculateHandValue()
        );

        DecideWinner();
    }

    void DecideWinner()
    {
        int playerTotal = playerHand.CalculateHandValue();

        int dealerTotal = dealerHand.CalculateHandValue();

        Debug.Log("===== RESULT =====");

        if (dealerTotal > 21)
        {
            uiManager.ShowResult("Dealer Busts! Player Wins!");
        }
        else if (playerTotal > dealerTotal)
        {
            uiManager.ShowResult("Player Wins!");
        }
        else if (dealerTotal > playerTotal)
        {
            uiManager.ShowResult("Dealer Wins!");
        }
        else
        {
            uiManager.ShowResult("Push! It's a Tie!");
        }

        currentState = GameState.RoundOver;
    }

    public void StartNewRound()
    {
        ClearCardVisuals();

        playerHand = new Hand();

        dealerHand = new Hand();

        StartRound();
    }

    void ClearCardVisuals()
    {
        foreach (Transform child in playerCardArea)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in dealerCardArea)
        {
            Destroy(child.gameObject);
        }
    }
}