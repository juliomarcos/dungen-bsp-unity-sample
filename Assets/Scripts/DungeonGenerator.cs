using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour {

	public int dungeonSize;
	[Range (1, 6)]
	public int numberOfIterations;
	public Tile tile;
	public bool shouldDebugDrawBsp;

	private Tilemap map;
	private char[, ] dungeon;
	private BspTree tree;

	// Use this for initialization
	void Start () {

	}

	private void InitDungeonMatrix () {
		dungeon = new char[dungeonSize, dungeonSize];
	}

	private void DrawTilesBasedOnMatrix () {
		for (int i = 0; i < dungeonSize; i++) {
			for (int j = 0; j < dungeonSize; j++) {
				var tileDescriptor = dungeon[i, j];
				if (tileDescriptor == 'f') {
					map.SetTile (new Vector3Int (i, j, 0), tile);
				}
			}
		}
	}

	void OnDrawGizmos () {
		AttemptDebugDrawBsp();
	}

	private void OnDrawGizmosSelected() {
		AttemptDebugDrawBsp();
	}

	void AttemptDebugDrawBsp() {
		if (shouldDebugDrawBsp) {
			DebugDrawBsp ();
		}
	}

	public void DebugDrawBsp () {
		if (tree == null) return; // hasn't been generated yet

		DebugDrawBspNode (tree); // recursive call
	}

	public void DebugDrawBspNode (BspTree node) {
		// Container
		Gizmos.color = Color.green;
		// top		
		Gizmos.DrawLine (new Vector3 (node.container.x, node.container.y, 0), new Vector3Int (node.container.xMax, node.container.y, 0));
		// right
		Gizmos.DrawLine (new Vector3 (node.container.xMax, node.container.y, 0), new Vector3Int (node.container.xMax, node.container.yMax, 0));
		// bottom
		Gizmos.DrawLine (new Vector3 (node.container.x, node.container.yMax, 0), new Vector3Int (node.container.xMax, node.container.yMax, 0));
		// left
		Gizmos.DrawLine (new Vector3 (node.container.x, node.container.y, 0), new Vector3Int (node.container.x, node.container.yMax, 0));

		// children
		if (node.left != null) DebugDrawBspNode (node.left);
		if (node.right != null) DebugDrawBspNode (node.right);
	}

	private void InitReferences () {
		map = GetComponentInChildren<Tilemap> ();
		map.ClearAllTiles ();
	}

	private void GenerateRoomsInsideContainers () {
		BspTree.GenerateRoomsInsideContainersNode (tree);
	}

	private void GenerateContainersUsingBsp () {
		tree = BspTree.Split (numberOfIterations, new RectInt (0, 0, dungeonSize, dungeonSize));
	}

	private void UpdateTilemapUsingTreeNode (BspTree node) {
		if (node.left == null && node.right == null) {
			for (int i = node.room.x; i < node.room.xMax; i++)
			{
				for (int j = node.room.y; j < node.room.yMax; j++)
				{
					map.SetTile(new Vector3Int(i, j, 0), tile);
				}
			}
			
		} else {
			if (node.left != null) UpdateTilemapUsingTreeNode (node.left);
			if (node.right != null) UpdateTilemapUsingTreeNode (node.right);
		}
	}

	private void UpdateTilemapUsingTree () {
		UpdateTilemapUsingTreeNode (tree);
	}

	private void GenerateCorridors()
	{
		// for each parent
		// find their center
		// find a direction and connect these centers
		GenerateCorridorsNode(tree);
	}

	private void GenerateCorridorsNode(BspTree node) {
		if (node.left != null && node.right != null) {
			RectInt leftContainer = node.left.container;
			RectInt rightContainer = node.right.container;
			Vector2 leftCenter = leftContainer.center;
			Vector2 rightCenter = rightContainer.center;
			Vector2 direction = (rightCenter - leftCenter).normalized; // arbitrarily choosing right as the target point
			while (Vector2.Distance(leftCenter, rightCenter) > 1) {
				leftCenter.x += direction.x;
				leftCenter.y += direction.y;
				map.SetTile(new Vector3Int((int) leftCenter.x, (int) leftCenter.y, 0), tile);
			}
		} else {
			if (node.left != null) GenerateCorridorsNode (node.left);
			if (node.right != null) GenerateCorridorsNode (node.right);
		}
	}

	public void GenerateDungeon () {
		InitReferences ();
		InitDungeonMatrix ();
		GenerateContainersUsingBsp ();
		GenerateRoomsInsideContainers ();
		GenerateCorridors();
		UpdateTilemapUsingTree ();
		//CreatePathsBetweenContainersCenters();
		DrawTilesBasedOnMatrix ();
	}

}