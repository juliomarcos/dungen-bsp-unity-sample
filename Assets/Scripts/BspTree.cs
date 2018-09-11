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

    internal static BspTree Split (int numberOfIterations, float ratio, RectInt container) {
        var node = new BspTree (container);
        if (numberOfIterations == 0) return node;

        var splittedContainers = SplitContainer (container, ratio);
        node.left = Split (numberOfIterations - 1, ratio, splittedContainers[0]);
        node.right = Split (numberOfIterations - 1, ratio, splittedContainers[1]);

        return node;
    }

    private static RectInt[] SplitContainer (RectInt container, float ratio) {
        RectInt c1, c2;
        float c1Width, c2Width;
        float c1Height, c2Height;
        float c1Min, c1Max;
        float c2Min, c2Max;
        int iCount = 0;
        do {
            if ( /*UnityEngine.Random.Range (0f, 1f) > 0.5f*/ true) {
                // vertical
                c1 = new RectInt (container.x, container.y, container.width, UnityEngine.Random.Range (1, container.height));
                c2 = new RectInt (container.x, container.y + c1.height, container.width, container.height - c1.height);
            } else {
                // horizontal
                c1 = new RectInt (container.x, container.y, container.width, UnityEngine.Random.Range (1, container.height));
                c2 = new RectInt (container.x, container.y + c1.height, container.width, container.height - c1.height);
            }

            c1Width = c1.width;
            c2Width = c2.width;
            c1Height = c1.height;
            c2Height = c2.height;

            iCount++;

            if (c1Width > c1Height) {
                c1Min = c1Height;
                c1Max = c1Width;
            } else {
                c1Min = c1Width;
                c1Max = c1Height;
            }

            if (c2Width > c2Height) {
                c2Min = c2Height;
                c2Max = c2Width;
            } else {
                c2Min = c2Width;
                c2Max = c2Height;
            }

        } while ((c1Min / c1Max > ratio || c2Min / c2Max > ratio) && iCount < 30); // should actually keep the best attempt

        return new RectInt[] { c1, c2 };
    }
}