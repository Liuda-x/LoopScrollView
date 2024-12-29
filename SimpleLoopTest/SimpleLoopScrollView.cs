using Liudax.LoopScrollView;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLoopScrollView : MonoBehaviour
{
     public LoopScrollView loopView;
    public int spawnCount = 5;
    public int jump = 0;
    public void Start()
    {
        loopView.Init();
        List<string > list= new List<string>();
        for(int i = 0; i < spawnCount; i++)
        {
            list.Add($"item:{i}");
        }
        loopView.SetData(list, jump);
        //loopView.MoveToIndex(99);
    }
    public void OnDestroy()
    {
        loopView.Dispse();
    }
}
