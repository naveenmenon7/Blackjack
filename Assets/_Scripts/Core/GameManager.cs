using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Card Visuals")]

    public GameObject cardPrefab;

    public Transform playerCardArea;

    public Transform dealerCardArea;

    public Transform deckPosition;

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

    GameObject hiddenDealerCardObject;

    Card hiddenDealerCardData;

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
        hiddenDealerCardData = deckManager.DrawCard();

        dealerHand.AddCard(hiddenDealerCardData);

        hiddenDealerCardObject = SpawnCardVisual(
            hiddenDealerCardData,
            dealerCardArea,
            true
        );

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

    GameObject SpawnCardVisual(Card cardData, Transform parentArea, bool hidden = false)
    {
        GameObject newCard = Instantiate(cardPrefab, parentArea);

        AudioManager.Instance.PlayCardDeal();

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();

        if (hidden)
        {
            Sprite backSprite = Resources.Load<Sprite>("Cards/card_back");

            cardDisplay.cardImage.sprite = backSprite;
        }
        else
        {
            cardDisplay.SetupCard(cardData);
        }

        RectTransform rectTransform = newCard.GetComponent<RectTransform>();

        int cardIndex = parentArea.childCount - 1;

        float spacing = 100f;

        Vector2 finalPosition = new Vector2(
            cardIndex * spacing,
            0f
        );

        Vector2 startPosition =
            parentArea.InverseTransformPoint(deckPosition.position);

        rectTransform.anchoredPosition = startPosition;

        rectTransform.localScale = Vector3.zero;

        rectTransform.DOAnchorPos(finalPosition, 0.4f)
            .SetEase(Ease.OutCubic);

        rectTransform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack);

        return newCard;
    }

    public void PlayerHit()
    {
        AudioManager.Instance.PlayButtonClick();
        
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

            AudioManager.Instance.PlayLose();

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
        AudioManager.Instance.PlayButtonClick();

        if (currentState != GameState.PlayerTurn)
            return;

        Debug.Log("PLAYER STANDS!");

        currentState = GameState.DealerTurn;

        DealerTurn();
    }

    void DealerTurn()
    {
        RevealDealerHiddenCard();

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
            AudioManager.Instance.PlayWin();

            uiManager.ShowResult("Dealer Busts! Player Wins!");
        }
        else if (playerTotal > dealerTotal)
        {
            AudioManager.Instance.PlayWin();

            uiManager.ShowResult("Player Wins!");
        }
        else if (dealerTotal > playerTotal)
        {
            AudioManager.Instance.PlayLose();

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
        StartCoroutine(StartNewRoundCoroutine());
    }

    System.Collections.IEnumerator StartNewRoundCoroutine()
    {
        ClearCardVisuals();

        yield return null;

        playerHand = new Hand();

        dealerHand = new Hand();

        StartRound();
    }

    void ClearCardVisuals()
    {
        for (int i = playerCardArea.childCount - 1; i >= 0; i--)
        {
            Destroy(playerCardArea.GetChild(i).gameObject);
        }

        for (int i = dealerCardArea.childCount - 1; i >= 0; i--)
        {
            Destroy(dealerCardArea.GetChild(i).gameObject);
        }
    }

    void RevealDealerHiddenCard()
    {
        CardDisplay hiddenCardDisplay =
            hiddenDealerCardObject.GetComponent<CardDisplay>();

        hiddenCardDisplay.SetupCard(hiddenDealerCardData);
    }
}