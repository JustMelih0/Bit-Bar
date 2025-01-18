using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newCleanerData", menuName = "SO/WorkerData/CleanerData", order = 0)]
public class CleanerStatsData : WorkerStatsData
{
    public List<CleanerStats> cleanerStats = new List<CleanerStats>();

    [System.Serializable]
    public class CleanerStats: WorkerStats
    {
        public float walkSpeed;
    }
}
