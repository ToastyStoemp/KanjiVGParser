using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.UI;
using System.Linq;
using System.Xml;

public static class KanjiManager {

    public static KanjiList KanjiLibrary = new KanjiList();

    public static void Init()
    {
        if (KanjiLibrary.Library.Count == 0)
            LoadKanjiXML("kanji");
        if (KanjiLibrary.Library.Count == 0)
            Debug.LogError("The library did not load any file");
    }

    public static List<Kanji> GetMatchingKanji(string path)
    {
        List<Kanji> results = new List<Kanji>();
        foreach (Kanji kanji in KanjiLibrary.Library)
        {
            if (kanji.Strokes.Length >= path.Length)
            {
                if (kanji.Strokes.Substring(0, path.Length) == path)
                {
                    //Debug.Log(kanji.StrokeString + " " + path);
                    results.Add(kanji);
                }
            }
        }
        return results;
    }

    private static bool compArr<T, S>(T[] arrayA, S[] arrayB)
    {
        if (arrayA.Length != arrayB.Length) return false;

        for (int i = 0; i < arrayA.Length; i++)
        {
            if (!arrayA[i].Equals(arrayB[i])) return false;
        }

        return true;
    }

    private static void WriteKanjiXML(string file)
    {
        KanjiLibrary.ToRaw();
        using (var fs = File.Create(file))
        {
            var xmlFormatter = new XmlSerializer(typeof(KanjiList));
            xmlFormatter.Serialize(fs, KanjiLibrary);
        }
    }

    private static void LoadKanjiXML(string file)
    {
        string xmlText = (Resources.Load(file) as TextAsset).text;

        var serializer = new XmlSerializer(typeof(KanjiList));
        using (var reader = new StringReader(xmlText))
        {
            KanjiLibrary = serializer.Deserialize(reader) as KanjiList;
        }
        KanjiLibrary.FromRaw();
    }

    public static void AddKanji(Kanji kanji)
    {
        if (KanjiLibrary == null) KanjiLibrary = new KanjiList();
        KanjiLibrary.Library.Add(kanji);
    }

    public static void SaveLibrary()
    {
        WriteKanjiXML("test.xml");
    }

    public static Kanji GetRandomKanji()
    {
        return KanjiLibrary.Library[(int)(Random.value * KanjiLibrary.Library.Count)];
    }

    public static List<Kanji> GetAllKanji()
    {
        return KanjiLibrary.Library;
    }

    public static Kanji[] GetAllRawKanji()
    {
        return KanjiLibrary.RawLibrary;
    }

    public static Kanji GetKanjiFromID(int ID)
    {
        foreach (Kanji kanji in KanjiLibrary.Library)
        {
            if (kanji.ID == ID)
                return kanji;
        }
        return null;
    }
}
