// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
var main = new Main();
var f = File.ReadAllLines("Input.txt");
main.MakeWalls(f);
main.MakeFloor();
// Console.WriteLine($"The {main.FindMaxSand()}th sand did not stop.");
Console.WriteLine($"The total sand is {main.RunMovers()}.");



public class Main {
  public void MakeFloor(){
    int bottom = lowestPoint.y + 2;
    int mid = spawnAt.x;
    MakeWalls(new string[]{
      $"{mid-bottom},{bottom} -> {mid+bottom},{bottom}"
    });
  }
  public void MakeWalls(string[] walls) {
    // 498,4 -> 498,6 -> 496,6
    // 503,4 -> 502,4 -> 502,9 -> 494,9

    foreach (var line in walls)
    {
      var pointSet = line.Split("->", StringSplitOptions.TrimEntries)
      .Select(csv => {
        var s = csv.Split(',');
        return new Point(int.Parse(s[0]), int.Parse(s[1]));
      }).ToArray();
      if (pointSet.Length == 0) {
        //TODO
        return;
      }
      for (int i = 1; i < pointSet.Length; i++)
      {
        var from = pointSet[i - 1];
        var to = pointSet[i];
        // Console.WriteLine($"Point Set {from} -> {to}");
        while(from.GetDir(to) != Dir.None) {
          // Console.WriteLine($"From {from} -Going {from.GetDir(to)}- To {to}");
          MakeRock(from);
          // PrintDict();
          from = from.Offset(from.GetDir(to));
        }
        MakeRock(to);
      }
      // Console.WriteLine();
      // PrintDict();
      // Console.WriteLine();
    }
  }
  Point lowestPoint = new Point(0, 0);
  public static Point spawnAt = new Point(500, 0);
  public enum Type {Sand, Rock}
  public enum Dir {
      Up, Down,Left,Right, None
    }
  public struct Point {
    public int x, y;
    public override string ToString()
    {
      return (x, y).ToString();
    }
    public Point(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
    public Point Offset(Dir d) {
      var p = new Point(x, y);
      switch (d) {
        case Dir.Down:
          p.y++;
          break;
        case Dir.Up:
          p.y--;
          break;
        case Dir.Left:
          p.x--;
          break;
        case Dir.Right:
          p.x++;
          break;
      }
      return p;
    }

    public Dir GetDir(Point to) {
      var xDif = to.x - x;
      var yDif = to.y - y;
      if (xDif > 0) return Dir.Right;
      if (xDif < 0) return Dir.Left;
      if (yDif > 0) return Dir.Down;
      if (yDif < 0) return Dir.Up;
      return Dir.None;
    }
  }

  class Block {
    public Type type;
    public Block(Type type)
    {
      this.type = type;
    }
  }

  class Mover {
    public Point p;
    public Mover()
    {
      p = spawnAt;
    }
  }

  Dictionary<Point, Block> dict = new Dictionary<Point, Block>();
  public void PrintDict() {
    Console.WriteLine("<Dict>");
    foreach (var item in dict)
    {
      Console.WriteLine("  " + item.Key);
    }
    Console.WriteLine("</Dict>");
  }

  bool pastRocks;
  int sandCount;
  public int FindMaxSand() {
    pastRocks = false;
    sandCount = 0;
    while(!pastRocks) {
      SpawnNewSand();
      sandCount++;
      // Console.Write("#");
      while(FallAllSand() && !pastRocks) {
        // Console.Write("S");
      }
    }
    return sandCount;
  }

  public int RunMovers() {
    moversSet = 0;
    while(!dict.ContainsKey(spawnAt)) {
      while(FallMover()){

      }
    }
    return moversSet;
  }

  void MakeRock(Point p) {
    if (dict.TryAdd(p, new Block(Type.Rock))) {
      // Console.WriteLine("Rock Made at " + p);
      if (p.y > lowestPoint.y) lowestPoint = p;
    }
  }

  void SpawnNewSand() {
    if (!dict.TryAdd(spawnAt, new Block(Type.Sand))) {
      throw new Exception("Spawnd sand on sand " + sandCount);
    }
  }

  Mover mover = new Mover();
  int moversSet = 0;
  bool FallMover() {
    var next = CheckDown(mover.p);
    if (next == null) {
      SetMover();
      return false;
    } else {
      mover.p = next.Value;
      return true;
    }
  }

  void SetMover() {
    if (dict.TryAdd(mover.p, new Block(Type.Sand)))
    {
      moversSet++;
      mover = new Mover();
    }
    else
    {
      throw new Exception("Setting mover on something " + moversSet);
    }
  }

  bool FallAllSand() {
    Dictionary<Point, Block> nextDict = new Dictionary<Point, Block>();
    bool sandStillFalling = false;
    foreach (var kv in dict)
    {
      if (kv.Value.type != Type.Sand) {
        nextDict.Add(kv.Key, kv.Value);
        // Console.Write("Not Sand");
        continue;
      }
      var next = CheckDown(kv.Key);
      if (next == null) {
        nextDict.Add(kv.Key, kv.Value);
        // Console.Write("No Down");
        continue;
      }
      if (next.Value.y >= lowestPoint.y) pastRocks = true;
      sandStillFalling = true;
      // Console.Write("Going Down: " + next.Value);
      nextDict.Add(next.Value, kv.Value);
    }
    dict.Clear();
    dict = nextDict;
    return sandStillFalling;
  }

  Point? CheckDown(Point p) {
    var d = p.Offset(Dir.Down);
    if (!dict.ContainsKey(d)) return d;
    var dl = d.Offset(Dir.Left);
    if (!dict.ContainsKey(dl)) return dl;
    var dr = d.Offset(Dir.Right);
    if (!dict.ContainsKey(dr)) return dr;
    return null;
  }

}