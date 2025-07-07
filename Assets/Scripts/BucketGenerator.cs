using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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

    private List<string> _imgFilePaths;

    void Start()
    {
        _imgFilePaths = new List<string>();
        _generatedBuckets = new GameObject[0];
        if (StatusDisplay == null)
            StatusDisplay = FindObjectOfType<BucketCountDisplay>().GetComponent<TextMeshProUGUI>();
        if (StartingDigitDisplay == null)
            StartingDigitDisplay = FindObjectOfType<StartingDigitDisplay>().GetComponent<TextMeshProUGUI>();
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

        // TODO: Load this list!
        _imgFilePaths = new List<string> { @"C:\Code\Personal\Art\SanFranciscoRush2049.png", @"C:\Code\Personal\Art\MortalKombatMythologiesSub-Zero.png" };

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
        var newGameObject = CreateNewBucketPrefab(currentXPosition, textSpacing);
        var loadedImage = LoadImageTexture(newGameObject, number);
        SetBucketText(newGameObject, number, loadedImage);
        
        return newGameObject;
    }

    private GameObject CreateNewBucketPrefab(float currentXPosition, float textSpacing)
    {
        var textPosition = new Vector3(currentXPosition + textSpacing, YPosition, 0);
        var textResource = Resources.Load(BucketText);
        var newGameObject = Instantiate(textResource, textPosition, Quaternion.identity) as GameObject;
        return newGameObject;
    }

    private bool LoadImageTexture(GameObject newGameObject, int number)
    {
        var imgSprite = newGameObject.GetComponentInChildren<SpriteRenderer>();
        //Debug.Log($"loading filepath {number} (index {number - 1}) from list of {_imgFilePaths.Count} filepaths");
        var filepath = number < _imgFilePaths.Count ? _imgFilePaths[number] : "";

        if (imgSprite == null || !System.IO.File.Exists(filepath))
        {
            if (imgSprite != null)
                imgSprite.color = new Color(0, 0, 0, 0);
            Debug.Log("Skipping loading texture!");
            return false;
        }

        imgSprite.color = new Color(1, 1, 1, 1);
        var textureData = System.IO.File.ReadAllBytes(filepath);
        var texture = new Texture2D(2, 2);
        texture.LoadImage(textureData);
        //imgSprite.texture = texture;
        //imgSprite.sprite.texture.LoadImage(textureData);

        var pivot = imgSprite.sprite.pivot;
        var pixelPerUnit = imgSprite.sprite.pixelsPerUnit;
        uint extrude = 1;
        
        imgSprite.sprite = Sprite.Create(texture, imgSprite.sprite.textureRect, pivot, pixelPerUnit, extrude, SpriteMeshType.Tight);
        imgSprite.sprite.name = $"Bucket{number + 1}-Img-Sprite";

        Debug.Log("Finished loading texture!");
        return true;
    }

    private void SetBucketText(GameObject newGameObject, int number, bool imageLoaded)
    {
        var textComponent = newGameObject.GetComponent<TextMeshPro>();
        textComponent.text = (number + _startingDigit).ToString();
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
