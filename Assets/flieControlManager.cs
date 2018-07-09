using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;



[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogFile
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String dirTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

public class DllBox
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenDialogFile ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenDialogFile ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);

}
public struct DataItem{

    public int id;
    public String TargetDirPath;
    public String BaseDirPath;
    public String TxtPath;
    public String FolderPath;
    public int Number;

}
public class flieControlManager : MonoBehaviour
{


    public Text text;

    enum Mode
    {
        NON,
        TXT,
        FOLDER,
        DIRCET,
    }



    private Mode mode=Mode.NON;
    private Dictionary<int, GameObject> TxtItems = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> FolderItems = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> DirectItems = new Dictionary<int, GameObject>();


    private int dataItemCount = 0;
    private string LogPath = "d:/文档转移Log/";

    // Use this for initialization
    void Start()
    {
        Screen.SetResolution(600,450,false);
    }


    // Update is called once per frame
    void Update()
    {

    }


    public void ModelToggle(int value)
    {

        foreach(GameObject o in TxtItems.Values) o.SetActive(false);
        foreach (GameObject o in FolderItems.Values) o.SetActive(false);
        foreach (GameObject o in DirectItems.Values) o.SetActive(false);

        switch (value)
        {
            case 1:
                mode = Mode.TXT;
                foreach (GameObject o in TxtItems.Values) o.SetActive(true);
                break;
            case 2:
                mode = Mode.FOLDER;
                foreach (GameObject o in FolderItems.Values) o.SetActive(true);
                break;
            case 3:
                mode = Mode.DIRCET;
                foreach (GameObject o in DirectItems.Values) o.SetActive(true);
                break;

        }
    }

    public void addItem()
    {
        if (modeCheck()) return;
        String pname="";
        Dictionary<int, GameObject> items = TxtItems;
        switch (mode)
        {
            case Mode.TXT:
                pname = "txtmodel";
                items = TxtItems;
                break;
            case Mode.FOLDER:
                pname = "foldermodel";
                items = FolderItems;
                break;
            case Mode.DIRCET:
                pname = "directmodel";
                items = DirectItems;
                break;
        }
        if (pname == "") return;
        UnityEngine.Object preb = Resources.Load(pname, typeof(GameObject));
        GameObject model = Instantiate(preb) as GameObject;
        model.transform.parent = GameObject.Find("Content").GetComponent<Transform>();
        model.transform.localScale = new Vector3(1, 1, 1);
        dataItemCount++;
        DataItem data = new DataItem();
        data.id = dataItemCount;
        int p;
        int.TryParse(model.transform.Find("InputField").GetComponent<InputField>().text,out p);
        data.Number = p;
        model.GetComponent<ItemScript>().Data = data;
        items.Add(data.id,model);
    }

    private bool modeCheck()
    {
        if (mode == Mode.NON)
        {
            DllBox.MessageBox(IntPtr.Zero, "先选模式，臭嫑脸！~", "你干嘛", 0);
            return true;
        }
        return false;
    }

    public void StartPickUp()
    {

        if (modeCheck()) return;



        switch (mode)
        {
            case Mode.TXT:
                StartCoroutine(TxtPick());
                //StartCoroutine("TxtPick");
                break;
            case Mode.FOLDER:

               StartCoroutine( FolderPick());
                break;
            case Mode.DIRCET:
                DirectPick();

                break;

        }

    }

