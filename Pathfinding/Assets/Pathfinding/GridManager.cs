using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager _instance;

    public static GridManager Instance
    {
        get
        {
            _instance = FindObjectOfType<GridManager>();
            if(_instance != null)
            {
                return _instance;
            }

            return null;
        }
    }


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

    public int GetCellIndex(Vector3 pos)
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

    private void AssignNeighbor(int row, int column, List<Node> neighbors)
    {
        if(row != -1 && column != -1 && row < nRows && column < nColumns)
        {
            Node nodeToAdd = nodes[row, column];
            if (!nodeToAdd.isObstacle)
            {
                neighbors.Add(nodeToAdd);
            }
        }
    }

    public void GetNeighbors(Node node, List<Node> neighbors)
    {
        Vector3 nodePos = node.position;
        int nodeIndex = GetCellIndex(nodePos);

        int row = GetRows(nodeIndex);
        int column = GetColumns(nodeIndex);

        //Bottom
        int nodeRow = row - 1;
        int nodeColumn = column;
        AssignNeighbor(nodeRow, nodeColumn, neighbors);

        //Top
        nodeRow = row + 1;
        nodeColumn = column;
        AssignNeighbor(nodeRow, nodeColumn, neighbors);

        //Right
        nodeRow = row;
        nodeColumn = column + 1;
        AssignNeighbor(nodeRow, nodeColumn, neighbors);

        //Left
        nodeRow = row;
        nodeColumn = column - 1;
        AssignNeighbor(nodeRow, nodeColumn, neighbors);

        
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
