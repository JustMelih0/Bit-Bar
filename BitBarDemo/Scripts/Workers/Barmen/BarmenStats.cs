using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newBarmenData", menuName = "SO/WorkerData/BarmenData", order = 0)]
public class BarmenStatsData: WorkerStatsData
{
    public List<BarmenStats> barmenStats = new List<BarmenStats>();
    
    [System.Serializable]
    public class BarmenStats: WorkerStats 
    {

    }
}



