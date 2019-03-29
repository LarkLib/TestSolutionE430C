using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;

namespace TestDirectoryServicesConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            //Test2();
        }

        private static void Test2()
        {
            var path = string.Format("WinNT://{0},computer", Environment.MachineName);

            using (var computerEntry = new DirectoryEntry(path))
            {
                var userNames = from DirectoryEntry childEntry in computerEntry.Children
                                where childEntry.SchemaClassName == "User" && childEntry.Name.StartsWith("a", StringComparison.InvariantCultureIgnoreCase)
                                select childEntry.Name;

                foreach (var name in userNames)
                    Console.WriteLine(name);
            }
            Console.ReadKey();
            //DirectoryEntry deRoot = new DirectoryEntry("WinNT://localhost");
            //DirectorySearcher DirSearch = new DirectorySearcher(deRoot, "(objectClass=user)");
            //SearchResultCollection ResultList = DirSearch.FindAll();
            //foreach (SearchResult SResult in ResultList)
            //{
            //    Console.WriteLine(SResult.Properties["sAMAccountName"].ToString());
            //}
        }

        private static void Test()
        {
            try
            {
                var UserName = "Shqny";
                DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                var users = from DirectoryEntry childEntry in AD.Children
                            where childEntry.SchemaClassName == "User" && childEntry.Name.StartsWith(UserName, StringComparison.InvariantCultureIgnoreCase)
                            select childEntry;
                if (users != null && users.Any())
                {
                    var user = users.First();
                    AD.Children.Remove(user);
                    Console.WriteLine("Remove test user");
                }
                DirectoryEntry NewUser = AD.Children.Add(UserName, "user");
                NewUser.Invoke("SetPassword", new object[] { "!QAZ2wsx3edc" });
                NewUser.Invoke("Put", new object[] { "Description", "Shqny User from .NET" });
                NewUser.CommitChanges();
                DirectoryEntry grp;

                grp = AD.Children.Find("Administrators", "group");
                if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                Console.WriteLine("Account Created Successfully");
                //Console.WriteLine("Press Enter to continue....");
                //Console.ReadLine();
                File.WriteAllText("d:\\temp.txt", "Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                File.WriteAllText("d:\\temp.txt", ex.Message);
                // Console.ReadLine();
            }
        }

        //public void GetADUsers()
        //{
        //    try
        //    {
        //        string DomainPath = "LDAP://DC=xxxx,DC=com";
        //        DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
        //        DirectorySearcher search = new DirectorySearcher(searchRoot);
        //        search.Filter = "(&(objectClass=user)(objectCategory=person))";
        //        search.PropertiesToLoad.Add("samaccountname");
        //        search.PropertiesToLoad.Add("mail");
        //        search.PropertiesToLoad.Add("usergroup");
        //        search.PropertiesToLoad.Add("displayname");//first name
        //        SearchResult result;
        //        SearchResultCollection resultCol = search.FindAll();
        //        if (resultCol != null)
        //        {
        //            for (int counter = 0; counter < resultCol.Count; counter++)
        //            {
        //                string UserNameEmailString = string.Empty;
        //                result = resultCol[counter];
        //                if (result.Properties.Contains("samaccountname") &&
        //                         result.Properties.Contains("mail") &&
        //                    result.Properties.Contains("displayname"))
        //                {
        //                    Users objSurveyUsers = new Users();
        //                    objSurveyUsers.Email = (String)result.Properties["mail"][0] +
        //                      "^" + (String)result.Properties["displayname"][0];
        //                    objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
        //                    objSurveyUsers.DisplayName = (String)result.Properties["displayname"][0];
        //                    lstADUsers.Add(objSurveyUsers);
        //                }
        //            }
        //        }
        //        return;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
    }
}
