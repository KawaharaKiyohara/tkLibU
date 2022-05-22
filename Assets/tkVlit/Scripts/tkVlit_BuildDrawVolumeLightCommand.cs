using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VolumeLight
{
    /// <summary>
    /// �{�����[�����C�g�`�悽�߂̃R�}���h���\�z���鏈���B
    /// </summary>
    public class tkVlit_BuildDrawVolumeLightCommand
    {
        public static void Build(
            CommandBuffer commandBuffer,
            tkVlit_RenderTextures renderTextures,
            List<Material> drawBackFaceMaterialList,
            List<Material> drawFrontFaceMaterialList,
            List<MeshFilter> drawBackMeshFilterList,
            List<MeshFilter> drawFrontMeshFilterList,
            List<tkVlit_DrawFinal> drawFinalList,
            List<tkVlit_SpotLight> volumeSpotLightList,
            tkLibU_AddCopyFullScreen addCopyFullScreen,
            Camera camera,
            RenderTargetIdentifier cameraRenderTargetID
        )
        {
            // �{�����[�����C�g�̍ŏI�`��Ń��C���V�[���̕`�挋�ʂ̃e�N�X�`���𗘗p�������̂����A
            // �����_�����O�^�[�Q�b�g�Ƃ��Ďw�肳��Ă���e�N�X�`����ǂݍ��݂ŗ��p���邱�Ƃ͂ł��Ȃ��̂ŁA
            // �ꎞ�I�ȃ����_�����O�^�[�Q�b�g���擾���Ă����ɃR�s�[����B
            int cameraTargetTextureID = Shader.PropertyToID("cameraTargetTexture");
            commandBuffer.GetTemporaryRT(
                cameraTargetTextureID,
                camera.pixelWidth,
                camera.pixelHeight,
                /*depth=*/0,
                FilterMode.Bilinear
            );
            commandBuffer.Blit(cameraRenderTargetID, cameraTargetTextureID);
            commandBuffer.SetRenderTarget(renderTextures.finalTexture);
            commandBuffer.ClearRenderTarget(
                /*clearDepth=*/true,
                /*clearColor=*/true,
                Color.black
            );

            // �S�ĂɃ{�����[�����C�g��`�悵�Ă����B
            for (int litNo = 0; litNo < drawBackFaceMaterialList.Count; litNo++)
            {
                Matrix4x4 mWorld = Matrix4x4.TRS(
                    drawBackMeshFilterList[litNo].transform.position,
                    drawBackMeshFilterList[litNo].transform.rotation,
                    drawBackMeshFilterList[litNo].transform.lossyScale
                );
                // �w�ʂ̐[�x�l��`��B
                commandBuffer.SetRenderTarget(renderTextures.backFaceDepthTexture);
                // todo �v���b�g�t�H�[���ɂ���Ă̓N���A����l��ύX����K�v�����邩���B
                commandBuffer.ClearRenderTarget(true, true, Color.white);
                commandBuffer.DrawMesh(
                    drawBackMeshFilterList[litNo].sharedMesh,
                    mWorld,
                    drawBackFaceMaterialList[litNo]
                );

                // �\�ʂ̐[�x�l��`��B
                commandBuffer.SetRenderTarget(renderTextures.frontFaceDepthTexture);
                // todo �v���b�g�t�H�[���ɂ���Ă̓N���A����l��ύX����K�v�����邩���B
                commandBuffer.ClearRenderTarget(true, true, Color.white);
                commandBuffer.DrawMesh(
                    drawFrontMeshFilterList[litNo].sharedMesh,
                    mWorld,
                    drawFrontFaceMaterialList[litNo]
                );
                
                commandBuffer.SetRenderTarget(renderTextures.finalTexture);
                
                drawFinalList[litNo].Draw(
                    camera,
                    renderTextures.frontFaceDepthTexture,
                    renderTextures.backFaceDepthTexture,
                    commandBuffer,
                    volumeSpotLightList[litNo].volumeSpotLightData
                );
            }
            
            commandBuffer.SetRenderTarget(cameraRenderTargetID);
            if (drawFinalList != null
                && drawFinalList.Count > 0
                && addCopyFullScreen != null)
            {
                addCopyFullScreen.Draw(
                    commandBuffer,
                    renderTextures.finalTexture,
                    BuiltinRenderTextureType.CameraTarget
                );
            }
            commandBuffer.ReleaseTemporaryRT(cameraTargetTextureID);
        }
    }

}