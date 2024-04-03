using System;
using TMPro;
using UnityEngine;

public class TextEffects : MonoBehaviour
{
    private TMP_Text textComponent;
    private TMP_TextInfo textInfo;
    private TMP_MeshInfo[] cachedMeshInfo;
    public TextEffect textEffect;
    private float curveTime;
    private Vector2 positionA,positionB;
    private Quaternion rotationA, rotationB;
    private Vector3 scaleA, scaleB;
    private Color colorA, colorB;
    public float indexTimeOffset;
    public bool active;
    public float frameStep;
    public float frameRate;
    private float nextFrame;
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        textComponent.ForceMeshUpdate();
        textInfo = textComponent.textInfo;
        cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        if (textEffect != null)
        {
            positionA = textEffect.positionA;
            positionB = textEffect.positionB;
            rotationA = Quaternion.Euler(textEffect.eulerAnglesA);
            rotationB = Quaternion.Euler(textEffect.eulerAnglesB);
            scaleA = textEffect.scaleA;
            scaleB = textEffect.scaleB;
            colorA = textEffect.colorA;
            colorB = textEffect.colorB;
            curveTime = 0f;
        }
    }

    private void Update()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            AnimateCharacter(charInfo);
        }
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
        curveTime += Time.unscaledDeltaTime;
    }

    void AnimateCharacter(TMP_CharacterInfo charInfo)
    {
        float evaluatedTime = 0;
        if (active)
        {
            evaluatedTime = textEffect.lerpCurve.Evaluate(curveTime + charInfo.index*indexTimeOffset);
        }
        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;
        Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
        Vector3 offset = new Vector3(
                (sourceVertices[vertexIndex].x + sourceVertices[vertexIndex+2].x)/2f,
                charInfo.baseLine,
                0f
            );
        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i] = sourceVertices[vertexIndex + i] - offset;
        }

        Vector3 translation = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(0,0,0);
        Vector3 scale = Vector3.one;
        Color color = Color.white;
        if (textEffect.flags.HasFlag(TextEffectFlags.Translation))
        {
            translation = Vector2.LerpUnclamped(positionA, positionB, evaluatedTime);
        }
        if (textEffect.flags.HasFlag(TextEffectFlags.Rotation))
        {
            rotation = Quaternion.SlerpUnclamped(rotationA, rotationB, evaluatedTime);
        }       
        if (textEffect.flags.HasFlag(TextEffectFlags.Scale))
        {
            scale = Vector2.LerpUnclamped(scaleA, scaleB, evaluatedTime);
        }        
        if (textEffect.flags.HasFlag(TextEffectFlags.Color))
        {
            color = Color.LerpUnclamped(colorA, colorB, evaluatedTime);
        }
        Matrix4x4 matrixTRS = Matrix4x4.TRS(translation, rotation, scale);
        Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i] = matrixTRS.MultiplyPoint3x4(destinationVertices[vertexIndex + i]) + offset;
            newVertexColors[vertexIndex + i] = color;
        }
    }

    public void ToggleEffectOn()
    {
        active = true;
    }
    
    public void ToggleEffectOff()
    {
        active = false;
    }
}