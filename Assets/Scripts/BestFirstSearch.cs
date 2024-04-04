using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BestFirstSearch : MonoBehaviour
{
    public class Node : IComparable<Node>
    {
        public Vector2Int position;
        public float value;

        public Node(Vector2Int _position , float _value)
        {
            position = _position;
            value = _value;
        }

        public int CompareTo(Node other)
        {
            if(value > other.value)
            {
                return 1;
            }
            else if(value == other.value)
            {
                if(position == other.position)
                {
                    return 0;
                }
                else if(position.x <= other.position.x)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            return -1;
        }
    }

    public GridMeshCreator gridMeshCreator;
    public SortedSet<Node> bfsSet = new SortedSet<Node>();
    Dictionary<Vector2Int, Vector2Int> parentDic = new Dictionary<Vector2Int, Vector2Int>();
    Dictionary<Vector2Int, bool> visDic = new Dictionary<Vector2Int, bool>();

    Vector2Int[] d = new Vector2Int[8] {
        new Vector2Int(1 , 0) , new Vector2Int(-1, 0) , new Vector2Int(0, 1) , new Vector2Int(0, -1),
        new Vector2Int(1, 1) , new Vector2Int(1, -1) ,new Vector2Int(-1,1) , new Vector2Int(-1,-1)
        };

    private int limit = 8;
    public void SetGridMeshCreator(GridMeshCreator GMC, Vector2Int beginPoint, Vector2Int endPoint , bool allowDiagnal)
    {
        gridMeshCreator = GMC;
        gridMeshCreator.ClearSearch();
        if(allowDiagnal == false)
        {
            limit = 4;
        }
        else
        {
            limit = 8;
        }
        StartCoroutine(BFS(beginPoint, endPoint));
    }

    IEnumerator BFS(Vector2Int beginPoint, Vector2Int endPoint)
    {
        bfsSet.Clear();
        parentDic.Clear();
        visDic.Clear();
        bfsSet.Add(new Node(beginPoint , CalcValue(beginPoint, endPoint)));
        visDic.Add(beginPoint, true);
        while(bfsSet.Count > 0)
        {
            Node nowNode = bfsSet.Min;
            bfsSet.Remove(nowNode);
            Vector2Int nowPoint = nowNode.position;
            if(nowPoint != beginPoint)
            {
                gridMeshCreator.grids[nowPoint.x, nowPoint.y].SetSearched();
            }
            bool isFind = false;
            for(int i = 0; i < limit; i++ )
            {
                var dep = d[i];
                Vector2Int nextPoint = nowPoint + dep;
                if (CheckNextPoint(nextPoint) == false)
                {
                    continue;
                }
                parentDic.Add(nextPoint, nowPoint);
                visDic.Add(nextPoint, true);
                if (nextPoint == endPoint)
                {
                    isFind = true;
                    break;
                }
                gridMeshCreator.grids[nextPoint.x, nextPoint.y].SetwillSearch();
                bfsSet.Add(new Node(nextPoint , CalcValue(nextPoint, endPoint)));
            }
            if(isFind == true)
            {
                break;
            }
            yield return new WaitForSeconds(0.001f);
        }
        gridMeshCreator.DrawPath(parentDic);
        StopAllCoroutines();
    }

    public float CalcValue(Vector2Int nowPoint , Vector2Int endPoint)
    {
        return Math.Abs(endPoint.x - nowPoint.x) + Math.Abs(endPoint.y - nowPoint.y);
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
        if (gridMeshCreator.grids[nextPoint.x, nextPoint.y].isHinder == true)
        {
            return false;
        }
        return true;
    }

}
