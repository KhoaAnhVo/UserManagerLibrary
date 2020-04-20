using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Security.Cryptography;
using System.Linq;


namespace UserMangager
{

    public class UserManager
    {
        public UserManager()
        {

        }
        public UserManager(string connectionstringdb)
        {
            ConnectionStringDB = connectionstringdb;
        }
        public string _ID_User { get; set; } = "1";
        public string _AccountName { get; set; } = "admin";
        public string _FullName { get; set; } = "Vo Anh Khoa";
        public string _StaffCode { get; set; } = "123";
        public string _Email { get; set; } = "voanhkhoa@gmail.com";
        public string _Pass { get; set; } = "admin123";
        public string _CryptPass { private get; set; } = "";
        public string _Permission { get; set; } = Permission.Admin.ToString();
        public string _DateCreate { get; set; } = DateTime.Now.ToString();
        public string _Enable { get; set; } = "Enabale or not";
        public string _Status { get; set; } = "Conection or not";
        //public string _HashPass { get; set; } = "";

        public string ConnectionStringDB { get; set; } = "Data Source=localhost;Initial Catalog=Syngenta_test;User ID=Admin_Khoa;Password=khoanhvo";

        public bool GetStatusLogin { get; private set; } = false;
        //
        public enum Permission
        {
            User,
            Admin,
            LineManager,
            SiftLeader,
            Technician,
            OP,
            EOP
        }
        //Hashtable buffUserInfo = new Hashtable(10);


        //Menthods
        /// <summary>
        /// Tạo user dựa trên các thông số cài đặt
        /// </summary>
        public void CreateUser()
        {
            using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
            {
                _CryptPass = CryptData.StringCipher.Encrypt(_Pass, _AccountName);

                StringBuilder sb = new StringBuilder();
                sb.Append(@"    INSERT INTO TBL_UserInfo(_AccountName, _FullName, _StaffCode, _Email, _Pass, _HashPass, _Permission, _DateCreate, _Enable,_Status )            ");
                sb.Append(@"    VALUES                  ('@AccountName','@FullName','@StaffCode','@Email','@Pass','@HashPass', '@Permission', '@DateCreate','@Enable','@Status');     ");

                sb.Replace("@AccountName", _AccountName);
                sb.Replace("@FullName", _FullName);
                sb.Replace("@StaffCode", _StaffCode);
                sb.Replace("@Email", _Email);
                sb.Replace("@Pass", _Pass);
                sb.Replace("@HashPass", _CryptPass);
                sb.Replace("@Permission", _Permission);
                sb.Replace("@DateCreate", _DateCreate);
                sb.Replace("@Enable", _Enable);
                sb.Replace("@status", _Status);
                sql.ExcuteQuery(sb);
            }
        }



