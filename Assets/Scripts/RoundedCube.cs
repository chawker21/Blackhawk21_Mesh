using UnityEngine;

// This script generates a rounded cube mesh
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour {

	// Public variables that define the size and roundness of the cube
	public int xSize, ySize, zSize;
	public int roundness;

	// Private variables to hold the generated mesh data
	private Mesh mesh;
	private Vector3[] vertices;
	private Vector3[] normals;
	private Color32[] cubeUV;

	// Awake is called when the script instance is being loaded
	private void Awake () {
		// Generate the cube mesh
		Generate();
	}

	// Generate creates the rounded cube mesh
	private void Generate () {
		// Get the MeshFilter component and assign a new mesh to it
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		// Give the mesh a name
		mesh.name = "Procedural Cube";
		// Create the vertices for the mesh
		CreateVertices();
		// Create the triangles for the mesh
		CreateTriangles();
		// Create colliders for the mesh
		CreateColliders();
	}

	// CreateVertices generates the vertices for the mesh
	private void CreateVertices () {
		// Calculate the number of vertices needed
		int cornerVertices = 8;
		int edgeVertices = (xSize + ySize + zSize - 3) * 4;
		int faceVertices = (
			(xSize - 1) * (ySize - 1) +
			(xSize - 1) * (zSize - 1) +
			(ySize - 1) * (zSize - 1)) * 2;
		vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
		normals = new Vector3[vertices.Length];
		cubeUV = new Color32[vertices.Length];

		// Set the vertices and normals for the mesh
		int v = 0;
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				SetVertex(v++, x, y, 0);
			}
			for (int z = 1; z <= zSize; z++) {
				SetVertex(v++, xSize, y, z);
			}
			for (int x = xSize - 1; x >= 0; x--) {
				SetVertex(v++, x, y, zSize);
			}
			for (int z = zSize - 1; z > 0; z--) {
				SetVertex(v++, 0, y, z);
			}
		}
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				SetVertex(v++, x, ySize, z);
			}
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.colors32 = cubeUV;
	}

	//This code is defining a function called SetVertex that takes in four integer arguments: i, x, y, and z.
	//The function first creates a new Vector3 object called inner that is initialized with the values of x, y, and z.

	//Next, the code checks if x is less than roundness, and if so, sets the x value of inner to roundness.
	//If x is greater than xSize - roundness, the x value of inner is set to xSize - roundness.
	//Similar checks are performed for the y and z values of inner.

	//The code then calculates the normalized value of (vertices[i] - inner) and stores it in the normals array at index i.
	//The vertices[i] value is then updated to be equal to inner + normals[i] * roundness. Finally, a new Color32 object is
	//created and stored in the cubeUV array at index i, using the values of x, y, and z, as well as a constant alpha value of 0.
	private void SetVertex (int i, int x, int y, int z) {
		Vector3 inner = vertices[i] = new Vector3(x, y, z);

		if (x < roundness) {
			inner.x = roundness;
		}
		else if (x > xSize - roundness) {
			inner.x = xSize - roundness;
		}
		if (y < roundness) {
			inner.y = roundness;
		}
		else if (y > ySize - roundness) {
			inner.y = ySize - roundness;
		}
		if (z < roundness) {
			inner.z = roundness;
		}
		else if (z > zSize - roundness) {
			inner.z = zSize - roundness;
		}

		normals[i] = (vertices[i] - inner).normalized;
		vertices[i] = inner + normals[i] * roundness;
		cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
	}

	//This code is a method for generating triangles for all the faces of a cube.

	//The method creates three arrays for storing the triangles for the three different axis-aligned faces of the cube (x, y, and z).
	//It also declares variables for storing the current index in each of these arrays and a value called ring, which represents the number of vertices around the edge of each face of the cube.

	//The method then iterates over each vertical layer of the cube, setting quads for the x- and z-aligned faces using the SetQuad method.
	//It then calls the CreateTopFace and CreateBottomFace methods to generate the triangles for the top and bottom faces of the cube, respectively.

	//Finally, the method sets the triangles for each face by calling SetTriangles on the mesh object,
	//passing in the corresponding triangle array and submesh index as arguments
	private void CreateTriangles () {
		int[] trianglesZ = new int[(xSize * ySize) * 12];
		int[] trianglesX = new int[(ySize * zSize) * 12];
		int[] trianglesY = new int[(xSize * zSize) * 12];
		int ring = (xSize + zSize) * 2;
		int tZ = 0, tX = 0, tY = 0, v = 0;

		for (int y = 0; y < ySize; y++, v++) {
			for (int q = 0; q < xSize; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < zSize; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < xSize; q++, v++) {
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			for (int q = 0; q < zSize - 1; q++, v++) {
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
		}

		tY = CreateTopFace(trianglesY, tY, ring);
		tY = CreateBottomFace(trianglesY, tY, ring);

		mesh.subMeshCount = 3;
		mesh.SetTriangles(trianglesZ, 0);
		mesh.SetTriangles(trianglesX, 1);
		mesh.SetTriangles(trianglesY, 2);
	}

	//This code is a method for generating triangles for the top face of a cube.

	//The method takes in an array of triangles and an index for the starting position in the array.
	//It also takes in a value called ring, which represents the number of vertices around the edge of the top face of the cube.

	//The method begins by setting the first row of quads of the top face using the SetQuad method.
	//It then calculates the indices of the vertices at the corners and middle of the top face.

	//Next, the method iterates over the remaining rows of the top face (excluding the first and last rows), setting quads using the SetQuad method.

	//Finally, the method sets the last row of quads of the top face using the SetQuad method.
	//It then returns the index of the next open position in the triangles array.
	private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * ySize;
		for (int x = 0; x < xSize - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

		int vMin = ring * (ySize + 1) - 1;
		int vMid = vMin + 1;
		int vMax = v + 2;

		for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
			for (int x = 1; x < xSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
			}
			t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
		}

		int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
		for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
		t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

		return t;
	}

	//This code is a method for generating triangles for the bottom face of a cube.

	//The method takes in an array of triangles and an index for the starting position in the array.
	//It also takes in a value called ring, which represents the number of vertices around the edge of the bottom face of the cube.

	//The method begins by setting the first quad of the bottom face using the SetQuad method, with the vertices at the corners of the face.
	//It then iterates over the remaining vertices of the first row of the bottom face (excluding the corners), setting quads using the SetQuad method.
	//Next, the method iterates over the remaining rows of the bottom face (excluding the first and last rows), setting quads using the SetQuad method.
	//Finally, the method sets the last row of quads of the bottom face using the SetQuad method.
	//It then returns the index of the next open position in the triangles array.
	private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;
		int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
		for (int x = 1; x < xSize - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= xSize - 2;
		int vMax = v + 2;

		for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
			for (int x = 1; x < xSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
		}

		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

		return t;
	}

	
	//The method takes in an array of triangles, an index for the starting position in the array,
	//and the indices of the four vertices that make up the quad. It sets the values of the triangles
	//array at the specified indices such that the quad is formed. The method then returns the index of
	//the next open position in the triangles array, which will be used when adding additional quads to the array.
	private static int
		
		
	SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) {
		triangles[i] = v00;
		triangles[i + 1] = triangles[i + 4] = v01;
		triangles[i + 2] = triangles[i + 3] = v10;
		triangles[i + 5] = v11;
		return i + 6;
	}

	//The CreateColliders method first adds three box colliders to the cube,
	//which cover the sides of the cube except for the rounded corners.
	//It then calculates the minimum and maximum positions of the colliders on each of the three axes (x, y, and z),
	//as well as the midpoint of the cube on each axis.

	//Next, it adds 12 capsule colliders to the cube, located at each of the rounded corners.
	//The orientation and position of each capsule collider is determined by the values of the min, max,
	//and half variables, as well as the index passed to the AddCapsuleCollider method
	//(0, 1, or 2, corresponding to the x, y, or z axis).
	private void CreateColliders () {
		AddBoxCollider(xSize, ySize - roundness * 2, zSize - roundness * 2);
		AddBoxCollider(xSize - roundness * 2, ySize, zSize - roundness * 2);
		AddBoxCollider(xSize - roundness * 2, ySize - roundness * 2, zSize);

		Vector3 min = Vector3.one * roundness;
		Vector3 half = new Vector3(xSize, ySize, zSize) * 0.5f; 
		Vector3 max = new Vector3(xSize, ySize, zSize) - min;

		AddCapsuleCollider(0, half.x, min.y, min.z);
		AddCapsuleCollider(0, half.x, min.y, max.z);
		AddCapsuleCollider(0, half.x, max.y, min.z);
		AddCapsuleCollider(0, half.x, max.y, max.z);
		
		AddCapsuleCollider(1, min.x, half.y, min.z);
		AddCapsuleCollider(1, min.x, half.y, max.z);
		AddCapsuleCollider(1, max.x, half.y, min.z);
		AddCapsuleCollider(1, max.x, half.y, max.z);
		
		AddCapsuleCollider(2, min.x, min.y, half.z);
		AddCapsuleCollider(2, min.x, max.y, half.z);
		AddCapsuleCollider(2, max.x, min.y, half.z);
		AddCapsuleCollider(2, max.x, max.y, half.z);
	}

	private void AddBoxCollider (float x, float y, float z) {
		BoxCollider c = gameObject.AddComponent<BoxCollider>();
		c.size = new Vector3(x, y, z);
	}

	private void AddCapsuleCollider (int direction, float x, float y, float z) {
		CapsuleCollider c = gameObject.AddComponent<CapsuleCollider>();
		c.center = new Vector3(x, y, z);
		c.direction = direction;
		c.radius = roundness;
		c.height = c.center[direction] * 2f;
	}

//	private void OnDrawGizmos () {
//		if (vertices == null) {
//			return;
//		}
//		for (int i = 0; i < vertices.Length; i++) {
//			Gizmos.color = Color.black;
//			Gizmos.DrawSphere(vertices[i], 0.1f);
//			Gizmos.color = Color.yellow;
//			Gizmos.DrawRay(vertices[i], normals[i]);
//		}
//	}
}