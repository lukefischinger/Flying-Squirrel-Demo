using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PortraitDictionary : ScriptableObject
{

    [Serializable]
    public struct portrait
    {
        public string name;
        public Sprite sprite;
    }

    public List<portrait> portraits;
}
