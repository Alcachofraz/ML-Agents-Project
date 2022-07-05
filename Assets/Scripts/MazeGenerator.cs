using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallState
{
    // 0000 -> NO WALLS
    // 1111 -> LEFT, RIGHT, UP, DOWN
    LEFT = 1, // 0001
    RIGHT = 2, // 0010
    UP = 4, // 0100
    DOWN = 8, // 1000

    VISITED = 128, // 1000 0000
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public static class MazeGenerator
{
    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }

    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        var rng = new System.Random();
        var positionStack = new Stack<Position>();
        var position = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };

        maze[position.X, position.Y] |= WallState.VISITED;
        positionStack.Push(position);

        while(positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);
                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.Position;
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);

                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Position position, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        if (position.X > 0)
        {
            if (!maze[position.X - 1, position.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = position.X - 1,
                        Y = position.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }
        if (position.Y > 0)
        {
            if (!maze[position.X, position.Y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = position.X,
                        Y = position.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }
        if (position.X < width - 1)
        {
            if (!maze[position.X + 1, position.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = position.X + 1,
                        Y = position.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }
        if (position.Y < height - 1)
        {
            if (!maze[position.X, position.Y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = position.X,
                        Y = position.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        return list;
    }

    public static WallState[,] Generate(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.LEFT | WallState.RIGHT | WallState.UP | WallState.DOWN;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
                
            }
        }

        return ApplyRecursiveBacktracker(maze, width, height);
    }
}
