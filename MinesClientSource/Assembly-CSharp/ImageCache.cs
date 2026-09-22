using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class ImageCache : MonoBehaviour
{
	// Token: 0x06000121 RID: 289 RVA: 0x00016844 File Offset: 0x00014A44
	private void Start()
	{
		ImageCache.THIS = this;
		WebImage.ImgCache.Add("http://minesgame.ru/img/p_st.png", this.p_st.texture);
		WebImage.ImgCache.Add("http://minesgame.ru/img/p_sn.png", this.p_sn.texture);
		WebImage.ImgCache.Add("http://minesgame.ru/img/p_cr.png", this.p_cr.texture);
		WebImage.ImgCache.Add("http://minesgame.ru/img/p_tr.png", this.p_tr.texture);
		WebImage.ImgCache.Add("http://minesgame.ru/img/p_ss.png", this.p_ss.texture);
		WebImage.ImgCache.Add("http://minesgame.ru/img/p_os.png", this.p_os.texture);
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000210 RID: 528
	public Sprite p_st;

	// Token: 0x04000211 RID: 529
	public Sprite p_sn;

	// Token: 0x04000212 RID: 530
	public Sprite p_cr;

	// Token: 0x04000213 RID: 531
	public Sprite p_tr;

	// Token: 0x04000214 RID: 532
	public Sprite p_ss;

	// Token: 0x04000215 RID: 533
	public Sprite p_os;

	// Token: 0x04000216 RID: 534
	public static ImageCache THIS;
}
