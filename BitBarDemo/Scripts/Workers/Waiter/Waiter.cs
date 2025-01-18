using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Waiter : Worker
{

    private float walkSpeed;
    private float waitTime;
    private Barmen currentBarmen;
    [SerializeField]private BarOrder currentOrder;
    [SerializeField]private Task currentTask;
    [SerializeField]private WaiterStatsData waiterStatsData;
    [SerializeField]private short facingRight = 1;
    Vector2 startPoint;
    Vector2 barmenPoint;
    private Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }
    private void Start() {  
        float randomStart = Random.Range(0f, 1f); 
        anim.Play("waiter_idle", -1, randomStart);
        currentTask = Task.Wait;
        startPoint = transform.position;
        int level = PlayerPrefs.GetInt("WaiterLevel");
        walkSpeed = waiterStatsData.waiterStats[level].workSpeed;
        waitTime = waiterStatsData.waiterStats[level].waitTime;
    }
    public bool IsWaiterEmpty()
    {
        return !isWorking;
    }

    public void SetOrder(BarOrder barOrder, Vector2 newBarmenPoint, Barmen barmen)
    {
        currentBarmen = barmen;
        currentOrder = barOrder;
        barmenPoint = newBarmenPoint;
        currentTask = Task.GoToBar;
        ControlMachine();
    }
    private void ControlMachine()
    {
        switch (currentTask)
        {        
            case Task.Wait: if (!isWorking && currentOrder.order != null)
            {
                anim.SetBool("isWalking", false);
                isWorking = true;
                currentTask = Task.GoToBar;
                goingToBarCoroutine ??= StartCoroutine(GoingToBar());
            }
            break;

            case Task.GoToTable: if (!isWorking)
            {
                isWorking = true;
                goingTableCoroutine ??= StartCoroutine(GoingToTable());
            }
            break;

            case Task.GoToMidle: if (!isWorking)
            {
                goingToMiddleCoroutine ??= StartCoroutine(GoingToMiddle());
            }
            break;

            case Task.GoToBar: if(!isWorking)
            {
                isWorking = true;
                goingToBarCoroutine ??= StartCoroutine(GoingToBar());
            }
            break;
        }
    }

    Coroutine goingToMiddleCoroutine;
    IEnumerator GoingToMiddle()
    {
        anim.SetBool("isWalking", false);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isWalking", true);
        FaceToTarget(startPoint);
        while (Vector2.Distance(transform.position, startPoint) >= 0.1 && currentOrder.order == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPoint, Time.deltaTime * walkSpeed);
            yield return null;
        }

        if (currentOrder.order != null)
        {
            currentTask = Task.GoToBar;
        }
        else
        {
            anim.SetBool("isWalking", false);
            currentTask = Task.Wait;
        }
        goingToMiddleCoroutine = null;
        ControlMachine();
        
    }

    Coroutine goingToBarCoroutine;
    IEnumerator GoingToBar()
    {
        
        anim.SetBool("isWalking", false);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isWalking", true);

        yield return StartCoroutine(MoveToTarget(barmenPoint, 0.6f));
        
        if (currentOrder.order != null)
        {
            anim.SetTrigger("getDrink");
            currentBarmen.TakeBeer(currentOrder.order);
            currentTask = Task.GoToTable;
        }
        else
        {
            currentTask = Task.GoToMidle;
        }
        isWorking = false;
        currentBarmen = null;
        goingToBarCoroutine = null;
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
    Coroutine goingTableCoroutine;
    IEnumerator GoingToTable()
    {
        anim.SetBool("isWalking", false);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isWalking", true);

        Table table = currentOrder.table.GetComponent<Table>();
        FaceToTarget(currentOrder.table.transform.position);
        while (Vector2.Distance(transform.position, currentOrder.table.transform.position) >= 0.1 && table.currentCustomer != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentOrder.table.transform.position, Time.deltaTime * walkSpeed);
            yield return null;
        }
        anim.SetBool("isWalking", false);

        if (table.currentCustomer != null)
        {
            anim.SetTrigger("giveDrink");
            currentOrder.table.GetComponent<Table>().GetDrink(currentOrder.order);
            yield return new WaitForSeconds(waitTime);
        }
        goingTableCoroutine = null;
        isWorking = false;
        currentOrder.order = null;  
        currentTask = Task.GoToMidle;
        ControlMachine();
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

    public  enum Task
    {
        GoToBar,
        GoToTable,
        GoToMidle,
        Wait
    }
}
