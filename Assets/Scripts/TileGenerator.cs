using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class TileData
{
    public GameObject tile;
    public GameObject building;
}

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private GameObject treeObject;
    [SerializeField] private GameObject houseObject;

    private List<TileData> tilesList = new List<TileData>();
    private Vector3 spawningCursor = new Vector3(0, 0, 0);

    private static TileGenerator _instance;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateTile();
        StartCoroutine(StartAutoSpawnTree());
    }

    private void GenerateTile()
    {
        // Fill the list with one of each tile so that all tile will spawn
        for (int i = 0; i < tiles.Length; i++)
        {
            TileData tileData = new TileData();
            tileData.tile = tiles[i];

            tilesList.Add(tileData);
        }

        // Fill the rest of the list with random tiles
        for (int i = 0; i < 64 - tiles.Length; i++)
        {
            int num = Random.Range(0, tiles.Length - 1);
            
            TileData tileData = new TileData();
            tileData.tile = tiles[num];

            tilesList.Add(tileData);
        }

        // Shuffle the tiles to eliminate sorted items in the first 7 index, using Fisher–Yates shuffle algorithm
        System.Random rnd = new System.Random();
        int n = tilesList.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int r = i + rnd.Next(n - i);
            TileData t = tilesList[i];
            tilesList[i] = tilesList[r];
            tilesList[r] = t;
        }

        // Spawn the tile, then replace the tile in the tile list from prefab to the instantiated GameObject
        for (int i = 0; i < tilesList.Count; i++)
        {
            GameObject tile = tilesList[i].tile;

            GameObject spawnedTile = Instantiate(tile, spawningCursor, tile.transform.rotation);
            spawnedTile.name = i.ToString();
            tilesList[i].tile = spawnedTile;

            spawningCursor.x += 1f;
            if (spawningCursor.x > 7f)
            {
                spawningCursor.x = 0f;
                spawningCursor.z += 1f;
            }
        }
    }

    private void SpawnTree()
    {
        // Get a list of tile where its a dirt and no buildings
        List<TileData> dirtList = tilesList.Where(e => e.tile.tag == "Dirt" && e.building == null).ToList();

        if (dirtList.Count <= 0)
        {
            Debug.Log("Tree is finished planting");
            StopAllCoroutines();
            return;
        }

        // Spawn the tree
        int tileNum = Random.Range(0, dirtList.Count - 1);
        GameObject dirtTileToPlant = dirtList[tileNum].tile;
        Vector3 treeLocation = dirtTileToPlant.transform.position + new Vector3(0f, 0.14f, 0f);
        GameObject tree = Instantiate(treeObject, treeLocation, treeObject.transform.rotation);

        // Record the building (which is tree) into the tile list
        tilesList[int.Parse(dirtTileToPlant.name)].building = tree;
    }

    // tree spawn offset: 0 0.14 0
    // house spawn offset: 0 0.12 0.2

    IEnumerator StartAutoSpawnTree()
    {
        // Automatic tree spawner, stops after all dirt tile were occupied, either tree or house
        while (true)
        {
            yield return new WaitForSeconds(1f);

            SpawnTree();
        }
    }

    public static void SpawnHouse(int tileNum)
    {
        // Will return if the tile selected is already have building
        if (_instance.tilesList[tileNum].building != null) return;

        // Spawn the house
        GameObject tileToBuild = _instance.tilesList[tileNum].tile;
        Vector3 houseLocation = tileToBuild.transform.position + new Vector3(0f, 0.12f, 0.2f);
        GameObject house = Instantiate(_instance.houseObject, houseLocation, _instance.houseObject.transform.rotation);

        // Add the score based on which tile the house is built on
        if (tileToBuild.tag == "Dirt")
        {
            UIScore.AddScore(10);
        }
        else if (tileToBuild.tag == "Desert")
        {
            UIScore.AddScore(2);
        }

        // Record the building (which is house) into the tile list
        _instance.tilesList[int.Parse(tileToBuild.name)].building = house;
    }
}