    private IEnumerator TxtPick()
    {
        if (TxtItems.Values.Count <= 0)
        {
            DllBox.MessageBox(IntPtr.Zero, "没东西执行毛线！", "XXX", 0);
            yield return null;
        }

        text.text = "开始执行";
        foreach (GameObject obj in TxtItems.Values)
        {

            DataItem data = obj.GetComponent<ItemScript>().Data;

            if (!Directory.Exists(data.BaseDirPath) || !Directory.Exists(data.TargetDirPath) ||
                !File.Exists(data.TxtPath)) continue;


            string[] baseNames = Directory.GetFiles(data.BaseDirPath);
            string[] targetNames = Directory.GetFiles(data.TargetDirPath);

            FileStream txtfs = new FileStream(data.TxtPath,FileMode.Open);
            StreamReader sr = new StreamReader(txtfs);
            Queue<string> txtNames = new Queue<string>();

            for (int c = 0; ;c++)
            {
                
                string s = sr.ReadLine();
                txtNames.Enqueue( s.Split('#')[0]);
                if (sr.EndOfStream) break;
            }
            sr.Close();
            txtfs.Close();
            Debug.Log("zhixinglema " + txtNames.Count +"123");
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            FileStream fs = new FileStream(LogPath + data.id + ".txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now);
            sw.WriteLine("源目录" + data.BaseDirPath);
            sw.WriteLine("目标目录" + data.TargetDirPath);
            sw.WriteLine("索引目录" + data.FolderPath);
            sw.WriteLine("索引文件" + data.TxtPath);
            sw.WriteLine("移动张数" + data.Number);
            int count = 0;

            while (txtNames.Count>0)
            {
                string txtname = txtNames.Dequeue();

                if (targetNames.Contains(data.TargetDirPath +"\\"+ txtname))
                {
                    sw.WriteLine("跳过文件" + txtname + "目标已存在");
                    continue;
                }
                count++;
                if (count > data.Number) break;
                bool copy = false;

                for (int j = 0; j < baseNames.Length; j++)
                {

                    if (baseNames[j].Equals(data.BaseDirPath + "\\" + txtname))
                    {
                        copy = true;
                        try
                        {
                            File.Copy(baseNames[j], data.TargetDirPath + baseNames[j].Replace(data.BaseDirPath, ""), false);
                            sw.WriteLine("根据" + txtname + "成功复制文件" + baseNames[j] + "到" + data.TargetDirPath);
                            text.text = "根据" + txtname + "成功复制文件" + baseNames[j] + "到" + data.TargetDirPath+"移动了"+count+"张";

                        }
                        catch
                        {
                            sw.WriteLine("复制失败了" + baseNames[j]);
                            text.text = "复制失败了" + baseNames[j];
                            copy = false;
                        }
                        yield return new WaitForSeconds(0.05f);

                    }
                }
                if (!copy) sw.WriteLine("失败了，没找到或不成功" + txtname);

            }
            sw.Close();
            fs.Close();

        }
        text.text = "执行完毕";

        StopCoroutine("TxtPick");

    }

    private IEnumerator  FolderPick()
    {
        if (FolderItems.Values.Count <= 0)
        {
            DllBox.MessageBox(IntPtr.Zero, "没东西执行毛线！","XXX", 0);
            yield return null; 
        }yield return null;
        text.text = "开始执行";
        foreach (GameObject obj in FolderItems.Values)
        {

            DataItem data = obj.GetComponent<ItemScript>().Data;

            if (!Directory.Exists(data.BaseDirPath)|| !Directory.Exists(data.TargetDirPath)||
                !Directory.Exists(data.FolderPath)) continue;


            string[] baseNames = Directory.GetFiles(data.BaseDirPath);
            string[] targetNames = Directory.GetFiles(data.TargetDirPath);
            string[] FolderNames = Directory.GetFiles(data.FolderPath);
                
            
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            FileStream fs = new FileStream(LogPath + data.id + ".txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now);
            sw.WriteLine("源目录" + data.BaseDirPath);
            sw.WriteLine("目标目录" + data.TargetDirPath);
            sw.WriteLine("索引目录" + data.FolderPath);
            sw.WriteLine("索引文件" + data.TxtPath);
            sw.WriteLine("移动张数" + data.Number);
            int count = 0;

            for(int i = 0; i < FolderNames.Length; i++)
            {
                if (targetNames.Contains(data.TargetDirPath + FolderNames[i].Replace(data.FolderPath, "")))
                {
                    sw.WriteLine("跳过文件"+FolderNames[i]+"目标已存在");
                    continue;
                }
                count++;
                if (count > data.Number) break;
                bool copy=false;
                
                for(int j = 0; j < baseNames.Length; j++)
                {
                   
                    if(baseNames[j].Equals(data.BaseDirPath + FolderNames[i].Replace(data.FolderPath, "")))
                    {
                        copy = true;
                        try
                        {
                            File.Copy(baseNames[j], data.TargetDirPath + baseNames[j].Replace(data.BaseDirPath, ""), false);
                            sw.WriteLine("根据"+FolderNames[i]+"成功复制文件"+baseNames[j]+"到"+data.TargetDirPath);
                            text.text = "根据" + FolderNames[i] + "成功复制文件" + baseNames[j] + "到" + data.TargetDirPath;

                        }
                        catch
                        {
                            sw.WriteLine("复制失败了"+baseNames[j]);
                            text.text = "复制失败了" + baseNames[j];
                            copy = false;
                        }
                        yield return new WaitForSeconds(0.05f);

                    }
                }
                if (!copy) sw.WriteLine("失败了，没找到或不成功"+FolderNames[i]);
                
            }
            sw.Close();
            fs.Close();

        }
        text.text = "执行完毕";
        Debug.Log("running");

        StopCoroutine("FolderPick");

    }


    private void DirectPick()
    {
        if (DirectItems.Values.Count <= 0)
        {
            DllBox.MessageBox(IntPtr.Zero, "没东西执行毛线！", "XXX", 0);
            return;
        }
        text.text = "开始执行";
        foreach (GameObject obj in DirectItems.Values)
        {
            DataItem data = obj.GetComponent<ItemScript>().Data;


            string[] baseNames = Directory.GetFiles(data.BaseDirPath);
            string[] targetNames = Directory.GetFiles(data.TargetDirPath);

            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
            FileStream fs = new FileStream(LogPath + data.id + ".txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now);
            sw.WriteLine("源目录" + data.BaseDirPath);
            sw.WriteLine("目标目录" + data.TargetDirPath);
            sw.WriteLine("索引目录" + data.FolderPath);
            sw.WriteLine("索引文件" + data.TxtPath);
            sw.WriteLine("移动张数" + data.Number);
            int count = 0;
            for (int j = 0; j < baseNames.Length; j++)
            {
                Debug.Log(targetNames);
                Debug.Log(data.TargetDirPath + baseNames[j].Replace(data.BaseDirPath, ""));

                if (targetNames.Contains(data.TargetDirPath + baseNames[j].Replace(data.BaseDirPath, "")))
                {
                    sw.WriteLine("跳过文件" + baseNames[j] + "目标已存在");
                    continue;
                }
                count++;
                if (count > data.Number) break;
                try
                {
                    File.Copy(baseNames[j], data.TargetDirPath + baseNames[j].Replace(data.BaseDirPath, ""), true);
                    sw.WriteLine("根据" + baseNames[j] + "成功复制文件" + baseNames[j] + "到" + data.TargetDirPath);
                }
                catch
                {
                    sw.WriteLine("复制失败了" + baseNames[j]);
                }
               
                
            }

            sw.Close();
            fs.Close();

        }
        text.text = "执行完毕";
    }

}
#region  旧代码
/*
 * 
 * using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Packager
{
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();
    private static string version = "1";

    ///-----------------------------------------------------------
    static string[] exts = { ".txt", ".xml", ".lua", ".assetbundle", ".json" };
    static bool CanCopy(string ext)
    {   //能不能复制
        foreach (string e in exts)
        {
            if (ext.Equals(e)) return true;
        }
        return false;
    }

    /// <summary>
    /// 载入素材
    /// </summary>
    static UnityEngine.Object LoadAsset(string file)
    {
        if (file.EndsWith(".lua")) file += ".txt";
        return AssetDatabase.LoadMainAssetAtPath("Assets/Resources/" + file);
    }

    /// <summary>
    /// 生成绑定素材
    /// </summary>
    [MenuItem("Game/Build Bundle Resource")]
    public static void BuildAssetResource()
    {
        Object mainAsset = null;        //主素材名，单个
        Object[] addis = null;     //附加素材名，多个
        string assetfile = string.Empty;  //素材文件名

        BuildAssetBundleOptions options = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle;

        BuildTarget target;
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
#if UNITY_5
            target = BuildTarget.iOS;
#else
            target = BuildTarget.iPhone;
#endif
        }
        else
        {
            target = BuildTarget.Android;
        }
        string assetPath = (Application.dataPath + "/StreamingAssets/Files/res/");
        if (!Directory.Exists(assetPath)) Directory.CreateDirectory(assetPath);

        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Atlas");



        ///-----------------------------生成共享的关联性素材绑定-------------------------------------
        BuildPipeline.PushAssetDependencies();


        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
        {
            List<Sprite> assets = new List<Sprite>();
            string path = assetPath + "/" + dirInfo.Name + ".assetbundle";
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                string LPath = allPath.Substring(allPath.IndexOf("Assets"));
                assets.Add(AssetDatabase.LoadAssetAtPath<Sprite>(LPath));
            }
            if (BuildPipeline.BuildAssetBundle(null, assets.ToArray(), path, options, target))
            {
            }
        }	


        ///-------------------------------刷新---------------------------------------
        BuildPipeline.PopAssetDependencies();

        HandleLuaFile();
        AssetDatabase.Refresh();
    }



    /// <summary>
    /// 处理Lua文件
    /// </summary>
    static void HandleLuaFile()
    {
        BuildTarget target;
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
#if UNITY_5
            target = BuildTarget.iOS;
#else
            target = BuildTarget.iPhone;
#endif
        }
        else
        {
            target = BuildTarget.Android;
        }
        string resPath = (Application.dataPath + "/StreamingAssets/Files/");
        string luaPath = resPath + "/xml/";

        //----------复制Lua文件----------------
        if (Directory.Exists(luaPath))
        {
            Directory.Delete(luaPath, true);
        }
        Directory.CreateDirectory(luaPath);

        paths.Clear(); files.Clear();
        string luaDataPath = Application.dataPath + "/UpdateFiles/".ToLower();
        Recursive(luaDataPath);
        foreach (string f in files)
        {
            if (f.EndsWith(".meta")) continue;
            string newfile = f.Replace(luaDataPath, "");
            string newpath = luaPath + newfile;
            string path = Path.GetDirectoryName(newpath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.Copy(f, newpath, true);
        }

        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine("version|"+version);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta")) continue;

            string md5 = md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close(); 
        fs.Close();

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename./Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
    private static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("md5file() fail, error:" + ex.Message);
        }
    }

}
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;


public delegate void AssetDownLoadHandler(string path);

public class DownLoadArgs
{
    private string _url;
    private string _fileSource;
    public DownLoadArgs(string url, string fileSource)
    {
        _url = url;
        _fileSource = fileSource;
    }
    public string DownLoadUrl { get { return _url; } }
    public string DownLoadFileSource { get { return _fileSource; } }

}


public class WebClientHelper
{
    private Thread thread;
    private Queue<DownLoadArgs> queues = new Queue<DownLoadArgs>();

    public event AssetDownLoadHandler downLoadEvent;

    private string DownLoadFileSource;
    private readonly object my_lock = new object();
    public WebClientHelper()
    {
        thread = new Thread(UpdateBegin);
    }

    public void DownLoadStart()
    {
        thread.Start();
    }


    private void UpdateBegin()
    {
        while (true)
        {
            UnityEngine.Debug.LogWarning("线程在执行>>");
            lock (my_lock)
            {

                if (queues.Count > 0)
                {
                    DownLoadArgs opration = queues.Dequeue();
                    try
                    {
                        DownLoadFileSource = opration.DownLoadFileSource;
                        WebClient client = new WebClient();
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChange);


                        client.DownloadFileAsync(new System.Uri(opration.DownLoadUrl), opration.DownLoadFileSource);
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.LogError(e.Message);
                    }



                }
            }

            Thread.Sleep(20);

        }


    }





    private void ProgressChange(object sender, DownloadProgressChangedEventArgs e)
    {
        string value = string.Format("{0}kb", (e.BytesReceived / 1024d).ToString("0.00"));
        UnityEngine.Debug.Log(value + "," + (e.BytesReceived / e.TotalBytesToReceive * 100).ToString() + "%");
        if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive && DownLoadFileSource != null)
        {
            dispathEvent(DownLoadFileSource);
            DownLoadFileSource = null;
        }

    }

    public void AddOperation(DownLoadArgs arg)
    {

        lock (my_lock)
        {
            queues.Enqueue(arg);
        }

    }
    private void dispathEvent(string path)
    {
        if (downLoadEvent != null)
        {
            downLoadEvent(path);
        }
    }

    public void Destory()
    {
        if (this.downLoadEvent != null)
        {
            this.downLoadEvent = null;
        }

        this.thread.Abort();
    }

}
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

public class UpdateScript : MonoBehaviour
{

    private WebClientHelper webClientHelper;
    private List<string> downloadFiles = new List<string>();
    // Use this for initialization
    void Start()
    {

        webClientHelper = new WebClientHelper();


    }
    // Update is called once per frame
    void Update()
    {

    }
    void Destroy()
    {
        webClientHelper.Destory();
    }


    #region 本地资源检查
    public void CheckExtractResource()
    {
        bool isExists = Directory.Exists(DataPath) && File.Exists(DataPath + "files.txt");

        if (isExists)
        {
            string[] prFiles = File.ReadAllLines(DataPath + "files.txt");  //解压目录
            string[] stFiles = File.ReadAllLines(AppContentPath() + "files.txt");  //streamAsset目录
            if (prFiles[0] != null && stFiles[0] != null)
            {
                string[] fs = prFiles[0].Split('|');
                string[] fs2 = stFiles[0].Split('|');
                if (Int32.Parse(fs[1]) >= Int32.Parse(fs2[1]))
                {
                    StartCoroutine(OnUpdateResource());
                    return;   //文件已经解压过了，自己可添加检查文件列表逻辑
                }

            }


        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }
    #endregion

    #region 解压
    IEnumerator OnExtractResource()
    {
        string resPath = AppContentPath(); //游戏包资源目录

        if (Directory.Exists(DataPath)) Directory.Delete(DataPath, true);
        Directory.CreateDirectory(DataPath);

        string infile = resPath + "files.txt";
        string outfile = DataPath + "files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);

        string message = "正在解包文件:>files.txt";
        Debug.Log(infile);
        Debug.Log(outfile);
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();
        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        for (int filecount = 1; filecount < files.Length; filecount++)
        {
            string[] fs = files[filecount].Split('|');
            infile = resPath + fs[0];  //
            outfile = DataPath + fs[0];

            message = "正在解包文件:>" + fs[0];
            Debug.Log("正在解包文件:>" + infile);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        message = "解包完成!!!";
        yield return new WaitForSeconds(0.1f);
        message = string.Empty;
        //释放完成，开始启动更新资源
        StartCoroutine(OnUpdateResource());
    }
    #endregion

    #region 更新

    private int UpdateFileCount = 0;

    /// <summary>
    /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
    /// </summary>
    IEnumerator OnUpdateResource()
    {
        OnResourceInited();
        yield break;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            OnResourceInited();
            yield break;
        }
        string dataPath = DataPath;  //数据目录
        string url = "http://192.169.1.30:85/";
        string message = string.Empty;
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string listUrl = url + "files.txt?v=" + random;
        Debug.LogWarning("LoadUpdate---->>>" + listUrl);

        WWW www = new WWW(listUrl); yield return www;
        if (www.error != null)
        {
            OnUpdateFailed(string.Empty);
            OnResourceInited();
            yield break;
        }
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
        File.WriteAllBytes(dataPath + "files.txt", www.bytes);
        string filesText = www.text;
        string[] files = filesText.Split('\n');
        UpdateFileCount = files.Length;
        for (int i = 1; i < files.Length; i++)
        {
            if (string.IsNullOrEmpty(files[i])) continue;
            string[] keyValue = files[i].Split('|');
            string f = keyValue[0];
            string localfile = (dataPath + f).Trim();
            string path = Path.GetDirectoryName(localfile);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileUrl = url + f + "?v=" + random;
            bool canUpdate = !File.Exists(localfile);
            if (!canUpdate)
            {
                string remoteMd5 = keyValue[1].Trim();
                string localMd5 = md5file(localfile);
                canUpdate = !remoteMd5.Equals(localMd5);
                if (canUpdate) File.Delete(localfile);
            }
            if (canUpdate)
            {   //本地缺少文件
                Debug.Log(fileUrl);
                message = "downloading>>" + fileUrl;


                AddDownload(fileUrl, localfile);
                while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }

            }
        }
        yield return new WaitForEndOfFrame();

        message = "更新完成!!";
        Debug.Log(message);
    }



    void OnUpdateFailed(string file)
    {
        string message = "更新失败!>" + file;
        Debug.Log(message);
    }

    /// <summary>
    /// 是否下载完成
    /// </summary>
    bool IsDownOK(string file)
    {
        return downloadFiles.Contains(file);
    }

    /// <summary>
    /// 线程下载
    /// </summary>
    void AddDownload(string url, string file)
    {     //线程下载

        webClientHelper.AddOperation(new DownLoadArgs(url, file));

        if (!IsDownLoadRuning)
        {
            BeginDownload();
        }

    }
    private bool IsDownLoadRuning = false;
    void BeginDownload()
    {
        IsDownLoadRuning = true;
        webClientHelper.downLoadEvent += new AssetDownLoadHandler(DownLoadHandler);
        webClientHelper.DownLoadStart();
    }
    private void DownLoadHandler(string path)
    {
        downloadFiles.Add(path);
    }


    #endregion




    public void UpdateStart()
    {
        CheckExtractResource(); //释放资源
        //webClientHelper.AddOperation(new DownLoadArgs("http://192.169.1.30:85/haha.xml", DataPath + "haha.xml"));
        //webClientHelper.downLoadEvent += new AssetDownLoadHandler(DownLoadHandler);
        //webClientHelper.DownLoadStart();
    }


    //更新之后初始化
    private void OnResourceInited()
    {
        webClientHelper.Destory();
    }

    #region 工具

    private string DataPath
    {
        get
        {
            string game = "test_download";
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }


            return "c:/" + game + "/";
        }
    }
    private string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.dataPath + "/" + "StreamingAssets" + "/" + "Files/";
                break;

        }
        return path;
    }
    private string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("md5file() fail, error:" + ex.Message);
        }
    }


    #endregion
}

 * 
 * 
 * 
 * */
#endregion