using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour {
    public static World Instance { get; private set; }
    public static int chunkSize = 4;
    public static int radius = 6;
    public Material textureAtlas;
    public NavMeshSurface meshSurface;
    public enum NDIR { UP, DOWN, LEFT, RIGHT, FRONT, BACK }
    public Vector3[,,] allVertices = new Vector3[chunkSize + 1, chunkSize + 1, chunkSize + 1];
    public static ConcurrentDictionary<string, Chunk> chunks;
    public Chunk c;
    public GridMap grid;
    public Transform startPosition;
    public Transform endPosition;

    public GameObject selectedDetails;
    public GameObject towerDetails;
    public GameObject canvas;
    public Slider healthBar;

    int[,] map = new int[,]  {{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                                {1,2,1,0,0,0,0,0,0,0,0,1,1,3,1,1},
                                {1,0,1,0,1,1,1,1,1,1,0,1,1,0,1,1},
                                {1,0,1,0,0,0,0,0,1,1,0,1,1,0,1,1},
                                {1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1},
                                {1,0,1,0,0,0,1,0,1,1,0,1,1,0,1,1},
                                {1,0,1,0,1,0,1,0,1,1,0,1,1,0,1,1},
                                {1,0,0,0,1,0,1,0,1,1,0,1,1,0,1,1},
                                {1,1,1,1,1,0,1,0,1,1,0,1,1,0,1,1},
                                {1,0,0,0,0,0,1,0,1,1,0,1,1,0,1,1},
                                {1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1},
                                {1,0,1,1,1,1,1,0,1,1,0,1,1,0,1,1},
                                {1,0,0,0,0,0,0,0,1,1,0,1,1,0,1,1},
                                {1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,1},
                                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};

    public static string BuildChunkName(Vector3 v) {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    void Awake() {
        
        Instance = this;
        //Debug.Log (totalChunks);
    }

    Chunk BuildChunkAt(int x, int y, int z) {
        Vector3 chunkPostion = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);

        string n = BuildChunkName(chunkPostion);
        c = new Chunk(chunkPostion, textureAtlas);
        c.chunk.transform.parent = this.transform;
        chunks.TryAdd(c.chunk.name, c);
        if (y > 0) {
            c.chunk.tag = "PlacebleObject";
        }
        return c;
    }

    Chunk BuildChunkAt(int x, int y, int z, ItemTexture top, ItemTexture bot) {
        Vector3 chunkPostion = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        string n = BuildChunkName(chunkPostion);
        c = new Chunk(chunkPostion, top, bot ,textureAtlas);
        c.chunk.transform.parent = this.transform;
        chunks.TryAdd(c.chunk.name, c);
        if (y > 0) {
            c.chunk.tag = "PlacebleObject";
        }
        return c;
    }

    void buildAtIndex(int i, int j) {
        if (map[7 + i, 7 + j] != 1)
            BuildChunkAt(i, 1, j);
        else
            BuildChunkAt(i, 1, j, ItemTexture.Grass, ItemTexture.Dirt);
    }

    void buildAtIndex(int i, int k, int j) {
        if (map[7 + i, 7 + j] == 1)
            BuildChunkAt(i, k, j);
        else if (map[7 + i, 7 + j] == 2) {
            //BuildChunkAt(i, k, j);
            startPosition.position = new Vector3(BuildChunkAt(i, k, j, ItemTexture.Stone, ItemTexture.Stone).chunk.transform.position.x,World.chunkSize, c.chunk.transform.position.z);
        }

        else if (map[7 + i, 7 + j] == 3) {
            endPosition.position = new Vector3(BuildChunkAt(i, k, j, ItemTexture.Stone, ItemTexture.Stone).chunk.transform.position.x, World.chunkSize, c.chunk.transform.position.z);
        }

        else {
            BuildChunkAt(i, k, j, ItemTexture.Stone, ItemTexture.Stone);

        }
    }

    // Use this for initialization
    void Start() {
        //generate all vertices
        for (int z = 0; z <= chunkSize; z++)
            for (int y = 0; y <= chunkSize; y++)
                for (int x = 0; x <= chunkSize; x++) {
                    allVertices[x, y, z] = new Vector3(x, y, z);
                }
        chunks = new ConcurrentDictionary<string, Chunk>();
        grid = new GridMap(chunkSize, 8);
        for (int j = 0; j < 8; j++) {
            for (int i = 0; i < 8; i++) {

                //BuildChunkAt(i, 0, j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(i, 0, j);
                buildAtIndex(i, j);

                //BuildChunkAt(-i, 0, -j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(-i, 0, -j);
                buildAtIndex(-i, -j);

                //BuildChunkAt(i, 0, -j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(i, 0, -j);
                buildAtIndex(i, -j);

                //BuildChunkAt(-i, 0, j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(-i, 0, j);
                buildAtIndex(-i, j);
            }
        }

        // lift up spawnpoint and put into center of chunk
        startPosition.Translate(new Vector3(.5f*chunkSize, .5f*chunkSize, .5f*chunkSize));

        foreach (KeyValuePair<string, Chunk> c in chunks) {
            if (c.Value.status == Chunk.ChunkStatus.DRAW) {
                c.Value.DrawChunk();
            }
        }

        meshSurface = this.gameObject.AddComponent<NavMeshSurface>();
        meshSurface.layerMask = 1;

        //StaticEditorFlags.OffMeshLinkGeneration = true;
        meshSurface.BuildNavMesh();

    }

    // Update is called once per frame
    void Update() {
        
    }
}
