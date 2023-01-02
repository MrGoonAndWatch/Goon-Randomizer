using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BucketGenerator : MonoBehaviour
{
    public TextMeshProUGUI StatusDisplay;
    public TextMeshProUGUI StartingDigitDisplay;

    public float StartXPosition = -12.0f;
    public float EndXPosition = 12.0f;
    public float YPosition = -3.5f;

    public int NumBuckets = 10;
    private const int MaxBuckets = 10;
    private const int MinBuckets = 2;
    private int _startingDigit = -1;

    private GameObject[] _generatedBuckets;

    private const string BucketWall = "Bucket-Wall";
    private const string BucketText = "Bucket-Text";

    void Start()
    {
        _generatedBuckets = new GameObject[0];
        FlipStartingNum();
    }
    
    void Update()
    {
        ProcessInput();
        GenerateBuckets();
    }

    private void ProcessInput()
    {
        var updated = true;

        if ((Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) && NumBuckets < MaxBuckets)
            NumBuckets++;
        else if ((Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) && NumBuckets > MinBuckets)
            NumBuckets--;
        else
            updated = false;

        if (Input.GetKeyDown(KeyCode.Z))
            FlipStartingNum();

        if(updated)
            StatusDisplay.text = NumBuckets + " Buckets";
    }

    private void FlipStartingNum()
    {
        _startingDigit = (_startingDigit + 1) % 2;
        StartingDigitDisplay.text = "Start at " + _startingDigit;
    }

    private void GenerateBuckets()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        CleanupOldObjects();

        var length = EndXPosition - StartXPosition;

        var wallSpacing = length / NumBuckets;
        var textSpacing = wallSpacing / 2;

        var newObjects = new List<GameObject>();
        var currentXPosition = StartXPosition;
        for (var i = 0; i < NumBuckets - 1; i++)
        {
            newObjects.Add(GenerateText(currentXPosition, textSpacing, i));

            currentXPosition += wallSpacing;

            var wallPosition = new Vector3(currentXPosition, YPosition, 0);
            var ballResource = Resources.Load(BucketWall);
            newObjects.Add(Instantiate(ballResource, wallPosition, Quaternion.identity) as GameObject);
        }

        newObjects.Add(GenerateText(currentXPosition, textSpacing, NumBuckets - 1));

        _generatedBuckets = newObjects.ToArray();
    }

    private GameObject GenerateText(float currentXPosition, float textSpacing, int number)
    {
        var textPosition = new Vector3(currentXPosition + textSpacing, YPosition, 0);
        var textResource = Resources.Load(BucketText);
        var newGameObject = Instantiate(textResource, textPosition, Quaternion.identity) as GameObject;
        var textComponent = newGameObject.GetComponent<TextMeshPro>();
        textComponent.text = (number + _startingDigit).ToString();
        return newGameObject;
    }

    private void CleanupOldObjects()
    {
        foreach (var bucket in _generatedBuckets)
        {
            Destroy(bucket);
        }
        _generatedBuckets = new GameObject[0];
    }
}
