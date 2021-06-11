using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTexture : MonoBehaviour
{
    // I GRAB A GAME OBJECT
    [SerializeField] GameObject currentGameObject;
    [SerializeField] Texture[] textures;
    private Renderer cubeRenderer;
    private int textureIndex=0;
    void Start()
    {
        cubeRenderer = currentGameObject.GetComponent<Renderer>();
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeCubeTexture);
    }

   private void ChangeCubeTexture()
    {
        textureIndex += 1;
        cubeRenderer.material.mainTexture = textures[textureIndex];
    }
    public void loadNextTexture()
    {
        textureIndex += 1;
        cubeRenderer.material.mainTexture = textures[textureIndex];
    }
    public void loadPreviousTexture()
    {
        textureIndex -= 1;
        cubeRenderer.material.mainTexture = textures[textureIndex];
    }

}
