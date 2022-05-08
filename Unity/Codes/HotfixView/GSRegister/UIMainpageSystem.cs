using System.IO;
using UnityEngine;
using UnityEngine.UI;
using BM;

namespace ET
{
    [ObjectSystem]
    public class UIMainpageAwakeSystem:AwakeSystem<UIMainpageComponent>
    {
        public override void Awake(UIMainpageComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.ProvinceInput = rc.Get<GameObject>("ProvinceInput");
            self.CityInput = rc.Get<GameObject>("CityInput");
            self.AreaInput = rc.Get<GameObject>("AreaInput");
            self.SchoolInput = rc.Get<GameObject>("SchoolInput");
            self.GenerateBtn = rc.Get<GameObject>("Generate");
            self.Error = rc.Get<GameObject>("Error");
            self.Read = rc.Get<GameObject>("Read");
            
            self.Read.GetComponent<Button>().onClick.AddListener(self.OnRead);
            self.GenerateBtn.GetComponent<Button>().onClick.AddListener(self.OnGenerate);
        }
    }
    [FriendClass(typeof(UIMainpageComponent))]
    public static class UIMainpageSystem
    {
        public static void OnGenerate(this UIMainpageComponent self)
        {
            if (self.ProvinceInput.GetComponent<InputField>().text==string.Empty || self.CityInput.GetComponent<InputField>().text==string.Empty||
                self.AreaInput.GetComponent<InputField>().text==string.Empty || self.SchoolInput.GetComponent<InputField>().text==string.Empty)
            {
                self.Error.GetComponent<Text>().text = "信息不得为空";
                return;
            }

            string data = self.ProvinceInput.GetComponent<InputField>().text + self.CityInput.GetComponent<InputField>().text +
                    self.AreaInput.GetComponent<InputField>().text + self.SchoolInput.GetComponent<InputField>().text;
            Debug.LogWarning($"data md5={VerifyHelper.GetMd5Hash(data)}");
            byte[] encryptData = VerifyHelper.CreateEncryptData(VerifyHelper.GetMd5Hash(data), "test");
            FileStream fs = new FileStream(Application.streamingAssetsPath + "/encrypt.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(encryptData);
            sw.Dispose();
            fs.Dispose();
            
        }

        public static void OnRead(this UIMainpageComponent self)
        {
            FileStream fs = new FileStream(Application.streamingAssetsPath + "/encrypt.txt", FileMode.OpenOrCreate,FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            VerifyHelper.GetDecryptDataAsync(sr.ReadLine(),null,"test".ToCharArray());
            sr.Dispose();
            fs.Dispose();
        }
    }
}