using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barmen : Worker
{
    [SerializeField]private BarmenStatsData barmenStatsData;
    BarOrder barOrder;
    public bool isBarmenWorking => isWorking;
    private float workSpeed;
    [SerializeField]private Image fillerBar;
    [SerializeField]private Button callWaiterButton;
    [SerializeField]private GameObject callWaiterUI;
    [SerializeField]private SpriteRenderer beerSprite;
    private List<Order> orders = new();

    private GameObject fillBarParent;



    [SerializeField]private Task currentTask = Task.Wait;
    private Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void Start() 
    {
        callWaiterButton.onClick.AddListener(() => CallWaiter());
        float randomStart = Random.Range(0f, 1f); 
        anim.Play("barmen_idle", -1, randomStart);
        int level = PlayerPrefs.GetInt("BarmenLevel");
        fillBarParent = fillerBar.transform.parent.gameObject;
        fillBarParent.SetActive(false);
        workSpeed = barmenStatsData.barmenStats[level].workSpeed;
        isWorking = false;
    }
    public void NewOrder(Order order, GameObject table)
    {
        if (isWorking)
        {
            return;
        }
        barOrder = new BarOrder(order, table);
        currentTask = Task.PrepareOrder;
        ControlMachine();
    }
    private void ControlMachine()
    {
        switch (currentTask)
        {   
            case Task.PrepareOrder:if (!isWorking)
            {
                orderCoroutine ??= StartCoroutine(PrepareOrder(barOrder.order.processTime));
            }
            break;
        }
    }
    Coroutine orderCoroutine;
    private IEnumerator PrepareOrder(float time)
    {
        isWorking = true;
        anim.SetBool("isWorking", true);
        float dTime = workSpeed + time;
        fillBarParent.SetActive(true);
        while (dTime > 0)
        {
            dTime -= Time.deltaTime;
            fillerBar.fillAmount = dTime / (workSpeed + time);
            yield return null;
        }
        fillBarParent.SetActive(false);
        ControlMachine();
        OrderIsCompleted();
        anim.SetBool("isWorking", false);
        orderCoroutine = null;
    }
    public void OrderIsCompleted()
    {
        orders.Add(barOrder.order);
        RefreshBeerImage();
        currentTask = Task.Wait;
        callWaiterUI.SetActive(true);
    }
    public void CallWaiter()
    {
        Waiter waiter = Bar.Instance.WaiterRequest(this);
        if (waiter == null)
        {
            return;
        }
        AudioManager.Instance.PlaySFX("Ring_Sound");
        waiter.SetOrder(barOrder, transform.position, this);
        callWaiterUI.SetActive(false);
        barOrder = null;
        isWorking = false;
    }
    private void RefreshBeerImage()
    {
        foreach (Order item in orders)
        {
            beerSprite.sprite = item.orderSprite;
            return;
        }
        beerSprite.sprite = null;
    }
    public void TakeBeer(Order order)
    {
        orders.Remove(order);
        RefreshBeerImage();
    }
    public enum Task
    {
        Wait,
        PrepareOrder
    }
   
}