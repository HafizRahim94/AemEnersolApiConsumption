using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net.Cache;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AemEnersolProject
{
    class Program
    {
        
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().AddCommandLine(args)
            .Build();
            Console.Title =  "AEMEnersol API consumption";
            Console.WriteLine("---Login to AEMEnersol API---");
            Console.WriteLine("Please Enter UserName:");
            string Username = Console.ReadLine();
            Console.WriteLine("Please Enter Password:");
            string Password = Console.ReadLine();
            Console.WriteLine("Authenticating...");
            string CallWebApi = await CallWebAPIAsync(Username, Password);
            string AuthToken = CallWebApi.Trim('"').ToString();
            var modelPlatformWellActuals = await GetPlatformWellActual(AuthToken);

            foreach (var repo in modelPlatformWellActuals)
            {
                //To handle invalid datetime
                DateTime PCreatedAt;
                DateTime PupdatedAt;
                if (DateTime.TryParse(repo.createdAt.ToString(), out PCreatedAt))
                {

                    if (repo.createdAt < SqlDateTime.MinValue.Value || repo.createdAt > SqlDateTime.MaxValue.Value)
                    { PCreatedAt = DateTime.UtcNow; }else { PCreatedAt = repo.createdAt;}

                }
                else { PCreatedAt = DateTime.UtcNow; }

                if (DateTime.TryParse(repo.createdAt.ToString(), out PupdatedAt))
                {
                    if (repo.updatedAt < SqlDateTime.MinValue.Value || repo.updatedAt > SqlDateTime.MaxValue.Value)
                    { PupdatedAt = DateTime.UtcNow; }
                    else { PupdatedAt = repo.updatedAt; }

                 }else { PupdatedAt = DateTime.UtcNow; }
                //To Handle Invalid DateTime

                //SqlConnection connection;
                SqlCommand cmdInsPlatfrom = new SqlCommand();
                var connectionstring = Configuration.GetConnectionString("localDb") ;
                try {
                    Console.WriteLine("Inserting Platform");
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("PlatformID = " + repo.id);
                    Console.WriteLine("Platform Name = " + repo.uniqueName);
                    Console.WriteLine("Latitude = " + repo.latitude);
                    Console.WriteLine("Longitude = " + repo.longitude);
                    Console.WriteLine("Created at = " + PCreatedAt);
                    Console.WriteLine("Updated At = " + PupdatedAt);
                    using (SqlConnection connection = new SqlConnection(connectionstring))
                    {
                        cmdInsPlatfrom.Connection = connection;
                        cmdInsPlatfrom.CommandType = CommandType.StoredProcedure;
                        cmdInsPlatfrom.CommandText = "Merge_GetPlatformWellActual";
                        cmdInsPlatfrom.Parameters.Add("@strInsType", SqlDbType.NVarChar).Value = "P";//p = Platform
                        cmdInsPlatfrom.Parameters.Add("@intPlatformID", SqlDbType.Int).Value = 0;
                        cmdInsPlatfrom.Parameters.Add("@intID", SqlDbType.Int).Value = repo.id;
                        cmdInsPlatfrom.Parameters.Add("@strUniqueName", SqlDbType.VarChar).Value = repo.uniqueName;
                        cmdInsPlatfrom.Parameters.Add("@Fltlatitude", SqlDbType.Float).Value = repo.latitude;
                        cmdInsPlatfrom.Parameters.Add("@Fltlongitude", SqlDbType.Float).Value = repo.longitude;
                        cmdInsPlatfrom.Parameters.Add("@DtcreatedAt", SqlDbType.DateTime).Value = PCreatedAt;
                        cmdInsPlatfrom.Parameters.Add("@DtupdatedAt", SqlDbType.DateTime).Value = PupdatedAt;
                        cmdInsPlatfrom.Connection.Open();
                        cmdInsPlatfrom.ExecuteNonQuery();
                        cmdInsPlatfrom.Parameters.Clear();
                    }
                    Console.WriteLine("Platform " + repo.id+ " Successfully Inserted into DB!\n");
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                
                
                foreach (var well in repo.well)
                {
                    //To Handle Invalid DateTime
                    DateTime WCreatedAt;
                    DateTime WupdatedAt;
                    if (DateTime.TryParse(well.createdAt.ToString(), out WCreatedAt))
                    {

                        if (well.createdAt < SqlDateTime.MinValue.Value || well.createdAt > SqlDateTime.MaxValue.Value)
                        { WCreatedAt = DateTime.UtcNow; }
                        else { WCreatedAt = repo.createdAt; }

                    }
                    else { WCreatedAt = DateTime.UtcNow; }

                    if (DateTime.TryParse(well.updatedAt.ToString(), out WupdatedAt))
                    {

                        if (well.updatedAt < SqlDateTime.MinValue.Value || well.updatedAt > SqlDateTime.MaxValue.Value)
                        { WupdatedAt = DateTime.UtcNow; }
                        else { WupdatedAt = repo.createdAt; }

                    }
                    else { WupdatedAt = DateTime.UtcNow; }
                    //To Handle Invalid DateTime
                    try
                    {
                        Console.WriteLine("Inserting Well");
                        Console.WriteLine("--------------------------");
                        Console.WriteLine("Platform ID = " + well.platformId);
                        Console.WriteLine("Well ID = " + well.id);
                        Console.WriteLine("Well Name = " + well.uniqueName);
                        Console.WriteLine("Latitude = " + well.latitude);
                        Console.WriteLine("Longitude = " + well.longitude);
                        Console.WriteLine("Created At = " + WCreatedAt);
                        Console.WriteLine("Updated At = " + WupdatedAt);
                        using (SqlConnection connection = new SqlConnection(connectionstring))
                        {
                            cmdInsPlatfrom.Connection = connection;
                            cmdInsPlatfrom.CommandType = CommandType.StoredProcedure;
                            cmdInsPlatfrom.CommandText = "Merge_GetPlatformWellActual";
                            cmdInsPlatfrom.Parameters.Add("@strInsType", SqlDbType.NVarChar).Value = "W";//w = Well
                            cmdInsPlatfrom.Parameters.Add("@intID", SqlDbType.Int).Value = well.id;
                            cmdInsPlatfrom.Parameters.Add("@intPlatformID", SqlDbType.Int).Value = well.platformId;
                            cmdInsPlatfrom.Parameters.Add("@strUniqueName", SqlDbType.VarChar).Value = well.uniqueName;
                            cmdInsPlatfrom.Parameters.Add("@Fltlatitude", SqlDbType.Float).Value = well.latitude;
                            cmdInsPlatfrom.Parameters.Add("@Fltlongitude", SqlDbType.Float).Value = well.longitude;
                            cmdInsPlatfrom.Parameters.Add("@DtcreatedAt", SqlDbType.DateTime).Value = WCreatedAt;
                            cmdInsPlatfrom.Parameters.Add("@DtupdatedAt", SqlDbType.DateTime).Value = WupdatedAt;
                            cmdInsPlatfrom.Connection.Open();
                            cmdInsPlatfrom.ExecuteNonQuery();
                            cmdInsPlatfrom.Parameters.Clear();

                        }
                        
                        Console.WriteLine("Well " + well.id + " Successfully Inserted into DB!\n");
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.ToString());
                    }
                    

                };

            }
            Console.ReadLine();
        }

        static async Task<string> CallWebAPIAsync(string UserName, string Password)
        {
            var msg = "";
            var login = "{'username':'" + UserName + "','password':'" + Password + "'}";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://test-demo.aem-enersol.com/");
            var response = await client.PostAsync("api/Account/Login", new StringContent(login, System.Text.Encoding.UTF8, "application/json"));

            if (response != null)
            {
                if (response.StatusCode.ToString() == "OK")
                {
                    var StreamResponse = response.Content.ReadAsStringAsync();
                    Console.WriteLine("Authentication Successful...");
                    Console.WriteLine("Processing...");
                    msg = await StreamResponse;

                }
                else
                {
                    Console.WriteLine("Authentication Failed!");
                    Console.WriteLine(response.StatusCode);
                    Console.ReadKey();
                    Environment.Exit(2);
                    msg = response.StatusCode.ToString();
                }

            }
            return msg;
        }

        static async Task<List<Platform>> GetPlatformWellActual(string AuthToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + AuthToken);
                client.BaseAddress = new Uri("http://test-demo.aem-enersol.com/");
                //var streamTask = client.GetStreamAsync("/api/PlatformWell/GetPlatformWellDummy");//Dummy to retrieve different key.
                var streamTask = client.GetStreamAsync("/api/PlatformWell/GetPlatformWellActual");
                var repositories = await JsonSerializer.DeserializeAsync<List<Platform>>(await streamTask);
                return repositories;

            }


        }

    }
}
