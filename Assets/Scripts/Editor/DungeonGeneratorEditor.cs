using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (DungeonGenerator))]
public class DungeonGeneratorEditor : Editor {

	// Use this for initialization
	void Start () {

	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		DungeonGenerator myScript = (DungeonGenerator) target;
		if (GUILayout.Button ("Generate Dungeon")) {
			myScript.GenerateDungeon ();
		}
	}

}