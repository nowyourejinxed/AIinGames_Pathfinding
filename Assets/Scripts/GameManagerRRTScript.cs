using UnityEngine;
using System.Collections.Generic;

public class RRTPathfinding : MonoBehaviour
{
    public GameObject startLocationPrefab;
    public GameObject endLocationPrefab;
    public GameObject obstaclePrefab;
    public Transform boundary;
    public float maxStepSize = 1.0f;
    public int maxIterations = 1000;
    public int obstacleCount = 30;

    private List<Vector3> nodes = new List<Vector3>();
    private List<int> parents = new List<int>();
    private LineRenderer pathRenderer;
    private List<Transform> obstacles = new List<Transform>();

    private Transform startLocation;
    private Transform endLocation;

    void Start()
    {
        pathRenderer = GetComponent<LineRenderer>();
        pathRenderer.positionCount = 0;

        // Initialize RRT with a random starting point
        Vector3 randomStartPoint = GenerateRandomPoint();
        nodes.Add(randomStartPoint);
        parents.Add(0);

        // Place the start location at the random start point
        startLocation = Instantiate(startLocationPrefab, randomStartPoint, Quaternion.identity).transform;

        GenerateObstacles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearObstacles();
            GenerateObstacles();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRRT();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Left-click to place the start location
            PlaceStartLocation();
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Right-click to place the end location
            PlaceEndLocation();
        }
    }

    void GenerateObstacles()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 randomPos = GenerateRandomPoint();
            Transform obstacle = Instantiate(obstaclePrefab, randomPos, Quaternion.identity).transform;
            obstacles.Add(obstacle);
        }
    }

    void ClearObstacles()
    {
        foreach (Transform obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }
        obstacles.Clear();
    }

    void PlaceStartLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Boundary"))
            {
                Vector3 position = hit.point;

                if (IsInBoundary(position))
                {
                    if (startLocation != null)
                    {
                        Destroy(startLocation.gameObject);
                    }

                    startLocation = Instantiate(startLocationPrefab, position, Quaternion.identity).transform;
                    pathRenderer.positionCount = 0;
                }
            }
        }
    }

    void PlaceEndLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Boundary"))
            {
                Vector3 position = hit.point;

                if (IsInBoundary(position))
                {
                    if (endLocation != null)
                    {
                        Destroy(endLocation.gameObject);
                    }

                    endLocation = Instantiate(endLocationPrefab, position, Quaternion.identity).transform;
                    pathRenderer.positionCount = 0;
                }
            }
        }
    }

    void GenerateRRT()
    {
        for (int i = 0; i < maxIterations; i++)
        {
            Vector3 randomPoint = GenerateRandomPoint();
            int nearestNodeIndex = FindNearestNode(randomPoint);
            Vector3 newNode = Steer(nearestNodeIndex, randomPoint);
            if (IsInBoundary(newNode) && !IsObstacleInPath(nodes[nearestNodeIndex], newNode))
            {
                nodes.Add(newNode);
                parents.Add(nearestNodeIndex);
                DrawLine(nodes[nearestNodeIndex], newNode);

                if (Vector3.Distance(newNode, endLocation.position) < maxStepSize)
                {
                    pathRenderer.positionCount = nodes.Count;
                    BuildPath(nodes.Count - 1);
                    break;
                }
            }
        }
    }

    Vector3 GenerateRandomPoint()
    {
        float x = Random.Range(-boundary.localScale.x / 2f, boundary.localScale.x / 2f);
        float y = Random.Range(-boundary.localScale.y / 2f, boundary.localScale.y / 2f);
        float z = Random.Range(-boundary.localScale.z / 2f, boundary.localScale.z / 2f);
        return new Vector3(x, y, z);
    }

    int FindNearestNode(Vector3 point)
    {
        int nearestNodeIndex = 0;
        float nearestDistance = Vector3.Distance(nodes[0], point);

        for (int i = 1; i < nodes.Count; i++)
        {
            float distance = Vector3.Distance(nodes[i], point);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNodeIndex = i;
            }
        }

        return nearestNodeIndex;
    }

    Vector3 Steer(int nearestNodeIndex, Vector3 randomPoint)
    {
        Vector3 direction = randomPoint - nodes[nearestNodeIndex];
        direction.Normalize();
        return nodes[nearestNodeIndex] + direction * maxStepSize;
    }

    bool IsInBoundary(Vector3 point)
    {
        return Mathf.Abs(point.x) <= boundary.localScale.x / 2f &&
               Mathf.Abs(point.y) <= boundary.localScale.y / 2f &&
               Mathf.Abs(point.z) <= boundary.localScale.z / 2f;
    }

    bool IsObstacleInPath(Vector3 start, Vector3 end)
    {
        // Check for obstacles in the path between start and end
        RaycastHit hit;
        if (Physics.Linecast(start, end, out hit))
        {
            return hit.transform != null && hit.transform.CompareTag("Obstacle");
        }
        return false;
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        pathRenderer.positionCount++;
        pathRenderer.SetPosition(pathRenderer.positionCount - 1, end);
    }

    void BuildPath(int nodeIndex)
    {
        if (nodeIndex == 0)
            return;

        BuildPath(parents[nodeIndex]);
        pathRenderer.SetPosition(nodeIndex, nodes[nodeIndex]);
    }
}
