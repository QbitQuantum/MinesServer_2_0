using System;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class SurfaceRenderer : MonoBehaviour
{
	// Token: 0x060002AD RID: 685 RVA: 0x00028880 File Offset: 0x00026A80
	private void Start()
	{
		this.TransitMeshFilter = this.transit.GetComponent<MeshFilter>();
		this.PerspectiveMeshFilter = this.perspectiveSurface.GetComponent<MeshFilter>();
		this.TransitMesh = new Mesh();
		this.PerspectiveMesh = new Mesh();
		this.TransitMeshFilter.mesh = this.TransitMesh;
		this.PerspectiveMeshFilter.mesh = this.PerspectiveMesh;
		this._uvPers = new Vector2[4];
		this._uvPers2 = new Vector2[4];
		this._verticesPers = new Vector3[4];
		this._uvTransit = new Vector2[4];
		this._verticesTransit = new Vector3[4];
		this._triangles = new int[6];
		this._triangles[0] = 0;
		this._triangles[1] = 1;
		this._triangles[2] = 2;
		this._triangles[3] = 3;
		this._triangles[4] = 2;
		this._triangles[5] = 1;
		this._uvPers2[0] = new Vector2(0f, 0f);
		this._uvPers2[1] = new Vector2(0f, 1f);
		this._uvPers2[2] = new Vector2(1f, 0f);
		this._uvPers2[3] = new Vector2(1f, 1f);
		this.TransitMesh.vertices = this._verticesTransit;
		this.TransitMesh.uv = this._uvTransit;
		this.TransitMesh.triangles = this._triangles;
		this.PerspectiveMesh.vertices = this._verticesPers;
		this.PerspectiveMesh.uv = this._uvPers;
		this.PerspectiveMesh.uv2 = this._uvPers2;
		this.PerspectiveMesh.triangles = this._triangles;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00028A48 File Offset: 0x00026C48
	private void UpdateMeshes()
	{
		float num = this.terrainRenderer.cx - (float)Screen.width / 30f;
		float num2 = this.terrainRenderer.cx + (float)Screen.width / 30f;
		float num3 = -(num - Mathf.Floor(num / 30f) * 30f) / 30f;
		float num4 = num3 + (num - num2) / 30f;
		this._uvTransit[0] = new Vector2(num3, 0f);
		this._uvTransit[1] = new Vector2(num3, 1f);
		this._uvTransit[2] = new Vector2(num4, 0f);
		this._uvTransit[3] = new Vector2(num4, 1f);
		this._verticesTransit[0] = new Vector3(num, 0f, this.Z);
		this._verticesTransit[1] = new Vector3(num, 2f, this.Z);
		this._verticesTransit[2] = new Vector3(num2, 0f, this.Z);
		this._verticesTransit[3] = new Vector3(num2, 2f, this.Z);
		float num5 = 15f + 0.5f * (num3 + num4);
		float num6 = num4 - num3;
		this._uvPers[0] = new Vector2(num5 - 0.5f * num6, 0f);
		this._uvPers[1] = new Vector2(num5 - 0.5f * num6, 1f);
		this._uvPers[2] = new Vector2(num5 + 0.5f * num6, 0f);
		this._uvPers[3] = new Vector2(num5 + 0.5f * num6, 1f);
		this._verticesPers[0] = new Vector3(num, 2f, this.Z);
		this._verticesPers[1] = new Vector3(num, 4f, this.Z);
		this._verticesPers[2] = new Vector3(num2, 2f, this.Z);
		this._verticesPers[3] = new Vector3(num2, 4f, this.Z);
		this.TransitMesh.bounds = new Bounds(new Vector3(this.terrainRenderer.cx, 0f, 0f), new Vector3(100f, 100f, 10f));
		this.TransitMesh.vertices = this._verticesTransit;
		this.TransitMesh.uv = this._uvTransit;
		this.PerspectiveMesh.bounds = new Bounds(new Vector3(this.terrainRenderer.cx, 0f, 0f), new Vector3(100f, 100f, 10f));
		this.PerspectiveMesh.vertices = this._verticesPers;
		this.PerspectiveMesh.uv = this._uvPers;
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00028D50 File Offset: 0x00026F50
	private void Update()
	{
		this.UpdateMeshes();
		this.horizon.transform.position = new Vector3(this.terrainRenderer.cx, 0f, -0.02f);
		if (TerrainRendererScript.unitSize < 16f)
		{
			this.transit.SetActive(false);
			this.perspectiveSurface.SetActive(false);
			return;
		}
		this.transit.SetActive(true);
		this.perspectiveSurface.SetActive(true);
	}

	// Token: 0x0400050C RID: 1292
	public GameObject transit;

	// Token: 0x0400050D RID: 1293
	public GameObject perspectiveSurface;

	// Token: 0x0400050E RID: 1294
	public GameObject horizon;

	// Token: 0x0400050F RID: 1295
	public TerrainRendererScript terrainRenderer;

	// Token: 0x04000510 RID: 1296
	private Mesh TransitMesh;

	// Token: 0x04000511 RID: 1297
	private MeshFilter TransitMeshFilter;

	// Token: 0x04000512 RID: 1298
	private Mesh PerspectiveMesh;

	// Token: 0x04000513 RID: 1299
	private MeshFilter PerspectiveMeshFilter;

	// Token: 0x04000514 RID: 1300
	private Vector2[] _uvTransit;

	// Token: 0x04000515 RID: 1301
	private Vector3[] _verticesTransit;

	// Token: 0x04000516 RID: 1302
	private int[] _triangles;

	// Token: 0x04000517 RID: 1303
	private Vector2[] _uvPers;

	// Token: 0x04000518 RID: 1304
	private Vector2[] _uvPers2;

	// Token: 0x04000519 RID: 1305
	private Vector3[] _verticesPers;

	// Token: 0x0400051A RID: 1306
	private float Z = -0.04f;
}
