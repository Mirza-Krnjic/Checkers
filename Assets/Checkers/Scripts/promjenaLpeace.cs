using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class promjenaLpeace : MonoBehaviour
{
    // Start is called before the first frame update
    // I GRAB A MATERIAL
    [SerializeField] GameObject WhiteCheckerButton;
    [SerializeField] Material currentMaterial;
    [SerializeField] Texture[] textures;
    private Renderer cubeRenderer;

    private int textureIndex = 0;
    

    private void Awake()
    {
        currentMaterial.mainTexture = textures[PlayerPrefs.GetInt("indexLpeace")];
        WhiteCheckerButton.GetComponent<ChangeWhiteColor>().changeColor(PlayerPrefs.GetInt("indexLpeace"));
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
        PlayerPrefs.SetInt("indexLpeace", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
        WhiteCheckerButton.GetComponent<ChangeWhiteColor>().changeColor(PlayerPrefs.GetInt("indexLpeace"));
    }
    public void NextPeace()
    {
        textureIndex += 1;

        int size = textures.Length;
        if (textureIndex == size)
            textureIndex = 0;
        PlayerPrefs.SetInt("indexLpeace", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
        WhiteCheckerButton.GetComponent<ChangeWhiteColor>().changeColor(PlayerPrefs.GetInt("indexLpeace"));
    }
    public void PreviousPeace()
    {
        textureIndex -= 1;

        int size = textures.Length;
        if (textureIndex == -1)
            textureIndex = size - 1;
        PlayerPrefs.SetInt("indexLpeace", textureIndex);
        currentMaterial.mainTexture = textures[textureIndex];
        WhiteCheckerButton.GetComponent<ChangeWhiteColor>().changeColor(PlayerPrefs.GetInt("indexLpeace"));
    }
}