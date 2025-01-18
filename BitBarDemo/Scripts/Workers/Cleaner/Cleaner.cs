using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class Cleaner : Worker
{
    [SerializeField]private Task currentTask;
    private float walkSpeed;
    private float cleanTime;
    [SerializeField]private short facingRight = 1;
    [SerializeField]private CleanerStatsData cleanerStatsData;
    private Animator anim;
    private Table currentTable;
    private Vector2 startPoint;


    private void Awake() {
        anim = GetComponent<Animator>();
    }
    private void Start() 
    {

        
        float randomStart = Random.Range(0f, 1f); 
        anim.Play("cleaner_idle", -1, randomStart);

        int level = PlayerPrefs.GetInt("CleanerLevel");
        walkSpeed = cleanerStatsData.cleanerStats[level].walkSpeed;
        cleanTime = cleanerStatsData.cleanerStats[level].workSpeed;
        isWorking = false;
        startPoint = transform.position;
    }

    public bool IsCleanerEmpty()
    {
        return currentTable == null;
    }
    public void GetTable(Table newTable)
    {
        ResetCleaner();
        currentTable = newTable;
        currentTask = Task.GoToTable;
        ControlMachine();
    }
    private void ResetCleaner()
    {
        StopAllCoroutines();
        cleanTableCoroutine = null;
        goToTableCoroutine = null;
        goToBaseCoroutine = null;
        isWorking = false;
        anim.SetBool("isWalking", false);
    }

    private void ControlMachine()
    {
        switch (currentTask)
        {
            
            case Task.Wait: anim.SetBool("isWalking", false);
            break;

            case Task.GoToTable: if (!isWorking)
            {
                goToTableCoroutine ??= StartCoroutine(GoToTable());
            }
            break;

            case Task.CleanTable: if(!isWorking)
            {
                cleanTableCoroutine ??= StartCoroutine(CleanTable());
            }
            break;

            case Task.ReturnBase: if (!isWorking)
            {
                goToBaseCoroutine ??= StartCoroutine(GoToBase());
            }
            break;
        }
    }

    Coroutine cleanTableCoroutine;
    private IEnumerator CleanTable()
    {
        isWorking = true;
        anim.SetBool("isWorking", true);
        yield return new WaitForSeconds(cleanTime);
        anim.SetBool("isWorking", false);
        isWorking = false;
        cleanTableCoroutine = null;
        Debug.Log("masa temizlendi");
        currentTable.CleanedTable();
        
        currentTable = null;
        currentTask = Task.ReturnBase;
        ControlMachine();
    }
    Coroutine goToTableCoroutine;
    private IEnumerator GoToTable()
    {
        isWorking = true;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToTarget(currentTable.transform.position, 0.3f));
        Debug.Log("masaya gitti");
        goToTableCoroutine = null;
        isWorking = false;
        currentTask = Task.CleanTable;
        ControlMachine();
    }
    Coroutine goToBaseCoroutine;
    private IEnumerator GoToBase()
    {
        isWorking = true;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToTarget(startPoint, 0.1f));
        Debug.Log("geri döndü");
        goToBaseCoroutine = null;
        isWorking = false;
        currentTask = Task.Wait;
        ControlMachine();
    }
    public IEnumerator MoveToTarget(Vector2 targetPoint, float space)
    {
        FaceToTarget(targetPoint);
        anim.SetBool("isWalking", true);
        while (Vector2.Distance(targetPoint, transform.position) >= space)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint, walkSpeed * Time.deltaTime);
            yield return null;
        }
        anim.SetBool("isWalking", false);
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



    public enum Task
    {
        Wait,
        GoToTable,
        CleanTable,
        ReturnBase
    }
}
