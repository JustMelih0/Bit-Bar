using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI moneyText;
    [SerializeField]private Image moneyImage;
    [SerializeField]private float maxScale;
    [SerializeField]private float effectDuration;
    [SerializeField]private Color earnTextColor;
    [SerializeField]private Color loseTextColor;


    private int totalMoney;
    private int earnedMoney;

    public static MoneyController Instance;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable() {
        GameManager.OnDayEnd += DayEndBonus;
        RewardAd.giveReward += RewardAdBonus;
    }
    private void OnDisable() {
        GameManager.OnDayEnd -= DayEndBonus;
        RewardAd.giveReward -= RewardAdBonus;
    }
    private void RewardAdBonus()
    {
        int completedDay = PlayerPrefs.GetInt("completedDay");
        int rewardBonus = PlayerPrefs.GetInt("rewardBonus");
        if (completedDay < 50 && rewardBonus == 1)
        {
            PlayerPrefs.SetInt("rewardBonus", 0); 
            EarnMoney(completedDay * 5);
        }
    }
    private void DayEndBonus()
    {
        EarnMoney(earnedMoney / 3);
    }
    private void Start() {
        totalMoney = PlayerPrefs.GetInt("TotalMoney");
        moneyText.text = "x " + totalMoney;
    }
    public int GetMoney()
    {
        return totalMoney;
    }

    public void EarnMoney(int amount)
    {
        Color sendColor = loseTextColor;
        if (totalMoney < totalMoney + amount)
        { 
            sendColor = earnTextColor;
            earnedMoney += amount;
            AudioManager.Instance.PlaySFX("Cash_In");
        }
        else
        {
            earnedMoney = 0;
            AudioManager.Instance.PlaySFX("Cash_Out");
        }
        totalMoney += amount;

        if(earnedMoney <= 0)
            earnedMoney = 0;

        if (totalMoney <= 0)
            totalMoney = 0;

        PlayerPrefs.SetInt("TotalMoney", totalMoney);
        moneyText.text = "x " + totalMoney;
        moneyEffectCoroutine ??= StartCoroutine(MoneyEffect(sendColor));
    }


    Coroutine moneyEffectCoroutine;
    IEnumerator MoneyEffect(Color newColor)
    {
        Vector3 originalScale = moneyImage.rectTransform.localScale; 
        Vector3 targetScale = originalScale * maxScale; 
        float elapsedTime = 0f;
        Color originalColor = moneyText.color;
        moneyText.color = newColor;

        while (elapsedTime < effectDuration / 2)
        {
            float t = elapsedTime / (effectDuration / 2);
            moneyImage.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < effectDuration / 2)
        {
            float t = elapsedTime / (effectDuration / 2);
            moneyImage.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        moneyText.color = originalColor;
        moneyImage.rectTransform.localScale = originalScale;
        moneyEffectCoroutine = null;
    }
}
