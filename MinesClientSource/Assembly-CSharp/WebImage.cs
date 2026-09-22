using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000074 RID: 116
public class WebImage : MonoBehaviour
{
	// Token: 0x060002DB RID: 731 RVA: 0x00006C31 File Offset: 0x00004E31
	public void SetSizeAndUrl(int w, int h, string url)
	{
		this.w = w;
		this.h = h;
		this.url = url;
		this.loading = true;
		if (this.inited)
		{
			this.UpdateSizeAndUrl();
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00006C5D File Offset: 0x00004E5D
	public int GetHeight()
	{
		return this.h;
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0002D47C File Offset: 0x0002B67C
	private void UpdateSizeAndUrl()
	{
		if (this.url.StartsWith("inner:"))
		{
			string[] array = this.url.Split(new char[]
			{
				':'
			});
			string a = array[1];
			int num = int.Parse(array[2]);
			Sprite[] sprites = InventoryItem.sprites;
			if (!(a == "CLAN"))
			{
				if (!(a == "PACK"))
				{
					if (!(a == "INV"))
					{
						if (!(a == "SKILL"))
						{
							if (!(a == "PROG"))
							{
								if (a == "SKIN")
								{
									sprites = RobotScript.sprites;
								}
							}
							else
							{
								sprites = ProgAction.sprites;
							}
						}
						else
						{
							sprites = SkillButtonScript.sprites;
						}
					}
					else
					{
						sprites = InventoryItem.sprites;
					}
				}
				else
				{
					sprites = PackSpriteScript.sprites;
				}
			}
			else
			{
				sprites = ClanSpriteScript.sprites;
			}
			Sprite sprite = sprites[num];
			this.image.sprite = sprite;
			this.image.SetNativeSize();
			Vector2 sizeDelta = this.image.rectTransform.sizeDelta;
			if (this.w > 0)
			{
				sizeDelta.x = (float)this.w * sizeDelta.x;
				sizeDelta.y = (float)this.w * sizeDelta.y;
			}
			else if (this.w == -1)
			{
				sizeDelta.x = 0.5f * sizeDelta.x;
				sizeDelta.y = 0.5f * sizeDelta.y;
			}
			this.image.rectTransform.sizeDelta = sizeDelta;
			this.loading = false;
			this.image.color = new Color(1f, 1f, 1f, 1f);
			return;
		}
		if (this.url != "")
		{
			this.loading = true;
			this.UpdateSizeAndUrlNoCheck();
			return;
		}
		Debug.Log("no url to load");
	}

    // Token: 0x060002DE RID: 734 RVA: 0x00006C65 File Offset: 0x00004E65
    private IEnumerator LoadImage()
    {
        using (WWW www = new WWW(this.url))
        {
            yield return www;
            this.loading = false;
            this.image.color = new Color(1f, 1f, 1f, 1f);
            Texture2D texture2D = M3Decompressor.M3Decompress(www.bytes);
            if (texture2D != null)
            {
                Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
                sprite.texture.filterMode = FilterMode.Trilinear;
                this.image.sprite = sprite;
                WebImage.ImgCache.Add(this.url, texture2D);
            }
        }
    }

    // Token: 0x060002DF RID: 735 RVA: 0x0002D640 File Offset: 0x0002B840
    private void UpdateSizeAndUrlNoCheck()
	{
		if (!this.off)
		{
			this.image.sprite = this.Loader;
			Vector2 sizeDelta = this.image.rectTransform.sizeDelta;
			sizeDelta.x = (float)this.w;
			sizeDelta.y = (float)this.h;
			this.image.rectTransform.sizeDelta = sizeDelta;
			if (WebImage.ImgCache.ContainsKey(this.url))
			{
				Texture2D texture2D = WebImage.ImgCache[this.url];
				this.loading = false;
				this.image.color = new Color(1f, 1f, 1f, 1f);
				Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
				sprite.texture.filterMode = FilterMode.Trilinear;
				this.image.sprite = sprite;
				return;
			}
			base.StartCoroutine(this.LoadImage());
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00006C74 File Offset: 0x00004E74
	private void Start()
	{
		this.inited = true;
		this.image = base.GetComponent<Image>();
		if (!this.off)
		{
			this.UpdateSizeAndUrl();
		}
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0002D754 File Offset: 0x0002B954
	private void Update()
	{
		if (!this.off && this.loading)
		{
			this.image.color = new Color(1f, 1f, 1f, 0.1f + 0.1f * Mathf.Sin(5f * Time.unscaledTime));
		}
	}

    public static Sprite LoadPNG(string localPath)
    {
        string path = Path.Combine(Application.dataPath, "../" + localPath);
        string url = "file:///" + path;

        using (WWW www = new WWW(url))
        {
            while (!www.isDone) { }

            if (string.IsNullOrEmpty(www.error))
            {
                Texture2D tex = www.texture;
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }
        return null;
    }

    // Token: 0x04000570 RID: 1392
    public Sprite Loader;

	// Token: 0x04000571 RID: 1393
	private bool inited;

	// Token: 0x04000572 RID: 1394
	public static bool cacheInited = false;

	// Token: 0x04000573 RID: 1395
	public static Dictionary<string, Texture2D> ImgCache = new Dictionary<string, Texture2D>();

	// Token: 0x04000574 RID: 1396
	public string url = "";

	// Token: 0x04000575 RID: 1397
	public int w;

	// Token: 0x04000576 RID: 1398
	public int h;

	// Token: 0x04000577 RID: 1399
	private bool loading = true;

	// Token: 0x04000578 RID: 1400
	public bool off;

	// Token: 0x04000579 RID: 1401
	private Image image;
}
