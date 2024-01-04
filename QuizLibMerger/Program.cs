// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hello, World!");

Course[] c = new Course[]
{
};

foreach (Course course in c)
{
    JObject root = new();
    DirectoryInfo di = new(course.dstname);
    DirectoryInfo di2 = new(course.examname);



    int count = 0;

    foreach (var file in di.GetFiles())
    {
        try
        {
            JObject o1 = JObject.Parse(File.ReadAllText(file.FullName));


            o1.Remove("status");
            o1.Remove("message");
            o1.Remove("date");
            JObject? o2 = o1.SelectToken("rt") as JObject;
            if(o2 == null) continue;
            o2.Remove("wrongQuestionNum");
            o2.Remove("correctQuestionNum");
            o2.Remove("paperTotalQuestionNum");
            o2.Remove("correctQuestionList");
            o2.Remove("wrongQuestionList");
            JArray? o3 = o2.SelectToken("allQuestionList") as JArray;
            foreach (JObject o4 in o3)
            {
                o4.Remove("analysisDto");
                o4.Remove("content");
                o4.Remove("createTime");
                o4.Remove("diff");
                o4.Remove("isCorrect");
                o4.Remove("isMarker");
                o4.Remove("parentId");
                o4.Remove("questionIndex");
                o4.Remove("studentResult");
                o4.Remove("result");
                o4.Remove("analysisUrl");
                o4.Remove("analysis");
                o4.Remove("fileType");
                o4.Remove("fileId");
                o4.Remove("dataId");
                o4.Remove("isManualReview");
                o4.Remove("questionUuID");
                o4.Remove("fileSuffix");
                o4.Remove("fileSize");
                o4.Remove("updateTime");
                o4.Remove("questionFileList");
                o4.Remove("questionDtoList");
                o4.Remove("chapterNam");
                o4.Remove("evalList");
                o4.Remove("aiQuestionKnowledgeAimList");
                o4.Remove("contentUnHtml");
                o4.Remove("isDisplay");
                JArray? o5 = o4.SelectToken("questionOptionList") as JArray;
                foreach (JObject o6 in o5)
                {
                    o6.Remove("content");
                    o6.Remove("sort");
                    o6.Remove("isFrame");
                    o6.Remove("paperId");
                }
                o5 = new JArray(o5.OrderBy(obj => (int)obj["id"]));
            }
            root.Merge(o1, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
            GC.Collect();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); continue; }
    }
    JArray a = root["rt"]["allQuestionList"] as JArray;
    foreach (var file in di2.GetFiles())
    {
        JObject o1 = JObject.Parse(File.ReadAllText(file.FullName));
        JArray o2 = o1["data"]["optionVos"] as JArray;
        foreach (JObject o4 in o2)
        {
            o4.Remove("content");
            o4.Remove("sort");
        }
        JObject o3 = new JObject
    {
        { "id", (int)o1["data"]["id"] },
        { "questionOptionList", o2 }
    };
        a.Add(o3);
        GC.Collect();
    }

    root["rt"]["allQuestionList"] = new JArray((root["rt"]["allQuestionList"] as JArray).OrderBy(obj => (int)obj["id"]).DistinctBy(p => p["id"]));

}

static string Encrypt(string sourceString, string key, string iv)
{
    try
    {
        byte[] btKey = Encoding.UTF8.GetBytes(key);
        byte[] btIV = Encoding.UTF8.GetBytes(iv);
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        using (MemoryStream ms = new MemoryStream())
        {
            byte[] inData = Encoding.UTF8.GetBytes(sourceString);
            try
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(inData, 0, inData.Length);

                    cs.FlushFinalBlock();
                }

                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return sourceString;
            }
        }
    }
    catch { }

    return "DES加密出错";
}


public struct Course
{
    public string dstname;
    public string examname;
    public string identifier;
}
