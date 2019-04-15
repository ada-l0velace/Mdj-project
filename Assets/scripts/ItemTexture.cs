using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : ScriptableObject {
    static List<Item> items_;

    public static void Init() {
        items_ = new List<Item>();
        Add(new Item(1, "Grass", 64, ItemTexture.Grass));
        Add(new Item(3, "Stone", 64, ItemTexture.Stone));

    }

    public static List<Item> GetItems() {
        return items_;
    }

    public static void Add(Item item) {
        items_.Add(item);
    }

    public static void Remove(Item item) {
        items_.Remove(item);
    }

    public static Item GetItemById(int id) {
        foreach (Item item in items_) {
            if (item.id == id) {
                return item;
            }
        }
        return null;
    }
    public static Item GetItemByName(string name) {
        foreach (Item item in items_) {
            if (item.name == name) {
                return item;
            }
        }
        return null;
    }
}

[System.Serializable]
public class Item {
    public int id;
    public string name;
    public int maxStackSize;
    public ItemTexture texture;

    public Item(int id, string name, int maxStackSize, ItemTexture texture) {
        this.id = id;
        this.name = name;
        this.maxStackSize = maxStackSize;
        this.texture = texture;
    }
}

[System.Serializable]
public class ItemTexture {
    public static float resoution = 0.0625f;
    public Vector2 front = Vector2.one * resoution,
    back = Vector2.one * resoution,
    left = Vector2.one * resoution,
    right = Vector2.one * resoution,
    top = Vector2.one * resoution,
    bottom = Vector2.one * resoution;

    public ItemTexture(Vector2 front, Vector2 back, Vector2 left, Vector2 right, Vector2 top, Vector2 bottom) {
        this.front = front * resoution;
        this.back = back * resoution;
        this.left = left * resoution;
        this.right = right * resoution;
        this.top = top * resoution;
        this.bottom = bottom * resoution;
    }

    public ItemTexture(Vector2 all) {
        this.front = all * resoution;
        this.back = all * resoution;
        this.left = all * resoution;
        this.right = all * resoution;
        this.top = all * resoution;
        this.bottom = all * resoution;
    }

    public static ItemTexture Grass = new ItemTexture(new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(2f, 6f), new Vector2(2f, 15f));
    public static ItemTexture Stone = new ItemTexture(new Vector2(1f, 15f));
    public static ItemTexture Dirt = new ItemTexture(new Vector2(2f, 15f));
    public static ItemTexture Air = new ItemTexture(new Vector2(0f, 0f));

}