using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance;
    public Image[] imageSlots;
    public Button[] answerButtons;
    public Button nextButton;
    public Button repeatButton;
    public ProgressController progressController;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI topbarCoinsText;
    public Sprite[] allSprites;
    public GameObject popupWin;
    public GameObject popupLose;
    public Image[] starIcons;
    public TextMeshProUGUI coinsText;

    private int[] roundIndices;
    private Dictionary<Button, int> mapping;
    private bool[] matched;
    private int matchedCount;
    private int attempts;
    private int currentRound;
    private int selectedSlot = -1;
    private int totalCoins;
    private float timeRemaining;
    private bool timerRunning;

    private const int totalRounds = 21;
    private readonly int[] coinsPerStar = { 10, 30, 60, 100 };

    void Awake() { Instance = this; }

    void Start()
    {
        int init;
        var parts = topbarCoinsText.text.Split(' ');
        totalCoins = int.TryParse(parts[0], out init) ? init : 0;
        topbarCoinsText.text = totalCoins + " ";
        nextButton.onClick.AddListener(OnNextRound);
        repeatButton.onClick.AddListener(OnRepeatRound);
        currentRound = 0;
        SetupRound();
    }

    void Update()
    {
        if (!timerRunning) return;
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        if (timeRemaining <= 0f)
        {
            timerRunning = false;
            popupLose.SetActive(true);
            repeatButton.gameObject.SetActive(true);
        }
    }

    public void SelectImage(int slot)
    {
        if (timerRunning) selectedSlot = slot;
    }

    void SetupRound()
    {
        if (currentRound < totalRounds) currentRound++;
        attempts = 0;
        matched = new bool[3];
        matchedCount = 0;
        selectedSlot = -1;
        popupWin.SetActive(false);
        popupLose.SetActive(false);
        nextButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
        timeRemaining = 60f;
        timerRunning = true;
        timerText.text = "60";
        roundIndices = Enumerable.Range(0, allSprites.Length).OrderBy(i => Random.value).Take(3).ToArray();
        mapping = new Dictionary<Button, int>();
        var options = roundIndices.Select(i => new { n = allSprites[i].name, idx = i }).OrderBy(x => Random.value).ToArray();
        for (int i = 0; i < 3; i++)
        {
            imageSlots[i].sprite = allSprites[roundIndices[i]];
            var click = imageSlots[i].GetComponent<ClickableImage>();
            if (click != null) click.slotIndex = i;
            var btn = answerButtons[i];
            btn.onClick.RemoveAllListeners();
            mapping[btn] = options[i].idx;
            btn.GetComponentInChildren<TextMeshProUGUI>().text = options[i].n;
            btn.onClick.AddListener(() => OnAnswerSelected(btn));
        }
    }

    public void OnAnswerSelected(Button btn)
    {
        if (!timerRunning || selectedSlot < 0 || matched[selectedSlot]) return;

        if (mapping[btn] == roundIndices[selectedSlot])
        {
            matched[selectedSlot] = true;
            matchedCount++;
            if (matchedCount == 3)
            {
                timerRunning = false;
                popupWin.SetActive(true);
                int stars = attempts == 0 ? 3 : attempts <= 3 ? 2 : attempts <= 5 ? 1 : 0;
                for (int i = 0; i < starIcons.Length; i++)
                    starIcons[i].color = i < stars ? Color.yellow : Color.gray;
                int earned = coinsPerStar[stars];
                totalCoins += earned;
                coinsText.text = "+" + earned;
                topbarCoinsText.text = totalCoins + " ";
                progressController.AdvanceRound();
                nextButton.gameObject.SetActive(currentRound < totalRounds);
                repeatButton.gameObject.SetActive(stars < 3);
            }
        }
        else
        {
            attempts++;
            if (attempts >= 6)
            {
                timerRunning = false;
                popupLose.SetActive(true);
                repeatButton.gameObject.SetActive(true);
            }
        }

        selectedSlot = -1;
    }

    public void OnNextRound()
    {
        if (currentRound < totalRounds)
            SetupRound();
        else
            nextButton.gameObject.SetActive(false);
    }

    public void OnRepeatRound()
    {
        attempts = 0;
        matched = new bool[3];
        matchedCount = 0;
        selectedSlot = -1;
        popupWin.SetActive(false);
        popupLose.SetActive(false);
        timerRunning = true;
        timeRemaining = 60f;
        timerText.text = "60";
        nextButton.gameObject.SetActive(false);
        repeatButton.gameObject.SetActive(false);
    }
}
