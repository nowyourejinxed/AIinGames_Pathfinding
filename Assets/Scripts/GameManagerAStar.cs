//Shandor Korda assisted by Chat GPT 3.5

using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject startLocationPrefab;
    public GameObject endLocationPrefab;
    public Transform boundary;
    public LineRenderer pathRenderer;

    public int gridSize = 50;//Set in inspector but corresponds to boundary size
    public int obstacleCount = 30;
    public LayerMask obstacleLayer;

    private GridNode[,] grid;
    private List<Transform> obstacles = new List<Transform>();
    private Transform startLocation;
    private Transform endLocation;
    private List<GridNode> path;

    void Start()
    {
        InitializeGrid();
        //GenerateObstacles();// Generate initial random obstacles
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //ClearObstacles();
            //GenerateObstacles();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlaceStartLocation();
            Debug.Log("mouse left clicked");
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlaceEndLocation();
            Debug.Log("mouse right clicked");
        }

        if (startLocation != null && endLocation != null)
        {
            FindPath();
        }
    }

    void InitializeGrid()
    {
        grid = new GridNode[gridSize, gridSize];
        float cellSize = boundary.localScale.x / gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(x * cellSize - boundary.localScale.x / 2f, y * cellSize - boundary.localScale.y / 2f, 0f);
                Collider2D hit = Physics2D.OverlapPoint(position, obstacleLayer);

                bool isObstacle = hit != null;
                grid[x, y] = new GridNode(x, y, position, isObstacle);
            }
        }
    }

    void PlaceStartLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, obstacleLayer))
        {
            Vector3 position = hit.point;
            if (IsPositionWithinBoundary(position) && !IsObstacleAtPosition(position))
            {
                if (startLocation != null)
                {
                    Destroy(startLocation.gameObject);
                    Debug.Log("start location Destroyed");
                }
                Debug.Log("Clicked in Boundary");
                startLocation = Instantiate(startLocationPrefab, position, Quaternion.identity).transform;
                pathRenderer.positionCount = 0; // Clear the path renderer
            }
        }
    }

    void PlaceEndLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, obstacleLayer))
        {
            Vector3 position = hit.point;
            if (IsPositionWithinBoundary(position) && !IsObstacleAtPosition(position))
            {
                if (endLocation != null)
                {
                    Destroy(endLocation.gameObject);
                }

                endLocation = Instantiate(endLocationPrefab, position, Quaternion.identity).transform;
                pathRenderer.positionCount = 0; // Clear the path renderer
            }
        }
    }

    // Check if a position is within the defined boundary
    bool IsPositionWithinBoundary(Vector3 position)
    {
        return Mathf.Abs(position.x) <= boundary.localScale.x / 2f &&
               Mathf.Abs(position.z) <= boundary.localScale.z / 2f;
    }

    // Check if there is an obstacle at a position
    bool IsObstacleAtPosition(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.1f, obstacleLayer);
        return hitColliders.Length > 0;
    }





    void FindPath()
    {
        // Implement A* algorithm with Manhattan distance heuristic here
        // You can use a library like AstarPathfindingProject for more complex scenarios
        // Here's a simplified example of A* pathfinding:

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        GridNode startNode = GetNodeFromPosition(startLocation.position);
        GridNode goalNode = GetNodeFromPosition(endLocation.position);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == goalNode)
            {
                // Path found, reconstruct and visualize the path
                List<GridNode> path = RetracePath(startNode, goalNode);
                VisualizePath(path);
                return;
            }

            foreach (GridNode neighbor in GetNeighbors(currentNode))
            {
                if (neighbor.isObstacle || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, goalNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }


    List<GridNode> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    void VisualizePath(List<GridNode> path)
    {
        // Implement visualization of the path here (e.g., drawing lines)
        // You can also use Unity's LineRenderer component for this purpose.
    }

    int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        // Implement Manhattan distance calculation here
        return Mathf.Abs(nodeA.gridX - nodeB.gridX) + Mathf.Abs(nodeA.gridY - nodeB.gridY);
    }

    GridNode GetNodeFromPosition(Vector3 position)
    {
        float cellSize = boundary.localScale.x / gridSize;
        int x = Mathf.RoundToInt((position.x + boundary.localScale.x / 2f) / cellSize);
        int y = Mathf.RoundToInt((position.y + boundary.localScale.y / 2f) / cellSize);

        return grid[x, y];
    }

    List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();

        if (node.gridX > 0)
            neighbors.Add(grid[node.gridX - 1, node.gridY]);
        if (node.gridX < gridSize - 1)
            neighbors.Add(grid[node.gridX + 1, node.gridY]);
        if (node.gridY > 0)
            neighbors.Add(grid[node.gridX, node.gridY - 1]);
        if (node.gridY < gridSize - 1)
            neighbors.Add(grid[node.gridX, node.gridY + 1]);

        return neighbors;
    }
}

public class GridNode
{
    public int gridX;
    public int gridY;
    public Vector3 position;
    public bool isObstacle;

    public int gCost; // Cost from the start node
    public int hCost; // Heuristic cost to the goal node
    public GridNode parent; // Parent node for pathfinding

    public int fCost { get { return gCost + hCost; } }

    public GridNode(int _gridX, int _gridY, Vector3 _position, bool _isObstacle)
    {
        gridX = _gridX;
        gridY = _gridY;
        position = _position;
        isObstacle = _isObstacle;
    }
}
