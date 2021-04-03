using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test
{
  public int i = 42;
  public Test()
  {
  }
}
public class MazeRenderer : MonoBehaviour
{
  [SerializeField]
  [Range(1, 50)]
  private int w = 10;

  [SerializeField]
  [Range(1, 50)]
  private int h = 10;

  [SerializeField]
  private float uw = 1;

  [SerializeField]
  private float uh = 1;

  [SerializeField]
  private Transform wallPrefab = null;

  [SerializeField]
  private Transform startPrefab = null;

  [SerializeField]
  private Transform endPrefab = null;

  [SerializeField]
  private List<Transform> objPrefabs = null;

  [SerializeField]
  [Range(1, 50)]
  private int objDensity = 10;

  private int DrawCycle = 0;

  // Start is called before the first frame update
  void Start()
  {
    var maze = new CellHive(w, h, uw, uh);
    DrawWalls(maze, wallPrefab);
    DrawKeys(maze, startPrefab, endPrefab);
    DrawObjects(maze, objPrefabs);
  }

  public void DrawObjects(CellHive maze, List<Transform> objPrefabs)
  {
    var rng = new System.Random();

    foreach (Transform obj in objPrefabs)
    {
      for (int i = 0; i < objDensity; i++)
      {
        var cell = maze.GetRandomCell();
        var objT = Instantiate(obj, transform) as Transform;
        var offset = new Vector3((float)rng.NextDouble(), (float)rng.NextDouble(), 0);
        var sign = rng.Next(0, 2);
        if (0 == sign) objT.position = cell.Position + offset;
        else objT.position = cell.Position - offset;
        objT.eulerAngles = new Vector3(rng.Next(0, 360), rng.Next(0, 360), rng.Next(0, 360));
      }
    }
  }

  public void DrawWalls(CellHive maze, Transform wallPrefab)
  {
    DrawCycle++;
    foreach (Cell cell in maze.Cells)
    {
      foreach (Wall wall in cell.Walls.Values)
      {
        if (wall.DrawCycle < DrawCycle)
        {
          wall.DrawCycle++;
          var wallT = Instantiate(wallPrefab, transform) as Transform;
          wallT.position = wall.Position;
          wallT.localScale = new Vector3(wall.Scale, wallT.localScale.y, wallT.localScale.z);
          wallT.eulerAngles = wall.Rotation;
        }
      }
    }
  }

  public void DrawKeys(CellHive maze, Transform startPrefab, Transform endPrefab)
  {
    var start = Instantiate(startPrefab, transform) as Transform;
    start.position = maze.GetStart().Position;
    var end = Instantiate(endPrefab, transform) as Transform;
    end.position = maze.GetEnd().Position;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