        /// <summary>
        /// Login by Account user and password. Return true if login sucessfull!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool Login(string user, string pass)
        {
            bool result = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT _HashPass FROM TBL_UserInfo WHERE _AccountName = '@user';");
            sb.Replace("@user", user);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(sb.ToString());
                    string hashpass = data.Rows[0].Field<string>(0);
                    string EnCryptPass = CryptData.StringCipher.Decrypt(hashpass, user);
                    if (pass == EnCryptPass)
                        result = true;
                    _AccountName = user;
                    GetStatusLogin = result;
                    UpdateFieldUserLogin();
                }
            }
            catch
            {
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateFieldUserLogin()
        {
            DataTable data = GetUserInfoByLongin();
            try
            {
                _ID_User = data.Rows[0][0].ToString();
                _AccountName = data.Rows[0][1].ToString();
                _FullName = data.Rows[0][2].ToString();
                _StaffCode = data.Rows[0][3].ToString();
                _Email = data.Rows[0][4].ToString();
                _Pass = data.Rows[0][5].ToString();
                _CryptPass = data.Rows[0][6].ToString();
                _Permission = data.Rows[0][7].ToString();
                _DateCreate = data.Rows[0][8].ToString();
                _Enable = data.Rows[0][9].ToString();
            }
            catch { }
        }

        public bool CheckAccountExist(string user, string pass)
        {
            bool result = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT _HashPass FROM TBL_UserInfo WHERE _AccountName = '@AccountName';");
            sb.Replace("@AccountName", user);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(sb.ToString());
                    string hashpass = data.Rows[0].Field<string>(0);
                    string EnCryptPass = CryptData.StringCipher.Decrypt(hashpass, user);
                    if (pass == EnCryptPass)
                        result = true;
                    //_AccountName = user;
                }
            }
            catch (Exception ex)
            {
            }
            //GetStatusLogin = result;
            return result;
        }

        public bool CheckAccountExist(string user)
        {
            bool result = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(0) FROM TBL_UserInfo WHERE _AccountName = '@AccountName';");
            sb.Replace("@AccountName", user);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(sb.ToString());
                    int count = data.Rows[0].Field<int>(0);
                    if (count >= 1)
                    {
                        result = true;
                    }
                    //_AccountName = user;
                }
            }
            catch (Exception ex)
            {
            }
            //GetStatusLogin = result;
            return result;
        }

        /// <summary>
        ///Logout 
        /// </summary>
        public void Logout()
        {
            GetStatusLogin = false;
            _Status = "LogOut";
        }

        /// <summary>
        /// Get infomation of the user login
        /// </summary>
        /// <returns></returns>
        public DataTable GetUserInfoByLongin()
        {
            if (GetStatusLogin)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM TBL_UserInfo WHERE _AccountName = '@AccountName';");
                sb.Replace("@AccountName", _AccountName);
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(sb.ToString());
                    _Status = "Login";
                    GetStatusLogin = true;
                    return data;
                }
            }
            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllUserInfo()
        {
            if (GetStatusLogin)
            {
                string query = "SELECT * FROM TBL_UserInfo";

                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(query);
                    return data;
                }
            }
            return null;
        }

        public void ForgotPasswordInit()
        {

        }

        public void ForgotPasswordFinish()
        {

        }


        /// <summary>
        /// Đổi mật khẩu thông qua 3 thông số chính. Acount, oldpass, newpass
        /// Hành động sau khi đổi mật khẩu sẽ thoát ra ngoài.
        /// -1: Error / 1: thành công.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public int ChangePassword(string accountName, string oldPass, string newPass)
        {
            int result = -1;
            if (!CheckAccountExist(accountName, oldPass)) //kiểm tra tài khoảng và mật khẩu cũ có tồn tại hay ko chưa?
            {
                result = -1; //Giá trị có nghĩa tài khoảng hoặc mật khẩu không đúng.
            }
            else
            {
                //thực hiện update mật khẩu tại đây.
                string hashPass = CryptData.StringCipher.Encrypt(newPass, accountName); //má hóa mật khẩu mới.
                StringBuilder query = new StringBuilder();
                query.Append("UPDATE TBL_UserInfo set _Pass = '@pass',_HashPass = '@hashpass'");
                query.Append("WHERE _AccountName ='@AccountName';");
                query.Replace("@AccountName", accountName);
                query.Replace("@pass", newPass);
                query.Replace("@hashpass", hashPass);
                using (SqlServerMethod sqlMethod = new SqlServerMethod(ConnectionStringDB))
                {
                    sqlMethod.ExcuteQuery(query);
                }

                if (CheckAccountExist(accountName, newPass)) // kiểm tra mật khẩu mới có hoạt động hay ko
                {
                    result = 1;
                }
            }
            return result;
        }

        public int UpdateUser(string value, string column, string id_user)
        {
            int result = -1;
            StringBuilder sb = new StringBuilder();
            sb.Append("     UPDATE TBL_UserInfo SET @column = '@value' WHERE _ID_User = @id_user;     ");
            sb.Replace("@column", column);
            sb.Replace("@value", value);
            sb.Replace("@id_user", id_user);

            try
            {
                using (
                    SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    sql.ExcuteQuery(sb);
                }
            }
            catch (Exception)
            {
                result = -1;
            }
            result = CheckValueExisted(value, column, id_user);
            return result;
        }


        /// <summary>
        /// Check exist value at column
        /// return: -1 - lỗi không kiểm tra được, 1 - giá trị đã tồn tại, 0 - giá trị không tồn tại
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public int CheckValueExisted(string value, string column)
        {
            int result = -1;
            StringBuilder query = new StringBuilder();
            query.Append("    SELECT * FROM TBL_UserInfo WHERE @column = '@value';     ");
            query.Replace("@column", column);
            query.Replace("@value", value);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(query.ToString());
                    int icheck = data.Rows.Count;
                    if (icheck == 0)
                    {
                        result = 0; //giá trị không tồn tại
                    }
                    else if (icheck > 0)
                    {
                        result = icheck; //giá trị tồn tại
                    }
                }
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <param name="iduser"></param>
        /// <returns></returns>
        public int CheckValueExisted(string value, string column, string id_user)
        {
            int result = -1;
            StringBuilder query = new StringBuilder();
            query.Append("    SELECT * FROM TBL_UserInfo WHERE @column = '@value' AND _ID_User = '@id_user';    ");
            query.Replace("@column", column);
            query.Replace("@value", value);
            query.Replace("@id_user", id_user);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(query.ToString());
                    int icheck = data.Rows.Count;
                    if (icheck == 0)
                    {
                        result = 0; //giá trị không tồn tại
                    }
                    else if (icheck == 1)
                    {
                        result = icheck; //giá trị tồn tại
                    }
                }
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }



        public DataTable SearchUser(string infoSearch, string collum)
        {
            DataTable result = null;
            StringBuilder query = new StringBuilder();
            query.Append("     SELECT * FROM TBL_UserInfo WHERE @collum LIKE '%@infoSearch%';     ");
            query.Replace("@collum", collum);
            query.Replace("@infoSearch", infoSearch);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    DataTable data = sql.GetDataSqlToTable(query.ToString());
                    result = data;
                }
            }
            catch { }
            return result;
        }
        /// <summary>
        /// -1  : lỗi
        /// 0   : Xóa không thành công
        /// 1   : Xóa tài khoản thành công
        /// </summary>
        /// <param name="account_name"></param>
        /// <returns></returns>
        public int DeleteUser(string account_name)
        {
            int result = -1;
            StringBuilder query = new StringBuilder();
            query.Append("   DELETE FROM TBL_UserInfo WHERE _AccountName = '@accountname';   ");
            query.Replace("@accountname", account_name);
            try
            {
                using (SqlServerMethod sql = new SqlServerMethod(ConnectionStringDB))
                {
                    sql.ExcuteQuery(query);
                }
            }
            catch (Exception)
            {
                result = -1;
            }

            if (CheckAccountExist(account_name))
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return result;
        }
    }

    /// <summary>
    /// Xử lý giao tiếp với Database
    /// </summary>
    class SqlServerMethod : IDisposable
    {
        //Tạo thông tin kết nối đến csdl 
        SqlConnectionStringBuilder SqlBuilder = new SqlConnectionStringBuilder();

        public void SetConnStringDB(string SqlConnectionString)
        {
            SqlBuilder.ConnectionString = SqlConnectionString;
        }

        public SqlServerMethod()
        {
            SqlBuilder.DataSource = "localhost";
            SqlBuilder.UserID = "Admin_Khoa";
            SqlBuilder.Password = "khoanhvo";
            SqlBuilder.InitialCatalog = "Syngenta_test";
        }

        public SqlServerMethod(string ConnectionString)
        {
            SqlBuilder.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// Excute query cmd form text
        /// </summary>
        /// <param name="cmd_txt">Text Command</param>
        public bool ExcuteQuery(StringBuilder cmd_txt)
        {
            bool result;
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    conn.Open(); // thuc hien ket noi voi sql
                    using (SqlCommand cmd = new SqlCommand(cmd_txt.ToString(), conn))
                    {
                        cmd.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Excute query cmd from _.sql file save in direction source folder..\bin\Debug
        /// </summary>
        /// <param name="query_file_name">File .sql name</param>
        /// <returns></returns>
        public bool ExcuteQuery(string query_file_name)
        {
            bool result;
            //get direction to source code
            string Path = Environment.CurrentDirectory;
            string cmd_txt = File.ReadAllText(Path + @"\" + query_file_name);
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    conn.Open(); // thuc hien ket noi voi sql
                    using (SqlCommand cmd = new SqlCommand(cmd_txt, conn))
                    {
                        cmd.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (SqlException ex)
            {
                //MessageBox.Show(ex.ToString());
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get list of tables in database
        /// </summary>
        /// <returns></returns>
        public List<string> GetListTableNameDB()
        {
            List<string> result = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    string cmd_txt = string.Format(@"select TABLE_NAME from information_schema.tables;");
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(cmd_txt, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return result;
        }

        //Sql data to Datatable
        public DataTable GetDataSqlToTable(string query)
        {
            DataTable result = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(SqlBuilder.ConnectionString))
                {
                    //string cmd_txt = string.Format(@"select * from information_schema.tables;");
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(result);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

            }
            return result;
        }

        //get all collums in table
        public DataTable GetListCollumName(string tableName)
        {
            var result = new DataTable();
            StringBuilder query = new StringBuilder();
            query.Append("    SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'@tablename';    ");
            query.Replace("@tablename", tableName);
            result = GetDataSqlToTable(query.ToString());
            return result;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SqlServerMethod()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    class CryptData
    {
        #region vd1 Encrypt và decrypt MD% với mật khẩu
        public string key1 { get; set; } = "j83ud7snc0jd82h7dje7hde2";

        //static void Main(string[] args)
        //{
        //    var text = "kjdhucgdtehsnchdushc";
        //    Console.WriteLine(text);

        //    var cipher = Encrypt(text);
        //    Console.WriteLine(cipher); Console.ReadKey();

        //    Console.ReadKey();

        //    text = Decrypt(cipher);
        //    Console.WriteLine(text);

        //    Console.ReadKey();
        //}
        public string Encrypt_Md5(string text)
        {

            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key1));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;
                    //Console.WriteLine(Convert.ToString(tdes.Key));
                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        //byte[] bytes = transform.TransformFinalBlock(textBytes, 0, 20);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                        //return Convert.ToBase64String(bytes, 0, 20);
                    }
                }
            }
        }

        public string Decrypt_Md5(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key1));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        //byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, 20);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        //public  string Encrypt_Sha1(string cipher)
        //{
        //    using (var Sha1 = new SHA1CryptoServiceProvider())
        //    {
        //        using (var tdes = new TripleDESCryptoServiceProvider())
        //        {
        //            byte[] bs = System.Text.Encoding.UTF8.GetBytes(key1);
        //            tdes.Key = Sha1.ComputeHash(bs);
        //            tdes.Mode = CipherMode.ECB;
        //            tdes.Padding = PaddingMode.PKCS7;
        //            using (var transform = tdes.CreateEncryptor())
        //            {
        //                byte[] cipherBtyes = Convert.FromBase64String(cipher);
        //                byte[] bytes = transform.TransformFinalBlock(cipherBtyes, 0, cipherBtyes.Length);
        //                return UTF8Encoding.UTF8.GetString(bytes);
        //            }
        //        }
        //    }
        //}
        //public  string Decrypt_Sha1(string cipher)
        //{
        //    using (var Sha1 = new SHA1CryptoServiceProvider())
        //    {
        //        using (var tdes = new TripleDESCryptoServiceProvider())
        //        {
        //            tdes.Key = Sha1.ComputeHash(UTF8Encoding.UTF8.GetBytes(key1));
        //            tdes.Mode = CipherMode.ECB;
        //            tdes.Padding = PaddingMode.PKCS7;
        //            using (var transform = tdes.CreateDecryptor())
        //            {
        //                byte[] cipherBtyes = Convert.FromBase64String(cipher);
        //                byte[] bytes = transform.TransformFinalBlock(cipherBtyes, 0, cipherBtyes.Length);
        //                return UTF8Encoding.UTF8.GetString(bytes);
        //            }
        //        }
        //    }
        //}



        #endregion

        #region Sử dụng thuật toán mã hóa Rijndael rất hay.tạo ra nhiều mã khác nhau trên cùng một mã góc
        //https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

        public static class StringCipher
        {
            // This constant is used to determine the keysize of the encryption algorithm in bits.
            // We divide this by 8 within the code below to get the equivalent number of bytes.
            private const int Keysize = 256;//256

            // This constant determines the number of iterations for the password bytes generation function.
            private const int DerivationIterations = 1000;

            public static string Encrypt(string plainText, string passPhrase)
            {
                // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = Generate256BitsOfRandomEntropy();
                var ivStringBytes = Generate256BitsOfRandomEntropy();
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256; //256
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                    var cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }

            public static string Decrypt(string cipherText, string passPhrase)
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;//256
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }

            private static byte[] Generate256BitsOfRandomEntropy()
            {
                var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    // Fill the array with cryptographically secure random bytes.
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }
        }

        #endregion

        #region MD5 and SHA Encode not Decode

        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Please enter a string to EncodeMD5:");
        //    string StringMD5 = Console.ReadLine();
        //    Console.WriteLine("Please enter a string to EncodeSHA:");
        //    string StringSHA = Console.ReadLine();
        //    Console.WriteLine("");

        //    Console.WriteLine("Your EncodeMD5 string is:");
        //    string encryptedstring = EncodeSHA1(StringSHA);
        //    Console.WriteLine(encryptedstring);
        //    Console.WriteLine("");

        //    Console.WriteLine("Your EncodeSHA string is:");
        //    string decryptedstring = EncodeMD5(StringMD5);
        //    Console.WriteLine(decryptedstring);
        //    Console.WriteLine("");

        //    Console.WriteLine("Press any key to exit...");
        //    Console.ReadLine();
        //}

        public static string EncodeMD5(string pass)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);

            bs = md5.ComputeHash(bs);

            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)

            {

                s.Append(b.ToString("x1").ToLower());

            }

            pass = s.ToString();

            return pass;

        }
        private static string EncodeSHA1(string pass)

        {

            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);

            bs = sha1.ComputeHash(bs);

            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)

            {

                s.Append(b.ToString("x1").ToLower());

            }

            pass = s.ToString();

            return pass;

        }
        public static string EncodeSHA256(string pass)
        {

            SHA256CryptoServiceProvider sha1 = new SHA256CryptoServiceProvider();

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);

            bs = sha1.ComputeHash(bs);

            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)

            {

                s.Append(b.ToString("x1").ToLower());

            }

            pass = s.ToString();

            return pass;

        }
        #endregion

        #region encrypt và decrypt với mật khẩu
        //https://www.c-sharpcorner.com/blogs/encrypt-and-decrypt-a-string-in-asp-net1

        //static void Main(string[] args)
        //{
        //    Program1 p = new Program1();
        //    Console.WriteLine("Nhap text :");
        //    string txt = Console.ReadLine();
        //    Console.WriteLine("Ma hao:");

        //    txt = p.Encrypt_Code(txt);
        //    Console.WriteLine(txt + "  " + txt.Length);
        //    Console.ReadLine();
        //    Console.WriteLine("Giai ma:");
        //    Console.WriteLine(p.Decrypt_Code(txt));
        //    Console.ReadLine();
        //}

        public string Encrypt_Code(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public string Decrypt_Code(string cipherText)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion
    }

}
