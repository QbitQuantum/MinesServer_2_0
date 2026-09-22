using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000034 RID: 52
public class MiniSkillManager : MonoBehaviour
{
	// Token: 0x06000170 RID: 368 RVA: 0x00005B09 File Offset: 0x00003D09
	private void Start()
	{
		MiniSkillManager.THIS = this;
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0001ABD8 File Offset: 0x00018DD8
	public void AddIcon(int progress, string code)
	{
		if (MiniSkillScript.minis.ContainsKey(code))
		{
			MiniSkillScript.minis[code].SetModel(progress, code);
			MiniSkillScript.minis[code].InitGfx();
			return;
		}
		Image image = UnityEngine.Object.Instantiate<Image>(this.miniPrefab);
		image.transform.SetParent(base.gameObject.transform, false);
		MiniSkillScript.minis.Add(code, image.GetComponent<MiniSkillScript>());
		MiniSkillScript.minis[code].SetModel(progress, code);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x0400028F RID: 655
	public static MiniSkillManager THIS;

	// Token: 0x04000290 RID: 656
	public Image miniPrefab;
}
