using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200003A RID: 58
public class MyPipeline : RenderPipeline
{
	// Token: 0x06000185 RID: 389 RVA: 0x0001B1FC File Offset: 0x000193FC
	protected override void Render(ScriptableRenderContext context, Camera[] cameras)
	{
		foreach (Camera camera in cameras)
		{
			context.SetupCameraProperties(camera, false);
			if (camera.TryGetCullingParameters(out this.cullingParameters))
			{
				this.cullResults = context.Cull(ref this.cullingParameters);
				this.clearFlags = camera.clearFlags;
				this.buffer.ClearRenderTarget((this.clearFlags & CameraClearFlags.Depth) > (CameraClearFlags)0, (this.clearFlags & CameraClearFlags.Color) > (CameraClearFlags)0, camera.backgroundColor);
				SortingSettings sortingSettings = default(SortingSettings);
				sortingSettings.criteria = SortingCriteria.CommonOpaque;
				DrawingSettings drawingSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sortingSettings);
				FilteringSettings filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
				drawingSettings.enableDynamicBatching = true;
				context.DrawRenderers(this.cullResults, ref drawingSettings, ref filteringSettings);
				context.DrawSkybox(camera);
				sortingSettings.criteria = SortingCriteria.CommonTransparent;
				drawingSettings.sortingSettings = sortingSettings;
				filteringSettings.renderQueueRange = RenderQueueRange.transparent;
				context.DrawRenderers(this.cullResults, ref drawingSettings, ref filteringSettings);
				context.ExecuteCommandBuffer(this.buffer);
				this.buffer.Clear();
				context.Submit();
			}
		}
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0001B328 File Offset: 0x00019528
	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	private void DrawDefaultPipeline(ScriptableRenderContext context, Camera camera)
	{
		if (this.errorMaterial == null)
		{
			Shader shader = Shader.Find("Hidden/InternalErrorShader");
			this.errorMaterial = new Material(shader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		SortingSettings sortingSettings = default(SortingSettings);
		sortingSettings.criteria = SortingCriteria.CommonOpaque;
		DrawingSettings drawingSettings = new DrawingSettings(new ShaderTagId("ForwardBase"), sortingSettings);
		drawingSettings.SetShaderPassName(1, new ShaderTagId("PrepassBase"));
		drawingSettings.SetShaderPassName(2, new ShaderTagId("Always"));
		drawingSettings.SetShaderPassName(3, new ShaderTagId("Vertex"));
		drawingSettings.SetShaderPassName(4, new ShaderTagId("VertexLMRGBM"));
		drawingSettings.SetShaderPassName(5, new ShaderTagId("VertexLM"));
		drawingSettings.overrideMaterial = this.errorMaterial;
		FilteringSettings filteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
		context.DrawRenderers(this.cullResults, ref drawingSettings, ref filteringSettings);
	}

	// Token: 0x040002A7 RID: 679
	private Material errorMaterial;

	// Token: 0x040002A8 RID: 680
	private ScriptableCullingParameters cullingParameters;

	// Token: 0x040002A9 RID: 681
	private CameraClearFlags clearFlags;

	// Token: 0x040002AA RID: 682
	private CommandBuffer buffer = new CommandBuffer
	{
		name = "RenderCamera"
	};

	// Token: 0x040002AB RID: 683
	private CullingResults cullResults;
}
