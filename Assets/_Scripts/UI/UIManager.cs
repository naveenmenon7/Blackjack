using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;

    public TextMeshProUGUI dealerScoreText;

    public TextMeshProUGUI resultText;

    public TextMeshProUGUI balanceText;

    public TextMeshProUGUI betText;

    public void UpdateScores(int playerScore, int dealerScore)
    {
        playerScoreText.text = "Player: " + playerScore;

        dealerScoreText.text = "Dealer: " + dealerScore;
    }

    public void ShowResult(string resultMessage)
    {
        resultText.gameObject.SetActive(true);

        resultText.text = resultMessage;
    }

    public void HideResult()
    {
        resultText.gameObject.SetActive(false);
    }

    public void UpdateBetUI(int balance, int bet)
    {
        balanceText.text = "Balance: $" + balance;

        betText.text = "Bet: $" + bet;
    }
}