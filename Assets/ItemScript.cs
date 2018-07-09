
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

       gameObject.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener(setNumber);
       
    }
	
	// Update is called once per frame
	void Update () {

    }


    private DataItem data;
    public DataItem Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }


    public void FindFileDir(int value)
    {
        if (checkDataError()) return;
        Text tt = null;
        OpenDialogDir ofn2 = new OpenDialogDir();
        ofn2.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区    

        switch (value)
        {
            case 1:
                ofn2.dirTitle = "设置源目录";
                tt = gameObject.transform.Find("BaseDirText").GetComponent<Text>();

                break;
            case 2:
                ofn2.dirTitle = "设置目标目录";
                tt = gameObject.transform.Find("TargetDirText").GetComponent<Text>();
                break;
            case 3:
                ofn2.dirTitle = "设置索引目录";
                tt = gameObject.transform.Find("LogText").GetComponent<Text>();
                break;
        }
 

        //ofn2.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // 新的样式,带编辑框    
        IntPtr pidlPtr = DllBox.SHBrowseForFolder(ofn2);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllBox.SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));
        Debug.Log(fullDirPath);//这个就是选择的目录路径。  

        if (tt == null) return;
        tt.text = fullDirPath;
        //for (int i = 20; i < tt.text.Length; i += 20)
        //    tt.text = tt.text.Insert(i, "\n");
        if (value == 1)
        {
            data.BaseDirPath = fullDirPath;

        }
        else if(value ==2)
        {
            
            data.TargetDirPath = fullDirPath;
        }
        else if(value==3)
        {
            data.FolderPath = fullDirPath;
        }
    }
    public void FindTxt()
    {
        if (checkDataError()) return;
        OpenDialogFile ofn = new OpenDialogFile();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "All Files\0*.*\0\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = Application.streamingAssetsPath;
        path = path.Replace('/', '\\');
        ofn.title = "选择索引文件";
        //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (DllBox.GetOpenFileName(ofn))
        {
            gameObject.transform.Find("LogText").GetComponent<Text>().text = ofn.file;
            data.TxtPath = ofn.file;
        }
    }
    
    public void setNumber(string content)
    {
        Debug.Log(content);
        if (checkDataError()) return;
        int i;
        if (int.TryParse(content, out i))
        {
            data.Number = i;
        }
        else
        {
            DllBox.MessageBox(IntPtr.Zero, "填正数", "佩奇", 0);
        }
    }

    private bool checkDataError()
    {
        if (data.id == 0)
        {
            DllBox.MessageBox(IntPtr.Zero, "为什么会没有ID，世界人民都震惊了！！！", "异常", 0);
            return true;
        }
        return false;
    }
}
