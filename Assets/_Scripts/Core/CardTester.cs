using UnityEngine;

public class CardTester : MonoBehaviour
{
    void Start()
    {
        Card testCard = new Card("Hearts", "A", 11, null);

        Debug.Log("Card Created: " + testCard.rank + " of " + testCard.suit);
        Debug.Log("Card Value: " + testCard.value);
    }
}