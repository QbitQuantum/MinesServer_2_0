using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000028 RID: 40
public class InventoryPanel : MonoBehaviour
{
	// Token: 0x0600012A RID: 298 RVA: 0x00005869 File Offset: 0x00003A69
	private void Start()
	{
		InventoryPanel.THIS = this;
		this.button.GetComponent<Button>().onClick.AddListener(delegate()
		{
			ClientController.CanGoto = false;
			ServerTime.THIS.SendTypicalMessage(-1, "INVN", 0, 0, "_");
		});
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00016CF0 File Offset: 0x00014EF0
	private void removeAll()
	{
		foreach (object obj in this.inventoryGrid.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00016D50 File Offset: 0x00014F50
	private void makeGrid(string[] parts)
	{
		for (int i = 0; i < parts.Length; i += 2)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.inventoryItemPrefab);
			gameObject.transform.SetParent(this.inventoryGrid.transform, false);
			int _type = int.Parse(parts[i]);
			int num = int.Parse(parts[i + 1]);
			gameObject.GetComponent<InventoryItem>().Setup(_type, num, this._selected == _type, "", "");
			if (num > 0)
			{
				gameObject.GetComponent<Button>().onClick.AddListener(delegate()
				{
					ClientController.CanGoto = false;
					ServerTime.THIS.SendTypicalMessage(-1, "INCL", 0, 0, _type.ToString());
				});
			}
		}
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00016E00 File Offset: 0x00015000
	public void ShowFullGrid(string grid, int selected)
	{
		Vector3 localScale = this.triangle.transform.localScale;
		localScale.x = -1f;
		this.triangle.transform.localScale = localScale;
		this.removeAll();
		this._selected = selected;
		if (grid == "")
		{
			this.button.SetActive(false);
			return;
		}
		string[] array = grid.Split(new char[]
		{
			'#'
		});
		this.makeGrid(array);
		if (array.Length < 10)
		{
			this.button.SetActive(false);
			return;
		}
		this.button.SetActive(true);
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00016E9C File Offset: 0x0001509C
	public void ShowInventory(string grid, int selected, int all)
	{
		Vector3 localScale = this.triangle.transform.localScale;
		localScale.x = 1f;
		this.triangle.transform.localScale = localScale;
		this.removeAll();
		this._selected = selected;
		if (grid == "")
		{
			this.button.SetActive(false);
			return;
		}
		string[] parts = grid.Split(new char[]
		{
			'#'
		});
		this.makeGrid(parts);
		if (all < 5)
		{
			this.button.SetActive(false);
			return;
		}
		this.button.SetActive(true);
	}

	// Token: 0x04000223 RID: 547
	public GameObject inventoryItemPrefab;

	// Token: 0x04000224 RID: 548
	public GameObject button;

	// Token: 0x04000225 RID: 549
	public GameObject inventoryGrid;

	// Token: 0x04000226 RID: 550
	public Image triangle;

	// Token: 0x04000227 RID: 551
	public static InventoryPanel THIS;

	// Token: 0x04000228 RID: 552
	private int _selected = -1;
}
