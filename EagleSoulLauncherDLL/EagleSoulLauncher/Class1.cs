using System;
using System.Net;

namespace EagleSoulLauncher
{
    public class DbConnector
    {

        public static WebClient wclient = new WebClient();
        public static string login(string username,string password)
        {
            string checkLogin = wclient.DownloadString("http://66.70.154.61/handler.php?action=login&username=" + username + "&password=" + password + "&key=mkfcrYIXnZQDRMXh");
            return checkLogin;
        }

        public static string register(string username, string password)
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string checkRegister = wclient.DownloadString("http://66.70.154.61/handler.php?action=register&username=" + username + "&password=" + password + "&timestamp=" + timestamp + "&key=mkfcrYIXnZQDRMXh");
            return checkRegister;
        }
    }
}
