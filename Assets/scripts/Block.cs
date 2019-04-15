using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Block {
    public enum BlockType { DIRT, GRASS, AIR };
    public enum Cubeside { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public BlockType bType;
    public Vector3 position;
    public Chunk owner;
    public bool isSolid;
    protected GameObject parent;
    ItemTexture texture;
    public Block(BlockType b, Vector3 pos, GameObject p, Chunk o) {
        texture = ItemTexture.Stone;
        bType = b;
        position = pos;
        parent = p;
        owner = o;
        isSolid = b != BlockType.AIR;
    }

    public Block(BlockType b, Vector3 pos, GameObject p, Chunk o, ItemTexture tex) : this(b, pos, p, o) {
        texture = tex;
    }

    int ConvertBlockIndexToLocal(int i) {
        if (i <= -1)
            i = World.chunkSize + i;
        else if (i >= World.chunkSize)
            i = i - World.chunkSize;
        return i;
    }
    public bool HasSolidNeighbour(int x, int y, int z) {
        Block b = GetBlock(x, y, z);
        if (b != null)
            return (b.isSolid || b.bType == bType);

        return false;
    }

    public Block GetBlock(int x, int y, int z) {
        Block[,,] chunks;

        // Block in a neighbouring chunk
        if (x < 0 || x >= World.chunkSize ||
            y < 0 || y >= World.chunkSize ||
            z < 0 || z >= World.chunkSize) {

            int newX = x, newY = y, newZ = z;
            newX = (x - (int)(position.x)) * World.chunkSize;
            newY = (y - (int)(position.y)) * World.chunkSize;
            newZ = (z - (int)(position.z)) * World.chunkSize;

            Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3(newX,
                newY, newZ);

            string nName = World.BuildChunkName(neighbourChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk nChunk;
            if (World.chunks.TryGetValue(nName, out nChunk))
                chunks = nChunk.chunkData;
            else {
                return null;
            }
        }
        else
            chunks = owner.chunkData;

        try {
            return chunks[x, y, z];
        } catch (System.IndexOutOfRangeException ex) { Debug.Log(ex.Message); }
        return null;
    }

    protected void CreateQuad(Cubeside side, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t) {

        float resolution = 0.0625f;

        //all possible UVs
        Vector2 uv00 = new Vector2(0f, 0f) * resolution;
        Vector2 uv10 = new Vector2(1f, 0f) * resolution;
        Vector2 uv01 = new Vector2(0f, 1f) * resolution;
        Vector2 uv11 = new Vector2(1f, 1f) * resolution;


        int x = (int)position.x, y = (int)position.y, z = (int)position.z;

        //all possible vertices
        Vector3 p0 = World.Instance.allVertices[x, y, z + 1];
        Vector3 p1 = World.Instance.allVertices[x + 1, y, z + 1];
        Vector3 p2 = World.Instance.allVertices[x + 1, y, z];
        Vector3 p3 = World.Instance.allVertices[x, y, z];
        Vector3 p4 = World.Instance.allVertices[x, y + 1, z + 1];
        Vector3 p5 = World.Instance.allVertices[x + 1, y + 1, z + 1];
        Vector3 p6 = World.Instance.allVertices[x + 1, y + 1, z];
        Vector3 p7 = World.Instance.allVertices[x, y + 1, z];

        int trioffset = 0;

        switch (side) {
            case Cubeside.BOTTOM:
                trioffset = v.Count;
                v.Add(p0); v.Add(p1); v.Add(p2); v.Add(p3);
                n.Add(Vector3.down);
                n.Add(Vector3.down);
                n.Add(Vector3.down);
                n.Add(Vector3.down);
                u.Add(uv11 + texture.bottom); u.Add(uv01 + texture.bottom); u.Add(uv00 + texture.bottom); u.Add(uv10 + texture.bottom);
                t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
                break;

            case Cubeside.TOP:
                trioffset = v.Count;
                v.Add(p7); v.Add(p6); v.Add(p5); v.Add(p4);
                n.Add(Vector3.up);
                n.Add(Vector3.up);
                n.Add(Vector3.up);
                n.Add(Vector3.up);
                u.Add(uv11 + texture.top); u.Add(uv01 + texture.top); u.Add(uv00 + texture.top); u.Add(uv10 + texture.top);
                t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
                break;

            case Cubeside.LEFT:
                trioffset = v.Count;
                v.Add(p7); v.Add(p4); v.Add(p0); v.Add(p3);
                n.Add(Vector3.left);
                n.Add(Vector3.left);
                n.Add(Vector3.left);
                n.Add(Vector3.left);
                u.Add(uv11 + texture.left); u.Add(uv01 + texture.left); u.Add(uv00 + texture.left); u.Add(uv10 + texture.left);
                t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
                break;

            case Cubeside.RIGHT:
                trioffset = v.Count;
                v.Add(p5); v.Add(p6); v.Add(p2); v.Add(p1);
                n.Add(Vector3.right);
                n.Add(Vector3.right);
                n.Add(Vector3.right);
                n.Add(Vector3.right);
                u.Add(uv11 + texture.right); u.Add(uv01 + texture.right); u.Add(uv00 + texture.right); u.Add(uv10 + texture.right);
                t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
                break;

            case Cubeside.FRONT:
                trioffset = v.Count;
                v.Add(p4); v.Add(p5); v.Add(p1); v.Add(p0);
                n.Add(Vector3.forward);
                n.Add(Vector3.forward);
                n.Add(Vector3.forward);
                n.Add(Vector3.forward);
                u.Add(uv11 + texture.front); u.Add(uv01 + texture.front); u.Add(uv00 + texture.front); u.Add(uv10 + texture.front);
                t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
                break;

            case Cubeside.BACK:
                trioffset = v.Count;
                v.Add(p6); v.Add(p7); v.Add(p3); v.Add(p2);
                n.Add(Vector3.back);
                n.Add(Vector3.back);
                n.Add(Vector3.back);
                n.Add(Vector3.back);
                u.Add(uv11 + texture.back); u.Add(uv01 + texture.back); u.Add(uv00 + texture.back); u.Add(uv10 + texture.back);
                t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
                break;


            default:
                break;
        }
    }

        // Use this for initialization
    public virtual void Draw(List<Vector3> v, List<Vector3> n, List<Vector2> u, List<int> t) {
        if (bType == BlockType.AIR)
            return;
        int[][] b = new int[][] { new int[] { 0, -1, 0 }, new int[] { 0, 1, 0 }, new int[] { -1, 0, 0 }, new int[] { 1, 0, 0 }, new int[] { 0, 0, 1 }, new int[] { 0, 0, -1 } };
        int i = 0;
        foreach (Cubeside side in Enum.GetValues(typeof(Cubeside))) {
            if (!HasSolidNeighbour((int)position.x + b[i][0], (int)position.y + b[i][1], (int)position.z + b[i][2])) {
                CreateQuad(side, v, n, u, t);
            }
            i += 1;
        }
        //CombineQuads();
    }
}
