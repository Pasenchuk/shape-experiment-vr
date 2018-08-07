using UnityEngine;
using System.IO;

public class RorshahController : MonoBehaviour
{

    public GameObject panel1;
    public GameObject panel2;


    public float time;
    public float targetTime;

    public float entryCooldown = 2;
    public float pictureCooldown = 5;
    public float pauseCooldown = 3;
    public int subPhasesCount = 3;

    public Texture[] textures;
    public Texture blackTexture;
    public Texture endTexture;

    private Renderer rend1;
    private Renderer rend2;

    private Phase currentPhase = Phase.ENTRY;

    int pos = 0;

    private int subPhase = 0;


    private int target = 10;



    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log(Application.persistentDataPath + "file");
        Debug.Log(System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        rend1 = panel1.GetComponent<Renderer>();
        rend1.material.mainTexture = blackTexture;

        rend2 = panel2.GetComponent<Renderer>();
        rend2.material.mainTexture = blackTexture;

        targetTime = Time.time + entryCooldown;

    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time;
        if (Application.targetFrameRate != target)
            Application.targetFrameRate = target;
        if (Input.GetMouseButtonDown(0))
        {
            pos++;
            rend1.material.mainTexture = blackTexture;
            rend2.material.mainTexture = blackTexture;
            targetTime = Time.time + entryCooldown;
            currentPhase = Phase.ENTRY;

        }
        if (Time.time >= targetTime)
        {
            switch (currentPhase)
            {
                case Phase.ENTRY:
                    targetTime = Time.time + pictureCooldown;
                    currentPhase = Phase.PICTURE;
                    rend1.material.mainTexture = textures[pos % textures.Length];
                    rend2.material.mainTexture = textures[pos % textures.Length];
                    break;
                case Phase.PICTURE:
                    targetTime = Time.time + pauseCooldown;
                    currentPhase = Phase.PAUSE;
                    if (subPhase < subPhasesCount - 1)
                    {
                        rend1.material.mainTexture = blackTexture;
                        rend2.material.mainTexture = blackTexture;
                    }
                    else
                    {
                        rend1.material.mainTexture = endTexture;
                        rend2.material.mainTexture = endTexture;
                    }
                    break;
                case Phase.PAUSE:
                    subPhase++;
                    if (subPhase < subPhasesCount)
                    {
                        targetTime = Time.time + pictureCooldown;
                        rend1.material.mainTexture = textures[pos % textures.Length];
                        rend2.material.mainTexture = textures[pos % textures.Length];
                        currentPhase = Phase.PICTURE;
                    }
                    else
                    {
                        pos++;
                        subPhase = 0;
                        targetTime = Time.time + entryCooldown;
                        rend1.material.mainTexture = blackTexture;
                        rend2.material.mainTexture = blackTexture;
                        currentPhase = Phase.ENTRY;
                    }
                    break;
            }
        }
    }


     public void SaveFile(string name, string content)
    {
        string destination = Application.persistentDataPath + "/" + name;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        //file.w

        //GameData data = new GameData(currentScore, currentName, currentTimePlayed);
        //BinaryFormatter bf = new BinaryFormatter();
        //bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        //string path = "Assets/Resources/test.txt";

        ////Read the text from directly from the test.txt file
        //StreamReader reader = new StreamReader(path);
        //Debug.Log(reader.ReadToEnd());
        //reader.Close();
    }

    enum Phase
    {
        ENTRY, PICTURE, PAUSE
    }

}
