using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;

public class LoadManager : MonoBehaviour
{

    public InputField field;
    public Text fieldText;

    private void Start()
    {
        ReadBuildProp();
    }

    //InputField의 텍스트에 메세지 설정
    public void SetText(string message)
    {
        field.text = message;
    }
    public void SetText(List<string> message)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < message.Count; i++)
        {
            builder.Append(message[i]).Append("\n");
        }
        field.text = builder.ToString();
    }

    //현재 클립보드에 문자 저장
    public void CopyText()
    {
        field.text.CopyToClipboard();
    }

    //게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }

    //핵심
    void ReadBuildProp()
    {
        FileInfo f;
        try
        {
            //파일 찾기
            f = new FileInfo("/system/build.prop");
        }catch(Exception e)
        {
            //못 찾았을 때 중단
            SetText("ERR: Message:" + e.Message + "\n\nStackTrace:\n" + e.StackTrace);
            fieldText.color = Color.red;
            return;
        }
        if (!f.Exists)
        {
            //파일 존재하지 않을 때 중단 (루트권한이 없는 경우도)
            SetText("ERR: /system/build.prop doesn't exists");
            fieldText.color = Color.red;
            return;
        }
        //텍스트 형태로 파일 가져오기
        List<string> lines;
        try
        {
            lines = GetTexts(f);
        }
        catch(UnauthorizedAccessException e)
        {
            SetText("cant access /system/build.prop due to permission.\n you are not rooted user.");
            fieldText.color = Color.green;
            return;
        }
        catch(Exception e)
        {
            SetText("ERR: Message:" + e.Message + "\n\nStackTrace:\n" + e.StackTrace);
            return;
        }
        //InputField의 text에 파일 내용 적기
        SetText(lines);
        //루팅 여부
        bool root = false;
        //모든 줄에 대해서 반복
        for (int i = 0; i < lines.Count; i++)
        {
            //주석 제거
            if (lines[i].StartsWith("#"))
            {
                //앞에 # 글자 지우기
                lines[i].Substring(1);
            }
            //문자열이 ro.modversion를 포함하는 경우 감지
            if (lines[i].Contains("ro.modversion"))
            {
                SetText(field.text + "\n\nTHIS DEVICE ROOTED!!!! (finded ro.modversion)");
                root = true;
            }

        }
        if (root)
        {
            fieldText.color = Color.magenta;
        }
        else
        {
            fieldText.color = Color.green;
        }
    }

    //C# 파일입출력
    //파일을 텍스트 형태로 받아오기
    public static List<string> GetTexts(FileInfo file)
    {

        FileStream fs = new FileStream(file.FullName, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        List<string> texts = new List<string>();
        string source = sr.ReadLine();
        while (source != null)
        {
            if (source.Length == 0)
            {
                continue;
            }
            texts.Add(source);
            source = sr.ReadLine();
        }
        sr.Close();
        fs.Close();
        return texts;
    }

}

//클립보드 관련 확장 클래스
//상기 CopyText()에 별개 클래스 없이 바로 GUIUtility.systemCopyBuffer = ~~~를 사용해도 됨
public static class ClipboardExtension
{
    public static void CopyToClipboard(this string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
}
