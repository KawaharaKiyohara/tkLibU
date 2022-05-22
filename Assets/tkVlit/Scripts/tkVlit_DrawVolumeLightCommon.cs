using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumeLight
{
    /// <summary>
    /// �{�����[�����C�g�`��̋��ʏ����B
    /// </summary>
    public class tkVlit_DrawVolumeLightCommon
    {
        CommandBuffer m_commandBuffer;                  // �R�}���h�o�b�t�@�B
        Camera m_camera;                                // �J�����B
        tkVlit_RenderTextures m_renderTextures;         // �����_�����O�e�N�X�`�������B
        tkLibU_AddCopyFullScreen m_addCopyFullScreen;   // �S��ʂɃt���X�N���[���R�s�[�B
        List<tkVlit_SpotLight> m_volumeSpotLightList;   // �{�����[���X�|�b�g���C�g�̃��X�g�B
        List<Material> m_drawBackFaceMaterialList;      // �w�ʂ̐[�x�l�`��Ŏg�p����}�e���A���̃��X�g�B
        List<Material> m_drawFrontFaceMaterialList;     // �\�ʂ̐[�x�l�`��Ŏg�p����}�e���A���̃��X�g�B
        List<MeshFilter> m_drawBackMeshFilterList;      // �w�ʂ̐[�x�l�`��Ŏg�p���郁�b�V���t�B���^�[�̃��X�g�B
        List<MeshFilter> m_drawFrontMeshFilterList;     // �\�ʂ̐[�x�l�`��Ŏg�p���郁�b�V���t�B���^�[�̃��X�g�B
        List<tkVlit_DrawFinal> m_drawFinalList;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="camera">�J����</param>
        public tkVlit_DrawVolumeLightCommon(Camera camera)
        {
            m_camera = camera;
            m_camera.depthTextureMode = DepthTextureMode.Depth;
            m_commandBuffer = new CommandBuffer();
            m_volumeSpotLightList = new List<tkVlit_SpotLight>();
            m_drawBackFaceMaterialList = new List<Material>();
            m_drawFrontFaceMaterialList = new List<Material>();
            m_drawBackMeshFilterList = new List<MeshFilter>();
            m_drawFrontMeshFilterList = new List<MeshFilter>();
            m_drawFinalList = new List<tkVlit_DrawFinal>();
            m_addCopyFullScreen = GameObject.FindObjectOfType<tkLibU_AddCopyFullScreen>();

            m_renderTextures = new tkVlit_RenderTextures();
            m_renderTextures.Init();
        }
        /// <summary>
        /// �X�|�b�g���C�g��ǉ��B
        /// </summary>
        /// <param name="spotLight">�ǉ����ꂽ�X�|�b�g���C�g</param>
        public void AddSpotLight(tkVlit_SpotLight spotLight)
        {
            // �w�ʂ̐[�x�l�`��Ɋւ��鏉���������B
            {
                var trans = spotLight.transform.Find("BackRenderer");
                // Unity�̃����_�����O�p�C�v���C���ł͕`�悵�Ȃ��̂ŁAMeshRenderer�𖳌��ɂ���B
                var meshRenderer = trans.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                // Unity�̃����_�����O�p�C�v���C���O�ŕ`�悷�邽�߂ɂ̓}�e���A���ƃ��b�V���t�B���^�[���K�v�Ȃ̂ŁA
                // ���X�g�ɒǉ�����B
                if (m_camera.actualRenderingPath == RenderingPath.DeferredShading)
                {
                    //
                    meshRenderer.sharedMaterial.EnableKeyword("TK_DEFERRED_PASS");
                }
                else
                {
                    meshRenderer.sharedMaterial.DisableKeyword("TK_DEFERRED_PASS");
                }
                m_drawBackFaceMaterialList.Add(meshRenderer.sharedMaterial);
                m_drawBackMeshFilterList.Add(trans.GetComponent<MeshFilter>());
            }
            // �\�ʂ̐[�x�l�`��Ɋւ��鏉���������B
            {
                var trans = spotLight.transform.Find("FrontRenderer");
                // Unity�̃����_�����O�p�C�v���C���ł͕`�悵�Ȃ��̂ŁAMeshRenderer�𖳌��ɂ���B
                var meshRenderer = trans.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                // Unity�̃����_�����O�p�C�v���C���O�ŕ`�悷�邽�߂ɂ̓}�e���A���ƃ��b�V���t�B���^�[���K�v�Ȃ̂ŁA
                // ���X�g�ɒǉ�����B
                if (m_camera.actualRenderingPath == RenderingPath.DeferredShading)
                {
                    //
                    meshRenderer.sharedMaterial.EnableKeyword("TK_DEFERRED_PASS");
                }
                else
                {
                    meshRenderer.sharedMaterial.DisableKeyword("TK_DEFERRED_PASS");
                }
                m_drawFrontFaceMaterialList.Add(meshRenderer.sharedMaterial);
                m_drawFrontMeshFilterList.Add(trans.GetComponent<MeshFilter>());
            }
            // �ŏI�`��Ɋւ��鏉���������B
            {
                var trans = spotLight.transform.Find("FinalRenderer");
                m_drawFinalList.Add(trans.GetComponent<tkVlit_DrawFinal>());
            }
            m_volumeSpotLightList.Add(spotLight);
        }
        /// <summary>
        /// �X�|�b�g���C�g���폜�B
        /// </summary>
        /// <param name="spotLight">�폜����X�|�b�g���C�g�B</param>
        public void RemoveSpotLight(tkVlit_SpotLight spotLight)
        {
            for (int i = 0; i < m_volumeSpotLightList.Count; i++)
            {
                if (m_volumeSpotLightList[i] == spotLight)
                {
                    m_volumeSpotLightList.RemoveAt(i);
                    m_drawBackFaceMaterialList.RemoveAt(i);
                    m_drawFrontFaceMaterialList.RemoveAt(i);
                    m_drawBackMeshFilterList.RemoveAt(i);
                    m_drawFrontMeshFilterList.RemoveAt(i);
                    m_drawFinalList.RemoveAt(i);
                    break;
                }
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        public void Release()
        {
            if (m_commandBuffer != null)
            {
                m_commandBuffer.Release();
                m_commandBuffer = null;
            }
        }
        /// <summary>
        /// �`��
        /// </summary>
        /// <returns>�\�z�����`��R�}���h�o�b�t�@</returns>
        public CommandBuffer Draw()
        {
            if (m_commandBuffer == null
                || m_camera == null
                || m_renderTextures == null
            )
            {
                return null;
            }

            m_renderTextures.Update();
            // �`��R�}���h���\�z����B
            tkVlit_BuildDrawVolumeLightCommand.Build(
                m_commandBuffer,
                m_renderTextures,
                m_drawBackFaceMaterialList,
                m_drawFrontFaceMaterialList,
                m_drawBackMeshFilterList,
                m_drawFrontMeshFilterList,
                m_drawFinalList,
                m_volumeSpotLightList,
                m_addCopyFullScreen,
                m_camera
            );
            return m_commandBuffer;
        }
        public CommandBuffer EndDraw()
        {
            if (m_commandBuffer != null)
            {
                m_commandBuffer.Clear();
            }
            return m_commandBuffer;
        }
    }
}