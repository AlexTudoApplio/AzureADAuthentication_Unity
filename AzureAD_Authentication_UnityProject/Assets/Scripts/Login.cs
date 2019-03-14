using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Microsoft.Identity.Client;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Security;

namespace AzureADAuthentication
{
    public class Login : MonoBehaviour
    {
        public Text ResultText;
        
        public static string ClientId = ""; //your client/app id from azure
        public static string Tenant = ""; //your tenant id

        private string[] scopes = new string[] { "user.read" };

        public async void Action_LoginButton()
        {
            AuthenticationResult authResult = null;

            string authority = "https://login.microsoftonline.com/" + Tenant;
            var app = new PublicClientApplication(ClientId, authority, TokenCacheHelper.GetUserCache());

            var accounts = await app.GetAccountsAsync();

            try
            {
                authResult = await app.AcquireTokenSilentAsync(scopes, accounts.FirstOrDefault());
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. 
                // This indicates you need to call AcquireTokenAsync to acquire a token
                ResultText.text = $"MsalUiRequiredException: {ex.Message}";

                try
                {
                    authResult = await app.AcquireTokenAsync(scopes);
                }
                catch (MsalException msalex)
                {
                    ResultText.text = $"Error Acquiring Token:{msalex}";
                    Debug.Log(msalex);
                }
            }
            catch (Exception ex)
            {
                ResultText.text = $"Error Acquiring Token Silently:{ex}";
                Debug.Log(ex);
                return;
            }

            if (authResult != null)
            {
                ResultText.text = "username: " + authResult.Account.Username;
                Debug.Log("username: " + authResult.Account.Username);
                //ResultText.Text = await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
                //DisplayBasicTokenInfo(authResult);
                //this.SignOutButton.Visibility = Visibility.Visible;
            }

        }


        /// <summary>
        /// Perform an HTTP GET request to a URL using an HTTP Authorization header
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="token">The token</param>
        /// <returns>String containing the results of the GET operation</returns>
        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        ///// <summary>
        ///// Sign out the current user
        ///// </summary>
        //private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var accounts = await App.PublicClientApp.GetAccountsAsync();
        //    if (accounts.Any())
        //    {
        //        try
        //        {
        //            await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
        //            this.ResultText.Text = "User has signed-out";
        //            this.CallGraphButton.Visibility = Visibility.Visible;
        //            this.SignOutButton.Visibility = Visibility.Collapsed;
        //        }
        //        catch (MsalException ex)
        //        {
        //            ResultText.Text = $"Error signing-out user: {ex.Message}";
        //        }
        //    }
        //}

        ///// <summary>
        ///// Display basic information contained in the token
        ///// </summary>
        //private void DisplayBasicTokenInfo(AuthenticationResult authResult)
        //{
        //    TokenInfoText.Text = "";
        //    if (authResult != null)
        //    {
        //        TokenInfoText.Text += $"Username: {authResult.Account.Username}" + Environment.NewLine;
        //        TokenInfoText.Text += $"Token Expires: {authResult.ExpiresOn.ToLocalTime()}" + Environment.NewLine;
        //        TokenInfoText.Text += $"Access Token: {authResult.AccessToken}" + Environment.NewLine;
        //    }
        //}           
    }
}