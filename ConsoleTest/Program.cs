using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using UserMangager;

namespace ConsoleTest
{
    class Program
    {

        static void Main(string[] args)
        {

            UserManager userMan = new UserManager();
                string cmd;
            try
            {
                StringBuilder ab = new StringBuilder();
                string[] info = new string[10];
                Console.WriteLine("Start application. Wellcome!");
            MENU:
                ab.Clear();
                ab.Append("\n");
                ab.Append("0 >>  MENU\n");
                ab.Append("...Pleae chose action\n");
                ab.Append(">>  0 - Menu\n");
                ab.Append(">>  1 - Loging\n");
                ab.Append(">>  2 - Logout\n");
                ab.Append(">>  3 - Create New Account Infomation\n");
                ab.Append(">>  4 - Create New User\n");
                ab.Append(">>  6 - Delete Account\n");
                ab.Append(">>  7 - Change Password\n");
                ab.Append(">>  8 - Show user infomation\n");
                ab.Append(">>  9 - Find user\n");
                ab.Append(">>  Menu - Back to MENU\n");
                ab.Append(">>  Exit - Back to EXIT\n");
                ab.Append("\n");
                Console.WriteLine(ab.ToString() + "\n");
                ab.Clear();
            TOP:
                WriteFormat1("");
                string a = Console.ReadLine();
                switch (a)
                {
                    case "0":
                        goto MENU;
                    case "1":


                        Console.Write(" >>  LOGIN");
                        Console.WriteLine("");
                        goto LOGIN;
                    case "2":

                        Console.Write(" >>  LOGOUT");
                        Console.WriteLine("");
                        goto LOGOUT;
                    case "3":

                        Console.Write(" >>  CREATE");
                        Console.WriteLine("");
                        goto CREATE;
                    case "4":
                        Console.Write(" >>  CREATE NEW USER");
                        Console.WriteLine("");
                        goto UPDATE;
                    case "6":
                        Console.Write(" >>  DELETE ACCOUNT");
                        Console.WriteLine("");
                        goto DELETE;
                    case "7":
                        Console.Write(" >>  CHANGE PASSWORD");
                        Console.WriteLine("");
                        goto CHANGEPASS;
                    case "8":
                        Console.Write(" >>  SHOWINFO");
                        Console.WriteLine("");
                        goto SHOWINFO;
                    case "9":
                        Console.Write(" >>  FIND USER");
                        Console.WriteLine("");
                        goto FINDUSER;

                }
                a = "";
                goto MENU;
            #region LOGIN
            LOGIN:
                Console.Write(">>  Type Account Name: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    cmd = "";
                    goto MENU;

                }
                #endregion
                //Kiểm tra tài khoản đã tồn tại hay chưa
                if (userMan.CheckValueExisted(cmd, "_AccountName") == 1)
                {
                //check ok vaf yeu cau nhap mat khau de xac nhan
                typepassword:
                    Console.Write(">>  Type Password: ");
                    string pass = Console.ReadLine();
                    #region Exit Or Menu
                    if (pass.ToLower() == "exit")
                    {
                        goto EXIT;
                    }
                    else if (pass.ToLower() == "menu")
                    {
                        pass = "";
                        goto MENU;

                    }
                    #endregion
                    if (userMan.Login(cmd, pass))
                    {
                        Console.WriteLine("...Login successfull! Press any key to back to MENU\n");
                        userMan._AccountName = cmd;
                        UpdateUserLogin(userMan);
                        Console.ReadKey();
                        goto MENU;
                    }
                    Console.WriteLine("...Please try again!");
                    goto typepassword;
                }
                else
                {
                    Console.WriteLine("...User not found! try again!");
                    goto LOGIN;
                }
            #endregion

            #region LOGOUT
            LOGOUT:
                if (userMan.GetStatusLogin)
                {
                    userMan.Logout();
                }
                WriteFormat2("Logout ok!");
                WriteFormat2("Press any key to back to Menu!");
                cmd = Console.ReadLine();
                goto MENU;
            #endregion

            #region CREATE
            CREATE:
                ab.Clear();
                Console.WriteLine(">>  Type your information:\n");

            // ACCOUNT NAME em//////////////////////////////
            typeAccount:
                Console.Write(">>     Account Name: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                bool check = userMan.CheckAccountExist(cmd);
                if (check)
                {
                    Console.WriteLine("...Account Name existed. Try again!");
                    goto typeAccount;
                }
                else
                {
                    userMan._AccountName = cmd;
                    ab.Append(">>     Account Name: " + cmd + "\n");
                    Console.WriteLine("...OK!\n");
                }
            // PASSWORD //////////////////////////////
            typePass:
                Console.Write(">>     Password: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                if ((cmd.Length >= 8) && (cmd.Length <= 20))
                {
                    userMan._Pass = cmd;
                    Console.WriteLine("...Password OK!\n");
                    ab.Append(">>     Password: *********\n");
                }
                else
                {
                    Console.WriteLine("...Bad Password. try again!");
                    goto typePass;
                }
                // FULL NAME ////////////////////////////////
                Console.Write(">>     Full Name: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                userMan._FullName = cmd;
                ab.Append(">>     Full Name:" + cmd + "\n");
                Console.WriteLine("...OK!\n");
                // EMAIL ///////////////////////////////////////
                Console.Write(">>     Email: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                userMan._Email = cmd;
                ab.Append(">>     Email:" + cmd + "\n");
                Console.WriteLine("...OK!\n");
                // PERMISSON ////////////////////////////////////
                Console.Write(">>     Permission: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                userMan._Permission = cmd;
                ab.Append(">>     Permission: " + cmd + "\n");
                Console.WriteLine("...OK!\n");
                // ENABLE //////////////////////////////////////////////
                Console.Write(">>     Enable: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                userMan._Enable = cmd;
                ab.Append(">>     Enable: " + cmd + "\n");
                // STATUS /////////////////////////////////////////////////
                Console.Write(">>     Status: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "menu")
                {
                    goto MENU;
                }
                #endregion
                userMan._Status = cmd;
                ab.Append(">>     Status: " + cmd + "\n");
                cmd = userMan._DateCreate = DateTime.Now.ToString();
                ab.Append(">>     Date Create: " + cmd + "\n");

                //show result////////////////////////////////////////////
                Console.WriteLine("\n");
                Console.WriteLine(">> Press anykey to show your input!");
                Console.ReadKey();
                Console.WriteLine(">> Your Infomation completed!");
                Console.WriteLine("...OK!\n" + ab.ToString());

                Console.WriteLine(">> Do you wanna create new account (yes/no)?: ");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else if (cmd.ToLower() == "yes")
                {

                    goto UPDATE;
                }
                else
                {
                    goto MENU;
                }
            #endregion

            #endregion

            #region UPDATE
            UPDATE:
                WriteFormat1(string.Format( "Creating Account Name: {0}. . . . . . . ",userMan._AccountName));
                check = userMan.CheckAccountExist(userMan._AccountName, userMan._Pass) && userMan.CheckAccountExist(userMan._AccountName);


                if (!check)
                {
                    userMan.CreateUser();
                    WriteFormat2("Succesfull!\n");
                }
                else
                {
                    WriteFormat2("Unsuccesfull! User exited!\n");
                    WriteFormat2("Try agian? (yes)\n");
                }
                WriteFormat2("Press any key to back to Menu.\n");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "yes")
                {
                    goto UPDATE;
                }
                else
                {
                    goto MENU;
                }
            #endregion
            #endregion

            #region SHOWINFO
            SHOWINFO:

                //info[0] = "" + userMan._ID_User;
                //info[1] = userMan._AccountName;
                //info[2] = userMan._FullName;
                //info[3] = userMan._StaffCode;
                //info[4] = userMan._Email;
                //info[5] = userMan._Pass;
                //info[6] = userMan._Permission;
                //info[7] = userMan._Enable;
                //info[8] = userMan._Status;
                //info[9] = userMan._DateCreate;
                //showInfo(info);
                showTable(userMan.GetUserInfoByLongin());
                //UpdateUserLogin(userMan);
                Console.WriteLine("Press any key to back to Menu");
                cmd = Console.ReadLine();
                #region Exit Or Menu
                if (cmd.ToLower() == "yes")
                {
                    goto EXIT;
                }
                else
                {
                    goto MENU;
                }
            #endregion

            #endregion

            #region CHANGEPASS
            CHANGEPASS:
                WriteFormat1("Account Name:");
                string U = Console.ReadLine();
                WriteFormat1("Current password:");
                string oP = Console.ReadLine();
                WriteFormat1("New password:");
                string nP = Console.ReadLine();
                int result = userMan.ChangePassword(U, oP, nP);


                switch (result.ToString())
                {
                    case "-1":
                        WriteFormat1("Not success! try again?");
                        cmd = Console.ReadLine();
                        if(cmd.ToLower() == "yes")
                        {
                            goto CHANGEPASS;

                        }
                        else
                        {
                            break;
                        }
                    case "1":
                        WriteFormat1("Change password successfull! Login to check result!");
                        a = "";
                        goto TOP;
                }
            #endregion CHANGEPASS

            #region DELETE
            DELETE:
                WriteFormat1("Delete account name:");
                cmd = Console.ReadLine();
                check = userMan.CheckAccountExist(cmd);
                if (check)
                {
                    userMan.DeleteUser(cmd);
                    WriteFormat2("OK!\n");
                }
                else
                {
                    WriteFormat1("Not found acount name! Try again? (Yes/No)\n");
                    WriteFormat1("");
                    cmd = Console.ReadLine();
                    switch (cmd.ToLower())
                    {
                        case "yes":
                            goto DELETE;
                        case "no":
                            goto TOP;
                    }
                }
                #region Exit Or Menu
                if (cmd.ToLower() == "exit")
                {
                    goto EXIT;
                }
                else
                {
                    cmd = "";
                    goto TOP;
                }
            #endregion
            #endregion

            #region FINDUSER;
            FINDUSER:
                WriteFormat1("Find: ");
                DataTable data;
                cmd = Console.ReadLine();
                data = userMan.SearchUser(cmd, "_AccountName");
                showTable(data);
                WriteFormat1("Find more? (yes)");
                cmd = Console.ReadLine();
                if (cmd.ToLower() == "yes")
                {
                    goto FINDUSER;
                }

            #endregion FINDUSER

                goto TOP;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
            
        EXIT:
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }

        static void WriteFormat1(string txt)
        {
            Console.Write(">>   " + txt);
        }
        static void WriteFormat2(string txt)
        {
            Console.Write("..." + txt);
        }
        static void showInfo(string[] data)
        {
            foreach (string item in data)
            {
                Console.WriteLine(item);
            }
        }
        static void showTable(DataTable data)
        {
            try
            {
                foreach (DataRow dataRow in data.Rows)
                {
                    //Console.WriteLine(dataRow.Field<int>(0)); //ghi 1 cột
                    int i = 0;
                    foreach (var item in dataRow.ItemArray)
                    {
                        string name = data.Columns[i].ColumnName;
                        Console.Write(name + " = " + item);
                        Console.Write(" | ");
                        i++;
                    }
                    i = 0;
                    Console.WriteLine();
                }
            }
            catch { Console.WriteLine("Please login and try again!"); }
        }

        static void UpdateUserLogin(UserManager user)
        {
            DataTable data = user.GetUserInfoByLongin();

            for(int i = 0; i < data.Columns.Count; i++)
            {
                string name = data.Columns[i].ColumnName;
                WriteFormat2(name+ ": "+data.Rows[0][i].ToString()+"\n");
            }

            user._ID_User =         data.Rows[0][0].ToString();
            user._AccountName =     data.Rows[0][1].ToString();
            user._FullName =        data.Rows[0][2].ToString();
            user._StaffCode =       data.Rows[0][3].ToString();
            user._Email =           data.Rows[0][4].ToString();
            user._Pass =            data.Rows[0][5].ToString();
            user._CryptPass =       data.Rows[0][6].ToString();
            user._Permission =      data.Rows[0][7].ToString();
            user._DateCreate =      data.Rows[0][8].ToString();
            user._Enable =          data.Rows[0][9].ToString();
            user._Status =          data.Rows[0][10].ToString();
            //showTable(data);
        }
    }
}
