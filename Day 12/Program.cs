// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var f = File.ReadAllLines("Input.txt");
var map = new Map(f[0].Length, f.Length);
for (int y = 0; y < f.Length; y++)
{
  string? line = f[y];
  for (int x = 0; x < line.Length; x++)
  {
    char c = line[x];
    map.PlotPoint(x, y, c);
  }
}
// for (int y = 0; y < map.map.GetLength(1); y++)
// {
//   Console.WriteLine();
//   for (int x = 0; x < map.map.GetLength(0); x++)
//   {
//     Console.Write(map.map[x, y]?.h); 
//   }
// }
// Console.WriteLine();
// Console.WriteLine(map.GetStartingNode().x);
// Console.WriteLine(map.GetEndingNode().x);
// Console.WriteLine();
// Console.WriteLine();
// var t = new Traveler(new List<Node>(), map.GetStartingNode());
foreach (var starting in map.allANodes)
{
  Console.WriteLine("Starting at " + starting.ToString());
  Traveler.nextTravelers.Add(new Traveler(new List<Node>(), starting));
  while(Traveler.nextTravelers.Count + Traveler.currentTravelers.Count > 0) {
    Traveler.CycleTravel();
  }
  Traveler.Clear();
  Map.touched.Clear();
}
Console.WriteLine("\nBest: " + Traveler.best);

//150 20

public class Traveler {
  public static int best = int.MaxValue;
  public static int cycle = 0;
  public static List<Traveler>[] allTravelers = new List<Traveler>[]{
    new List<Traveler>(), new List<Traveler>()
  };
  public static List<Traveler> currentTravelers => allTravelers[cycle];
  public static List<Traveler> nextTravelers => allTravelers[(cycle + 1) % 2];
  public static void Clear() {
    currentTravelers.Clear();
    nextTravelers.Clear();
  }
  public List<Node> traveledNodes;
  public Node currentNode;

  public static void CycleTravel() {
    currentTravelers.Clear();
    cycle = (cycle + 1) % 2;
    foreach (var tra in currentTravelers)
    {
      //Main end
      if (tra.currentNode.x == 139 && tra.currentNode.y == 20) {
        Console.WriteLine("Reached the end in " + tra.traveledNodes.Count);
        Console.WriteLine("From Starting Point " + tra.traveledNodes[0].ToString());
        Console.WriteLine("To Ending Point " + tra.traveledNodes[tra.traveledNodes.Count - 1].ToString());
        if (best > tra.traveledNodes.Count) {
          best = tra.traveledNodes.Count;
        }
      }
      //Mini End
      // if (tra.currentNode.x == 4 && tra.currentNode.y == 2) {
      //   Console.WriteLine("Reached the end in " + tra.traveledNodes.Count);
      //   Console.WriteLine("From Starting Point " + tra.traveledNodes[0].ToString());
      //   Console.WriteLine("To Ending Point " + tra.traveledNodes[tra.traveledNodes.Count - 1].ToString());
      //   tra.PrintSteps();
      // }
      foreach (var node in tra.currentNode.neighbors)
      {
        if (node?.h == 'E') {
          // Console.WriteLine("E neighbor: " + tra.currentNode.ToString());
        }
      }
      tra.Travel();
    }
  }

  public Traveler(List<Node> pastNodes, Node currentNode)
  {
    traveledNodes = new List<Node>();
    traveledNodes.AddRange(pastNodes);
    traveledNodes.Add(currentNode);
    this.currentNode = currentNode;
  }

  public void Travel() {
    foreach (var node in currentNode.neighbors)
    {
      if (node == null) continue;
      if (Map.touched.Contains(node)) continue;
      // if (node.h == 'E') Console.WriteLine("More E neighbor: " + currentNode.ToString());
      if (node.h == 'E' && (currentNode.h == 'z' || currentNode.h == 'y')) {
        foreach (var item in traveledNodes)
        {
          Console.WriteLine((item.x, item.y, item.h));
        }
        Console.WriteLine($"\n\nFound the end in {traveledNodes.Count} steps\n");
        currentTravelers.Clear();
        nextTravelers.Clear();
        return;
      }
      if (Math.Abs(currentNode.h - node.h) > 1 && currentNode.h <= node.h && currentNode.h != 'S') continue;
      var t = new Traveler(traveledNodes, node);
      Map.touched.Add(node);
      nextTravelers.Add(t);
    }
  }
  public void PrintNeighbors() {
    string s = "";
    Console.WriteLine("ME: " + currentNode.ToString());
    foreach (var item in currentNode.neighbors)
    {
      s += item?.ToString() ?? "'";
    }
    Console.WriteLine(s);
  }
  public void PrintTraveled() {
    string s = "";
    foreach (var item in traveledNodes)
    {
      s += item.ToString();
    }
    Console.WriteLine(s);
  }
  public void PrintSteps() {
    string s = "";
    string last = "";
    string curr = "";
    int count = -1;
    for (int i = 1; i < traveledNodes.Count; i++)
    {
      Node? prev = traveledNodes[i - 1];
      Node? next = traveledNodes[i];
      switch (next.x - prev.x) {
        case -1:
          curr = "Left";
          break;
        case 1:
          curr = "Right";
          break;
      }
      switch (next.y - prev.y) {
        case -1:
          curr = "Up";
          break;
        case 1:
          curr = "Down";
          break;
      }
      if (count == -1) last = curr;
      if (curr == last) {
        count++;
      } else {
        s += last + ": " + (count + 1) + "\n" + prev.ToString() + "\n";
        last = curr;
        count = 0;
      }
    }
    s += last + ": " + (count + 1);

    Console.WriteLine(s);
  }

}

public class Map {
  public static HashSet<Node> touched = new HashSet<Node>();
  public List<Node> allANodes = new List<Node>();
  public Node[,] map;
  public Map(int width, int height) {
    map = new Node[width, height];
  }
  public void PlotPoint(int x, int y, char h) {
    var n = new Node(x, y, h);
    if (h == 'a') allANodes.Add(n);
    var node = GetNode(x, y + 1);
    n.neighbors[0] = node;
    if (node != null) node.neighbors[2] = n;
    node = GetNode(x + 1, y);
    n.neighbors[1] = node;
    if (node != null) node.neighbors[3] = n;
    node = GetNode(x, y - 1);
    n.neighbors[2] = node;
    if (node != null) node.neighbors[0] = n;
    node = GetNode(x - 1, y);
    n.neighbors[3] = node;
    if (node != null) node.neighbors[1] = n;
    map[x, y] = n;
  }
  public Node? GetNode(int x, int y) {
    if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1)) return null;
    return map[x, y];
  }

  public Node GetStartingNode() {
    foreach (var node in map)
    {
      if (node.h == 'S') return node;
    }
    throw new Exception("Starting Point Not Found");
  }

  public Node GetEndingNode() {
    foreach (var node in map)
    {
      if (node.h == 'E') return node;
    }
    throw new Exception("Ending Point Not Found");
  }

}

public class Node {
  public override string ToString()
  {
    return (x, y, h).ToString();
  }
  public int x, y;
  public char h;
  //0 => +y
  //1 => +x
  //2 => -y
  //3 => -x
  public Node?[] neighbors;
  public Node(int x, int y, char h)
  {
    this.x = x;
    this.y = y;
    this.h = h;
    this.neighbors = new Node[4];
    // Console.WriteLine(Convert.ToString(GetHashCode(), 2));
  }

  public override bool Equals(object? obj)
  {
    if (obj is Node n) {
      return x == n.x && y == n.y;
    }
    return false;
  }

  public override int GetHashCode()
  {
    return (x << 10) | y;
  }

}