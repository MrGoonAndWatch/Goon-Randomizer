using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System.Linq;
using Input = UnityEngine.Input;

public class BucketGenerator : MonoBehaviour
{
    public TextMeshProUGUI StatusDisplay;
    public TextMeshProUGUI StartingDigitDisplay;
    [SerializeField]
    private TMP_InputField _inputField;

    public float StartXPosition = -12.0f;
    public float EndXPosition = 12.0f;
    public float YPosition = -3.5f;

    public int NumBuckets = 10;
    private const int MaxBuckets = 10;
    private const int MinBuckets = 2;
    private int _startingDigit = -1;

    private string _csvDirectory;
    // TODO: Eventually maybe add a file picker to select the csv?
    private const string CsvFilename = "ImagePaths.csv";

    private GameObject[] _generatedBuckets;

    private const string BucketWall = "Bucket-Wall";
    private const string BucketText = "Bucket-Text";

    private List<string> _imgFilePaths;

    void Start()
    {
        _csvDirectory = Application.persistentDataPath;
        _imgFilePaths = new List<string>();
        _generatedBuckets = new GameObject[0];
        if (StatusDisplay == null)
            StatusDisplay = FindFirstObjectByType<BucketCountDisplay>().GetComponent<TextMeshProUGUI>();
        if (StartingDigitDisplay == null)
            StartingDigitDisplay = FindFirstObjectByType<StartingDigitDisplay>().GetComponent<TextMeshProUGUI>();
        if (_inputField == null)
            _inputField = FindFirstObjectByType<FirstDigitInput>().GetComponent<TMP_InputField>();
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
            StatusDisplay.text = $"{NumBuckets} Buckets";
    }

    private void FlipStartingNum()
    {
        _startingDigit = (_startingDigit + 1) % 2;
        StartingDigitDisplay.text = $"Start at {_startingDigit}";
    }

    private void GenerateBuckets()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        CleanupOldObjects();

        var firstDigitSpecified = int.TryParse(_inputField?.text, out var firstDigitOffset);
        var csvFilePath = Path.Combine(_csvDirectory, CsvFilename);
        if (File.Exists(csvFilePath) && firstDigitSpecified)
            _imgFilePaths = File.ReadAllLines(csvFilePath).ToList();
        else
            _imgFilePaths = new List<string>();

        var length = EndXPosition - StartXPosition;

        var wallSpacing = length / NumBuckets;
        var textSpacing = wallSpacing / 2;

        var newObjects = new List<GameObject>();
        var currentXPosition = StartXPosition;
        for (var i = 0; i < NumBuckets - 1; i++)
        {
            newObjects.Add(GenerateText(currentXPosition, textSpacing, i, firstDigitOffset));

            currentXPosition += wallSpacing;

            var wallPosition = new Vector3(currentXPosition, YPosition, 0);
            var ballResource = Resources.Load(BucketWall);
            newObjects.Add(Instantiate(ballResource, wallPosition, Quaternion.identity) as GameObject);
        }

        newObjects.Add(GenerateText(currentXPosition, textSpacing, NumBuckets - 1, firstDigitOffset));

        _generatedBuckets = newObjects.ToArray();
    }

    private GameObject GenerateText(float currentXPosition, float textSpacing, int number, int firstDigitOffset)
    {
        var newGameObject = CreateNewBucketPrefab(currentXPosition, textSpacing);
        var loadedImage = LoadImageTexture(newGameObject, number, firstDigitOffset);
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

    private bool LoadImageTexture(GameObject newGameObject, int number, int firstDigitOffset)
    {
        number += _startingDigit + (firstDigitOffset * 10) - 1;
        var imgSprite = newGameObject.GetComponentInChildren<SpriteRenderer>();
        Debug.Log($"loading filepath {number} (index {number - 1}) from list of {_imgFilePaths.Count} filepaths");
        var filepath = number < _imgFilePaths.Count && number >= 0 ? _imgFilePaths[number] : "";

        if (imgSprite == null || !System.IO.File.Exists(filepath))
        {
            if (imgSprite != null)
                imgSprite.color = new Color(0, 0, 0, 0);
            else
                Debug.Log("imgSprite is null");
            Debug.Log("Skipping loading texture!");
            return false;
        }
        
        Debug.Log("Actually loading texture!");

        imgSprite.color = new Color(1, 1, 1, 1);
        var textureData = System.IO.File.ReadAllBytes(filepath);
        var texture = new Texture2D(2, 2);
        texture.LoadImage(textureData);
        //imgSprite.texture = texture;
        //imgSprite.sprite.texture.LoadImage(textureData);

        var pivot = imgSprite.sprite.pivot;
        var pixelPerUnit = imgSprite.sprite.pixelsPerUnit;
        uint extrude = 1;
        
        var textureRect = new Rect(0, 0, texture.width, texture.height);
        float xScale = 2.0f;
        if (textureRect.width > 0)
            xScale = 2 * imgSprite.sprite.textureRect.width / textureRect.width;
        float yScale = 2.0f;
        if (textureRect.height > 0)
            yScale = 2 * imgSprite.sprite.textureRect.height / textureRect.height;
        imgSprite.sprite = Sprite.Create(texture, textureRect, pivot, pixelPerUnit, extrude, SpriteMeshType.FullRect);
        imgSprite.sprite.name = $"Bucket{number + 1}-Img-Sprite";
        imgSprite.gameObject.transform.localScale = new Vector3(xScale, yScale, 1);
        imgSprite.gameObject.transform.localPosition = new Vector3(255, 255, 0);

        Debug.Log($"Finished loading texture!");
        return true;
    }

    private void SetBucketText(GameObject newGameObject, int number, bool imageLoaded)
    {
        var textComponent = newGameObject.GetComponent<TextMeshPro>();
        if (imageLoaded)
            textComponent.text = "";
        else
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
