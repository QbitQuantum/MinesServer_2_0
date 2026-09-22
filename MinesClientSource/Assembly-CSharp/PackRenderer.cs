using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class PackRenderer : MonoBehaviour
{
	// Token: 0x060001B2 RID: 434 RVA: 0x0001B8E0 File Offset: 0x00019AE0
	public bool IsPackOn(int x, int y)
	{
		if (ClientController.map == null)
		{
			return false;
		}
		int key = (x >> 5) + (y >> 5) * (ClientController.map.width >> 5);
		if (this.objectsInBlock.ContainsKey(key))
		{
			foreach (GameObject gameObject in this.objectsInBlock[key])
			{
				ObjectModel obj = gameObject.GetComponent<PackSpriteScript>()._Obj;
				if (obj.x == x && obj.y == y)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0001B984 File Offset: 0x00019B84
	public void RemoveObjectInBlock(int blockId)
	{
		if (this.objectsInBlock.ContainsKey(blockId))
		{
			foreach (GameObject obj in this.objectsInBlock[blockId])
			{
				UnityEngine.Object.Destroy(obj);
			}
			this.objectsInBlock[blockId].Clear();
			return;
		}
		this.objectsInBlock[blockId] = new List<GameObject>();
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0001BA0C File Offset: 0x00019C0C
	public void AddObject(ObjectModel obj, int blockId)
	{
		if (!this.objectsInBlock.ContainsKey(blockId))
		{
			this.objectsInBlock[blockId] = new List<GameObject>();
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.packSpritePrefab);
		gameObject.transform.SetParent(this.RenderWrapper.transform, false);
		Vector3 position = gameObject.transform.position;
		position.x = (float)obj.x;
		position.y = -(float)obj.y;
		position.z = -6f;
		gameObject.transform.position = position;
		this.objectsInBlock[blockId].Add(gameObject);
		gameObject.GetComponent<PackSpriteScript>().SetObj(obj);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0001BABC File Offset: 0x00019CBC
	private void ObjectsGarbageCollector()
	{
		new List<int>();
		foreach (KeyValuePair<int, List<GameObject>> keyValuePair in this.objectsInBlock)
		{
			int key = keyValuePair.Key;
			int num = TerrainRendererScript.map.width / 32;
			int num2 = TerrainRendererScript.map.height / 32;
			int num3 = 32 * (key % num);
			int num4 = 32 * Mathf.FloorToInt((float)(key / num));
			float num5 = (float)ClientController.THIS.myBot.gx;
			int gy = ClientController.THIS.myBot.gy;
			float num6 = num5 - (float)num3;
			float num7 = (float)(gy - num4);
			if (num6 * num6 + num7 * num7 > 40000f)
			{
				this.RemoveObjectInBlock(key);
			}
		}
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x00005E5E File Offset: 0x0000405E
	private void Start()
	{
		PackRenderer.THIS = this;
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00005E66 File Offset: 0x00004066
	private void Update()
	{
		if (Time.unscaledTime > this.lastObjectGCTime + 10f)
		{
			this.ObjectsGarbageCollector();
			this.lastObjectGCTime = Time.unscaledTime;
		}
	}

	// Token: 0x040002CE RID: 718
	public GameObject packSpritePrefab;

	// Token: 0x040002CF RID: 719
	private float lastObjectGCTime;

	// Token: 0x040002D0 RID: 720
	private Dictionary<int, List<GameObject>> objectsInBlock = new Dictionary<int, List<GameObject>>();

	// Token: 0x040002D1 RID: 721
	public static PackRenderer THIS;

	// Token: 0x040002D2 RID: 722
	public GameObject RenderWrapper;
}
