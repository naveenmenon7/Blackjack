using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    Image cardImage;

    void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void SetupCard(Card cardData)
    {
        cardImage.sprite = cardData.artwork;
    }
}