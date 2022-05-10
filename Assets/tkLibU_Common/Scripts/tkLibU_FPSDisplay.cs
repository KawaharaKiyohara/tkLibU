using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tkLibU_FPSDisplay : MonoBehaviour
{
    // �ϐ�
    int frameCount;
    float prevTime;
    float fps;

    // ����������
    void Start()
    {
        Application.targetFrameRate = 60;
        // �ϐ��̏�����
        frameCount = 0;
        prevTime = 0.0f;
    }

    // �X�V����
    void Update()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fps = frameCount / time;
            Debug.Log(fps);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }

    // �\������
    private void OnGUI()
    {
        var styleState = new GUIStyleState();
        styleState.textColor = Color.white;
        var style = new GUIStyle();
        style.fontSize = 50;
        style.normal = styleState;
        GUILayout.BeginArea(new Rect(50, 50, 400, 150));
        string text = string.Format("FPS = {0:F2}", fps);
        GUILayout.Label(text, style);
        GUILayout.EndArea();
    }
}
