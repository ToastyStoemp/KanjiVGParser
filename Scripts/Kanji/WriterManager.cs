using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Linq;

public delegate void OnWinPerfect();
public delegate void OnWinAlmost();
public delegate void OnWinPoor();

public delegate void OnNewKanji(Kanji kanji);

public class WriterManager : MonoBehaviour {

    public Kanji SeekingKanji;

    private Writer WriterRef;

    public Camera TargetCamera;
    public GameObject TrailPrefab;

    public bool IsDebug;
    private string DebugString;

    #region Event related code

    //------------------------
    //EVENT RELATED Code
    //------------------------

    public event OnWinPerfect OnWinPerfect;
    public event OnWinAlmost OnWinAlmost;
    public event OnWinPoor OnWinPoor;

    public event OnNewKanji OnNewKanji;

    public void AddOnWinPerfectEvent(OnWinPerfect perfectEvent)
    {
        OnWinPerfect += perfectEvent;
    }

    public void RemoveOnWinPerfectEvent(OnWinPerfect perfectEvent)
    {
        OnWinPerfect -= perfectEvent;
    }

    public void AddOnWinAlmostEvent(OnWinAlmost almostEvent)
    {
        OnWinAlmost += almostEvent;
    }

    public void RemoveOnWinAlmostEvent(OnWinAlmost almostEvent)
    {
        OnWinAlmost -= almostEvent;
    }

    public void AddOnWinPoorEvent(OnWinPoor poorEvent)
    {
        OnWinPoor += poorEvent;
    }

    public void RemoveOnWinPoorEvent(OnWinPoor poorEvent)
    {
        OnWinPoor -= poorEvent;
    }

    public void AddOnNewKanjiEvent(OnNewKanji newKanjiEvent)
    {
        OnNewKanji += newKanjiEvent;
    }

    public void RemoveOnNewKanjiEvent(OnNewKanji newKanjiEvent)
    {
        OnNewKanji -= newKanjiEvent;
    }

    #endregion

    void Awake()
    {
        KanjiManager.Init();
        WriterRef = GetComponent<Writer>() ?? gameObject.AddComponent<Writer>();
        WriterRef.writerManager = this;
    }

    public void ParseKanji(string userstrokeCount, string path)
    {
        if (path == "")
            return;

        string results = "";
        Kanji firstResult = null;
        foreach (Kanji kanji in KanjiManager.GetMatchingKanji(path))
        {
            if (firstResult == null)
            {
                firstResult = kanji;
                results += kanji.Name;
            }
            else
                results += kanji.Name;// + " " + kanji.Meaning;
        }
        if (IsDebug)
            DebugString = results;

        if (path == SeekingKanji.Strokes)
        {
            OnWinPerfect?.Invoke();
        }
        else
        {
            List<string> diff;
            string[] set1 = Regex.Matches(path, @"\(.+?(?=\))+.").Cast<Match>().Select(match => match.Value).ToArray();
            string[] set2 = Regex.Matches(SeekingKanji.Strokes, @"\(.+?(?=\))+.").Cast<Match>().Select(match => match.Value).ToArray();

            if (set1.Count() > set2.Count())
            {
                diff = getDifferences(set1, set2);
            }
            else
            {
                diff = getDifferences(set2, set1);
            }
            if (diff.Count == 0 && userstrokeCount == SeekingKanji.strokeCount)
            {
                OnWinAlmost?.Invoke();
                return;
            }


            if (set2.Count() > set1.Count())
            {
                diff = set2.Except(set1).ToList();
            }
            else
            {
                diff = set1.Except(set2).ToList();
            }

            List<string> diffCopy = diff;
            for (int i = 0; i < diffCopy.Count; i++)
            {
                if (diffCopy[i].Replace("-", "") == "(0.5, 0.5)")
                    diff.RemoveAt(i);
            }

            if (diff.Count == 0 && userstrokeCount == SeekingKanji.strokeCount)
            {
                OnWinPoor?.Invoke();
            }
            else if (IsDebug)
            {
                string res = "";
                foreach (var item in diff)
                {
                    res += item;
                }
                if (res != "")
                {
                    Debug.Log(res);
                }
            }
        }
    }

    private List<string> getDifferences(string[] set1, string[] set2)
    {
        List<string> diff = new List<string>();
        int offset = 0;
        for (int i = 0; i < set1.Length; i++)
        {
            if (i - offset < set2.Length)
            {
                if (set1[i] != set2[i - offset])
                {
                    if (set1[i].Replace("-", "") == "(0.5, 0.5)")
                    {
                        if (i != 0 && i != set1.Length - 1)
                        {
                            offset++;
                        }
                        else
                        {
                            diff.Add(set1[i]);
                        }
                    }
                    else
                    {
                        diff.Add(set1[i]);
                    }
                }
            }
            else
            {
                if (set1[i].Replace("-", "") != "(0.5, 0.5)")
                {
                    diff.Add(set1[i]);
                }
            }
        }
        return diff;
    }


    public void SkipKanji()
    {
        NewKanji();
    }

    public void ClearWriter()
    {
        WriterRef.Clear();
    }

    public void NewKanji()
    {
        SetKanji(KanjiManager.GetRandomKanji());
    }

    public void SetKanji(Kanji kanji)
    {
        SeekingKanji = kanji;
        ClearWriter();
        OnNewKanji?.Invoke(SeekingKanji);
        if (IsDebug) Debug.Log(SeekingKanji.Name);
    }

    void OnGUI()
    {
        if (IsDebug)
        {
            GUILayout.Label(DebugString);
        }
    }
}
