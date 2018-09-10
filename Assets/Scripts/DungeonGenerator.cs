using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour {

	public int dungeonSize;
	public Tile tile;
	public bool shouldDebugDrawBsp;
	
	private Tilemap map;	
	private char[,] dungeon;

    // Use this for initialization
    void Start () {
		
	}

	private void InitDungeonMatrix() {
		dungeon = new char[dungeonSize,dungeonSize];
	}

	private void DrawTilesBasedOnMatrix()
    {
		for (int i = 0; i < dungeonSize; i++)
		{
			for (int j = 0; j < dungeonSize; j++)
			{
				var tileDescriptor = dungeon[i,j];
				if (tileDescriptor == 'f') {
					map.SetTile(new Vector3Int(i, j, 0), tile);		
				}
			}
		}
    }
	
	// Update is called once per frame
	void OnDrawGizmos () {
		if (shouldDebugDrawBsp) {
			DebugDrawBSP();
		}
	}

	public void DebugDrawBSP() {
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, new Vector3(5, 5, 0));
	}

	private void InitReferences()
    {
        map = GetComponentInChildren<Tilemap>();
		map.ClearAllTiles();
    }

	public void GenerateDungeon () {
		InitReferences();
		InitDungeonMatrix();
		DrawTilesBasedOnMatrix();
	}

    
}