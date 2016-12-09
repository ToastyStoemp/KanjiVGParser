using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.UI;
using System.Linq;

public class Kanji
{
    [XmlAttribute("ID")]
    public int ID { get; set; }
    [XmlAttribute("sign")]
    public string Name { get; set; }
    [XmlAttribute("meaning")]
    public string Meaning { get; set; }
    [XmlAttribute("strokes")]
    public string Strokes { get; set; }
    [XmlAttribute("count")]
    public string strokeCount { get; set; }
}

[XmlRoot("KanjiLibrary")]
public class KanjiList
{
    [XmlElement("Kanji")]
    public Kanji[] RawLibrary { get; set; }

    [XmlIgnore]
    public List<Kanji> Library = new List<Kanji>();

    public void ToRaw()
    {
        RawLibrary = Library.ToArray();
    }

    public void FromRaw()
    {
        Library = new List<Kanji>(RawLibrary);
    }
}
