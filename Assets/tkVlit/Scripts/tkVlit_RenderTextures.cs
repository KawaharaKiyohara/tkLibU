using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �{�����[�����C�g�̕`��Ŏg�������_�����O�e�N�X�`�������B
/// </summary>
public class tkVlit_RenderTextures
{
    RenderTexture m_backFaceDepthTexture;   // �w�ʂ̐[�x�l���������܂�Ă���e�N�X�`���B
    RenderTexture m_frontFaceDepthTexture;  // �\�ʂ̐[�x�l���������܂�Ă���e�N�X�`���B
    RenderTexture m_finalTexture;           // �ŏI�`�挋�ʂ̏������ݐ�B
    int m_depthMapWidth;                    // �[�x�}�b�v�̕��B
    int m_depthMapHeight;                   // �[�x�}�b�v�̍����B
    bool m_isInitedRenderTexture = false;   // �����_�����O�e�N�X�`�����������ς݂��ǂ����̃t���O�B
    /// <summary>
    /// �w�ʂ̐[�x�l���������܂�Ă���e�N�X�`���̃v���p�e�B
    /// </summary>
    public RenderTexture backFaceDepthTexture
    {
        get => m_backFaceDepthTexture;
    }
    /// <summary>
    /// �O�ʂ̐[�x�l���������܂�Ă���e�N�X�`���̃v���p�e�B�B
    /// </summary>
    public RenderTexture frontFaceDepthTexture
    {
        get => m_frontFaceDepthTexture;
    }
    /// <summary>
    /// �ŏI���ʂ�`�挋�ʂ��������ރe�N�X�`���̃v���p�e�B�B
    /// </summary>
    public RenderTexture finalTexture
    {
        get => m_finalTexture;
    }
    /// <summary>
    /// ���t���[���Ăяo���X�V����
    /// </summary>
    public void Update()
    {
        if (m_depthMapWidth != Screen.width || m_depthMapHeight != Screen.height)
        {
            Init();
        }
    }
    /// <summary>
    /// �e�탌���_�����O�e�N�X�`��������������B
    /// </summary>
    public void Init()
    {
        // �[�x�e�N�X�`���̕��ƍ������������B
        m_depthMapWidth = Screen.width;
        m_depthMapHeight = Screen.height;

        if (m_isInitedRenderTexture)
        {
            // �ď������ɂȂ�̂ŁA�Â����\�[�X��j���B
            m_backFaceDepthTexture.Release();
            m_frontFaceDepthTexture.Release();
            m_finalTexture.Release();
        }
        // �w�ʂ̐[�x�l���������ރe�N�X�`�����쐬�B
        m_backFaceDepthTexture = new RenderTexture(
            m_depthMapWidth,
            m_depthMapHeight,
            /*depth=*/0,
            RenderTextureFormat.RHalf
        );
        // �A���`�͂���Ȃ��̂ŁA�T���v�����O����1�ɂ���B
        m_backFaceDepthTexture.antiAliasing = 1;
        // �\�ʂ̐[�x�l���������ރe�N�X�`�����쐬�B
        // m_backFaceDepthTexture�Ɠ����ł����̂ŃR�s�[�R���X�g���N�^���Ăяo���B
        m_frontFaceDepthTexture = new RenderTexture(m_backFaceDepthTexture);

        // �ŏI�`����s���e�N�X�`�����������B
        // ���o�C���̃s�N�Z�������\�́A���Ƀ������ш悪�������̂ŁA
        // �����_�����O�e�N�X�`���̉𑜓x��1/4�ɂ��Ă���B
        m_finalTexture = new RenderTexture(m_depthMapWidth / 4, m_depthMapHeight / 4, 0, RenderTextureFormat.RGB111110Float);
        // �������A���`�͂���Ȃ��B
        m_finalTexture.antiAliasing = 1;

        // �������ς݂̈�B
        m_isInitedRenderTexture = true;
    }
}
