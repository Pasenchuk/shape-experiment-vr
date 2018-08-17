using UnityEngine;
using System.IO;
using System.Collections.Generic;

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

    private int[] order;

    private string logFile;

    bool endShow = false;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }

    // Use this for initialization
    void Start()
    {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        var orderStr = LoadFile("input.txt").Split(' ');
        order = new int[orderStr.Length];
        for (int i = 0; i < order.Length; i++)
        {
            order[i] = int.Parse(orderStr[i]);
        }
        Debug.Log(order);
        Debug.Log(GetTime());

        logFile = "log " + GetTime() + ".csv";
        SaveFile(logFile, "Picture;Number;Phase;Subphase;Time");

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
        if (Time.time >= targetTime)
        {
            switch (currentPhase)
            {
                case Phase.ENTRY:
                    targetTime = Time.time + pictureCooldown;
                    currentPhase = Phase.PICTURE;
                    rend1.material.mainTexture = textures[order[pos]];
                    rend2.material.mainTexture = textures[order[pos]];
                    AppendLog("PICTURE");
                    break;
                case Phase.PICTURE:
                    targetTime = Time.time + pauseCooldown;
                    currentPhase = Phase.PAUSE;

                    rend1.material.mainTexture = blackTexture;
                    rend2.material.mainTexture = blackTexture;
                    AppendLog("PAUSE");
                    break;
                case Phase.PAUSE:
                    subPhase++;
                    if (subPhase < subPhasesCount)
                    {
                        targetTime = Time.time + pictureCooldown;
                        rend1.material.mainTexture = textures[order[pos]];
                        rend2.material.mainTexture = textures[order[pos]];
                        currentPhase = Phase.PICTURE;
                        AppendLog("PICTURE");
                    }
                    else
                    {
                        pos++;
                        if (pos < order.Length)
                        {
                            subPhase = 0;
                            targetTime = Time.time + entryCooldown;
                            rend1.material.mainTexture = blackTexture;
                            rend2.material.mainTexture = blackTexture;
                            currentPhase = Phase.ENTRY;
                            AppendLog("ENTRY");
                        }
                        else
                        {
                            rend1.material.mainTexture = endTexture;
                            rend2.material.mainTexture = endTexture;

                            if (!endShow)
                            {
                                AppendLog("END");
                                endShow = true;
                            }
                        }
                    }
                    break;
            }
        }
    }

    void AppendLog(string phase) {
        var ord = pos < order.Length ? order[pos] : -1;
        File.AppendAllText(Application.persistentDataPath + "/"+ logFile, string.Format("{0};{1};{2};{3};{4}\n", ord.ToString(), pos.ToString(), phase, subPhase.ToString(), GetTime()));
    }

    string GetTime()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }


    public void SaveFile(string name, string content)
    {
        string destination = Application.persistentDataPath + "/" + name;
        FileStream file;

        if (!File.Exists(destination))
        {
            file = File.Create(destination);
            file.Close();
        }

        StreamWriter writer = new StreamWriter(destination, true);
        Debug.Log(content);
        writer.WriteLine(content);
        writer.Flush();
        writer.Close();
    }

    public string LoadFile(string name)
    {
        string destination = Application.persistentDataPath + "/" + name;

        Debug.Log(destination);

        if (File.Exists(destination))
        {
            StreamReader reader = new StreamReader(destination);
            string result = reader.ReadToEnd();
            reader.Close();
            return result;
        }
        else
        {
            Debug.Log("File not found");
            string defaultRes = "1 5 13 10";
            SaveFile(name, defaultRes);
            return defaultRes;
        }
    }

    enum Phase
    {
        ENTRY, PICTURE, PAUSE
    }

}
