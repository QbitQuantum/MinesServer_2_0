using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class StatePanel : MonoBehaviour
{
	// Token: 0x0600029E RID: 670 RVA: 0x000285DC File Offset: 0x000267DC
	public void RemoveAll()
	{
		while (this.lines.Count > 0)
		{
			using (Dictionary<string, StateLineScript>.Enumerator enumerator = this.lines.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<string, StateLineScript> keyValuePair = enumerator.Current;
					this.RemoveLine(keyValuePair.Key);
				}
			}
		}
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00006933 File Offset: 0x00004B33
	public void RemoveLine(string tag)
	{
		if (this.lines.ContainsKey(tag))
		{
			UnityEngine.Object gameObject = this.lines[tag].gameObject;
			this.lines.Remove(tag);
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00028644 File Offset: 0x00026844
	public void AddLine(string tag, string[] text, bool blinking, Color color)
	{
		if (this.lines.ContainsKey(tag))
		{
			this.lines[tag].SetLine(text, blinking, color);
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.stateLinePrefab);
		gameObject.GetComponent<StateLineScript>().SetLine(text, blinking, color);
		this.lines.Add(tag, gameObject.GetComponent<StateLineScript>());
		if (this.inited)
		{
			gameObject.transform.SetParent(base.gameObject.transform, false);
		}
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00006966 File Offset: 0x00004B66
	private void Start()
	{
		this.UpdateListFull();
		this.inited = true;
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x000286C4 File Offset: 0x000268C4
	private void UpdateListFull()
	{
		foreach (object obj in base.gameObject.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		foreach (KeyValuePair<string, StateLineScript> keyValuePair in this.lines)
		{
			keyValuePair.Value.gameObject.transform.SetParent(base.gameObject.transform, false);
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000503 RID: 1283
	public GameObject stateLinePrefab;

	// Token: 0x04000504 RID: 1284
	private Dictionary<string, StateLineScript> lines = new Dictionary<string, StateLineScript>();

	// Token: 0x04000505 RID: 1285
	private bool inited;
}
