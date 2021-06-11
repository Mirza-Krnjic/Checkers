using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class promjenaStola : MonoBehaviour
{
    // Start is called before the first frame update
    // I GRAB A MATERIAL
    [SerializeField] Material currentMaterial;
    [SerializeField] Texture[] textures;
    private Renderer cubeRenderer;

    private int textureIndex = 0;


    private void Awake()
    {
        currentMaterial.mainTexture = textures[PlayerPrefs.GetInt("indexSto")];
    }
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeCubeMaterial);
    }

    private void ChangeCubeMaterial()
    {
        textureIndex += 1;
        
        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        PlayerPrefs.SetInt("indexSto", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
    }
    public void SetNext()
    {
        textureIndex += 1;

        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        PlayerPrefs.SetInt("indexSto", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
    }
    public void SetPrev()
    {
        textureIndex -= 1;

        int size = textures.Length;
        if (textureIndex == -1)
            textureIndex = size - 1;
        PlayerPrefs.SetInt("indexSto", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
    }



}
