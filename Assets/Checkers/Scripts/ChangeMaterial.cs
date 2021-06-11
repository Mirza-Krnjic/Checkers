using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMaterial : MonoBehaviour
{
    // I GRAB A MATERIAL
    [SerializeField] Material currentMaterial;
    [SerializeField] Texture[] textures;
    private Renderer cubeRenderer;
    
    private int textureIndex = 0;
    private void Awake()
    {
        currentMaterial.mainTexture = textures[PlayerPrefs.GetInt("indexer")];
    }
    void Start()
    {
        PlayerPrefs.SetInt("index", textureIndex);
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeCubeMaterial);
    }

    private void ChangeCubeMaterial()
    {
        textureIndex += 1;
        PlayerPrefs.SetInt("index", textureIndex);
        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        currentMaterial.mainTexture = textures[textureIndex];
    }
    public void loadNextTexture()
    {
        int size = textures.Length;
        if (textureIndex == size - 1)
            textureIndex = -1;
        textureIndex += 1;
        currentMaterial.mainTexture = textures[textureIndex];
    }
    public void loadPreviousTexture()
    {
        int size = textures.Length;
        if (textureIndex == 0)
            textureIndex = size;
        textureIndex -= 1;
        currentMaterial.mainTexture = textures[textureIndex];
    }

}
