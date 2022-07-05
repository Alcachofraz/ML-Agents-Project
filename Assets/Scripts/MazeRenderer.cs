using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private float size = 1.0f;

    [SerializeField]
    private Transform wallPrefab;

    [SerializeField]
    private Transform checkpointPrefab;

    public Material floorMaterial;

    GameObject floor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void RegenerateMaze(int width, int height)
    {
        var walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
        {
            Destroy(wall);
        }
        Destroy(floor);
        Draw(MazeGenerator.Generate(width, height), width, height);
    }

    private void Draw(WallState[,] maze, int width, int height)
    {
        floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.parent = transform;
        floor.transform.localPosition = new Vector3(-size / 2, -size / 2, -size / 2);
        floor.transform.localScale = new Vector3(width, 0.1f, height);
        floor.GetComponent<MeshRenderer>().material = floorMaterial;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0.0f, -height / 2 + j);

                // Top:
                var topWall = Instantiate(cell.HasFlag(WallState.UP) ? wallPrefab : checkpointPrefab, transform);
                topWall.position = position + new Vector3(0.0f, 0.0f, size / 2);
                topWall.localScale = new Vector3(size, topWall.localScale.y, topWall.localScale.z);
                
                // LEFT:
                var leftWall = Instantiate(cell.HasFlag(WallState.LEFT) ? wallPrefab : checkpointPrefab, transform);
                leftWall.position = position + new Vector3(-size / 2, 0.0f, 0.0f);
                leftWall.localScale = new Vector3(size, leftWall.localScale.y, leftWall.localScale.z);
                leftWall.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                
                if (i == width - 1)
                {
                    // RIGHT:
                    var rightWall = Instantiate(cell.HasFlag(WallState.RIGHT) ? wallPrefab : checkpointPrefab, transform);
                    rightWall.position = position + new Vector3(size / 2, 0.0f, 0.0f);
                    rightWall.localScale = new Vector3(size, rightWall.localScale.y, rightWall.localScale.z);
                    rightWall.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
                }
                if (j == 0)
                {
                    // DOWN:
                    var bottomWall = Instantiate(cell.HasFlag(WallState.DOWN) ? wallPrefab: checkpointPrefab, transform);
                    bottomWall.position = position + new Vector3(0.0f, 0.0f, -size / 2);
                    bottomWall.localScale = new Vector3(size, bottomWall.localScale.y, bottomWall.localScale.z);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
