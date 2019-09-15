

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class HighscoreTable : MonoBehaviour
{


    public Transform entryContainer;
    public Transform entryTemplate;
    public List<Transform> highscoreEntryTransformList;
    public GameObject canvasObject;
    public Color colorFirst;
    public Color colorRest;



    private float nextActionTime = 0.0f;
    public float period = 7.1f;

    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            entryContainer = transform.Find("highscoreEntryContainer");
            entryTemplate = entryContainer.Find("highscoreEntryTemplate");

            entryTemplate.gameObject.SetActive(false);

            string jsonString = PlayerPrefs.GetString("highscoreTable");
            var highscores = JsonUtility.FromJson<Highscores>(jsonString);

            if (highscores == null)
            {
                Debug.Log("Initializing table with default values...");
                AddHighscoreEntry(1, " ", "-1");

                // Reload
                jsonString = PlayerPrefs.GetString("highscoreTable");
                highscores = JsonUtility.FromJson<Highscores>(jsonString);
            }

            highscores.highscoreEntryList.Sort((x, y) => x.score.CompareTo(y.score));
            highscores.highscoreEntryList.Reverse();

            var newhighscores = new Highscores();
            if (highscores.highscoreEntryList.Count > 6)
            {
                newhighscores.highscoreEntryList = highscores.highscoreEntryList.GetRange(0, 6);
            }
            else
            {
                newhighscores.highscoreEntryList = highscores.highscoreEntryList;
            }

            highscoreEntryTransformList = new List<Transform>();

            var gameobjectList = GameObject.FindGameObjectsWithTag("higscoretext");
            for (var i = 0; i < gameobjectList.Length - 6; i++)
            {
                if (gameobjectList[i].name == "highscoreEntryTemplate(Clone)" && gameobjectList.Length > 6)
                {
                    Destroy(gameobjectList[i]);
                }
            }

            foreach (HighscoreEntry highscoreEntry in newhighscores.highscoreEntryList)
            {
                CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
            }
        }
    }

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");



        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            Debug.Log("Initializing table with default values...");
            AddHighscoreEntry(1, " ", "-1");

            // Reload
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        highscores.highscoreEntryList.Sort((x, y) => x.score.CompareTo(y.score));
        highscores.highscoreEntryList.Reverse();

        var newhighscores = new Highscores();
        if (highscores.highscoreEntryList.Count > 6)
        {
            newhighscores.highscoreEntryList = highscores.highscoreEntryList.GetRange(0, 6);
        }
        else
        {
            newhighscores.highscoreEntryList = highscores.highscoreEntryList;
        }

        highscoreEntryTransformList = new List<Transform>();
        //for (int i = 0; i <=highscores.highscoreEntryList.Count; i++)
        //{
        //    CreateHighscoreEntryTransform(highscores.highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
        //}
        foreach (HighscoreEntry highscoreEntry in newhighscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }



    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
        int rank = transformList.Count + 1;
        if (rank < 10)
        {

            string rankString;
            switch (rank)
            {
                default:
                    rankString = rank + "TH"; break;

                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }

            entryTransform.Find("posText").GetComponent<Text>().text = rankString;

            int score = highscoreEntry.score;

            entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

            string name = highscoreEntry.name;
            entryTransform.Find("nameText").GetComponent<Text>().text = name;

             // Highlight First
            if (rank == 1)
            {
                entryTransform.Find("posText").GetComponent<Text>().color = colorFirst;
                entryTransform.Find("scoreText").GetComponent<Text>().color = colorFirst;
                entryTransform.Find("nameText").GetComponent<Text>().color = colorFirst;
            }
            else
            {
                entryTransform.Find("posText").GetComponent<Text>().color = colorRest;
                entryTransform.Find("scoreText").GetComponent<Text>().color = colorRest;
                entryTransform.Find("nameText").GetComponent<Text>().color = colorRest;
            }


            transformList.Add(entryTransform);
        }
        Debug.Log(transformList.Count);
    }

    public static void AddHighscoreEntry(int score, string name, string phone)
    {
        Debug.Log("Trying to add..");
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            highscores = new Highscores()
            {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        var newEntry = new HighscoreEntry { score = score, name = name, phone = phone };

        if (highscores.highscoreEntryList.Contains(newEntry))
        {
            var currentEntry = highscores.highscoreEntryList.Where(x => x.phone == phone).First();
            if (newEntry.score > currentEntry.score)
            {
                Debug.Log("Better score!");
                highscores.highscoreEntryList.Remove(currentEntry);
                highscores.highscoreEntryList.Add(newEntry);

            }
            else
            {
                return;
            }
        }
        else
        {
            highscores.highscoreEntryList.Add(newEntry);
        }

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable]
    private class HighscoreEntry : IEquatable<HighscoreEntry>
    {
        public int score;
        public string name;
        public string phone;

        public bool Equals(HighscoreEntry other)
        {
            return this.phone == other.phone;
        }
    }

}
