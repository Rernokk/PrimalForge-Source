using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

[Serializable]
public class TileEntry
{
	public Color pixelColor;
	public string referenceMapName;
	public int pairingValue = 0;

	public bool isOpenTerrain = false;
	public bool hasMultipleVariations = false;
	public int sheetCount = 1;
	public bool customMaterial = false;
	[ShowIf("customMaterial")]
	public Material myMaterial = null;

	[HideInInspector]
	public Sprite[][] spriteArray;
}

public class BitmaskManager : MonoBehaviour
{
	[SerializeField]
	private GameObject tilePrefab;

	public Texture2D refMap;
	public List<GameObject> OpenTiles = new List<GameObject>();
	public List<TileEntry> MapDictionary = new List<TileEntry>();
	public static BitmaskManager instance;
	private GameObject[,] tiles;
	private byte[,] byteArray;
	private int[,] maskArray;

	public void Start()
	{
		if (MapDictionary == null)
		{
			MapDictionary = new List<TileEntry>();
		}
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(this);
		}
	}

	public void GenerateMapTiles()
	{
		if (refMap == null)
			return;

		//string[] bitmaskPairs = File.ReadAllLines((Resources.Load<TextAsset>("Constants/MaskPairs.txt").text.Split('\n'));
		string[] bitmaskPairs = (Resources.Load<TextAsset>("Constants/MaskPairs").text.Split('\n'));
		tiles = new GameObject[refMap.width, refMap.height];
		byteArray = new byte[refMap.width, refMap.height];
		maskArray = new int[refMap.width, refMap.height];

		for (int i = 0; i < MapDictionary.Count; i++)
		{
			if (MapDictionary[i].hasMultipleVariations)
			{
				MapDictionary[i].spriteArray = new Sprite[MapDictionary[i].sheetCount][];
				for (int j = 0; j < MapDictionary[i].sheetCount; j++)
				{
					MapDictionary[i].spriteArray[j] = Resources.LoadAll<Sprite>("Tilesets/" + MapDictionary[i].referenceMapName + "-" + j);
				}
			}
			else
			{
				MapDictionary[i].spriteArray = new Sprite[1][];
				MapDictionary[i].spriteArray[0] = Resources.LoadAll<Sprite>("Tilesets/" + MapDictionary[i].referenceMapName);
			}
		}

		//Populating Data Fields for Map.
		Transform mapHolder = new GameObject("Map Holder").transform;
		for (int i = 0; i < refMap.width; i++)
		{
			for (int j = 0; j < refMap.height; j++)
			{
				Color pixelColor = refMap.GetPixel(i, j);
				bool matched = false;
				foreach (TileEntry entry in MapDictionary)
				{
					if (entry.pixelColor.Equals(pixelColor))
					{
						GameObject obj = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
						obj.transform.parent = mapHolder;
						obj.GetComponent<SpriteRenderer>().sprite = entry.spriteArray[0][0];

						if (entry.customMaterial)
						{
							obj.GetComponent<SpriteRenderer>().material = entry.myMaterial;
						}

						if (entry.hasMultipleVariations)
						{
							obj.GetComponent<TileReference>().myTileSet = entry.spriteArray[UnityEngine.Random.Range(0, entry.spriteArray.Length)];
						}
						else
						{
							obj.GetComponent<TileReference>().myTileSet = entry.spriteArray[0];
						}
						byteArray[i, j] = (byte)entry.pairingValue;
						tiles[i, j] = obj;
						matched = true;
						if (entry.isOpenTerrain)
						{
							OpenTiles.Add(tiles[i, j]);
						}
						break;
					}
				}
				if (!matched)
				{
					GameObject obj = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
					obj.transform.parent = mapHolder;
					obj.GetComponent<SpriteRenderer>().color = new Color(1, 0f, 1f);
					obj.GetComponent<TileReference>().myTileSet = null;
					tiles[i, j] = obj;
					byteArray[i, j] = 0;
				}
			}
		}

		//Swapping sprites to appropriate shapes.
		for (int i = 0; i < refMap.width; i++)
		{
			for (int j = 0; j < refMap.height; j++)
			{
				int counter = 0;
				int[] neighborArray = new int[8];
				for (int l = -1; l <= 1; l++)
				{
					for (int m = -1; m <= 1; m++)
					{
						if (l == 0 && m == 0)
						{
							continue;
						}
						else
						{
							if ((i + l >= 0 && i + l < refMap.width) && (j + m >= 0 && j + m < refMap.height))
							{
								neighborArray[counter] = (byteArray[i + l, j + m] == byteArray[i, j] ? 1 : 0);
							}
							else
							{
								neighborArray[counter] = 0;
							}
							counter++;
						}
					}
				}
				maskArray[i, j] = 0;
				for (int z = 0; z < 8; z++)
				{
					maskArray[i, j] += (int)(Mathf.Pow(2, z) * neighborArray[z]);
				}
			}
		}

		//Correcting Sprites
		for (int i = 0; i < refMap.width; i++)
		{
			for (int j = 0; j < refMap.height; j++)
			{
				if (tiles[i, j] == null || tiles[i, j].GetComponent<TileReference>() == null || tiles[i, j].GetComponent<TileReference>().myTileSet == null)
				{
					continue;
				}
				else
				{
					tiles[i, j].GetComponent<SpriteRenderer>().sprite = tiles[i, j].GetComponent<TileReference>().myTileSet[int.Parse(bitmaskPairs[maskArray[i, j]])];
					tiles[i, j].GetComponent<TileReference>().MaskValue = maskArray[i, j];
				}
			}
		}

	}
}
