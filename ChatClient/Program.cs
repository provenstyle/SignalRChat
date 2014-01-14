using System;
using System.Net;
using System.Web.Security;
using Microsoft.AspNet.SignalR.Client;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var cn = @"https://localhost:443/";
                var hubConnection = new HubConnection(cn);

                Cookie returnedCookie;
                var authenticated = AuthenticateUser("lvs", "Pass@word1", out returnedCookie);
                Console.WriteLine("Authenticated: {0}", authenticated);
                hubConnection.CookieContainer = new CookieContainer();
                hubConnection.CookieContainer.Add(returnedCookie);


                var proxy = hubConnection.CreateHubProxy("ChatHub");
                proxy.On<string, string>("broadcastMessage", (name, message) =>
                    Console.WriteLine("{0} : {1}", name, message)
                    );

                hubConnection.Error += exception => Console.WriteLine(exception.Message);
                hubConnection.StateChanged += change => Console.WriteLine("State Changed was: {0} is: {1}", change.OldState, change.NewState);
                hubConnection.Received += data => Console.WriteLine(data);

                hubConnection.TraceLevel = TraceLevels.All;
                hubConnection.TraceWriter = Console.Out;

                //var hubConfiguration = new HubConfiguration();
                //hubConfiguration.EnableDetailedErrors = true;
                //app.MapSignalR(hubConfiguration);

                hubConnection.Start().Wait();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            

            Console.WriteLine("Press and key to exit.");
            Console.ReadKey();
        }

        private static bool AuthenticateUser(string user, string password, out Cookie authCookie)
        {
            var request = WebRequest.Create("https://localhost/account/LogIn") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();

            var authCredentials = "UserName=" + user + "&Password=" + password;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(authCredentials);
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                authCookie = response.Cookies[FormsAuthentication.FormsCookieName];
            }

            if (authCookie != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
