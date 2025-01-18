using System.Collections;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField]private float totalLifetime = 5f; 
    [SerializeField]private float fadeStartTime = 3f; 
    public float rotationSpeed = 180f;
    [SerializeField]private GameObject moneyIcon;
    private SpriteRenderer spriteRenderer; 
    int totalAmount;
    
    private void Awake() {
        spriteRenderer = moneyIcon.GetComponent<SpriteRenderer>();
    }
    public void Spawn(int amount)
    {
        totalAmount = amount;
        destroyCoroutine ??= StartCoroutine(DestroyTimer());
    }
    
    Coroutine destroyCoroutine;
    private IEnumerator DestroyTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < totalLifetime)
        {
            moneyIcon.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            if (elapsedTime >= fadeStartTime)
            {
                float fadeProgress = (elapsedTime - fadeStartTime) / (totalLifetime - fadeStartTime);
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, fadeProgress); 
                spriteRenderer.color = color;
            }

            elapsedTime += Time.deltaTime; 
            yield return null; 
        }
        destroyCoroutine = null;
        gameObject.SetActive(false);
    }
    public void Click()
    {
        StopAllCoroutines();
        Color color = spriteRenderer.color;
        color.a = 1f; 
        spriteRenderer.color = color;
        MoneyController.Instance.EarnMoney(totalAmount);
        destroyCoroutine = null;
        gameObject.SetActive(false);
    }
}
