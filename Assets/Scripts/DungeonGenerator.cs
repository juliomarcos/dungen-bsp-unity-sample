using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour {

	public int dungeonSize;

	[Range (0, 6)]
	public int numberOfIterations;

	[Range (1, 4)]
	public int corridorThickness;

	public bool shouldDebugDrawBsp;

	public Tile debugTile;
	public const int MIN_ROOM_DELTA = 2;

	[HideInInspector]
	public Tile tlTile;
	[HideInInspector]
	public Tile tmTile;
	[HideInInspector]
	public Tile trTile;
	[HideInInspector]
	public Tile mlTile;
	[HideInInspector]
	public Tile mmTile;
	[HideInInspector]
	public Tile mrTile;
	[HideInInspector]
	public Tile blTile;
	[HideInInspector]
	public Tile bmTile;
	[HideInInspector]
	public Tile brTile;

	private Tilemap map;
	private BspTree tree;

	// Use this for initialization
	void Start () {

	}

	void OnDrawGizmos () {
		AttemptDebugDrawBsp ();
	}

	private void OnDrawGizmosSelected () {
		AttemptDebugDrawBsp ();
	}

	void AttemptDebugDrawBsp () {
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
		Random.InitState(3);
	}

	private void GenerateRoomsInsideContainers () {
		BspTree.GenerateRoomsInsideContainersNode (tree);
	}

	private void GenerateContainersUsingBsp () {
		tree = BspTree.Split (numberOfIterations, new RectInt (0, 0, dungeonSize, dungeonSize));
	}

	private void UpdateTilemapUsingTreeNode (BspTree node) {
		if (node.left == null && node.right == null) {
			for (int i = node.room.x; i < node.room.xMax; i++) {
				for (int j = node.room.y; j < node.room.yMax; j++) {
					map.SetTile (new Vector3Int (i, j, 0), mmTile);
				}
			}

		} else {
			if (node.left != null) UpdateTilemapUsingTreeNode (node.left);
			if (node.right != null) UpdateTilemapUsingTreeNode (node.right);
		}
	}

	private void FillRoomsOnTilemap () {
		UpdateTilemapUsingTreeNode (tree);
	}

	private void GenerateCorridors () {
		// for each parent
		// find their center
		// find a direction and connect these centers
		GenerateCorridorsNode (tree);
	}

	private void GenerateCorridorsNode (BspTree node) {
		print ('a');
		if (node.IsInternal()) {
			print('b');
			RectInt leftContainer = node.left.container;
			RectInt rightContainer = node.right.container;
			Vector2 leftCenter = leftContainer.center;
			Vector2 rightCenter = rightContainer.center;
			Vector2 direction = (rightCenter - leftCenter).normalized; // arbitrarily choosing right as the target point
			while (Vector2.Distance (leftCenter, rightCenter) > 1) {
				if (direction.Equals (Vector2.right)) {
					for (int i = 0; i < corridorThickness; i++) {
						map.SetTile (new Vector3Int ((int) leftCenter.x, (int) leftCenter.y + i, 0), mmTile);
					}
				} else if (direction.Equals (Vector2.up)) {
					for (int i = 0; i < corridorThickness; i++) {
						map.SetTile (new Vector3Int ((int) leftCenter.x + i, (int) leftCenter.y, 0), mmTile);
					}
				}
				leftCenter.x += direction.x;
				leftCenter.y += direction.y;
			}
			if (node.left != null) GenerateCorridorsNode (node.left);
			if (node.right != null) GenerateCorridorsNode (node.right);
		}
	}

	private Tile GetTileByNeihbors (int i, int j) {
		var mmGridTile = map.GetTile (new Vector3Int (i,   j, 0));
		if (mmGridTile == null) return null; // you shouldn't repaint a null

		var blGridTile = map.GetTile (new Vector3Int (i-1, j-1, 0));
		var bmGridTile = map.GetTile (new Vector3Int (i,   j-1, 0));
		var brGridTile = map.GetTile (new Vector3Int (i+1, j-1, 0));

		var mlGridTile = map.GetTile (new Vector3Int (i-1, j, 0));
		var mrGridTile = map.GetTile (new Vector3Int (i+1, j, 0));

		var tlGridTile = map.GetTile (new Vector3Int (i-1, j+1, 0));
		var tmGridTile = map.GetTile (new Vector3Int (i,   j+1, 0));
		var trGridTile = map.GetTile (new Vector3Int (i+1, j+1, 0));

		// we have 8 + 1 cases
		
		// left
		if (mlGridTile == null && tmGridTile == null) return tlTile;
		if (mlGridTile == null && tmGridTile != null && bmGridTile != null) return mlTile;
		if (mlGridTile == null && bmGridTile == null && tmGridTile != null) return blTile;
		
		// middle
		if (mlGridTile != null && tmGridTile == null && mrGridTile != null) return tmTile;
		if (mlGridTile != null && bmGridTile == null && mrGridTile != null) return bmTile;
		
		// right
		if (mlGridTile != null && tmGridTile == null && mrGridTile == null) return trTile;
		if (tmGridTile != null && bmGridTile != null && mrGridTile == null) return mrTile;
		if (tmGridTile != null && bmGridTile == null && mrGridTile == null) return brTile;

		return mmTile; // default case
	}

	private void PaintTilesAccordingToTheirNeighbors () {
		for (int i = MIN_ROOM_DELTA; i < dungeonSize; i++) {
			for (int j = MIN_ROOM_DELTA; j < dungeonSize; j++) {
				var tile = GetTileByNeihbors (i, j);
				if (tile != null) {
					map.SetTile(new Vector3Int(i, j, 0), tile);
				}
			}
		}
	}

	public void GenerateDungeon () {
		InitReferences ();
		GenerateContainersUsingBsp ();
		GenerateRoomsInsideContainers ();
		GenerateCorridors ();
		FillRoomsOnTilemap ();
		//PaintTilesAccordingToTheirNeighbors ();
	}

}