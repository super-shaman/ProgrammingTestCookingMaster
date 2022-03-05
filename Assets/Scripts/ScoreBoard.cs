using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class ScoreBoard : MonoBehaviour
{

    public Text[] scores;
    [SerializeField]
    public float[] topScores = new float[10];

    void Start()
    {
    }

    public void Load()
    {

        string path = "test.txt";
        if (File.Exists(path))
        {
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            string s = reader.ReadLine();
            int counter = 0;
            while (s != null)
            {
                topScores[counter] = float.Parse(s);
                counter++;
                s = reader.ReadLine();
            }
            reader.Close();
            UpdateScores();
        }
    }

    public void Save()
    {

        string path = "test.txt";
        //Write some text to the test.txt file
        FileStream fs = File.Create(path);
        fs.Close();
        StreamWriter writer = new StreamWriter(path, true);
        for (int i = 0; i < topScores.Length; i++)
        {
            writer.WriteLine(topScores[i]);
        }
        writer.Close();
    }

    public void UpdateScores()
    {
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i].text = "" + (i+1) + ":  " + topScores[i];
        }
        
    }
    public void AddScore(float score)
    {
        for (int i = 0; i < topScores.Length; i++)
        {
            if (score > topScores[i])
            {
                for (int ii = topScores.Length - 2; ii >= i; ii--)
                {
                    topScores[ii + 1] = topScores[ii];
                }
                topScores[i] = score;
                UpdateScores();
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
