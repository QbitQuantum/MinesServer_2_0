using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class GunShotScript : MonoBehaviour
{
	// Token: 0x0600011B RID: 283 RVA: 0x00015EBC File Offset: 0x000140BC
	public void Setup(int gunx, int guny, int bid, int col, int _index)
	{
		this.gunPos = new Vector2((float)gunx, -(float)guny);
		this.index = _index;
		this.startTime = Time.unscaledTime;
		this.botId = bid;
		this.col = col;
		if (RobotRenderer.THIS.bots.ContainsKey(this.botId))
		{
			this.botPos = RobotRenderer.THIS.bots[this.botId].transform.position;
		}
		else
		{
			this.botId = -1;
		}
		if (this._tailx != null)
		{
			for (int i = 1; i < this.NUM_SECTORS; i++)
			{
				float d = (float)i / (float)this.NUM_SECTORS;
				this._tailx[i] = d * (this.botPos - this.gunPos);
			}
		}
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00015F90 File Offset: 0x00014190
	private void Start()
	{
		this.startTime = Time.unscaledTime;
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		component.mesh = this.mesh;
		this._triangles = new int[6 * this.NUM_SECTORS];
		this._vertices = new Vector3[4 * this.NUM_SECTORS];
		this._uvs = new Vector2[4 * this.NUM_SECTORS];
		this._colors = new Color[4 * this.NUM_SECTORS];
		this._tailx = new Vector2[this.NUM_SECTORS + 1];
		for (int i = 0; i < this.NUM_SECTORS; i++)
		{
			this._triangles[6 * i] = 4 * i;
			this._triangles[6 * i + 1] = 4 * i + 1;
			this._triangles[6 * i + 2] = 4 * i + 2;
			this._triangles[6 * i + 3] = 4 * i + 1;
			this._triangles[6 * i + 4] = 4 * i + 3;
			this._triangles[6 * i + 5] = 4 * i + 2;
			this._uvs[4 * i] = new Vector2(0f, 0f);
			this._uvs[4 * i + 1] = new Vector2(0f, 1f);
			this._uvs[4 * i + 2] = new Vector2(1f, 0f);
			this._uvs[4 * i + 3] = new Vector2(1f, 1f);
		}
		this.mesh.vertices = this._vertices;
		this.mesh.colors = this._colors;
		this.mesh.triangles = this._triangles;
		this.mesh.uv = this._uvs;
		this._tailx[0] = this.gunPos - this.gunPos;
		this._tailx[this.NUM_SECTORS] = this.botPos - this.gunPos;
		for (int j = 1; j < this.NUM_SECTORS; j++)
		{
			float d = (float)j / (float)this.NUM_SECTORS;
			this._tailx[j] = d * (this.botPos - this.gunPos);
		}
	}

	// Token: 0x0600011D RID: 285 RVA: 0x000161E8 File Offset: 0x000143E8
	private void Update()
	{
		if (RobotRenderer.THIS.bots.ContainsKey(this.botId))
		{
			this.botPos = RobotRenderer.THIS.bots[this.botId].transform.position;
		}
		this.UpdateMesh();
		if (Time.unscaledTime - this.startTime > 0.7f)
		{
			ClientController.THIS.gunShotPool.Free(this.index);
		}
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00016264 File Offset: 0x00014464
	private void UpdateMesh()
	{
		base.transform.position = new Vector3(this.gunPos.x, this.gunPos.y, -8f);
		this._tailx[0] = new Vector2(0.5f, -0.5f);
		this._tailx[this.NUM_SECTORS] = this.botPos - this.gunPos;
		for (int i = 1; i < this.NUM_SECTORS; i++)
		{
			float d = (float)i / (float)this.NUM_SECTORS;
			Vector2[] tailx = this._tailx;
			int num = i;
			tailx[num].x = tailx[num].x + (UnityEngine.Random.value - 0.5f);
			Vector2[] tailx2 = this._tailx;
			int num2 = i;
			tailx2[num2].y = tailx2[num2].y + (UnityEngine.Random.value - 0.5f);
			this._tailx[i] = 0.9f * this._tailx[i] + 0.1f * (d * (this.botPos - this.gunPos));
		}
		float d2 = 0.5f * Mathf.Sin(7f * (Time.unscaledTime - this.startTime));
		for (int j = 0; j < this.NUM_SECTORS; j++)
		{
			Vector2 vector2;
			Vector2 vector = vector2 = d2 * (this._tailx[j + 1] - this._tailx[j]).normalized;
			if (j > 0)
			{
				vector2 = d2 * (this._tailx[j + 1] - this._tailx[j - 1]).normalized;
			}
			if (j < this.NUM_SECTORS - 1)
			{
				vector = d2 * (this._tailx[j + 2] - this._tailx[j]).normalized;
			}
			this._vertices[4 * j] = new Vector3(this._tailx[j].x + vector2.y, this._tailx[j].y - vector2.x, -1f);
			this._vertices[4 * j + 1] = new Vector3(this._tailx[j].x - vector2.y, this._tailx[j].y + vector2.x, -1f);
			this._vertices[4 * j + 2] = new Vector3(this._tailx[j + 1].x + vector.y, this._tailx[j + 1].y - vector.x, -1f);
			this._vertices[4 * j + 3] = new Vector3(this._tailx[j + 1].x - vector.y, this._tailx[j + 1].y + vector.x, -1f);
			if (this.col == 1)
			{
				this._colors[4 * j] = new Color(1f, 0f, 0f, 0f);
				this._colors[4 * j + 1] = new Color(1f, 0f, 0f, 0f);
				this._colors[4 * j + 2] = new Color(1f, 0f, 0f, 0f);
				this._colors[4 * j + 3] = new Color(1f, 0f, 0f, 0f);
			}
			else
			{
				this._colors[4 * j] = new Color(0f, 1f, 1f, 1f);
				this._colors[4 * j + 1] = new Color(0f, 1f, 1f, 1f);
				this._colors[4 * j + 2] = new Color(0f, 1f, 1f, 1f);
				this._colors[4 * j + 3] = new Color(0f, 1f, 1f, 1f);
			}
			float num3 = UnityEngine.Random.value * 0.9f;
			this._uvs[4 * j] = new Vector2(num3, 0f);
			this._uvs[4 * j + 1] = new Vector2(num3, 1f);
			this._uvs[4 * j + 2] = new Vector2(num3 + 0.1f, 0f);
			this._uvs[4 * j + 3] = new Vector2(num3 + 0.1f, 1f);
		}
		if (this.botId != -1)
		{
			this.boundsCenter = new Vector3(0f, 0f, 0f);
			this.boundsSize = new Vector3(44f, 44f, 44f);
			this.mesh.bounds = new Bounds(this.boundsCenter, this.boundsSize);
			this.mesh.vertices = this._vertices;
			this.mesh.colors = this._colors;
			this.mesh.uv = this._uvs;
		}
	}

	// Token: 0x040001EB RID: 491
	private Mesh mesh;

	// Token: 0x040001EC RID: 492
	private Vector3[] _vertices;

	// Token: 0x040001ED RID: 493
	private Vector2[] _uvs;

	// Token: 0x040001EE RID: 494
	private Color[] _colors;

	// Token: 0x040001EF RID: 495
	private int[] _triangles;

	// Token: 0x040001F0 RID: 496
	private Vector2[] _tailx;

	// Token: 0x040001F1 RID: 497
	private Vector2 gunPos;

	// Token: 0x040001F2 RID: 498
	private Vector2 botPos;

	// Token: 0x040001F3 RID: 499
	private int botId;

	// Token: 0x040001F4 RID: 500
	private Vector3 boundsCenter;

	// Token: 0x040001F5 RID: 501
	private Vector3 boundsSize;

	// Token: 0x040001F6 RID: 502
	private int col;

	// Token: 0x040001F7 RID: 503
	private int index;

	// Token: 0x040001F8 RID: 504
	private int NUM_SECTORS = 7;

	// Token: 0x040001F9 RID: 505
	private float startTime;
}
