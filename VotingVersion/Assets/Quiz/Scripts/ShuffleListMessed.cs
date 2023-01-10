using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;

public abstract class ShuffleListMessed
{
    public static Sprite[] ShuffleListItems(Sprite[] inputList)

    {
        Sprite[] originalList = new Sprite[inputList.Length];
        //originalList.AddRange(inputList);
        Sprite[] randomList = new Sprite[inputList.Length];

        List<int> objects = new List<int>(3);

        for (int i = 1; i < 4; i++)
        {
            objects.Add(i);
        }

        Debug.Log((objects[0], objects[1], objects[2]));

        List<int> randomizedObjects = new List <int>(new int[3]); //makes it not be an empty list and have 3 0s to start with.

        for (int i = 0; i < 4; i++)
        {

            int rand = Random.Range(1, 3);

            while (randomizedObjects.Contains(rand))
            {
                rand = Random.Range(1, 3);

                if (randomizedObjects.Contains(1) && randomizedObjects.Contains(2))
                {
                    rand = 3;
                }
            }

            randomizedObjects[i] = rand;
            Debug.Log(randomizedObjects[i]);
        }


        //var rng = new Random();
        //rng.Shuffle(array);
        //rng.Shuffle(array); // different order from first call to Shuffle

        for (int i = 0; i < inputList.Length; i++)
        {
            int u = randomizedObjects[i];
            randomList[i] = originalList[u];

           // randomIndex = r.Next(0, originalList[r]); //Choose a random object in the list
           // randomList.Add(originalList[randomIndex]); //add it to the new, random list
           // originalList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }




}
