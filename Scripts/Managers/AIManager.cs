using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class AIManager : MonoBehaviour
{
	private int partitionWidth = 16;
	private int partitionHeight = 16;
	private List<Node> nodeSet;
	private List<Node>[,] nodePartitions;

	[SerializeField]
	private Texture2D mapObject;

	[SerializeField]
	private GameObject wallPrefab;

	[SerializeField]
	private List<Color> BoundaryColors;

	[SerializeField]
	private Dictionary<string, string> mapVersionListing = new Dictionary<string, string>();

	public static AIManager instance;

	public Texture2D MapObject
	{
		get
		{
			return mapObject;
		}

		set
		{
			mapObject = value;
		}
	}

	private void Start()
	{

		if (instance == null)
		{
			instance = this;
		} else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}

		if (mapObject == null)
			return;

		//Setting up collections for partitioning graph.
		nodePartitions = new List<Node>[mapObject.width / partitionWidth, mapObject.height / partitionHeight];
		for (int i = 0; i < mapObject.width / partitionWidth; i++)
		{
			for (int j = 0; j < mapObject.height / partitionWidth; j++)
			{
				nodePartitions[i, j] = new List<Node>();
			}
		}
		//Overall Graph
		nodeSet = new List<Node>();
		GameObject boundaryHolder = new GameObject("Boundaries");
		boundaryHolder.layer = LayerMask.NameToLayer("Walls");

		//Loading Map Version Control
		if (File.Exists(Application.persistentDataPath + "/MapVersionData.dat"))
		{
			LoadMapVersions();
			if (!File.Exists(Application.persistentDataPath + "/" + mapObject.name + ".dat") || !mapVersionListing.ContainsKey(mapObject.name) || (mapVersionListing[mapObject.name] != Application.version))
			{
				if (mapVersionListing.ContainsKey(mapObject.name) && mapVersionListing[mapObject.name] != Application.version)
				{
					print("Version Mismatch - Expected:" + Application.version + ", Found: " + mapVersionListing[mapObject.name]);
				} else if (!mapVersionListing.ContainsKey(mapObject.name))
				{
					print("Version not found!");
				} else if (!File.Exists(Application.persistentDataPath + "/" + mapObject.name + ".dat"))
				{
					print("Version found: " + mapVersionListing[mapObject.name] + ", data not found");
				}

				BuildMap();
				SaveMapToFile(mapObject.name + ".dat");
				if (mapVersionListing.ContainsKey(mapObject.name))
				{
					mapVersionListing[mapObject.name] = Application.version;
				} else
				{
					mapVersionListing.Add(mapObject.name, Application.version);
				}
				SaveMapVersions();
			}
			else
			{
				print("No issues found - Loading " + mapObject.name + " on version: " + mapVersionListing[mapObject.name]);
				LoadMapFromFile(mapObject.name + ".dat");
			}
		} else
		{
			BuildMap();
			SaveMapToFile(mapObject.name + ".dat");
			if (mapVersionListing.ContainsKey(mapObject.name))
			{
				mapVersionListing[mapObject.name] = Application.version;
			}
			else
			{
				mapVersionListing.Add(mapObject.name, Application.version);
			}
			SaveMapVersions();
		}

		for (int i = 0; i < mapObject.width; i++)
		{
			for (int j = 0; j < mapObject.height; j++)
			{
				for (int k = 0; k < BoundaryColors.Count; k++)
				{
					if (mapObject.GetPixel(i, j) == BoundaryColors[k])
					{
						GameObject temp = Instantiate(wallPrefab, new Vector3(i, j, 0), Quaternion.identity);
						temp.transform.parent = boundaryHolder.transform;
						continue;
					}
				}
			}
		}
		Rigidbody2D mapRgd2d = boundaryHolder.AddComponent<Rigidbody2D>();
		mapRgd2d.bodyType = RigidbodyType2D.Static;
		boundaryHolder.AddComponent<CompositeCollider2D>();
		boundaryHolder.tag = "Wall";
		GetComponent<BitmaskManager>().refMap = mapObject;
		GetComponent<BitmaskManager>().GenerateMapTiles();
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.D_DRAW_AI]))
		{
			foreach (Node n in nodeSet)
			{
				foreach (Node neigh in n.neighbors)
				{
					Debug.DrawLine(new Vector2(n.x, n.y), new Vector2(neigh.x, neigh.y), Color.red, 5f);
				}
			}
		}
	}
	public Node FetchNearestNode(GameObject obj)
	{
		foreach (Node n in nodeSet)
		{
			if (new Vector2(n.x, n.y) == new Vector2(Mathf.RoundToInt(obj.transform.position.x), Mathf.RoundToInt(obj.transform.position.y)))
			{
				return n;
			}
		}
		return null;
	}
	public Node FetchNearestNode(Vector3 obj)
	{
		List<Node> Partition = FetchPartition(obj);
		foreach (Node n in Partition)
		{
			if (new Vector2(n.x, n.y) == new Vector2(Mathf.RoundToInt(obj.x), Mathf.RoundToInt(obj.y)))
			{
				return n;
			}
		}
		return null;
	}
	public List<Node> FetchPartition(Vector3 location)
	{
		List<Node> part = new List<Node>();
		part = nodePartitions[Mathf.FloorToInt(location.x / partitionWidth), Mathf.FloorToInt(location.y / partitionHeight)];
		return part;
	}
	public void ResetGraph()
	{
		foreach (Node n in nodeSet)
		{
			n.visited = false;
			n.backNode = null;
			n.costSoFar = 100000;
		}
	}
	public List<Node> Breadth(Vector3 start, Vector3 end)
	{
		print("Beginning Breadth");
		List<Node> path = new List<Node>();
		List<Node> toVisit = new List<Node>();
		ResetGraph();
		Node startNode = FetchNearestNode(start);
		toVisit.Add(startNode);
		while (toVisit.Count > 0)
		{
			Node curr = toVisit[0];
			toVisit[0] = null;
			toVisit.RemoveAt(0);
			//foreach (Edge e in curr.Edges){
			foreach (Node n in curr.neighbors)
			{
				if (n != null && !n.visited)
				{
					n.backNode = curr;
					n.visited = true;
					toVisit.Add(n);
					if (n == FetchNearestNode(end))
					{
						curr = n;
						startNode.backNode = null;
						while (curr != null)
						{
							path.Add(curr);
							curr = curr.backNode;
						}
						return path;
					}
				}
			}
		}
		return path;
	}
	public List<Node> Dijkstra(Vector3 start, Vector3 end)
	{
		//print("Beginning Dijkstra");
		List<Node> path = new List<Node>();
		List<Node> toVisit = new List<Node>();
		ResetGraph();
		Node startNode = FetchNearestNode(start);
		Node endNode = FetchNearestNode(end);
		if (endNode == null)
		{
			return path;
		}
		toVisit.Add(startNode);
		while (toVisit.Count > 0)
		{
			toVisit.Sort((a, b) => a.costSoFar.CompareTo(b.costSoFar));
			Node curr = toVisit[0];
			toVisit[0] = null;
			toVisit.RemoveAt(0);
			if (curr != null)
			{
				foreach (Node n in curr.neighbors)
				{
					if (n != null)
					{
						float currDist = Vector2.Distance(new Vector2(n.x, n.y), new Vector2(curr.x, curr.y));
						if (!n.visited)
						{
							n.backNode = curr;
							n.visited = true;
							n.costSoFar = currDist + curr.costSoFar;
							toVisit.Add(n);
							if (n == FetchNearestNode(end))
							{
								curr = n;
								startNode.backNode = null;
								while (curr != null)
								{
									path.Add(curr);
									curr = curr.backNode;
								}
								path.Reverse();
								return path;
							}
						}
						else
						{
							if (curr.costSoFar + currDist <= n.costSoFar)
							{
								n.costSoFar = curr.costSoFar + currDist;
								n.backNode = curr;
							}
						}
					}
				}
			}
		}
		path.Reverse();
		return path;
	}
	public List<Node> Best(Vector3 start, Vector3 end)
	{
		print("Beginning Best");
		List<Node> path = new List<Node>();
		List<Node> toVisit = new List<Node>();
		ResetGraph();
		Node startNode = FetchNearestNode(start);
		Node endNode = FetchNearestNode(end);
		if (endNode == null)
		{
			return path;
		}
		toVisit.Add(startNode);
		while (toVisit.Count > 0)
		{
			toVisit.Sort((a, b) => (Vector2.Distance(new Vector2(a.x, a.y), new Vector2(end.x, end.x)).CompareTo(Vector2.Distance(new Vector2(b.x, b.y), new Vector2(end.x, end.y)))));
			Node curr = toVisit[0];
			toVisit[0] = null;
			toVisit.RemoveAt(0);
			foreach (Node n in curr.neighbors)
			{
				if (n != null)
				{
					float currDist = Vector2.Distance(new Vector2(n.x, n.y), new Vector2(curr.x, curr.y));
					if (!n.visited)
					{
						if (curr != null)
						{
							n.backNode = curr;
							n.visited = true;
							n.costSoFar = currDist + curr.costSoFar;
						}
						toVisit.Add(n);
						if (n == FetchNearestNode(end))
						{
							curr = n;
							startNode.backNode = null;
							while (curr != null)
							{
								path.Add(curr);
								curr = curr.backNode;
							}
							return (path);
						}
					}
					else
					{
						if (curr.costSoFar + currDist <= n.costSoFar)
						{
							n.costSoFar = curr.costSoFar + currDist;
							n.backNode = curr;
						}
					}
				}
			}
		}
		return path;
	}
	public List<Node> AStar(Vector3 start, Vector3 end)
	{

		print("Beginning A*");
		List<Node> path = new List<Node>();
		List<Node> toVisit = new List<Node>();
		ResetGraph();
		Node startNode = FetchNearestNode(start);
		toVisit.Add(startNode);
		while (toVisit.Count > 0)
		{
			//toVisit.Sort((a, b) => a.costSoFar.CompareTo(b.costSoFar));
			toVisit.Sort((a, b) => (a.costSoFar + Vector3.Distance(new Vector3(a.x, a.y), end)).CompareTo(b.costSoFar + Vector3.Distance(new Vector3(b.x, b.y), end)));
			Node curr = toVisit[0];
			toVisit[0] = null;
			toVisit.RemoveAt(0);
			//foreach (Edge e in curr.Edges){
			foreach (Node n in curr.neighbors)
			{
				if (n != null)
				{
					float currDist = curr.costSoFar + Vector3.Distance(new Vector3(curr.x, curr.y), end);
					if (!n.visited)
					{
						n.backNode = curr;
						n.visited = true;
						n.costSoFar = currDist + curr.costSoFar;
						toVisit.Add(n);
						if (n == FetchNearestNode(end))
						{
							curr = n;
							startNode.backNode = null;
							while (curr != null)
							{
								path.Add(curr);
								curr = curr.backNode;
							}
							return path;
						}
					}
					else
					{
						if (curr.costSoFar + currDist <= n.costSoFar)
						{
							n.costSoFar = curr.costSoFar + currDist;
							n.backNode = curr;
						}
					}
				}
			}
		}
		return path;
	}

	public void SaveMapVersions()
	{
		using (Stream stream = File.Open(Application.persistentDataPath + "/MapVersionData.dat", FileMode.Create))
		{
			BinaryFormatter fm = new BinaryFormatter();
			MapVersionData temp = new MapVersionData(mapVersionListing);
			fm.Serialize(stream, temp);
			stream.Close();
		}
	}

	public void LoadMapVersions()
	{
		using (Stream stream = File.Open(Application.persistentDataPath + "/MapVersionData.dat", FileMode.Open))
		{
			BinaryFormatter fm = new BinaryFormatter();
			MapVersionData temp = (MapVersionData)fm.Deserialize(stream);
			mapVersionListing = temp.mapDictionaryPairs;
		}
	}

	private void BuildMap()
	{

		for (int i = 0; i < mapObject.width; i++)
		{
			for (int j = 0; j < mapObject.height; j++)
			{
				if (mapObject.GetPixel(i, j) == Color.white || mapObject.GetPixel(i, j) == Color.red)
				{
					Color col = mapObject.GetPixel(i, j);
					Node tempNode = new Node(i, j, col.r, col.g, col.b, col == Color.red);
					nodeSet.Add(tempNode);
					nodePartitions[Mathf.FloorToInt(i / partitionWidth), Mathf.FloorToInt(j / partitionHeight)].Add(tempNode);
				}
			}
		}
		print(nodePartitions.Length);

		foreach (Node n in nodeSet)
		{
			int i = 0;
			foreach (Node n2 in nodeSet)
			{
				if (n != n2)
				{
					//if ((n.x == n2.x && Mathf.Abs(n.y - n2.y) == 1) || (n.y == n2.y && Mathf.Abs(n.x - n2.x) == 1))
					if (Vector2.Distance(new Vector2(n.x, n.y), new Vector2(n2.x, n2.y)) < 1.9f)
					{
						n.neighbors.Add(n2);
					}
				}
				if (n.neighbors.Count == 8)
				{
					continue;
				}
			}
		}
	}
	public void SaveMapToFile(string output)
	{
		float tStart = Time.realtimeSinceStartup;

		using (Stream stream = File.Open(Application.persistentDataPath + "/" + output, FileMode.Create))
		{
			BinaryFormatter fm = new BinaryFormatter();
			Graph temp = new Graph(nodeSet, nodePartitions);
			fm.Serialize(stream, temp);
			stream.Close();
		}
		print("Save Time: " + (Time.realtimeSinceStartup - tStart).ToString());
	}
	public void LoadMapFromFile(string input)
	{
		using (Stream stream = File.Open(Application.persistentDataPath + "/" + input, FileMode.Open))
		{
			BinaryFormatter fm = new BinaryFormatter();
			Graph temp = (Graph)fm.Deserialize(stream);
			nodeSet = temp.nodeList;
			nodePartitions = temp.nodePartitions;
		}
	}
}


[Serializable]
public class MapVersionData
{
	public Dictionary<string, string> mapDictionaryPairs = new Dictionary<string, string>();

	public MapVersionData()
	{
		mapDictionaryPairs = new Dictionary<string, string>();
	}

	public MapVersionData(Dictionary<string, string> referenceSet)
	{
		mapDictionaryPairs = referenceSet;
	}
}