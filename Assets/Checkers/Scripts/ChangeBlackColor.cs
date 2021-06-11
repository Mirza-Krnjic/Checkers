using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBlackColor : MonoBehaviour
{
    public GameObject square;
    private List<Color> chosenColors;

    // Start is called before the first frame update
    void Awake()
    {
        //square = GetComponent<Image>();


        chosenColors = new List<Color>();
        chosenColors.Add(new Color32(86, 15, 1, 255));   //dark brown
        chosenColors.Add(new Color32(246, 94, 2, 255));  //orange
        chosenColors.Add(new Color32(94, 178, 65, 255)); //green
        chosenColors.Add(new Color32(35, 92, 136, 255)); //blue
        chosenColors.Add(new Color32(90, 90, 90, 255)); //black


        square.GetComponent<Image>().color = chosenColors[0];
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void changeColor(int index = 0)
    {

        square.GetComponent<Image>().color = chosenColors[index];
    }
}
