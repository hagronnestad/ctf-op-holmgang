using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AESSolver {
    class Program {

        // KEY in app
        static string cipherToMatch = "3FE58FE3DD35BBA4CD63D62E4FFFD8D2";

        static void Main(string[] args) {

            // Just some tests to match against the Java version
            //var cipherToMatch = "C3979173FC6F7750CF25F5934D4C4FB0"; // "sin"
            //var cipherToMatch = "4C003A7050150F1D5CD0FC14C8599EEB"; // "67÷(t"

            // Wrong move, the formula is never checked against the key in the
            // app, only the result is
            BruteForceFormulaByMatchingAgainstKey();

            // Finding the expected result
            // Bruteforce...
            BruteforceResult();
            // By just decrypting the key from the app 🤦‍
            DecryptKeyFromAppToFindExpectedResult();

            // The correct formula
            Console.WriteLine(AesEncrypt("168÷4"));


            return;
        }


        static void BruteForceFormulaByMatchingAgainstKey() {
            var lines = File.ReadAllLines(@"allcombinations3.txt");

            foreach (var l in lines) {
                var aes = AesEncrypt(l);
                if (aes == cipherToMatch) {
                    Console.WriteLine($"MATCH! {l}");
                    return;
                }
            }

            Console.WriteLine("BruteForceFormulaByMatcingAgainstKey: NO MATCH!");
        }


        static void BruteforceResult() {
            // Bruteforce "result" by matching against KEY from app
            // I just assumed that the result would be a whole number and
            // started out with 0-100000...
            //
            // This matches at i==42, but as described below this can easily be determined
            // by decrypting the KEY in the app instead of bruteforcing...
            for (int i = 0; i < 100000; i++) {
                var n = i;
                var str = $"{n:F0}";

                //Console.WriteLine(str);

                var aes = AesEncrypt(str);
                if (aes == cipherToMatch) {
                    Console.WriteLine($"Result MATCH! N: {n}, AES: {aes}");
                    return;
                }
            }
        }

        static void DecryptKeyFromAppToFindExpectedResult() {
            // Decrypt the key from the app to find the expected Calculator result
            // I didn't see this obvious step, because I misread the code at first as
            // described in the README and I was too focused on the formula
            Console.WriteLine("Key in app decrypted (expected result in calc): " +
                AesDecrypt(Convert.FromHexString("3FE58FE3DD35BBA4CD63D62E4FFFD8D2")));
        }

        static string AesEncrypt(string plain) {
			// jobbhoskriposnc3
			byte[] key = { 106, 111, 98, 98, 104, 111, 115, 107, 114, 105, 112, 111, 115, 110, 99, 51 };

			AesManaged tdes = new AesManaged();
            tdes.Key = key; //Encoding.UTF8.GetBytes("jobbhoskriposnc3");
			tdes.Mode = CipherMode.ECB;
			tdes.Padding = PaddingMode.PKCS7;
			ICryptoTransform crypt = tdes.CreateEncryptor();
			byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
			byte[] cipher = crypt.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
			//String encryptedText = Convert.ToBase64String(cipher);

			return Convert.ToHexString(cipher);
		}

        static string AesDecrypt(byte[] cipherBytes) {
            // jobbhoskriposnc3
            byte[] key = { 106, 111, 98, 98, 104, 111, 115, 107, 114, 105, 112, 111, 115, 110, 99, 51 };

            AesManaged tdes = new AesManaged();
            tdes.Key = key; //Encoding.UTF8.GetBytes("jobbhoskriposnc3");
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateDecryptor();
            byte[] plain = crypt.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            //String encryptedText = Convert.ToBase64String(cipher);

            return Encoding.UTF8.GetString(plain);
        }

    }

}
