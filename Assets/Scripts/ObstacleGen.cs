//Sabrina Jackson
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class ObstacleGen : MonoBehaviour
{

    [SerializeField] private int obstacleCount;
    //[SerializeField] private IntVariable obstacleCount;
    private List<Transform> obstacles = new List<Transform>();
    [SerializeField] private List<GameObject> ObstacleSelect;
    [SerializeField] private Grid boundary;

    [SerializeField] private IntVariable obstacleType;

    // Start is called before the first frame update
    void Start()
    {
        GenerateObstacles();// Generate initial random obstacles
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearObstacles();
            GenerateObstacles();
        }
    }
    
    void GenerateObstacles()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-50, 50),
                0f,
                Random.Range(-50, 50));
            Transform obstacle = Instantiate(ObstacleSelect[obstacleType.Value], randomPos, Quaternion.identity).transform;
            int size = Random.Range(3, 10);
            obstacle.localScale = new Vector3(size, size, size);
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
}
