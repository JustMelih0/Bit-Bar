using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newWaiterData", menuName = "SO/WorkerData/WaiterData", order = 0)]
public class WaiterStatsData : WorkerStatsData
{
    public List<WaiterStats> waiterStats = new List<WaiterStats>();

    [System.Serializable]
    public class WaiterStats: WorkerStats
    {
        public float waitTime;
    }
}


