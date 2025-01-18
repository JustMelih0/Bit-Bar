using System;
using System.Collections;
using UnityEngine;

public class Customer : Worker
{
    [SerializeField]private Task currentTask = Task.Wait;
    [SerializeField]private Color customerAngryColor;
    private BarOrder currentOrder;
    private float walkSpeed;
    private Vector2 startPoint;
    private Vector2 barDoorPoint;
    [SerializeField]private short facingRight = 1;
    private Table currentTable; 
    public float waitTime;
    private float drinkTime;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public static event Action customerFinished;
    public static event Action customerAngry;
    public static event Action customerSuccess;
    
    private void Awake() {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Spawn(Order order, GameObject targetTable, CustomerData customerData, RuntimeAnimatorController runtimeAnimatorController)
    {
        spriteRenderer.color = Color.white;
        anim.runtimeAnimatorController = runtimeAnimatorController;
        isWorking = false;
        startPoint = transform.position;
        barDoorPoint = Bar.Instance.doorPoint.position;
        walkSpeed = customerData.walkSpeed;
        waitTime = customerData.waitTime;
        drinkTime = customerData.drinkTime;
        currentOrder = new(order, targetTable);
        currentTask = Task.GoToTable;
        currentTable = currentOrder.table.GetComponent<Table>();
        ControlMachine();
    }
    private void ControlMachine()
    {
        switch (currentTask)
        {
            case Task.GoToTable: if (!isWorking)
            {
                goingTableCoroutine ??= StartCoroutine(GoingToTable());
            }
            break;

            case Task.GiveOrder: if (!isWorking)
            {
                anim.SetTrigger("sit");
                anim.SetBool("waitOrder", true);
                currentTable.SitTable(currentOrder.order, this);
                FaceToTarget(currentOrder.table.transform.position);
                currentTask = Task.Wait;
            } 
            break;

            case Task.GoOut: if(!isWorking)
            {
                goingOutCoroutine ??= StartCoroutine(GoingToOut());
            }
            break;

            case Task.Angry: 
                customerAngry?.Invoke();
                MoneyController.Instance.EarnMoney(-currentOrder.order.orderPrice / 2);
                currentTask = Task.GoOut;
                goingOutCoroutine ??= StartCoroutine(GoingToOut());
            break;
        }
    }
    private Coroutine goingTableCoroutine;
    private IEnumerator GoingToTable()
    {
        isWorking = true;
        anim.SetBool("isWalking", true);
        yield return StartCoroutine(MoveToTarget(barDoorPoint, 0.1f));
        yield return StartCoroutine(MoveToTarget(currentOrder.table.transform.position, 0.3f));
        anim.SetBool("isWalking", false);
        yield return new WaitForSeconds(0.5f);
        goingTableCoroutine = null;
        currentTask = Task.GiveOrder;
        isWorking = false;
        
        ControlMachine();
    }
    private void ResetCustomer()
    {
        StopAllCoroutines();
        goingOutCoroutine = null;
        goingOutCoroutine = null;
        eatCoroutine = null;
    }
    public IEnumerator MoveToTarget(Vector2 targetPoint, float space)
    {
        FaceToTarget(targetPoint);
        while (Vector2.Distance(targetPoint, transform.position) >= space)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint, walkSpeed * Time.deltaTime);
            yield return null;
        }
    }
    
    private Coroutine goingOutCoroutine;
    private IEnumerator GoingToOut()
    {
        isWorking = true;
        anim.SetBool("isWalking", true);
        yield return StartCoroutine(MoveToTarget(barDoorPoint, 0.1f));
        yield return StartCoroutine(MoveToTarget(startPoint, 0.1f));
        goingOutCoroutine = null;
        isWorking = false;
        customerFinished?.Invoke();
        ResetCustomer();
        gameObject.SetActive(false);
    }

    public void EatOrder(Order order, Table currentTable)
    {
        isWorking = true;
        currentTask = Task.EatOrder;
        if (order == currentOrder.order)
        {  
            eatCoroutine ??= StartCoroutine(EatTimer(currentTable));
        }
    }
    private Coroutine eatCoroutine;
    private IEnumerator EatTimer(Table currentTable)
    {
        isWorking = true;
        anim.SetBool("drink", true);
        yield return new WaitForSeconds(drinkTime);
        anim.SetBool("drink", false);
        isWorking = false;
        eatCoroutine = null;
        currentTask = Task.GoOut;
        GameObject money = PoolManager.Instance.SpawnFromPool("Money", transform.position, Quaternion.identity);
        money.GetComponent<Money>().Spawn(currentOrder.order.orderPrice);
        currentTable.CustomerEatEnd();
        customerSuccess?.Invoke();
        ControlMachine();
    }
    public void AngerIsLoaded()
    {
        StopAllCoroutines();
        eatCoroutine = null;
        goingOutCoroutine = null;
        goingTableCoroutine = null;
        spriteRenderer.color = customerAngryColor;
        currentTask = Task.Angry;
        ControlMachine();
    }
    private void FaceToTarget(Vector2 target)
    {
        if (facingRight == 1 && transform.position.x > target.x || facingRight == -1 && transform.position.x < target.x)
        {
            CharacterFlip();
        }
    }
    private void CharacterFlip()
    {
        facingRight *= -1;
        transform.rotation = Quaternion.Euler(0f, facingRight == 1 ? 0f : 180f, 0f);
    }
    enum Task
    {
        Wait,
        EatOrder,
        GoToTable,
        GoOut,
        GiveOrder,
        Angry
    }
}
