using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;

    [Header("Sound Effects")]

    public AudioClip cardDealSound;

    public AudioClip buttonClickSound;

    public AudioClip winSound;

    public AudioClip loseSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayCardDeal()
    {
        audioSource.PlayOneShot(cardDealSound);
    }

    public void PlayButtonClick()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }

    public void PlayWin()
    {
        audioSource.PlayOneShot(winSound);
    }

    public void PlayLose()
    {
        audioSource.PlayOneShot(loseSound);
    }
}