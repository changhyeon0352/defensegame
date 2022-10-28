using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    string testString = "123,456";
    private void Awake()
    {
        float health = PlayerPrefs.GetFloat("Health");
    }
}
