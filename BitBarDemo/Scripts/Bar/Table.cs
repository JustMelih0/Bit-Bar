using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public bool isTableFulled ;
    public Order currentOrder {get; private set;}
    public Customer currentCustomer{get; private set;}

    [SerializeField]private GameObject orderScreen;
    [SerializeField]private GameObject trash;
    [SerializeField]private Button orderButton;
    [SerializeField]private Image fillBar;

    [SerializeField]private Transform sitPoint;
    [SerializeField]private Image orderImage;

    [SerializeField]private Button cleanButton;
    [SerializeField]private GameObject cleanScreen;


    private float orderTime;

    private void Start() 
    {
        isTableFulled = false;
        orderButton.onClick.AddListener(()=> GiveOrder());
        cleanButton.onClick.AddListener(() => CleanRequest());
    }
    public void SitTable(Order order, Customer customer)
    {
        Debug.Log("masaya oturuldu");
        orderScreen.SetActive(true);
        orderImage.gameObject.SetActive(true);

        currentCustomer = customer;
        currentOrder = order;

        orderImage.sprite = order.orderSprite;

        currentCustomer.transform.position = sitPoint.position;
        orderTime = currentCustomer.waitTime;

        waitingOrderCoroutine ??= StartCoroutine(WaitingOrder());
    }
    public void ReservedTable()
    {
        isTableFulled = true;
    }

    private Coroutine waitingOrderCoroutine;
    private IEnumerator WaitingOrder()
    {
        float newTime = orderTime;
        while (newTime > 0)
        {
            newTime -= Time.deltaTime;
            fillBar.fillAmount = newTime/orderTime;
            yield return null;
        }
        currentCustomer.AngerIsLoaded();
        currentCustomer = null;
        currentOrder = null;
        isTableFulled = false;
        ResetWaitingOrder();
    }
    private void ResetWaitingOrder()
    {
        fillBar.fillAmount = 1f;
        orderScreen.SetActive(false);
        waitingOrderCoroutine = null;
    }
    public void GiveOrder()
    {
        bool condition = Bar.Instance.NewOrder(currentOrder, gameObject);
        if (!condition)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("Give_Order");
        orderImage.gameObject.SetActive(false);
    }
    public void CustomerEatEnd()
    {
        currentCustomer = null;
        
        if (Random.Range(0, 2) == 0)
        {
            CleanedTable();
            return;
        }

        trash.SetActive(true);
        cleanScreen.SetActive(true);
    }
    public void CleanedTable()
    {
        trash.SetActive(false);
        isTableFulled = false;
    }
    public void CleanRequest()
    {
        if (!Bar.Instance.CleanTableRequest(this))
            return;

        AudioManager.Instance.PlaySFX("Mop_Sound");
        cleanScreen.SetActive(false);
    }
    public void GetDrink(Order order)
    {
        if (currentCustomer == null)
        {
            return;
        }

        AudioManager.Instance.PlaySFX("Beer_Drink");
        if (waitingOrderCoroutine != null)
        {
            StopCoroutine(waitingOrderCoroutine);
            ResetWaitingOrder();
        }
        currentCustomer.EatOrder(order, this);
    }
}
