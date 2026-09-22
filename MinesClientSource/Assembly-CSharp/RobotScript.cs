using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class RobotScript : MonoBehaviour
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000240 RID: 576 RVA: 0x00006578 File Offset: 0x00004778
	public int gx
	{
		get
		{
			return this._gx;
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000241 RID: 577 RVA: 0x00006580 File Offset: 0x00004780
	public int gy
	{
		get
		{
			return this._gy;
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00024AE8 File Offset: 0x00022CE8
	private void Start()
	{
		if (!RobotScript.inited)
		{
			RobotScript.sprites = ResourcesManager.LoadAll<Sprite>("skins");
			RobotScript.inited = true;
		}
		this.deathPingTime = -1f;
		this.body.GetComponent<SpriteRenderer>().sprite = RobotScript.sprites[0];
		MeshFilter component = this.tail.GetComponent<MeshFilter>();
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		component.mesh = this.mesh;
		this.boundsSize.x = 90f;
		this.boundsSize.y = 90f;
		this.boundsSize.z = 90f;
		this._triangles = new int[3 * this.NUM_TAILS * this.NUM_SECTORS + 3 * this.NUM_TAILS];
		this._vertices = new Vector3[4 * this.NUM_TAILS * this.NUM_SECTORS];
		this._uvs = new Vector2[4 * this.NUM_TAILS * this.NUM_SECTORS];
		this._colors = new Color[4 * this.NUM_TAILS * this.NUM_SECTORS];
		this._tailx = new Vector2[this.NUM_TAILS * this.NUM_SECTORS];
		this._tailxs = new Vector2[this.NUM_TAILS * this.NUM_SECTORS];
		for (int i = 0; i < this.NUM_TAILS; i++)
		{
			for (int j = 0; j < this.NUM_SECTORS; j++)
			{
				this._triangles[3 * (j + i * this.NUM_SECTORS)] = 4 * (j + i * this.NUM_SECTORS);
				this._triangles[3 * (j + i * this.NUM_SECTORS) + 1] = 4 * (j + i * this.NUM_SECTORS) + 1;
				this._triangles[3 * (j + i * this.NUM_SECTORS) + 2] = 4 * (j + i * this.NUM_SECTORS) + 2;
				if (j == 0)
				{
					this._triangles[3 * this.NUM_SECTORS * this.NUM_TAILS + 3 * i] = 4 * (j + i * this.NUM_SECTORS) + 1;
					this._triangles[3 * this.NUM_SECTORS * this.NUM_TAILS + 1 + 3 * i] = 4 * (j + i * this.NUM_SECTORS) + 3;
					this._triangles[3 * this.NUM_SECTORS * this.NUM_TAILS + 2 + 3 * i] = 4 * (j + i * this.NUM_SECTORS) + 2;
				}
				this._uvs[4 * (j + i * this.NUM_SECTORS)] = new Vector2(0f, 0f);
				this._uvs[4 * (j + i * this.NUM_SECTORS) + 1] = new Vector2(1f, 0f);
				this._uvs[4 * (j + i * this.NUM_SECTORS) + 2] = new Vector2(0f, 1f);
				this._uvs[4 * (j + i * this.NUM_SECTORS) + 3] = new Vector2(1f, 1f);
			}
		}
		this.mesh.vertices = this._vertices;
		this.mesh.colors = this._colors;
		this.mesh.triangles = this._triangles;
		this.mesh.uv = this._uvs;
		this.HideTail();
		this.lastPingTime = Time.unscaledTime;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00006588 File Offset: 0x00004788
	public void SetClan(int _cid)
	{
		this.clan.changeClanToId(_cid);
	}

	// Token: 0x06000244 RID: 580 RVA: 0x00006596 File Offset: 0x00004796
	public void SetSkin(int _skin)
	{
        this.skin = _skin;
        if (this._tailx != null && this.skin == 1)
		{
			this.HideTail();
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x00024E38 File Offset: 0x00023038
	public void HideTail()
	{
		if (this._tailx != null)
		{
			for (int i = 0; i < this.NUM_TAILS; i++)
			{
				for (int j = 0; j < this.NUM_SECTORS; j++)
				{
					this._tailx[j + i * this.NUM_SECTORS] = base.gameObject.transform.position;
					this._tailxs[j + i * this.NUM_SECTORS] = base.gameObject.transform.position;
				}
			}
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00024EC4 File Offset: 0x000230C4
	private void Update()
	{
		this.PositionUpdate();
		this.BodyUpdate();
		if (RobotRenderer.THIS.bots.Count > 60 || this.skin == 1)
		{
			this.tail.SetActive(false);
			return;
		}
		this.tail.SetActive(true);
		this.TailUpdate();
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00024F18 File Offset: 0x00023118
	private void PositionUpdate()
	{
		Vector3 position = base.gameObject.transform.position;
		this.renderDistance = Vector2.Distance(position, this.gamePosition);
		float num = 0.9f - this.renderDistance * 0.01f;
		if (num < 0.5f)
		{
			num = 0.5f;
		}
		if (this.renderDistance > 28f)
		{
			this.SyncXY();
			this.HideTail();
		}
		float num2 = 1f - num;
		position.x = num * position.x + num2 * this.gamePosition.x;
		position.y = num * position.y + num2 * this.gamePosition.y;
		position.z = -this.layerZ;
		if (this.tremor > 0.01f)
		{
			this.tremor *= 0.8f;
			position.x += this.tremor * (UnityEngine.Random.value - 0.5f);
			position.y += this.tremor * (UnityEngine.Random.value - 0.5f);
		}
		base.gameObject.transform.position = position;
		Quaternion rotation = this.body.transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		this.nowRotationAngle = eulerAngles.z;
		if (this.nowRotationAngle - this.rotationAngle > 180f)
		{
			this.rotationAngle += 360f;
		}
		if (this.nowRotationAngle - this.rotationAngle < -180f)
		{
			this.rotationAngle -= 360f;
		}
		float num3 = Vector2.Distance(position, this.gamePosition);
		float num4 = 12f * Time.unscaledDeltaTime;
		this.nowRotationAngle = (1f - num4) * this.nowRotationAngle + num4 * this.rotationAngle;
		if (this.skin != 1)
		{
			this.nowRotationAngle += 6.6f * num3 * (0.5f - UnityEngine.Random.value);
		}
		eulerAngles.z = this.nowRotationAngle;
		rotation.eulerAngles = eulerAngles;
		this.body.transform.rotation = rotation;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00025144 File Offset: 0x00023344
	public void SyncXY()
	{
		Vector3 position = base.gameObject.transform.position;
		position.x = this.gamePosition.x;
		position.y = this.gamePosition.y;
		base.gameObject.transform.position = position;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x000065B6 File Offset: 0x000047B6
	public void SetRotation(int dir)
	{
		this.dir = dir;
		this.SetRotationDegrees((float)(-90 * dir + 180));
	}

	// Token: 0x0600024A RID: 586 RVA: 0x000065D0 File Offset: 0x000047D0
	public void SetRotationDegrees(float degrees)
	{
		this.rotationAngle = degrees;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x000065D9 File Offset: 0x000047D9
	public void SetXY(float x, float y)
	{
		this.gamePosition = new Vector2(x + 0.5f, -y - 0.5f);
		this._gx = (int)x;
		this._gy = (int)y;
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00025198 File Offset: 0x00023398
	private void BodyUpdate()
	{
		this.body.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		int num = this.skin;
		if (num != 1)
		{
			if (num == 2)
			{
				this.body.GetComponent<SpriteRenderer>().sprite = RobotScript.sprites[3];
				this.layerZ = 2f;
				return;
			}
			this.body.GetComponent<SpriteRenderer>().sprite = RobotScript.sprites[this.skin];
			this.layerZ = 2f;
			return;
		}
		else
		{
			this.layerZ = 1f;
			this.body.GetComponent<SpriteRenderer>().sprite = RobotScript.sprites[1];
			if (UnityEngine.Random.value < 0.01f)
			{
				this.body.GetComponent<SpriteRenderer>().sprite = RobotScript.sprites[2];
			}
			if (!CellModel.isEmpty[TerrainRendererScript.map.GetCell(this.gx, this.gy)])
			{
				this.body.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
				return;
			}
			return;
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x000252B8 File Offset: 0x000234B8
	private void UpdateTailVertices(int t, int s, Vector2 from, Vector2 to)
	{
		Vector2 vector = to - from;
		Vector2 vector2 = new Vector2(-vector.y, vector.x);
		vector2.Normalize();
		vector2 /= 16f;
		this._vertices[4 * (s + t * this.NUM_SECTORS)] = from + vector2 - this.gamePosCache;
		this._vertices[4 * (s + t * this.NUM_SECTORS) + 1] = to + vector2 - this.gamePosCache;
		this._vertices[4 * (s + t * this.NUM_SECTORS) + 2] = from - vector2 - this.gamePosCache;
		this._vertices[4 * (s + t * this.NUM_SECTORS) + 3] = to - vector2 - this.gamePosCache;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x000253B8 File Offset: 0x000235B8
	private void TailUpdate()
	{
		if (Time.unscaledTime > this.lastTailUpdateTime + 0.016666668f)
		{
			this.lastTailUpdateTime = Time.unscaledTime;
			this.gamePosCache = base.gameObject.transform.position;
			float num = Vector2.Distance(this._tailxs[this.NUM_SECTORS - 1], this.gamePosCache);
			for (int i = 0; i < this.NUM_SECTORS; i++)
			{
				for (int j = 0; j < this.NUM_TAILS; j++)
				{
					float num2 = 0.35f + 800f / (2000f + (16f + 2f * (float)j) * num * num);
					if (num > 10f)
					{
						num2 = 0.2f;
					}
					if (num > 20f)
					{
						num2 = 0.1f;
					}
					float num3 = 1f - num2;
					this.pOut = num2;
					this.color.a = 1f;
					if (i == 0)
					{
						this._tailx[i + j * this.NUM_SECTORS] = num2 * num2 * num2 * this._tailx[i + j * this.NUM_SECTORS] + (1f - num2 * num2 * num2) * this.gamePosCache;
						this._tailx[i + j * this.NUM_SECTORS] += num3 * 2.5f * new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
					}
					else
					{
						this._tailx[i + j * this.NUM_SECTORS] += (num3 - 0.15f) * 3.5f * new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
						this._tailx[i + j * this.NUM_SECTORS] = num2 * this._tailx[i + j * this.NUM_SECTORS] + num3 * this._tailx[i + j * this.NUM_SECTORS - 1];
					}
					this._tailxs[i + j * this.NUM_SECTORS] = num2 * this._tailxs[i + j * this.NUM_SECTORS] + num3 * this._tailx[i + j * this.NUM_SECTORS];
					if (i == 0)
					{
						this.UpdateTailVertices(j, i, this.gamePosCache, this._tailxs[i + j * this.NUM_SECTORS]);
					}
					else
					{
						this.UpdateTailVertices(j, i, this._tailxs[i - 1 + j * this.NUM_SECTORS], this._tailxs[i + j * this.NUM_SECTORS]);
					}
					this._colors[4 * (i + j * this.NUM_SECTORS)] = this.color;
					this._colors[4 * (i + j * this.NUM_SECTORS) + 1] = this.color;
					this._colors[4 * (i + j * this.NUM_SECTORS) + 2] = this.color;
					this._colors[4 * (i + j * this.NUM_SECTORS) + 3] = this.color;
				}
			}
			this.mesh.vertices = this._vertices;
			this.mesh.colors = this._colors;
		}
	}

	// Token: 0x0400043E RID: 1086
	public ClanSpriteScript clan;

	// Token: 0x0400043F RID: 1087
	public GameObject body;

	// Token: 0x04000440 RID: 1088
	public GameObject tail;

	// Token: 0x04000441 RID: 1089
	public static Sprite[] sprites;

	// Token: 0x04000442 RID: 1090
	public static bool inited;

	// Token: 0x04000443 RID: 1091
	public int id;

	// Token: 0x04000444 RID: 1092
	private int _bodyType;

	// Token: 0x04000445 RID: 1093
	private int _tailType;

	// Token: 0x04000446 RID: 1094
	private Mesh mesh;

	// Token: 0x04000447 RID: 1095
	private Vector3[] _vertices;

	// Token: 0x04000448 RID: 1096
	private Vector2[] _uvs;

	// Token: 0x04000449 RID: 1097
	private Color[] _colors;

	// Token: 0x0400044A RID: 1098
	private int[] _triangles;

	// Token: 0x0400044B RID: 1099
	private Vector2[] _tailx;

	// Token: 0x0400044C RID: 1100
	private Vector2[] _tailxs;

	// Token: 0x0400044D RID: 1101
	private int NUM_TAILS = 4;

	// Token: 0x0400044E RID: 1102
	private int NUM_SECTORS = 4;

	// Token: 0x0400044F RID: 1103
	public float lastPingTime;

	// Token: 0x04000450 RID: 1104
	public float deathPingTime = -1f;

	// Token: 0x04000451 RID: 1105
	public float tremor;

	// Token: 0x04000452 RID: 1106
	private int _gx;

	// Token: 0x04000453 RID: 1107
	private int _gy;

	// Token: 0x04000454 RID: 1108
	private Vector2 gamePosition;

	// Token: 0x04000455 RID: 1109
	private float rotationAngle;

	// Token: 0x04000456 RID: 1110
	private float nowRotationAngle;

	// Token: 0x04000457 RID: 1111
	private int skin;

	// Token: 0x04000458 RID: 1112
	private float layerZ;

	// Token: 0x04000459 RID: 1113
	public float renderDistance;

	// Token: 0x0400045A RID: 1114
	public int dir = 2;

	// Token: 0x0400045B RID: 1115
	private float lastTailUpdateTime;

	// Token: 0x0400045C RID: 1116
	private Vector2 gamePosCache;

	// Token: 0x0400045D RID: 1117
	private Bounds bounds;

	// Token: 0x0400045E RID: 1118
	private Vector3 boundsCenter;

	// Token: 0x0400045F RID: 1119
	private Vector3 boundsSize;

	// Token: 0x04000460 RID: 1120
	private Color color = new Color(0.6f, 0.4f, 0.2f, 1f);

	// Token: 0x04000461 RID: 1121
	public float pOut;
}
