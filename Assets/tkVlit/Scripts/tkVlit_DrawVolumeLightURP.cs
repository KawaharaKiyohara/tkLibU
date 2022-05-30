

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace tkLibU
{
    /// <summary>
    /// URPでのボリュームライト描画処理。
    /// </summary>    
    public class tkVlit_DrawVolumeLightURP : ScriptableRenderPass
    {
        static tkVlit_DrawVolumeLightURP m_instance;    // URPのインスタンス
        static public tkVlit_DrawVolumeLightURP instance { get => m_instance; }
        tkVlit_DrawVolumeLightCommon m_drawVolumeLightCommon;   // ボリュームライトの描画処理の共通処理。
        RenderTargetIdentifier m_cameraRenderTargetID;          // カメラのレンダリング先のテクスチャのID
        public RenderTargetIdentifier cameraRenderTargetID { set => m_cameraRenderTargetID = value; }
        bool m_isInited = false;
        public void AddSpotLight(tkVlit_SpotLight spotLight)
        {
            m_drawVolumeLightCommon?.AddSpotLight(spotLight);
        }
        public void RemoveSpotLight(tkVlit_SpotLight spotLight)
        {
            m_drawVolumeLightCommon?.RemoveSpotLight(spotLight);
        }
        public void Releae()
        {
            m_drawVolumeLightCommon?.Release();
        }
        public tkVlit_DrawVolumeLightURP()
        {
            Debug.Log("Called tkVlit_DrawVolumeLightURP()");
            
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            if (!m_isInited)
            {
                var cameraGo = GameObject.Find("Main Camera");
                if (cameraGo == null)
                {
                    return;
                }
                var camera = cameraGo.GetComponent<Camera>();
                // フレームバッファに直接描画すると描画結果が異なることがあるので、
                // 必ず一時テクスチャに描画するようにする。
                camera.forceIntoRenderTexture = true;
                m_drawVolumeLightCommon = new tkVlit_DrawVolumeLightCommon(camera);
                m_instance = this;
                m_isInited = true;
            }
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {            
            var commandBuffer = m_drawVolumeLightCommon.Draw(m_cameraRenderTargetID);
            if (commandBuffer != null)
            {
                context.ExecuteCommandBuffer(commandBuffer);
            }
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            m_drawVolumeLightCommon.EndDraw();
        }
        
    }
}