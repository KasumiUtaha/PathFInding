using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : MonoBehaviour
{
    public GridMeshCreator gridMeshCreator;
    Queue<Vector2Int> bfsQueue = new Queue<Vector2Int>();
    Dictionary<Vector2Int , Vector2Int> parentDic = new Dictionary<Vector2Int, Vector2Int>();
    Dictionary<Vector2Int , bool> visDic = new Dictionary<Vector2Int, bool>();
    Vector2Int[] d = new Vector2Int[8] {
        new Vector2Int(1 , 0) , new Vector2Int(-1, 0) , new Vector2Int(0, 1) , new Vector2Int(0, -1),
        new Vector2Int(1, 1) , new Vector2Int(1, -1) ,new Vector2Int(-1,1) , new Vector2Int(-1,-1)
        };

    private int limit = 8;

    public void SetGridMeshCreator(GridMeshCreator GMC , Vector2Int beginPoint , Vector2Int endPoint , bool allowDiagnal)
    {
        gridMeshCreator = GMC;
        gridMeshCreator.ClearSearch();
        if (allowDiagnal == false)
        {
            limit = 4;
        }
        else
        {
            limit = 8;
        }
        StartCoroutine(BFS(beginPoint , endPoint));
    }

    IEnumerator BFS(Vector2Int beginPoint , Vector2Int endPoint)
    {
        bfsQueue.Clear();
        visDic.Clear();
        parentDic.Clear();
        bfsQueue.Enqueue(beginPoint);
        visDic.Add(beginPoint, true);
        while(bfsQueue.Count > 0)
        {
            Vector2Int nowPoint = bfsQueue.Dequeue();
            if(nowPoint !=  beginPoint)
            {
                gridMeshCreator.grids[nowPoint.x, nowPoint.y].SetSearched();
            }
            bool isFind = false;
            for(int i = 0; i < limit ; i++ )
            {
                var dep = d[i];
                Vector2Int nextPoint = nowPoint + dep;
                if (CheckNextPoint(nextPoint) == false)
                {
                    continue;
                }
                parentDic.Add(nextPoint, nowPoint);
                visDic.Add(nextPoint, true);
                if(nextPoint == endPoint)
                {
                    isFind = true;
                    break;
                }
                gridMeshCreator.grids[(int)nextPoint.x, (int)nextPoint.y].SetwillSearch();
                bfsQueue.Enqueue(nextPoint);
            }
            if (isFind == true)
            {
                break;
            }
            yield return new WaitForSeconds(0.001f);
        }
        gridMeshCreator.DrawPath(parentDic);
        StopAllCoroutines();
    }
    
    bool CheckNextPoint(Vector2Int nextPoint)
    {
        if (nextPoint.x < 0 || nextPoint.y < 0 || nextPoint.x >= gridMeshCreator.width || nextPoint.y >= gridMeshCreator.height)
        {
            return false;
        }
        if (visDic.ContainsKey(nextPoint))
        {
            return false;
        }
        if (gridMeshCreator.grids[nextPoint.x , nextPoint.y].isHinder == true)
        {
            return false;
        }
        return true;
    }
}
