using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Card Visuals")]

    public GameObject cardPrefab;

    public Transform playerCardArea;

    public Transform dealerCardArea;

    public Transform deckPosition;


    [Header("Betting System")]

    public int playerBalance;

    public int currentBet = 0;

    public enum GameState
    {
        BettingPhase,
        PlayerTurn,
        DealerTurn,
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

        playerBalance = GameDataManager.Instance.playerBalance;

        playerHand = new Hand();

        dealerHand = new Hand();

        uiManager.UpdateBetUI(playerBalance, currentBet);

        currentState = GameState.BettingPhase;
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
            GetVisibleDealerValue()
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

    int GetVisibleDealerValue()
    {
        if(dealerHand.cards.Count <= 0)
            return 0;

        return dealerHand.cards[0].value;
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

            currentBet = 0;

            uiManager.UpdateBetUI(playerBalance, currentBet);

            Invoke(nameof(PrepareNextRound), 2f);

            return;
        }

        uiManager.UpdateScores(
            playerHand.CalculateHandValue(),
            GetVisibleDealerValue()
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
            playerBalance += currentBet * 2;

            AudioManager.Instance.PlayWin();

            uiManager.ShowResult("Dealer Busts! Player Wins!");
        }
        else if (playerTotal > dealerTotal)
        {
            playerBalance += currentBet * 2;

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
            playerBalance += currentBet;

            uiManager.ShowResult("Push! It's a Tie!");
        }

        currentBet = 0;

        uiManager.UpdateBetUI(playerBalance, currentBet);

        GameDataManager.Instance.playerBalance = playerBalance;

        Invoke(nameof(PrepareNextRound), 2f);
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

    void PrepareNextRound()
    {
        ClearCardVisuals();

        playerHand = new Hand();

        dealerHand = new Hand();

        uiManager.UpdateScores(0, 0);

        uiManager.HideResult();

        currentState = GameState.BettingPhase;
    }

    void RevealDealerHiddenCard()
    {
        CardDisplay hiddenCardDisplay =
            hiddenDealerCardObject.GetComponent<CardDisplay>();

        hiddenCardDisplay.SetupCard(hiddenDealerCardData);
    }

    public void PlaceBet(int amount)
    {
        AudioManager.Instance.PlayButtonClick();

        if(currentState != GameState.BettingPhase)
            return;

        if(playerBalance < amount)
            return;

        playerBalance -= amount;

        GameDataManager.Instance.playerBalance = playerBalance;

        currentBet += amount;

        uiManager.UpdateBetUI(playerBalance, currentBet);
    }

    public void DealCards()
    {
        AudioManager.Instance.PlayButtonClick();

        if(currentState != GameState.BettingPhase)
            return;

        if(currentBet <= 0)
            return;

        StartNewRound();
    }

    public void ClearBet()
    {
        AudioManager.Instance.PlayButtonClick();

        if(currentState != GameState.BettingPhase)
            return;

        if(currentBet <= 0)
            return;

        playerBalance += currentBet;

        GameDataManager.Instance.playerBalance = playerBalance;

        currentBet = 0;

        uiManager.UpdateBetUI(playerBalance, currentBet);
    }
}