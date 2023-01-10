using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShuffleList
{
    public static List<Sprite> ShuffleListItems<Sprite>(List<Sprite> inputList)
    {
        List<Sprite> originalList = new List<Sprite>();
        originalList.AddRange(inputList);
        List<Sprite> randomList = new List<Sprite>();

        System.Random r = new System.Random();
        int randomIndex = 0;
        while (originalList.Count > 0)
        {
            randomIndex = r.Next(0, originalList.Count); //Choose a random object in the list
            randomList.Add(originalList[randomIndex]); //add it to the new, random list
            originalList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }
}
