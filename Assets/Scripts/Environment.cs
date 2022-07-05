using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Environment : MonoBehaviour
{
    [SerializeField]
    [Range(1, 50)]
    private int mazeWidth = 10;

    [SerializeField]
    [Range(1, 50)]
    private int mazeHeight = 10;

    public ExplorerAgent agent;
    public MazeRenderer maze;
    public Canon canon;

    // Start is called before the first frame update
    void Start()
    {
        agent.OnEpisodeBeginEvent += OnEpisodeBeginEvent;
    }

    private void OnEpisodeBeginEvent(object sender, EventArgs e)
    {
        canon.DestroyAllBalls();
        maze.RegenerateMaze(mazeWidth, mazeHeight);
        agent.Immobilize();
        agent.transform.position = transform.position + (Vector3.forward * (mazeHeight / 2 - 1)) + (Vector3.right * (mazeWidth / 2 - 1)) + Vector3.up * -0.3f; // Place agent in edge of labyrinth
    }
}
