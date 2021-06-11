using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class promjenaTable : MonoBehaviour
{
    // Start is called before the first frame update
    // I GRAB A MATERIAL
    [SerializeField] Material currentMaterial;
    [SerializeField] Texture[] textures;
    private Renderer cubeRenderer;

    private int textureIndex = 0;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("indexTabla") != 0)
        {
            if (PlayerPrefs.GetInt("indexTabla") == 5)
                PlayerPrefs.SetInt("indexTabla", 0);
            currentMaterial.mainTexture = textures[PlayerPrefs.GetInt("indexTabla")];
        }
    }

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeCubeMaterial);
    }

    private void ChangeCubeMaterial()
    {
        textureIndex += 1;

        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        PlayerPrefs.SetInt("indexTabla", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
    }
    public void setNext()
    {
        textureIndex += 1;

        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        PlayerPrefs.SetInt("indexTabla", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
    }
    public void setPrev()
    {
        textureIndex -= 1;

        int size = textures.Length;
        if (textureIndex == -1)
            textureIndex = size-1;
        PlayerPrefs.SetInt("indexTabla", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
    }



}
