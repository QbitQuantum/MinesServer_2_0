using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200003B RID: 59
[CreateAssetMenu(menuName = "Rendering/My Pipeline")]
public class MyPipelineAsset : RenderPipelineAsset
{
	// Token: 0x06000188 RID: 392 RVA: 0x00005C02 File Offset: 0x00003E02
	protected override RenderPipeline CreatePipeline()
	{
		return new MyPipeline();
	}
}
