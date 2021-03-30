/// Author: David Sesto (GitHub: @dsesto)
/// Digging Dinosaurs Games (Twitter @digging_dinos)

using UnityEngine;

/* List of vertices that define each of the possible cell configurations in the Marching Squares algorithm.
 * The order [0-15] is the one proposed in the documentation, i.e. southwest, southeast, northeast, northwest.
 * Each triangle (from the "trianglesLookupTable") is defined by the corresponding vertices in the "verticesLookupTable"
 */
public static class MarchingSquares {

	/// The vertices of our triangles are identified on a 2D space where:
	///    -1 0 1
	///   1 ·———·
	///   0 |   | Y
	///  -1 ·———·
	///       X
	public static Vector3[][] vertices = new Vector3[][]{
		// 0
		new Vector3[]{ },
		// 1
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(0, -1, 0), new Vector3(-1, -1, 0) },
		// 2
		new Vector3[]{  new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector3(1, -1, 0) },	
		// 3
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0) },	
		// 4
		new Vector3[]{  new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0) },	
		// 5
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(0, -1, 0), new Vector3(-1, -1, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0) },
		// 6
		new Vector3[]{  new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0)},	
		// 7
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0) },	
		// 8
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(-1, 1, 0)},	
		// 9
		new Vector3[]{  new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(-1, 1, 0), new Vector3(-1, -1, 0)},
		// 10
		new Vector3[]{  new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector3(1, -1, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(-1, 1, 0) },
		// 11
		new Vector3[]{  new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(-1, 1, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0)},
		// 12
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0)},
		// 13
		new Vector3[]{  new Vector3(0, -1, 0), new Vector3(1, 0, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(-1, -1, 0)},
		// 14
		new Vector3[]{  new Vector3(-1, 0, 0), new Vector3(0, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0)},
		// 15
		new Vector3[]{ new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0) }
	};

	/// Definition of the triangles needed to represent the Configuration of each of the GridCells.
	/// Each triangle is represented by 3 vertices, and each Cell Configuration may have more than 1 triangle.
	/// The order of the vertices matter, because the face of the triangles is on the vertices ordered clock-wise.
	public static int[][] triangles = new int[][]{
		// 0
		new int[] {},
		// 1
		new int[] { 0,1,2 },
		// 2
		new int[] { 0,1,2 },
		// 3
		new int[] { 0,1,2, 1,3,2 },
		// 4
		new int[] { 0,2,1 },
		// 5
		new int[] { 0,1,2, 5,4,3 },
		// 6
		new int[] { 0,2,1, 1,2,3 },
		// 7
		new int[] { 0,3,4, 0,1,3, 1,2,3 },
		// 8
		new int[] { 0,2,1 },
		// 9
		new int[] { 0,1,2, 1,3,2 },
		// 10
		new int[] { 0,1,2, 3,5,4 },
		// 11
		new int[] { 0,3,2, 0,1,3, 1,4,3 },
		// 12
		new int[] { 0,2,1, 1,2,3 },
		// 13
		new int[] { 2,3,1, 2,1,0, 2,0,4 },
		// 14
		new int[] { 0,2,3, 0,3,1, 1,3,4 },
		// 15
		new int[] { 0,1,3, 1,2,3 }
	};
}
