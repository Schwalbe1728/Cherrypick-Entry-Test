using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif
using UnityEngine;

public class MapSerializationScript : MonoBehaviour
{
    private const string FileFilter = "MAP files (*.map)|*.map";

    //TODO
    public bool SaveMap(char[,] mapToSave)
    {
        bool result = false;
        string path = UnityEngine.Application.persistentDataPath + @"/map1.map";

        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        path = StartSaveFileDialog();
        #endif

        string mapString = MapTemplateToString(mapToSave);

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write(mapString);
            result = true;
        }        

        return result;
    }

    //TODO
    public bool LoadMap(out char[,] map)
    {
        string textAssetString = "";
        string path = UnityEngine.Application.persistentDataPath + @"/map1.map";
        bool result = false;

        map = null;

        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        path = StartOpenFileDialog();
        #endif

        if (File.Exists(path))
        {
            using (StreamReader sr = new StreamReader(path))
            {
                textAssetString = sr.ReadToEnd();
                map = StringToMapTemplate(textAssetString);

                StringBuilder sb = new StringBuilder();

                for(int x = 0; x < map.GetLength(0); ++x)
                {
                    for (int y = 0; y < map.GetLength(1); ++y)
                    {
                        sb.Append(map[x, y]);
                    }
                    sb.AppendLine();
                }
                Debug.Log(sb.ToString());

                result = true;
            }
        }

        return result;
    }

    private string MapTemplateToString(char[,] mapToSave)
    {
        StringBuilder sb = new StringBuilder();

        for(int x = 0; x < mapToSave.GetLength(0); ++x)
        {
            for (int y = 0; y < mapToSave.GetLength(1); ++y)
            {
                sb.Append(mapToSave[x, y]);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private char[,] StringToMapTemplate(string mapString)
    {
        string[] lines = mapString.Split(new char[] { '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
        char[,] result = new char[lines.Length, lines.Length];

        for(int y = 0; y < lines.Length; ++y)
        {
            for(int x = 0; x < lines[y].Length; ++x)
            {
                result[x, y] = lines[x][y];
            }
        }
        
        return result;
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    private string StartOpenFileDialog()
    {
        return StartFileDialog<OpenFileDialog>();
    }

    private string StartSaveFileDialog()
    {
        return StartFileDialog<SaveFileDialog>();
    }

    private string StartFileDialog<T>() where T: FileDialog, new()
    {
        string resultPath = "";

        using (T ofd = new T())
        {
            ofd.Filter = FileFilter;

            switch (ofd.ShowDialog())
            {
                case DialogResult.OK:
                    Debug.Log(ofd.FileName);
                    resultPath = ofd.FileName;
                    break;
            }
        }

        return resultPath;
    }
#endif
}
