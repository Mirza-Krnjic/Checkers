using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class promjenaRpeace : MonoBehaviour
{
    // Start is called before the first frame update
    // I GRAB A MATERIAL
    [SerializeField] GameObject BlackButton;
    [SerializeField] Material currentMaterial;
    [SerializeField] Texture[] textures;
    private Renderer cubeRenderer;

    private int textureIndex = 0;


    private void Awake()
    {
        currentMaterial.mainTexture = textures[PlayerPrefs.GetInt("indexRpeace")];
        BlackButton.GetComponent<ChangeBlackColor>().changeColor(PlayerPrefs.GetInt("indexRpeace"));
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
        PlayerPrefs.SetInt("indexRpeace", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
        BlackButton.GetComponent<ChangeBlackColor>().changeColor(PlayerPrefs.GetInt("indexRpeace"));
    }
    public void NextPeace()
    {
        textureIndex += 1;

        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        PlayerPrefs.SetInt("indexRpeace", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
        BlackButton.GetComponent<ChangeBlackColor>().changeColor(PlayerPrefs.GetInt("indexRpeace"));
    }
    public void PreviousPeace()
    {
        textureIndex -= 1;

        int size = textures.Length;
        if (textureIndex == -1)
            textureIndex = size - 1;
        PlayerPrefs.SetInt("indexRpeace", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
        BlackButton.GetComponent<ChangeBlackColor>().changeColor(PlayerPrefs.GetInt("indexRpeace"));
    }
}