using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Direction
{
  W = 0,
  E = 1,
  N = 2,
  S = 3
}

[Flags]
public enum CellState
{
  EMPTY = 0x0,

  W = 0x1,
  E = 0x2,
  N = 0x4,
  S = 0x8,
  ALL = 0xF,

  PATH = 0x10,
  START = 0x20,
  END = 0x40,

  VISITED = 0x8000
}

[Flags]
public enum WallState
{
  EMPTY = 0x0,
  EXIST = 0x1,
  BROKEN = 0x2
}

public class Wall
{
  public Cell Parent1;
  public Cell Parent2;
  public WallState State;
  public int DrawCycle = 0;
  public Vector3 Position;
  public Vector3 Rotation;
  public float Scale;

  public Wall(Cell p1, Cell p2, WallState s, Direction dir)
  {
    Parent1 = p1;
    Parent2 = p2;
    State = s;

    switch (dir)
    {
      case Direction.W:
        Position = p1.Position + new Vector3(-p1.W / 2, 0, 0);
        Rotation = new Vector3(0, 90, 0);
        Scale = p1.H;
        break;
      case Direction.E:
        Position = p1.Position + new Vector3(p1.W / 2, 0, 0);
        Rotation = new Vector3(0, 270, 0);
        Scale = p1.H;
        break;
      case Direction.N:
        Position = p1.Position + new Vector3(0, 0, -p1.H / 2);
        Rotation = new Vector3(0, 0, 0);
        Scale = p1.W;
        break;
      case Direction.S:
        Position = p1.Position + new Vector3(0, 0, p1.H / 2);
        Rotation = new Vector3(0, 180, 0);
        Scale = p1.W;
        break;
    }
  }
}

public class Cell
{
  public int X;
  public int Y;
  public float W;
  public float H;
  public CellState State;
  public Vector3 Position;

  public Dictionary<Direction, Cell> Neighbours;
  public Dictionary<Direction, Wall> Walls;

  public Cell(int x, int y, float w, float h, CellState state, Vector3 pos)
  {
    X = x;
    Y = y;
    W = w;
    H = h;
    State = state;
    Position = pos;
    Neighbours = new Dictionary<Direction, Cell>();
    Walls = new Dictionary<Direction, Wall>();
  }

  public void MarkAsVisited()
  {
    State |= CellState.VISITED;
  }

  public void MarkAsStart()
  {
    State |= CellState.START;
  }

  public void MarkAsEnd()
  {
    State |= CellState.END;
  }
}

public class CellHive
{
  public Cell[] Cells;
  private int W;
  private int H;
  private float UW;
  private float UH;
  private System.Random Rng;
  private Cell Start;
  private Cell End;

  public Cell GetStart()
  {
    return Start;
  }

  public Cell GetEnd()
  {
    return End;
  }

  private void Initialize()
  {
    int x = 0;
    int y = 0;

    for (int i = 0; i < W * H; i++)
    {
      Cell cell = new Cell(x, y, UW, UH, CellState.ALL, new Vector3((-W / 2 + x) * UW, 0, (-H / 2 + y) * UH));

      foreach (Direction direction in Enum.GetValues(typeof(Direction)))
      {
        cell.Neighbours[direction] = GetNeighbour(cell, direction);

        if (null != cell.Neighbours[direction])
        {
          cell.Neighbours[direction].Neighbours[GetOpposite(direction)] = cell;
        }

        if (null == cell.Neighbours[direction] || null == cell.Neighbours[direction].Walls[GetOpposite(direction)])
        {
          Wall wall = new Wall(cell, cell.Neighbours[direction], WallState.EXIST, direction);
          cell.Walls[direction] = wall;
        }
        else
        {
          cell.Walls[direction] = cell.Neighbours[direction].Walls[GetOpposite(direction)];
          cell.Walls[direction].Parent2 = cell;
        }
      }

      Cells[i] = cell;

      x++;
      if (x >= W)
      {
        x = 0;
        y++;
      }
    }
  }

  private void Randomize()
  {
    var cellStack = new Stack<Cell>();
    var cell = GetRandomCell();
    cell.MarkAsVisited();
    cellStack.Push(cell);

    while (cellStack.Count > 0)
    {
      var cur = cellStack.Pop();
      var nei = GetRandomUnvisitedNeighbour(cur);

      if (null != nei)
      {
        cellStack.Push(cur);
        RemoveWall(cur, nei);
        nei.MarkAsVisited();
        cellStack.Push(nei);
      }
    }
  }

  private void MarkKeyPoints()
  {
    Start = GetRandomCell();
    Start.MarkAsStart();

    // TODO move them to different angles

    End = GetRandomCell();
    End.MarkAsEnd();
  }

