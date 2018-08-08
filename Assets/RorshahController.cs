using UnityEngine;
using System.IO;
using System.Collections;

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

    private ArrayList log = new ArrayList();

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
                    rend1.material.mainTexture = textures[order[pos]];
                    rend2.material.mainTexture = textures[order[pos]];
                    break;
                case Phase.PICTURE:
                    targetTime = Time.time + pauseCooldown;
                    currentPhase = Phase.PAUSE;

                    rend1.material.mainTexture = blackTexture;
                    rend2.material.mainTexture = blackTexture;
                    break;
                case Phase.PAUSE:
                    subPhase++;
                    if (subPhase < subPhasesCount)
                    {
                        targetTime = Time.time + pictureCooldown;
                        rend1.material.mainTexture = textures[order[pos]];
                        rend2.material.mainTexture = textures[order[pos]];
                        currentPhase = Phase.PICTURE;
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
                        }
                        else
                        {

                            rend1.material.mainTexture = endTexture;
                            rend2.material.mainTexture = endTexture;
                        }
                    }
                    break;
            }
        }
    }


    public void SaveFile(string name, string[] content)
    {
        string destination = Application.persistentDataPath + "/" + name;
        FileStream file;

        if (!File.Exists(destination))
        {
            file = File.Create(destination);
            file.Close();
        }

        StreamWriter writer = new StreamWriter(destination);
        Debug.Log(content);
        foreach (string s in content)
        {
            Debug.Log(s);
            writer.WriteLine();
        }
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
            SaveFile(name, new string[1] { defaultRes });
            return defaultRes;
        }
    }

    enum Phase
    {
        ENTRY, PICTURE, PAUSE
    }

}
