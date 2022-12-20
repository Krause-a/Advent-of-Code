// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var f = File.ReadAllLines("Input.txt");
new Board();
foreach (var line in f)
{
  RockParser.ParseLine(line);
}
Console.WriteLine("###################");
foreach (var item in Board.instance.board)
{
  // Console.WriteLine("Rock: " + item.Value.p);
}
  // Console.WriteLine("Rock built: " + Rock.rocksBuilt);
  Console.WriteLine("Lowest Rock: " + Board.instance.lowestRock);

Board.instance.RunBoard(int.Parse(Environment.GetCommandLineArgs()[1]));

foreach (var item in Board.instance.board)
{
  string name = "";
  if (item.Value is Rock) {
    name = "Rock";
  } else name = "Sand";
  // Console.WriteLine(name + ": " + item.Value.p);
}

public static class RockParser {
  public static void ParseLine(string line) {
    //498,4 -> 498,6 -> 496,6
    List<Point> points = new List<Point>();
    var s = line.Split("->", StringSplitOptions.TrimEntries);
    foreach (var item in s)
    {
      var nums = item.Split(',');
      var p = new Point(int.Parse(nums[0]),int.Parse(nums[1]));
      // Console.WriteLine(p);
      points.Add(p);
    }
    if (points.Count == 1) Rock.Build(points[0]);
    for (int i = 1; i < points.Count; i++)
    {
      BuildWalls(points[i - 1], points[i]);
    }
  }

  public static void BuildWalls(Point from, Point to) {
    Point frown = from;
    unsafe {
      // Console.WriteLine("From: " + (int)(&from));
      // Console.WriteLine("Frown: " + (int)(&frown));
    }
    while (from != to) {
      Point fart = frown.Copy();//This line is required
      Rock.Build(fart);
      if (frown.x < to.x)  {
        frown = frown.Right();
      } else if (frown.x > to.x) {
        frown = frown.Left();
      } else if (frown.y < to.y) {
        frown = frown.Down();
      } else if (frown.y > to.y) {
        frown = frown.Up();
      } else {
        // Console.WriteLine("TAG");
        return;
      }
    }
  }
}

public class Board {
  public readonly static Board instance = new Board();
  public Dictionary<Point, Spot> board = new Dictionary<Point, Spot>();
  public int lowestRock = 0;
  public void Move(Spot sp, Point p) {
    var saftey = p.Copy();
    board.Remove(sp.p);
    board[saftey] = sp;
    sp.p = saftey;
    // Console.WriteLine("Rock added at " + p);
    
// foreach (var item in board)
// {
  // Console.WriteLine(item.Key + ": " + "Rock " + item.Value.p);
// }
    if (sp is Rock) {
      lowestRock = Math.Max(lowestRock, saftey.y);
    }
    if (sp is Sand) {
      if (saftey.y >= lowestRock) {
        throw new Exception("LOWEST ROCK PASSED WITH " + sandCount);
      }
    }
  }

  public bool IsClear(Point p) {
    if (board.TryGetValue(p, out var sp)) {
      return !(sp is Rock || sp is Sand);
    }
    return true;
  }

  private Sand currentSand;
  public int SandCount => sandCount;
  private int sandCount;
  public void RunBoard(int totalSand = -1) {
    while(true) {
      if (totalSand != -1 && sandCount >= totalSand) break;
      // Console.WriteLine("New Sand");
      currentSand = new Sand(new Point(500,0));
      sandCount++;
      // Console.WriteLine(sandCount);
      while(currentSand.Fall()) {}
      foreach (var item in board)
      {
        if (item.Value is Sand) {
          // Console.WriteLine("Sand: " + item.Value.p);
        }
      }
    }
  }

}

public class Point {
  public override string ToString()
  {
    return (x, y).ToString();
  }
  public int x, y;
  public Point Copy() {
    return new Point(x, y);
  }
  public Point Up() => new Point(x, y - 1);
  public Point Left() => new Point(x - 1, y);
  public Point Right() => new Point(x + 1, y);
  public Point DownRight() => new Point(x + 1, y = y + 1);
  public Point DownLeft() => new Point(x - 1, y = y + 1);
  public Point Down() => new Point(x, y = y + 1);
  public Point(int x,int y)
  {
    this.x = x;
    this.y = y;
  }

  public override int GetHashCode()
  {
    // Console.WriteLine((x * 1000) + y);
    return (x * 1000) + y;
  }
  public override bool Equals(object obj)
  {
    if (obj is Point sp) return sp.x == x && sp.y == y;
    return false;
}
}
public class Spot {
  public Point p;

  public void Move(Point target) {
    p = target;
    Board.instance.Move(this, p);
  }

  public Spot(Point po)
  {
    p = po;
    Move(po);
  }
  
  public override int GetHashCode()
  {
    return p.GetHashCode();
  }
  public override bool Equals(object obj)
  {
    return p.Equals(obj);
  }
}

public class Sand : Spot {
  public Sand(Point po) : base(po)
  {
    // Console.WriteLine(po + ": " + "Sand");
  }

  public bool Fall() {
    var saftey = p.Copy();
    if (Board.instance.IsClear(saftey.Down())) {
      saftey = p.Copy();
      Board.instance.Move(this, saftey.Down());
      return true;
    } 
    saftey = p.Copy();
    if (Board.instance.IsClear(saftey.DownLeft())) {
      saftey = p.Copy();
      Board.instance.Move(this, saftey.DownLeft());
      return true;
    } 
    saftey = p.Copy();
    if (Board.instance.IsClear(saftey.DownRight())) {
      saftey = p.Copy();
      Board.instance.Move(this, saftey.DownRight());
      return true;
    } 
    return false;
  }
}

public class Rock : Spot {
  // public static int rocksBuilt = 0;
  public static void Build(Point po) => new Rock(po);
  private Rock(Point po) : base(po)
  {
    // Console.WriteLine(po + ": " + "Rock");
    // Console.WriteLine(Board.instance.board.ContainsKey(po));
    // rocksBuilt++;
  }
}
