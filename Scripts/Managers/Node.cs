using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Graph{
  public List<Node> nodeList;
  public List<Node>[,] nodePartitions;

  public Graph (List<Node> nodes, List<Node>[,] partitions){
    nodeList = nodes;
    nodePartitions = partitions;
  }
}

[Serializable]
public class Node {
  public List<Node> neighbors;
  public bool visited;
  public bool isObstacle;
  public Node backNode;
  public float costSoFar;
  public int x, y;
  public float r,g,b;
  
  public Node(int incX, int incY, float R, float G, float B, bool obstacle){
    x = incX;
    y = incY;
    neighbors = new List<Node>();
    r = R;
    g = G;
    b = B;
    isObstacle = obstacle;
  }
}
