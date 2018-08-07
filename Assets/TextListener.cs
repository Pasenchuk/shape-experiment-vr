using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextListener : MonoBehaviour
{

    public Mesh[] meshes;

    string text = "";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }


    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.type == EventType.KeyDown && e.keyCode != 0)
        {
            var code = e.keyCode;
            switch ((int)code)
            {
                case 8:
                    text = text.Substring(0, text.Length - 1);
                    break;
                case 32:
                    text += ' ';
                    break;
                case 27:
                    text = "";
                    break;
                case 13:
                    var hash = Sha512Sum(text);
                    foreach (int i in hash) Debug.Log(i);
                    text = BytesToHex(hash);
                    for (int i = 0; i < 16; i++)
                    {
                        var offset = i * 4;
                        var color = new Color(hash[offset] / 255f, hash[offset + 1] / 255f, hash[offset + 2] / 255f, 1);
                        var mesh = meshes[hash[offset + 3] % 8];
                        var child = this.gameObject.transform.GetChild(i);

                        Renderer rend = child.GetComponent<Renderer>();

                        //Set the main Color of the Material to green
                        rend.material.color = color;

                        child.GetComponent<MeshFilter>().sharedMesh = mesh;
                    }
                    break;
                default:
                    text += code;
                    break;
            }

            Debug.Log(code);
            Debug.Log((int)code);
        }
        GetComponent<TextMesh>().text = text;

    }

    public byte[] Sha512Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(text);

        // encrypt bytes
        System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
        byte[] hashBytes = sha512.ComputeHash(bytes);

        return hashBytes;
    }

    public string BytesToHex(byte[] hashBytes)
    {
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}
