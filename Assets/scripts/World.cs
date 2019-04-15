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
    public enum NDIR { UP, DOWN, LEFT, RIGHT, FRONT, BACK }
    public Vector3[,,] allVertices = new Vector3[chunkSize + 1, chunkSize + 1, chunkSize + 1];
    public static ConcurrentDictionary<string, Chunk> chunks;
    public Chunk c;
    int[,] map = new int[,]  {{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                                {1,0,1,0,0,0,0,0,0,0,0,1,1,0,1,1},
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

    void BuildChunkAt(int x, int y, int z) {
        Vector3 chunkPostion = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);

        string n = BuildChunkName(chunkPostion);
        c = new Chunk(chunkPostion, textureAtlas);
        c.chunk.transform.parent = this.transform;
        chunks.TryAdd(c.chunk.name, c);
    }

    void BuildChunkAt(int x, int y, int z, ItemTexture top, ItemTexture bot) {
        Vector3 chunkPostion = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
        string n = BuildChunkName(chunkPostion);
        c = new Chunk(chunkPostion, top, bot ,textureAtlas);
        c.chunk.transform.parent = this.transform;
        chunks.TryAdd(c.chunk.name, c);
    }

    void buildAtIndex(int i, int j) {
        if (map[7 + i, 7 + j] == 0)
            BuildChunkAt(i, 1, j);
        else
            BuildChunkAt(i, 1, j, ItemTexture.Grass, ItemTexture.Dirt);
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
        for (int j = 0; j < 8; j++) {
            for (int i = 0; i < 8; i++) {
                BuildChunkAt(i, 0, j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(i, j);

                BuildChunkAt(-i, 0, -j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(-i, -j);

                BuildChunkAt(i, 0, -j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(i, -j);

                BuildChunkAt(-i, 0, j, ItemTexture.Stone, ItemTexture.Stone);
                buildAtIndex(-i, j);
            }
        }

        foreach (KeyValuePair<string, Chunk> c in chunks) {
            if (c.Value.status == Chunk.ChunkStatus.DRAW) {
                c.Value.DrawChunk();
            }
        }

    }

    // Update is called once per frame
    void Update() {
        
    }
}
