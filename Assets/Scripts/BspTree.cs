using System;
using UnityEngine;

public class BspTree {

    public RectInt container;
    public RectInt room;
    public BspTree left;
    public BspTree right;

    public BspTree (RectInt container) {
        this.container = container;
    }

    public bool IsLeaf () {
        return left == null && right == null;
    }

    internal static BspTree Split (int numberOfIterations, RectInt container) {
        var node = new BspTree (container);
        if (numberOfIterations == 0) return node;

        var splittedContainers = SplitContainer (container);
        node.left = Split (numberOfIterations - 1, splittedContainers[0]);
        node.right = Split (numberOfIterations - 1, splittedContainers[1]);

        return node;
    }

    private static RectInt[] SplitContainer (RectInt container) {
        RectInt c1, c2;
        if (UnityEngine.Random.Range (0f, 1f) > 0.5f) {
                // vertical
                c1 = new RectInt (container.x, container.y, container.width, (int) UnityEngine.Random.Range (container.height * 0.3f, container.height * 0.5f));
                c2 = new RectInt (container.x, container.y + c1.height, container.width, container.height - c1.height);
            } else {
                // horizontal 
                c1 = new RectInt (container.x, container.y, (int) UnityEngine.Random.Range (container.width * 0.3f, container.width * 0.5f), container.height);
                c2 = new RectInt (container.x + c1.width, container.y, container.width - c1.width, container.height);
            }
        return new RectInt[] { c1, c2 };
    }

    public static void GenerateRoomsInsideContainersNode(BspTree node) 
	{
		// should create rooms for leafs
		if (node.left == null && node.right == null) {
            var randomX = UnityEngine.Random.Range(DungeonGenerator.MIN_ROOM_DELTA, node.container.width / 4);
            var randomY = UnityEngine.Random.Range(DungeonGenerator.MIN_ROOM_DELTA, node.container.height / 4);
            int roomX = node.container.x + randomX;
            int roomY = node.container.y + randomY;
            int roomW = node.container.width - (int) (randomX * UnityEngine.Random.Range(1f, 2f));
            int roomH = node.container.height - (int) (randomY * UnityEngine.Random.Range(1f, 2f));
			node.room = new RectInt(roomX, roomY, roomW, roomH);
		} else {
            if (node.left != null) GenerateRoomsInsideContainersNode(node.left);
            if (node.right != null) GenerateRoomsInsideContainersNode(node.right);
        }
	}

}