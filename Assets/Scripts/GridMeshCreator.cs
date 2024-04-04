using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridMeshCreator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public Vector3 startPoint;
    public GameObject gridPrefab;
    public Transform parentTransform;

    public Vector2Int beginPoint = new Vector2Int(15, 25);
    public Vector2Int endPoint = new Vector2Int(30, 25);

    
    public LineRenderer lineRenderer;
    public Color pathColor;
    public bool allowDiagnal = true;

    [HideInInspector]
    public Grid[,] grids;

    public BreadthFirstSearch breadthFirstSearch;
    public BestFirstSearch bestFirstSearch;
    public A_Search a_Search;

    public void SetGridColor(Vector2Int vector2 , Color color)
    {
        grids[vector2.x,vector2.y].color = color;
    }


    public void CreateMesh()
    {
        ClearMesh();
        grids = new Grid[width, height];
        for(int i=0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CreateGrid(i, j);
            }
        }
        SetGridColor(beginPoint, Color.green);
        SetGridColor(endPoint, Color.red);
    }

    void CreateGrid(int x,int y)
    {
        GameObject go = Instantiate(gridPrefab, parentTransform);
        Grid thisGrid = go.GetComponent<Grid>();
        float posX = startPoint.x + x * thisGrid.gridWidth;
        float posY = startPoint.y + y * thisGrid.gridHeight;
        go.transform.position = new Vector3(posX, posY, 0);
        grids[x, y] = thisGrid;
        grids[x, y].posX = x;
        grids[x, y].posY = y;

    }

    public void ClearMesh()
    {
        if (grids == null || grids.Length == 0)
        {
            return;
        }
        foreach(Grid grid in grids)
        {
            if(grid.gameObject != null)
            {
               Destroy(grid.gameObject);
            }
        }
        Array.Clear(grids, 0, grids.Length);
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            CreateMesh();
        }
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, Vector2.zero);
            
            if(hit.transform.gameObject.TryGetComponent<Grid>(out var grid))
            {
                Vector2Int position = new Vector2Int(grid.posX, grid.posY);
                if (position == beginPoint)
                {
                    Debug.Log("StartCoroutine");
                    StartCoroutine(DragPoint(0));
                }
                else if(position == endPoint)
                {
                    StartCoroutine(DragPoint(1));
                }
                else
                {
                    grid.SetHinder();
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            breadthFirstSearch.SetGridMeshCreator(this , beginPoint , endPoint , allowDiagnal);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bestFirstSearch.SetGridMeshCreator(this , beginPoint , endPoint , allowDiagnal);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            a_Search.SetGridMeshCreator(this , beginPoint , endPoint , allowDiagnal);
        }
    }

    public void ClearSearch()
    {
        for(int i = 0;i < width; i++)
        {
            for(int j=0; j < height;j++)
            {
                Vector2Int vector2Int = new Vector2Int(i,j);
                if(vector2Int == beginPoint || vector2Int == endPoint || grids[i,j].isHinder)
                {
                    continue;
                }
                grids[i, j].Recover();
            }
        }
        lineRenderer.positionCount = 0;
    }

    public void DrawPath(Dictionary<Vector2Int , Vector2Int> parentDic)
    {
        if(parentDic.ContainsKey(endPoint) == false)
        {
            return;
        }
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;
        lineRenderer.positionCount = 0;
        Vector2Int nowPoint = endPoint;
        while(true)
        {
            lineRenderer.positionCount++;
            Vector3 dotPosition = new Vector3(grids[nowPoint.x , nowPoint.y].gameObject.transform.position.x , 
                                              grids[nowPoint.x, nowPoint.y].gameObject.transform.position.y , 0);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, dotPosition);
            if(nowPoint == beginPoint)
            {
                break;
            }
            nowPoint = parentDic[nowPoint];
        }

    }

    IEnumerator DragPoint(int type)
    {
        while(true)
        {
            if(Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hit = Physics2D.Raycast(ray.origin, Vector2.zero);
                if(hit.transform.gameObject.TryGetComponent<Grid>(out var grid))
                {
                    if(grid.isHinder == false)
                    {
                        if(type == 0 && (grid.posX != endPoint.x || grid.posY != endPoint.y))
                        {
                            SetGridColor(beginPoint , Color.white);
                            beginPoint = new Vector2Int(grid.posX, grid.posY);
                            SetGridColor(beginPoint, Color.green);
                        }
                        else if((grid.posX != beginPoint.x || grid.posY != beginPoint.y))
                        {
                            SetGridColor(endPoint, Color.white);
                            endPoint = new Vector2Int(grid.posX, grid.posY);
                            SetGridColor(endPoint, Color.red);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Stopped");
                yield break;
            }
            yield return null;
        }
    }
}

