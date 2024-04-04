using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public float gridHeight = 1f;
    public float gridWidth = 1f;
    public Color color = Color.white;
    public Color hinderColor;
    public Color searchedColor;
    public Color willSearchColor;
    private SpriteRenderer spriteRenderer;
    LineRenderer lineRenderer;
    public bool isHinder = false;
    public int posX, posY;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(gridWidth, gridHeight, 1);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.startColor = Color.gray;
        lineRenderer.endColor = Color.gray;
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, new Vector3(-0.5f, 0.5f, 0));
        lineRenderer.SetPosition(1, new Vector3(0.5f, 0.5f, 0));
        lineRenderer.SetPosition(2, new Vector3(0.5f, -0.5f, 0));
        lineRenderer.SetPosition(3, new Vector3(-0.5f, -0.5f, 0));

    }

    private void Update()
    {
        spriteRenderer.color = color;
    }

    public void SetHinder()
    {
        //Debug.Log("Seted");
        if(color == Color.green || color == Color.red)
        {
            return;
        }
        if(isHinder == false)
        {
            isHinder = true;
            color = hinderColor;
            spriteRenderer.color = color;
        }
        else
        {
            isHinder = false;
            color = Color.white;
            spriteRenderer.color = color;
        }
    }

    public void SetSearched()
    {
        color = searchedColor;
        spriteRenderer.color = color;
    }

    public void SetwillSearch()
    {
        color = willSearchColor;
        spriteRenderer.color = color;
    }

    public void Recover()
    {
        color = Color.white;
        spriteRenderer.color = color;
    }
}
