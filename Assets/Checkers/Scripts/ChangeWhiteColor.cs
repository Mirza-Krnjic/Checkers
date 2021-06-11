using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWhiteColor : MonoBehaviour
{
    public GameObject square;
    private List<Color> chosenColors;

    // Start is called before the first frame update
    void Awake()
    {
        //square = GetComponent<Image>();
        

        chosenColors = new List<Color>();
        chosenColors.Add(new Color32(213, 67, 40, 255));   //red
        chosenColors.Add(new Color32(252, 207, 62, 255));  //yellow
        chosenColors.Add(new Color32(142, 78, 28, 255)); //brown
        chosenColors.Add(new Color32(187, 132, 169, 255)); //light purple
        chosenColors.Add(new Color32(254,230,194,255)); //cream


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
