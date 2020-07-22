using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
  [SerializeField]
  Node n1, n2;

  [SerializeField]
  float edgeWeight;

  public bool visited = false;

  public bool Visited {
    get {
      return visited;
    }

    set {
      visited = value;
    }
  }

  public float EdgeWeight {
    get {
      return edgeWeight;
    }

    set {
      edgeWeight = value;
    }
  }

  public Edge SetNodes(Node a, Node b)
  {
    n1 = a;
    n2 = b;
    return this;
  }

  public Node GetOtherNode(Node a){
    if (a == n1){
      return n2;
    } else {
      return n1;
    }
  }

  private void Start()
  {
    //edgeWeight = Random.Range(1f, 20f);
    edgeWeight = 1;
  }
}
