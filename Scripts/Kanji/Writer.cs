using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Writer : MonoBehaviour {

    public bool IsDebug = false;

    [SerializeField]
    private float Distance = 100.0f;

    [SerializeField]
    private float Depth = 10.0f;

    private bool previousFrameTouch = false;

    private List<GameObject> Objects = new List<GameObject>();

    private GameObject CurrentTrail;

    private List<List<Vector3>> Positions = new List<List<Vector3>>();

    private int StrokeCounter = 0;

    private List<List<Vector2>> Directions = new List<List<Vector2>>();

    public WriterManager writerManager;

    private bool isDrawing = false;

    // Update is called once per frame
    void Update() {
        Vector3 tempMousePos = Input.mousePosition;
        tempMousePos.z = Depth;
        Vector3 tempWorldMousePos = writerManager.TargetCamera.ScreenToWorldPoint(tempMousePos);

        if (tempMousePos.x < 128 || tempMousePos.x > 1777)
            return;


        if (Input.touches.Length > 0)
        {
            switch (Input.touches[0].phase)
            {
                case TouchPhase.Began:
                    BeginStroke(tempMousePos, tempWorldMousePos);
                    CurrentTrail.transform.position = tempWorldMousePos;
                    break;
                case TouchPhase.Moved:
                    CurrentTrail.transform.position = tempWorldMousePos;
                    DrawStroke(tempMousePos, tempWorldMousePos);
                    break;
                case TouchPhase.Ended:
                    CurrentTrail.transform.position = tempWorldMousePos;
                    EndStroke(tempMousePos, tempWorldMousePos);
                    break;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                BeginStroke(tempMousePos, tempWorldMousePos);
                CurrentTrail.transform.position = tempWorldMousePos;
            }
            else if (Input.GetMouseButton(0))
            {
                CurrentTrail.transform.position = tempWorldMousePos;
                DrawStroke(tempMousePos, tempWorldMousePos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                CurrentTrail.transform.position = tempWorldMousePos;
                EndStroke(tempMousePos, tempWorldMousePos);
            }
        }

        previousFrameTouch = Input.touchCount > 0;
    }

    void BeginStroke(Vector3 tempMousePos, Vector3 tempWorldMousePos)
    {
        isDrawing = true;
        GameObject parrent = new GameObject("Stroke " + StrokeCounter);
        Objects.Add(parrent);
        Positions.Add(new List<Vector3>() {tempMousePos});
        CurrentTrail = Instantiate(writerManager.TrailPrefab, tempWorldMousePos, Quaternion.identity) as GameObject;
        CurrentTrail.transform.parent = parrent.transform;
        //if (IsDebug)
           // (Instantiate(DataPointsPrefab, tempWorldMousePos, Quaternion.identity) as GameObject).transform.parent = parrent.transform;
    }

    void EndStroke(Vector3 tempMousePos, Vector3 tempWorldMousePos)
    {
        if (!isDrawing) return;
        isDrawing = false;
        if (Vector3.Distance(Positions[StrokeCounter].Last(), tempMousePos) > Distance)
        {
            Positions[StrokeCounter].Add(tempMousePos);
            //if (IsDebug)
              //  (Instantiate(DataPointsPrefab, tempWorldMousePos, Quaternion.identity) as GameObject).transform.parent = Objects.Last().transform;
        }

        List<Vector2> confirmDirections = new List<Vector2>();
        for (int i = 1; i < Positions[StrokeCounter].Count; i++)
        {
            Vector2 checkDirection = new Vector2(Positions[StrokeCounter][i].x - Positions[StrokeCounter][i-1].x, Positions[StrokeCounter][i].y - Positions[StrokeCounter][i-1].y);
            Vector2 result = HalfRoundVec2(checkDirection.normalized);
            if (confirmDirections.Count == 0)
                confirmDirections.Add(result);
            else if (confirmDirections.Last() != result)
                confirmDirections.Add(result);
        }

        if (confirmDirections.Count == 0)
        {
            Destroy(Objects.Last(), 0.2f);
            Positions.RemoveAt(StrokeCounter);
            return;
        }
        else if (confirmDirections.Count == 1 && Vector2.Distance(Positions[StrokeCounter][0], Positions[StrokeCounter][1]) < Distance)
        {
            Destroy(Objects.Last(), 0.2f);
            Positions.RemoveAt(StrokeCounter);
            return;
        }

        Directions.Add(new List<Vector2>(confirmDirections));
        StrokeCounter++;


        DefinePath(Directions);
    }

    void DrawStroke(Vector3 tempMousePos, Vector3 tempWorldMousePos)
    {
        if (!isDrawing) return;
        if (Vector3.Distance(Positions[StrokeCounter].Last(), tempMousePos) > Distance)
        {
            Positions[StrokeCounter].Add(tempMousePos);
            //if (IsDebug)
              //  (Instantiate(DataPointsPrefab, tempWorldMousePos, Quaternion.identity) as GameObject).transform.parent =
                //    Objects.Last().transform;
        }
    }

    void DefinePath(List<List<Vector2>> directions)
    {
        string res = "";
        if (directions == null)
        {
            //writerManager.ParseKanji("");
            Clear();
            return;
        }
        foreach (List<Vector2> item in directions)
        {
            res += item.Count;
            foreach (Vector2 dir in item)
                res += dir.ToString();
        }
        if (IsDebug)
            Debug.Log(res);

        writerManager.ParseKanji(directions.Count.ToString(), res);
    }

    Vector2 HalfRoundVec2(Vector2 vec)
    {
        float x = HalfRound(vec.x);
        float y = HalfRound(vec.y);

        if (Mathf.Abs(x) == 0.5f && Mathf.Abs(y) == 1)
            y *= 0.5f;

        if (Mathf.Abs(y) == 0.5f && Mathf.Abs(x) == 1)
            x *= 0.5f;

        return new Vector2(x, y);
    }

    float HalfRound(float value)
    {
        return Mathf.Round(value * 2) / 2.0f;
    }

    public void Undo()
    {

    }

    public void Clear()
    {
        foreach (var obj in Objects)
        {
            Destroy(obj);
        }
        StrokeCounter = 0;
        Objects = new List<GameObject>();
        Positions = new List<List<Vector3>>();
        Directions = new List<List<Vector2>>();
        previousFrameTouch = false;
    }
}