  public CellHive(int w, int h, float uw, float uh)
  {
    Cells = new Cell[w * h];
    this.W = w;
    this.H = h;
    this.UW = uw;
    this.UH = uh;
    Rng = new System.Random();

    Initialize();
    Randomize();
    MarkKeyPoints();
  }

  public int GetW() { return W; }

  public int GetH() { return H; }

  public Cell GetNeighbour(Cell current, Direction dir)
  {
    Cell neighbour = null;
    switch (dir)
    {
      case Direction.W:
        if (current.X > 0)
        {
          neighbour = GetCell(current.X - 1, current.Y);
        }
        break;
      case Direction.E:
        if (current.X < W - 1)
        {
          neighbour = GetCell(current.X + 1, current.Y);
        }
        break;
      case Direction.N:
        if (current.Y > 0)
        {
          neighbour = GetCell(current.X, current.Y - 1);
        }
        break;
      case Direction.S:
        if (current.Y < H - 1)
        {
          neighbour = GetCell(current.X, current.Y + 1);
        }
        break;
      default:
        neighbour = null;
        break;
    }

    return neighbour;
  }

  public Cell GetCell(int x, int y)
  {
    if (x < 0 || x >= W || y < 0 || y >= H) return null;
    return Cells[y * H + x];
  }

  //-------------------------------------

  public Cell GetNeighbour(Cell current, Direction dir, Func<CellState, bool> cond)
  {
    var cell = GetNeighbour(current, dir);

    if (null == cell) return null;

    if (cond(cell.State)) return cell;
    return null;
  }

  public Cell GetRandomCell()
  {
    var rnd = Rng.Next(0, W * H);
    return Cells[rnd];
  }

  public Cell GetRandomCell(Func<CellState, bool> cond)
  {
    var rnd = Rng.Next(0, W * H);
    for (int i = rnd; i < W * H; i++)
    {
      if (cond(Cells[rnd].State))
      {
        return Cells[rnd];
      }
    }

    for (int i = 0; i < rnd; i++)
    {
      if (cond(Cells[rnd].State))
      {
        return Cells[rnd];
      }
    }

    return null;
  }

  public Direction GetRandomDir()
  {
    return (Direction)Rng.Next(0, Enum.GetNames(typeof(Direction)).Length);
  }

  public List<Cell> GetNeighbours(Cell cell)
  {
    var list = new List<Cell>();

    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
    {
      var n = GetNeighbour(cell, direction);
      if (null != n) list.Add(n);
    }

    return list;
  }

  public List<Cell> GetNeighbours(Cell cell, Func<CellState, bool> cond)
  {
    var list = new List<Cell>();

    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
    {
      var n = GetNeighbour(cell, direction, cond);
      if (null != n) list.Add(n);
    }

    return list;
  }

  public Cell GetRandomNeighbour(Cell cell)
  {
    var neighbours = GetNeighbours(cell);

    if (neighbours.Count == 0) return null;

    var rnd = Rng.Next(0, neighbours.Count);
    return neighbours[rnd];
  }

  public Cell GetRandomNeighbour(Cell cell, Func<CellState, bool> cond)
  {
    var neighbours = GetNeighbours(cell, cond);

    if (neighbours.Count == 0) return null;

    var rnd = Rng.Next(0, neighbours.Count);
    return neighbours[rnd];
  }

  public Direction FindDir(Cell src, Cell dst)
  {
    if (src.X - dst.X == 1) return Direction.W;
    if (src.X - dst.X == -1) return Direction.E;
    if (src.Y - dst.Y == 1) return Direction.N;
    if (src.Y - dst.Y == -1) return Direction.S;
    // TODO here could be error
    return Direction.E;
  }

  //-------------------------------------

  public void RemoveWall(Cell cell, Direction dir)
  {
    cell.State &= ~DirectionToCellState(dir);
    cell.Walls.Remove(dir);
  }

  public void RemoveWall(Cell one, Cell two)
  {
    var wallDirOne = FindDir(one, two);
    RemoveWall(one, wallDirOne);

    var wallDirTwo = FindDir(two, one);
    RemoveWall(two, wallDirTwo);
  }

  //-------------------------------------

  public Cell GetRandomUnvisitedNeighbour(Cell cell)
  {
    return GetRandomNeighbour(cell, (state) => { return !state.HasFlag(CellState.VISITED); });
  }

  public Direction GetOpposite(Direction dir)
  {
    switch (dir)
    {
      case Direction.W: return Direction.E;
      case Direction.E: return Direction.W;
      case Direction.N: return Direction.S;
      case Direction.S: return Direction.N;
    }

    return Direction.W;
  }

  public CellState DirectionToCellState(Direction dir)
  {
    switch (dir)
    {
      case Direction.W: return CellState.W;
      case Direction.E: return CellState.E;
      case Direction.N: return CellState.N;
      case Direction.S: return CellState.S;
    }

    return CellState.EMPTY;
  }
}