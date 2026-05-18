using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI playerScoreText;

    public TextMeshProUGUI dealerScoreText;

    public TextMeshProUGUI resultText;

    public Button newRoundButton;

    public void UpdateScores(int playerScore, int dealerScore)
    {
        playerScoreText.text = "Player: " + playerScore;

        dealerScoreText.text = "Dealer: " + dealerScore;

        newRoundButton.gameObject.SetActive(true);
    }

    public void ShowResult(string resultMessage)
    {
        resultText.gameObject.SetActive(true);

        resultText.text = resultMessage;
    }

    public void HideResult()
    {
        resultText.gameObject.SetActive(false);

        newRoundButton.gameObject.SetActive(false);
    }
}