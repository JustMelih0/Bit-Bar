using System;
using System.Collections.Generic;
using UnityEngine;


public class Bar : MonoBehaviour
{

    [SerializeField]private List<Table> barTables = new();
    [SerializeField]private List<Order> drinks = new();
    [SerializeField]private List<Barmen> barmens = new();
    [SerializeField]private List<Cleaner> cleaners = new();
    [SerializeField]private List<Waiter> waiters = new();

    private int drinkLevel;
    public Transform doorPoint;

    public static Bar Instance;
    




    private void Start() {
        drinkLevel  = PlayerPrefs.GetInt("BeerLevel") + 1;
        if (drinkLevel >= drinks.Count)
        {
            drinkLevel = drinks.Count; 
        }
    }

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
    public Order GetDrink(int index)
    {
        return drinks[index];
    }
    public Order GetRandomDrink()
    {
        int randomIndex = UnityEngine.Random.Range(0, drinkLevel);
        return drinks[randomIndex];
    }

   public bool NewOrder(Order order, GameObject table)
   {
        foreach (Barmen item in barmens)
        {
            if (item.gameObject.activeInHierarchy && !item.isBarmenWorking)
            {
                item.NewOrder(order, table);
                return true;
            }
        }
        return false;
   }
   public Table IsEmptyTable()
   {
       for (int i = 0; i < barTables.Count; i++)
       {
            if (barTables[i].gameObject.activeInHierarchy && !barTables[i].isTableFulled)
            {   
                return barTables[i];
            }
       }
       return null;
   }
   public bool CleanTableRequest(Table table)
   {
        List<Cleaner> emptyCleaners = new();
        foreach (Cleaner item in cleaners)
        {
            if (item.gameObject.activeInHierarchy && item.IsCleanerEmpty())
            {
                emptyCleaners.Add(item);
            }
        }

        if (emptyCleaners.Count == 0)
        {
            Debug.Log("Boşta temizlikçi yok");
            return false;
        }

        float distance = float.MaxValue;
        Cleaner cleaner = null;
        foreach (Cleaner item in emptyCleaners)
        {
            float newDistance = Vector2.Distance(item.transform.position, table.transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                cleaner = item;
            }
        }
        cleaner.GetTable(table);
        return true;
   }
   public Waiter WaiterRequest(Barmen barmen)
   {
        List<Waiter> emptyWaiters = new();
        foreach (Waiter item in waiters)
        {
            if (item.gameObject.activeInHierarchy && item.IsWaiterEmpty())
            {
                emptyWaiters.Add(item);
            }
        }
        
        if (emptyWaiters.Count == 0)
        {
            return null;
        }

        float distance = float.MaxValue;
        Waiter waiter = null;
        foreach (Waiter item in emptyWaiters)
        {
            float newDistance = Vector2.Distance(item.transform.position, barmen.transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                waiter = item;
            }
        }
        return waiter;

   }


}
[System.Serializable]
public class BarOrder
{
   public Order order;
   public GameObject table;

    public BarOrder(Order newOrder, GameObject newTable)
    {
        order = newOrder;
        table = newTable;
    }
}


