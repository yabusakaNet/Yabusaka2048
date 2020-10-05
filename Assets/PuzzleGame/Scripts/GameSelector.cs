using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSelector : MonoBehaviour
{
    [SerializeField]
    GamePreset[] gamePresets;

    [SerializeField]
    GameObject navigation;
    [SerializeField]
    GameObject fieldBlocker;

    [SerializeField]
    Button next;
    [SerializeField]
    Button previous;
    [SerializeField]
    Toggle[] toggles;

    [SerializeField]
    GameObject restartButton;

    [SerializeField]
    PriceLabel priceLabel;

    [SerializeField]
    MonetizeButton monetizeButton;

    [SerializeField]
    GameObject gameOver;

    int currentGameIndex;
    BaseGameController currentGame;

    static readonly int BigField = Animator.StringToHash("Big");
    static readonly int MiddleField = Animator.StringToHash("Middle");
    static readonly int SmallField = Animator.StringToHash("Small");

    public void MinimizeCurrentGame(bool value)
    {
        if (!value)
        {
            MaximizeCurrentGame();
            return;
        }

        Time.timeScale = 0;
        ResetTriggers();
        currentGame.fieldAnimator.SetTrigger(SmallField);
        navigation.SetActive(false);
        restartButton.SetActive(false);
    }

    void MaximizeCurrentGame()
    {
        bool isGameAvailable = gamePresets[currentGameIndex].price.value <= 0 ||
                               UserProgress.Current.IsItemPurchased(currentGame.name);

        if (isGameAvailable && !gameOver.activeSelf)
        {
            Time.timeScale = 1;
            ResetTriggers();
            currentGame.fieldAnimator.SetTrigger(BigField);
            navigation.SetActive(true);
            restartButton.SetActive(true);
        }
        else
        {
            ResetTriggers();
            currentGame.fieldAnimator.SetTrigger(MiddleField);
            navigation.SetActive(true);
            fieldBlocker.SetActive(true);
        }
    }

    void ResetTriggers()
    {
        currentGame.fieldAnimator.ResetTrigger(BigField);
        currentGame.fieldAnimator.ResetTrigger(MiddleField);
        currentGame.fieldAnimator.ResetTrigger(SmallField);
    }

    void OnNextClick()
    {
        currentGameIndex++;
        currentGameIndex %= gamePresets.Length;

        UpdateCurrentGame();
    }

    void OnPreviousClick()
    {
        currentGameIndex--;
        if (currentGameIndex < 0)
            currentGameIndex += gamePresets.Length;

        UpdateCurrentGame();
    }

    void OnGamePurchased()
    {
        UserProgress.Current.OnItemPurchased(gamePresets[currentGameIndex].name);
        UpdateCurrentGame();
    }

    void UpdateCurrentGame()
    {
        if (currentGame)
            Destroy(currentGame.gameObject);

        currentGame = Instantiate(gamePresets[currentGameIndex].gamePrefab);
        currentGame.name = gamePresets[currentGameIndex].name;

        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = i == currentGameIndex;
        }

        gameOver.SetActive(false);

        Price price = gamePresets[currentGameIndex].price;

        bool isGameAvailable = price.value <= 0 ||
                               UserProgress.Current.IsItemPurchased(currentGame.name);

        Time.timeScale = isGameAvailable ? 1 : 0;

        priceLabel.gameObject.SetActive(!isGameAvailable);
        monetizeButton.gameObject.SetActive(!isGameAvailable);

        restartButton.SetActive(isGameAvailable);
        fieldBlocker.SetActive(!isGameAvailable);

        if (isGameAvailable)
        {
            currentGame.GameOver += OnGameOver;
            return;
        }

        priceLabel.SetPrice(currentGame.name, price);

        ResetTriggers();
        currentGame.fieldAnimator.SetTrigger(MiddleField);

        monetizeButton.SetPrice(currentGame.name, price);
    }

    void OnGameOver()
    {
        ResetTriggers();
        currentGame.fieldAnimator.SetTrigger(MiddleField);
        fieldBlocker.SetActive(true);

        restartButton.SetActive(false);
        gameOver.SetActive(true);
    }

    void Awake()
    {
        currentGameIndex = Array.FindIndex(gamePresets, g => g.name == UserProgress.Current.CurrentGameId);

        if (currentGameIndex < 0)
            currentGameIndex = 0;

        UpdateCurrentGame();

        next.onClick.AddListener(OnNextClick);
        previous.onClick.AddListener(OnPreviousClick);

        monetizeButton.PurchaseComplete += OnGamePurchased;
    }
}
