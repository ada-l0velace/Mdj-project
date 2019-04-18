using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.AI;
using UnityEditor;

[Serializable]
class BlockData {
    public Block.BlockType[,,] matrix;

    public BlockData() { }

    public BlockData(Block[,,] b) {
        matrix = new Block.BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++) {
            for (int y = 0; y < World.chunkSize; y++) {
                for (int x = 0; x < World.chunkSize; x++) {
                    matrix[x, y, z] = b[x, y, z].bType;
                }
            }
        }
    }
}

public class Chunk {

    public Material cubeMaterial;
    public Block[,,] chunkData;
    public GameObject chunk;

    public GameObject fluid;
    public enum ChunkStatus { DRAW, DONE, KEEP };
    public ChunkStatus status;
    public float touchedTime;

    List<Vector3> Verts = new List<Vector3>();
    List<Vector3> Norms = new List<Vector3>();
    List<Vector2> UVs = new List<Vector2>();
    List<int> Tris = new List<int>();

    Block.BlockType btype;
    
    ItemTexture surfaceTexture;
    ItemTexture bottomTexture;

    public bool changed = false;
    
    // Use this for initialization
    public Chunk(Vector3 position, Material c) {
        surfaceTexture = ItemTexture.Air;
        bottomTexture = ItemTexture.Air;
        btype = Block.BlockType.AIR;
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cubeMaterial = c;
        BuildChunk();
    }

    public Chunk(Vector3 position, ItemTexture top, ItemTexture bot, Material c) {
        surfaceTexture = top;
        bottomTexture = bot;
        btype = Block.BlockType.GRASS;
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cubeMaterial = c;
        BuildChunk();
    }

    void BuildChunk() {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++) {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + chunk.transform.position.x);
                    int worldY = (int)(y + chunk.transform.position.y);
                    int worldZ = (int)(z + chunk.transform.position.z);
                    if (y == World.chunkSize-1)
                        chunkData[x, y, z] = new Block(btype, pos,chunk.gameObject, this, surfaceTexture);
                    else {
                        chunkData[x, y, z] = new Block(btype, pos, chunk.gameObject, this, bottomTexture);
                    }
                    status = ChunkStatus.DRAW;

                }
    }

    public void DrawChunk() {

        Verts.Clear();
        Norms.Clear();
        UVs.Clear();
        Tris.Clear();

        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++) {

                    chunkData[x, y, z].Draw(Verts, Norms, UVs, Tris);

                }

        CombineQuads(chunk.gameObject, Verts, Norms, UVs, Tris, cubeMaterial);

        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
        //collider.tag = "PlacebleObject";
        status = ChunkStatus.DONE;
    }

    public void ReDraw() {
        GameObject.DestroyImmediate(chunk.GetComponent<MeshFilter>());
        GameObject.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
        GameObject.DestroyImmediate(chunk.GetComponent<Collider>());

        GameObject.DestroyImmediate(fluid.GetComponent<MeshFilter>());
        GameObject.DestroyImmediate(fluid.GetComponent<MeshRenderer>());
        DrawChunk();
    }


    public void CombineQuads(GameObject o, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t, Material m) {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        mesh.vertices = v.ToArray();
        mesh.normals = n.ToArray();
        mesh.uv = u.ToArray();
        //mesh.SetUVs(1, su);
        mesh.triangles = t.ToArray();


        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = (MeshFilter)o.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = o.AddComponent<MeshRenderer>();
        renderer.material = m;

    }

}