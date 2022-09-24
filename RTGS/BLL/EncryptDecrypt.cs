using System;
using System.Text;

namespace RTGS
{
    public static class EncryptDecrypt
    {
        public static string EncryptDecryptString(string szPlainText)
        {
            int szEncryptionKey = 200;
            StringBuilder szInputStringBuild = new StringBuilder(szPlainText);
            StringBuilder szOutStringBuild = new StringBuilder(szPlainText.Length);
            char Textch;
            for (int iCount = 0; iCount < szPlainText.Length; iCount++)
            {
                Textch = szInputStringBuild[iCount];
                Textch = (char)(Textch ^ szEncryptionKey);
                szOutStringBuild.Append(Textch);
            }
            return szOutStringBuild.ToString();
        }  

    }
}