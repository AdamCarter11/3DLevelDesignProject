using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int currentCrystal;
    public Text CrystalText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCrystal(int crystalToAdd)
    {
        currentCrystal += crystalToAdd;
        CrystalText.text = "Crystal:" + currentCrystal + "!";
    }
}
