using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BestHTTP;
using Entity;
using Newtonsoft.Json;

namespace Gateway
{
    public class AccountGateway
    {
        private bool debugResponseKeys = true;
        private static string ApiUrl = "https://linia-market.ru/api/";

        public void Login(AccountEntity details, Action<AccountEntity> onSuccess, Action<string> onError)
        {
            Debug.LogWarning("--- Logging in ---");

            Uri u = new Uri(ApiUrl + "login.php");

            HTTPRequest request = new HTTPRequest(u, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res == null)
                    {
                        onError("Подключение к Интернету отсутствует.");
                        return;
                    }

                    var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.DataAsText);
                    if (debugResponseKeys) PrintResponseKeys(response);

                    if (response.ContainsKey("error"))
                    {
                        onError(response["error"]);
                        return;
                    }

                    if (response.ContainsKey("id"))
                    {
                        details.Id = response["id"];
                        details.SessId = response["sessid"];
                        Debug.Log("Id: " + details.Id + ", SessId: " + details.SessId);
                        Authorization(details, onSuccess, onError);
                    }
                });

            request.AddHeader("Content-Type", "application/json");
            string data = "{\"form\":{\"login\":\"" + details.Email + "\", \"passwd\":\"" + details.Pw + "\"}}";
            request.RawData = Encoding.UTF8.GetBytes(data);
            request.Send();
        }

        public void Authorization(AccountEntity details, Action<AccountEntity> onSuccess, Action<string> onError)
        {
            Debug.LogWarning("--- Authorizing ---");

            Uri u = new Uri(ApiUrl + "auth.php");

            HTTPRequest request = new HTTPRequest(u, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res == null)
                    {
                        onError("Подключение к Интернету отсутствует.");
                        return;
                    }
                    
                    if (res.DataAsText.Contains("error"))
                    {
                        var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.DataAsText);
                        onError(error["error"]);
                        return;
                    }

                    string tempRes = res.DataAsText.Replace(",\"orders\":[]", "");
                    var response = JsonConvert.DeserializeObject<AccountDataEntity>(tempRes);
                    onSuccess(ConvertAccountDataToAccount(details, response));
                });

            request.AddHeader("Content-Type", "application/json");
            string data = "{\"id\":\"" + details.Id + "\", \"sessid\":\"" + details.SessId + "\"}";
            request.RawData = Encoding.UTF8.GetBytes(data);
            request.Send();
        }
        
        public void Register(AccountEntity details, Action<string> onSuccess, Action<string> onError)
        {
            Debug.LogWarning("--- Registering ---");

            Uri u = new Uri(ApiUrl + "register.php");

            HTTPRequest request = new HTTPRequest(u, HTTPMethods.Post,
                (req, res) =>
                {
                    if (res == null)
                    {
                        onError("Подключение к Интернету отсутствует.");
                        return;
                    }

                    var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.DataAsText);
                    if (debugResponseKeys) PrintResponseKeys(response);

                    if (response.ContainsKey("error")) onError(response["error"]);
                    if (response.ContainsKey("result"))
                    {
                        string result = "User: " + details.Email + ", Pw: " + details.Pw;
                        onSuccess(result);
                    }
                });

            request.AddHeader("Content-Type", "application/json");
            string data = GenerateRegistrationData(details);
            request.RawData = Encoding.UTF8.GetBytes(data);
            request.Send();
        }

        private string GenerateRegistrationData(AccountEntity account)
        {
            RegisterEntity registration = new RegisterEntity()
            {
                form = new RegisterForm()
                {
                    agreement = account.Terms,
                    card = account.Card,
                    login = account.Email,
                    name = account.Name,
                    passwd = account.Pw,
                    passwdConfirm = account.PwConfirm,
                    phone = account.Phone
                }
            };
            
            return JsonConvert.SerializeObject(registration);
        }
        
        private AccountEntity ConvertAccountDataToAccount(AccountEntity accountDetails, AccountDataEntity accountData)
        {
            accountDetails.Email = accountData.Login;
            accountDetails.Name = accountData.Name;
            accountDetails.Enabled = accountData.Enabled;
            accountDetails.Admin = accountData.Admin;
            accountDetails.Phone = accountData.Phone;
            accountDetails.Card = accountData.Card;
            accountDetails.Orders = accountData.Orders;
            
            return accountDetails;
        }

        private void PrintResponseKeys(Dictionary<string, string> response)
        {
            foreach(KeyValuePair<string, string> pair in response)
                Debug.Log("Response. Key: " + pair.Key + ", Value: " + pair.Value);
        }
    }
}