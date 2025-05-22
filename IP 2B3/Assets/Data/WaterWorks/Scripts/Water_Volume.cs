using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;

        private Material _material;

        private RTHandle tempRenderTarget;

        public CustomRenderPass(Material mat)
        {
            _material = mat;

            // Criação do RTHandle para os render targets temporários
            tempRenderTarget = RTHandles.Alloc("_TemporaryColourTexture", name: "_TemporaryColourTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get();

                // Configurar o Blit para usar RTHandles
                Blitter.BlitCameraTexture(commandBuffer, source, tempRenderTarget, _material, 0);
                Blitter.BlitCameraTexture(commandBuffer, tempRenderTarget, source);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Limpeza dos RTHandles
            if (tempRenderTarget != null)
            {
                tempRenderTarget.Release();
            }
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);

        // Configurar o evento de renderização
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Usar cameraColorTargetHandle em vez de cameraColorTarget
        m_ScriptablePass.source = renderer.cameraColorTargetHandle;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
