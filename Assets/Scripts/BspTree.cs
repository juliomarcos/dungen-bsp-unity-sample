using System;
using UnityEngine;

public class BspTree {

    public RectInt container;
    //public Rect room;
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
            c1 = new RectInt (container.x, container.y, container.width / 2, container.height);
            c2 = new RectInt (container.x + container.width / 2, container.y, container.width / 2, container.height);
        } else {
            // horizontal
            c1 = new RectInt (container.x, container.y, container.width, container.height / 2);
            c2 = new RectInt (container.x, container.y + container.height / 2, container.width, container.height / 2);
        }

        return new RectInt[] { c1, c2 };
    }
}