using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{
    private int width;
    private int height;
    private int[,] gridArr;

    public MyGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridArr = new int[width, height];
        Debug.Log(width + " " + height);

        for (int x = 0; x < gridArr.GetLength(0); x++)
        {
            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                
            }
        }
    }

}
