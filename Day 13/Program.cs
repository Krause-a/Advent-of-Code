using System.Text;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var f = File.ReadAllLines("Input.txt");
int score = 1;
List<Node> allPackets = new List<Node>();
for (int i = 0; i < f.Length; i++)
{
  if (string.IsNullOrEmpty(f[i])) continue;
  allPackets.Add(Parser.ParsePacket(f[i].Trim()));
}
allPackets.Sort((l, r) => Parser.Compare(l, r));
StringBuilder sb = new StringBuilder();
List<string> outs = new(allPackets.Count);
for (int i = 0; i < allPackets.Count; i++)
{
  if (allPackets[i].isDivider() && (allPackets[i].list[0].list[0].num == 2 || allPackets[i].list[0].list[0].num == 6)) {
    sb.AppendLine("#######################");
    score *= (i + 1);
  }
  if (i < 10) sb.Append(' ');
  if (i < 100) sb.Append(' ');
  sb.Append(i);
  sb.Append(':').Append(' ')
    .Append(allPackets[i]);
    
  if (allPackets[i].isDivider()) {
    // sb.AppendLine().Append("#######################");
  }
  outs.Add(sb.ToString());
  sb.Clear();
}
File.WriteAllLines("Output.txt", outs.ToArray());
Console.WriteLine(score);



public class Parser {
  public enum Comparison {
    Continue, Correct, Incorrect
  }
  public static int Compare(Node left, Node right) {
    //return true if they are in the correct order
    return Comp(left, right) switch {
      Comparison.Continue => 0,
      Comparison.Correct => -1,
      Comparison.Incorrect => 1,
      _ => 0
    };
  }
  private static Comparison Comp(Node left, Node right) {
    if (left.num.HasValue && right.num.HasValue) {
      if (left.num < right.num) return Comparison.Correct;
      if (left.num > right.num) return Comparison.Incorrect;
      return Comparison.Continue;
    }
    if (left.num.HasValue != right.num.HasValue) {
      if (left.num.HasValue) {
        left.list = new List<Node> {new Node{num = left.num}};
      }
      if (right.num.HasValue) {
        right.list = new List<Node> {new Node{num = right.num}};
      }
    }
    //two lists
    if (left.list == null || right.list == null) {
      if (left.list == null && right.list != null) return Comparison.Correct;
      if (left.list == null && right.list == null) return Comparison.Continue;
      if (left.list != null && right.list == null) return Comparison.Incorrect;

    }
    for (int i = 0; i < Math.Max(left.list.Count, right.list.Count); i++)
    {
      if ( i >= left.list.Count || i >= right.list.Count) {
        if (left.list.Count == right.list.Count) return Comparison.Continue;
        if (i >= left.list.Count) return Comparison.Correct;
        if (i >= right.list.Count) return Comparison.Incorrect;
      }
      switch (Comp(left.list[i], right.list[i])) {
        case Comparison.Correct:
          return Comparison.Correct;
          break;
        case Comparison.Incorrect:
          return Comparison.Incorrect;
          break;
      }
    }
    return Comparison.Continue;
  }

  //  \[[^\[\]]*\]
  // Matches the innermost []
  public static Node ParsePacket(string line) {
    //[[1],[2,[3,4]]]
    return MakeNode(line);
  }

  private static Node MakeNode(string line) {
    var n = new Node();
    line = line.Trim();
    // Console.WriteLine("MAKENODE: " + line);
    if (line.Length >= 3 && (line[1..^1].Contains("[") || line[1..^1].Contains("]"))) {
      n.list = new List<Node>();
      var walked = Walker.Walk(line[1..^1]);
      foreach (var sect in walked)
      {
        n.list.Add(MakeNode(sect));
      }
    } else {
      //Base case
      //All that is left are numbers
      //Potentially zero []
      //or more [1] ... [1, 2, 2, 2, 1]
      //Also 1    just a number by itself     4
      if (int.TryParse(line, out var r)) {
        return new Node{
          num = r
        };
      }
      if (line.Length <= 2) return new Node();
      var s = line[1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries);
      n.list = new List<Node>();
      foreach (var item in s)
      {
        n.list.Add(new Node{num = int.Parse(item)});
      }
    }
    return n;
  }
}

public class Walker {
  static int depth = 0;
  static int lastSub = 0;
  public static List<string> Walk(string line) {
    depth = 0;
    lastSub = 0;
    //Seperate the line by commas but only on nesting level 0.
    List<string> ret = new List<string>();
    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];
      if (c == '[') depth++;
      if (c == ']') depth--;
      if (depth > 0) continue;
      if (c != ',') continue;
      ret.Add(line[lastSub..i]);
      lastSub = i + 1;
    }
    ret.Add(line[lastSub..]);
    return ret;
  }
}

public class Node {
  public bool isDivider() {
    // Console.WriteLine("#");
    if (list == null) return false;
    // Console.WriteLine("##");
    if (list.Count != 1) return false;
    // Console.WriteLine("###");
    if (list[0].list == null) return false;
    // Console.WriteLine("####");
    if (list[0].list.Count != 1) return false;
    // Console.WriteLine("###### Ding");
    if (list[0].list[0].num.HasValue) return true;
    // Console.WriteLine("####### No!");
    return false;
  }
  public int? num;
  public List<Node>? list;

  public override string ToString()
  {
    if (num.HasValue) return num.ToString();
    if (list == null) return "[]";
    StringBuilder sb = new();
    sb.Append('[');
    foreach (var item in list)
    {
      if (sb.Length != 1) {
        sb.Append(", ");
      }
      sb.Append(item.ToString());
    }
    sb.Append(']');
    return sb.ToString();
  }
}