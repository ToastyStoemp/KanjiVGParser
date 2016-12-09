using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WritingGameExample : MonoBehaviour
{

    public int Score = 0;
    private WriterManager KanjiWriter;

    public Text OutPutTextField;
    public Camera TargetCamera;
    public GameObject TrailPrefab;

    void WinPerfectEvent()
    {
        Score += 300;
        Debug.Log("Perfect!");
        KanjiWriter.NewKanji();
    }

    void WinAlmostEvent()
    {
        Score += 100;
        Debug.Log("Almost!");
        KanjiWriter.NewKanji();
    }

    void WinPoorEvent()
    {
        Score += 50;
        Debug.Log("Close Enough!");
        KanjiWriter.NewKanji();
    }

    void NewKanjiEvent(Kanji kanji)
    {
        OutPutTextField.text = kanji.Meaning;
        Debug.Log(kanji.Meaning);
    }

    void Start()
    {
        KanjiWriter = gameObject.AddComponent<WriterManager>();
        KanjiWriter.TargetCamera = TargetCamera;
        KanjiWriter.TrailPrefab = TrailPrefab;
        KanjiWriter.AddOnWinPerfectEvent(new OnWinPerfect(WinPerfectEvent));
        KanjiWriter.AddOnWinAlmostEvent(new OnWinAlmost(WinAlmostEvent));
        KanjiWriter.AddOnWinPoorEvent(new OnWinPoor(WinPoorEvent));
        KanjiWriter.AddOnNewKanjiEvent(new OnNewKanji(NewKanjiEvent));

        KanjiWriter.NewKanji();
    }

    public void Clear()
    {
        KanjiWriter.ClearWriter();
    }
}
