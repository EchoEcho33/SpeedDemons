using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class GradientFogRenderer : ScriptableRendererFeature
{
    [SerializeField] GradientFogRendererSettings settings;
    GradientFogRendererPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new GradientFogRendererPass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

        // You can request URP color texture and depth buffer as inputs by uncommenting the line below,
        // URP will ensure copies of these resources are available for sampling before executing the render pass.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        // m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Depth);

        // You can request URP to render to an intermediate texture by uncommenting the line below.
        // Use this option for passes that do not support rendering directly to the backbuffer.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        //m_ScriptablePass.requiresIntermediateTexture = true;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    // Use this class to pass around settings from the feature to the pass
    [Serializable]
    public class GradientFogRendererSettings
    {
        public Shader ShaderFogGraph;
        
        public float StartDistance = 0;
        public float EndDistance = 100;
        public Color NearColor = new Color(0.23f, 0f, 0.35f, 1);
        public Color MiddleColor = new Color(1f, 0.64f, .62f, 1);
        public Color FarColor = new Color(0.98f, 1f, .85f, 1);

        public float MaxOpacity = 0.5f;
    }

    class GradientFogRendererPass : ScriptableRenderPass
    {
        readonly GradientFogRendererSettings settings;
        
        private Material fogMaterial;

        public GradientFogRendererPass(GradientFogRendererSettings settings)
        {
            this.settings = settings;
            
            if (settings.ShaderFogGraph == null) return;
            fogMaterial = new Material(settings.ShaderFogGraph);
        }

        private void SetupMaterial()
        {
            fogMaterial.SetFloat("_StartDistance", settings.StartDistance);
            fogMaterial.SetFloat("_EndDistance", settings.EndDistance);
            fogMaterial.SetColor("_NearColor", settings.NearColor);
            fogMaterial.SetColor("_MidColor", settings.MiddleColor);
            fogMaterial.SetColor("_FarColor", settings.FarColor);
            fogMaterial.SetFloat("_MaxOpacity", settings.MaxOpacity);
        }

        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
            
        }

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (fogMaterial == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            TextureHandle source = resourceData.activeColorTexture;
            if (!source.IsValid()) return;
            
            // Create destination texture with same properties as the camera color texture
            TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "Gradient Fog";
            destinationDesc.clearBuffer = false;
            destinationDesc.depthBufferBits = 0;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
            
            SetupMaterial();
            
            // Config a fullscreen material operation for the gradient fog
            var blitParams = new RenderGraphUtils.BlitMaterialParameters(source, destination, fogMaterial, 0);
            renderGraph.AddBlitPass(blitParams, "Gradient Fog");
            
            resourceData.cameraColor = destination;
        }
        
        public void Dispose()
        {
            if (fogMaterial != null)
            {
                CoreUtils.Destroy(fogMaterial);
                fogMaterial = null; }
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
        m_ScriptablePass = null;
    }
}
