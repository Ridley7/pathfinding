using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int nRows;
    public int nColumns;
    public float gridCellSize;
    public Node[,] nodes { get; set; }
    public Vector3 origin { get; set; }
    private GameObject[] obstacleList;

    private void Awake()
    {
        origin = new Vector3();
        obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
        SetupNodes();

    }

    private void SetupNodes()
    {
        nodes = new Node[nRows, nColumns];
        int index = 0;
        for (int i = 0; i < nColumns; i++)
        {
            for(int j = 0; j < nRows; j++)
            {
                Vector3 cellPos = GetGridCellCenterPos(index);
                Node node = new Node(cellPos);
                nodes[i, j] = node;
                index++;
            }
        }

        if(obstacleList != null && obstacleList.Length > 0)
        {
            foreach(GameObject obstacle in obstacleList)
            {
                int cellIndex = GetCellIndex(obstacle.transform.position);
                int col = GetColumns(cellIndex);
                int row = GetRows(cellIndex);
                nodes[row, col].MaskAsObstacle();
            }
        }
    }

    private bool IsInBounds(Vector3 pos)
    {
        float width = nColumns * gridCellSize;
        float height = nRows * gridCellSize;
        bool insideWidth = pos.x >= origin.x && pos.x <= origin.x + width;
        bool insideHeight = pos.z >= origin.z && pos.z <= origin.z + height;
        return insideHeight && insideWidth;
    }

    private int GetCellIndex(Vector3 pos)
    {
        if (!IsInBounds(pos))
        {
            return -1;
        }

        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);
        return row * nColumns + col;
    }

    private int GetRows(int index)
    {
        int row = index / nColumns;
        return row;
    }

    private int GetColumns(int index)
    {
        int column = index % nColumns;
        return column;
    }

    private Vector3 GetGridCellPosition(int index)
    {
        int row = GetRows(index);
        int column = GetColumns(index);
        float xPosInGrid = column * gridCellSize;
        float zPosInGrid = row * gridCellSize;
        return origin + new Vector3(xPosInGrid, 0, zPosInGrid);
    }

    public Vector3 GetGridCellCenterPos(int index)
    {
        Vector3 cellPos = GetGridCellPosition(index);
        cellPos.x = cellPos.x + gridCellSize / 2;
        cellPos.y = cellPos.y + gridCellSize / 2;

        return cellPos;
    }

    private void DebugDrawGrid(Vector3 origin, int nRows, int nColumns, float cellSize, Color color)
    {
        float width = nRows * cellSize;
        float height = nColumns * cellSize;

        for(int i = 0; i < nRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * Vector3.forward;
            Vector3 endPos = startPos + width * Vector3.right;
            Debug.DrawLine(startPos, endPos, color);
        }

        for(int i = 0; i < nColumns + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * Vector3.right;
            Vector3 endPos = startPos + height * Vector3.forward;
            Debug.DrawLine(startPos, endPos, color);
        }
    }

    private void OnDrawGizmos()
    {
        DebugDrawGrid(transform.position, nRows, nColumns, gridCellSize, Color.gray);

        if(nodes != null)
        {
            foreach (Node node in nodes)
            {
                if (node.isObstacle)
                {
                    Gizmos.DrawSphere(node.position, 1f);
                }
            }
        }
        
    }
}
