using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#region SuperClasses

public class APIRequest
{
    public string data;
}
public class DataRequest
{
    public string Message { set; get; }
    public string OtherMessage { set; get; }
}

public class UserDataBO
{
    public string userName { get; set; }

    public string email { get; set; }
    public string walletAddress { get; set; }
    public int AvatarID { get; set; }
}
public class userDataPayload
{
    public UserDataBO data { get; set; }
}

public class LoginDataBO
{
    public string walletAddress { get; set; }
}

public class LoginDataBOPayload
{
    public LoginDataBO data { get; set; }
}

public class LeaderboardCounter
{
    public int number { get; set; }
}

public class LeaderboardPayload
{
    public LeaderboardCounter data { get; set; }
}
#endregion

public enum RequestType
{
    NONE=0,
    LEADERBOARD=1,
    LOGIN=2,
    UPDATE=3
}
public class apiRequestHandler : MonoBehaviour
{
    //Production : https://us-central1-pong-tournaments.cloudfunctions.net/
    //pong production : AIzaSyDG7C0sHN4-hXhjOyTdyaBpQc3BArIBVsU
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void EncJS(string data, string objName, string callback);

    [DllImport("__Internal")]
    private static extern void DecJS(string data, string objName, string callback);
#endif
    private string BaseURL;
    private string loginURL;
    private const string firebaseLoginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private const string firebaseSignupUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=";
    private string forgetPassword = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=";
    private string emailVerification = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=";
    private string firebaseApiKey;
    private string signupBOUserURL;
    private string updateUserBoURL;
    private string leaderboardBOURL;
    public static apiRequestHandler Instance;
    string StoredTokenResult;
    string StoredReq;
    private RequestType requestType = RequestType.NONE;
    private string msgOther = "GC,,Ktj(ts]z";

    public void Start()
    {
        if (!Instance)
        {
            Instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }

        BaseURL = "https://us-central1-pong-tournaments.cloudfunctions.net/";
        firebaseApiKey = "AIzaSyDG7C0sHN4-hXhjOyTdyaBpQc3BArIBVsU";

        loginURL = BaseURL + "Login";
        signupBOUserURL = BaseURL + "SignUp";
        updateUserBoURL = BaseURL + "UpdateUserBO";
        leaderboardBOURL = BaseURL + "Leaderboard";

        forgetPassword = forgetPassword + firebaseApiKey;
        emailVerification = emailVerification + firebaseApiKey;
    }

    public void SetRequestType(RequestType _type)
    {
        requestType = _type;
    }

    public string CheckResponseType(string _data, bool _enc, bool _onenc, bool _dec, bool _ondec)
    {
        string _request = "";
        switch (requestType)
        {
            case RequestType.NONE:
                break;
            case RequestType.LEADERBOARD:
                if (_enc || _dec)
                {
                    DataRequest _Ddata = new DataRequest();
                    _Ddata.Message = _data;
                    _Ddata.OtherMessage = Constants.OtherMsg + msgOther;

                    _request = JsonConvert.SerializeObject(_Ddata);
                }
                else if (_onenc)
                {
                    APIRequest _newRequest = new APIRequest();
                    _newRequest.data = _data;
                    StoredReq = JsonConvert.SerializeObject(_newRequest);
                    GetLeaderboardAsync(StoredTokenResult, StoredReq);
                } else if (_ondec)
                {
                    FirebaseManager.Instance.OnQueryUpdate(_data);
                }
                break;
            case RequestType.LOGIN:
                if (_enc || _dec)
                {
                    DataRequest _Ddata = new DataRequest();
                    _Ddata.Message = _data;
                    _Ddata.OtherMessage = Constants.OtherMsg + msgOther;

                    _request = JsonConvert.SerializeObject(_Ddata);
                }
                else if (_onenc)
                {
                    APIRequest _newRequest = new APIRequest();
                    _newRequest.data = _data;
                    StoredReq = JsonConvert.SerializeObject(_newRequest);
                    ProcessLoginAsync(StoredTokenResult, StoredReq);
                } else if (_ondec)
                {
                    FirebaseManager.Instance.SetPlayerData(_data);
                    Events.DoFireLoginSuccess();
                }
                break;
            case RequestType.UPDATE:
                if (_enc || _dec)
                {
                    DataRequest _Ddata = new DataRequest();
                    _Ddata.Message = _data;
                    _Ddata.OtherMessage = Constants.OtherMsg + msgOther;

                    _request = JsonConvert.SerializeObject(_Ddata);
                }
                else if (_onenc)
                {
                    APIRequest _newRequest = new APIRequest();
                    _newRequest.data = _data;
                    StoredReq = JsonConvert.SerializeObject(_newRequest);
                    ProcessDataUpdateAsync(StoredTokenResult, StoredReq);
                }
                else if (_ondec)
                {
                    FirebaseManager.Instance.OnDocUpdate("");
                }
                break;
        }

        return _request;
    }
    public void PerfromEnc(string _data)
    {
        string _request = CheckResponseType(_data, true, false, false, false);

#if !UNITY_EDITOR
        EncJS(_request,this.gameObject.name,"OnEnc");
#else
        Debug.Log("can't call JS from browser");
#endif
    }

    public void OnEnc(string info)
    {
        if (info != "null" && info != "" && info != null && info != string.Empty)
        {
            CheckResponseType(info, false, true, false, false);
        }
        else
        {
            Debug.LogError(info);
        }
    }

    public void PerfromDec(string _data)
    {
        string _request = CheckResponseType(_data, false, false, true, false);

#if !UNITY_EDITOR
        DecJS(_request,this.gameObject.name,"OnDec");
#else
        Debug.Log("can't call JS from browser");
#endif
    }

    public void OnDec(string info)
    {
        if (info != "null" && info != "" && info != null && info != string.Empty)
        {
            CheckResponseType(info, false, false, false, true);
        }
        else
        {
            Debug.LogError(info);
        }
    }
    public void updatePlayerData()
    {
        if (Constants.PlayingAsGuest)
            return;

        ProccessDataUpdate(FirebaseManager.Instance.Credentails.Email, FirebaseManager.Instance.Credentails.Password);
    }
    public void signInWithEmail(string _email, string _pwd)
    {
        ProccessSignIn(_email, _pwd);
    }
    public void signUpWithEmail(string _email, string _pwd, string _username)
    {
        ProccessSignUp(_email, _pwd, _username);
    }
    public void getLeaderboard()
    {
        ProccessLeaderboard(FirebaseManager.Instance.Credentails.Email, FirebaseManager.Instance.Credentails.Password);
    }
    public void onForgetPassword(string _email)
    {
        StartCoroutine(processForgetRequest(_email));
    }
    async public Task<string> GetBearerToken(string _email, string _pwd, bool _isRegister = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _pwd);
        form.AddField("returnSecureToken", "true");
        string URL = firebaseLoginUrl + firebaseApiKey;

        if (_isRegister)
            URL = firebaseSignupUrl + firebaseApiKey;

        using UnityWebRequest request = UnityWebRequest.Post(URL, form);
        await request.SendWebRequest();


        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Events.DoReportMessage(new messageInfo(request.error));
            return "error_" + request.error;
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            return tID;
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            Events.DoReportMessage(new messageInfo((string)res.SelectToken("error").SelectToken("message")));
            return "error_" + (string)res.SelectToken("error").SelectToken("message");
        }

    }
    async public void ProccessSignIn(string _email, string _pwd)
    {
        string TokenResult = await GetBearerToken(_email, _pwd);

        if (TokenResult.Contains("error"))
        {
            Events.DoFireLoginFailed("");

            if (TokenResult.Contains("INVALID_PASSWORD"))
            {
                Events.DoReportMessage(new messageInfo("Incorrect password.", null, false, false));
            } else if (TokenResult.Contains("INVALID_EMAIL"))
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Email is invalid.", null, false, false));
            }
            else if (TokenResult.Contains("error_EMAIL_NOT_FOUND"))
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Email not found.", null, false, false));
            }
            else
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Something went wrong, please try again.", null, false, false));
            }

            Debug.Log("somthing went wrong while fetching bearer token : " + TokenResult);
            return;
        }

        FirebaseManager.Instance.Credentails.Email = _email;
        FirebaseManager.Instance.Credentails.Password = _pwd;

        LoginDataBO loginData = new LoginDataBO();
        StoredTokenResult = TokenResult;

        if (Constants.IsTest)
            loginData.walletAddress = Constants.TestWalletAddress;
        else
            loginData.walletAddress = Constants.WalletAddress;

        LoginDataBOPayload loginDataPayload = new LoginDataBOPayload();
        loginDataPayload.data = loginData;
        string req = "";
        if (Constants.AllowEnc)
        {
            req = JsonConvert.SerializeObject(loginDataPayload.data);
            SetRequestType(RequestType.LOGIN);
            PerfromEnc(req);
        } else
        {
            req = JsonConvert.SerializeObject(loginDataPayload);
            ProcessLoginAsync(StoredTokenResult, req);
        }
    }
    async public void ProcessLoginAsync(string TokenResult, string req)
    {
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL + "Login", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + StoredTokenResult;
        request.SetRequestHeader("Authorization", _reqToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Events.DoFireLoginFailed("");
            Debug.Log(request.error);
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (Constants.AllowEnc)
                {
                    SetRequestType(RequestType.LOGIN);
                    PerfromDec((string)res.SelectToken("data"));
                }
                else
                {
                    FirebaseManager.Instance.SetPlayerData(request.downloadHandler.text);
                    Events.DoFireLoginSuccess();
                }
            }
            else if ((string)res.SelectToken("message") == "Email is not verified")
            {
                Constants.ResendTokenID = TokenResult;
                FirebaseManager.Instance.showVerificationScreen();
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Email is not verified, check your inbox or spam, please verify your email address and try again.", null, true));
            }
            else if ((string)res.SelectToken("message") == "No User Found.")
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("No User Found"));
            }
            else if ((string)res.SelectToken("message") == "Unauthorized")
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Unauthorized"));
            }
            else if ((string)res.SelectToken("message") == "Required parameters are missing")
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Required parameters are missing"));
            }
            else if ((string)res.SelectToken("message") == "Invalid request..")
            {
                Events.DoFireLoginFailed("");
                Events.DoReportMessage(new messageInfo("Invalid request"));
            }
            else
            {
                Events.DoFireLoginFailed("");
            }
        }
    }
    async public void ProccessSignUp(string _email, string _pwd, string _username)
    {
        FirebaseManager.Instance.Credentails.Email = _email;
        FirebaseManager.Instance.Credentails.Password = _pwd;
        FirebaseManager.Instance.Credentails.UserName = _username;
        FirebaseManager.Instance.Credentails.AvatarID = Constants.FlagSelectedIndex;

        string TokenResult = await GetBearerToken(_email, _pwd, true);

        if (TokenResult.Contains("error"))
        {
            Events.DoFireRegsiterationFailed("");
            Debug.Log("somthing went wrong while fetching bearer token : " + TokenResult);
            return;
        }

        string _walletAddress = "";

        if (Constants.IsTest)
            _walletAddress = Constants.TestWalletAddress;
        else
            _walletAddress = Constants.WalletAddress;

        UserDataBO userDataObj = new UserDataBO();
        userDataObj.userName = _username;
        userDataObj.email = _email;
        userDataObj.walletAddress = _walletAddress;
        userDataObj.AvatarID = Constants.FlagSelectedIndex;

        userDataPayload obj = new userDataPayload();
        obj.data = userDataObj;
        string req = JsonConvert.SerializeObject(obj);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL + "SignUp", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + TokenResult;
        request.SetRequestHeader("Authorization", _reqToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
            Events.DoFireRegsiterationFailed("");
            Events.DoReportMessage(new messageInfo(request.error));
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                string tID = (string)res.SelectToken("idToken");
                StartCoroutine(sendVerificationLink(TokenResult));
                Events.DoReportMessage(new messageInfo((string)res.SelectToken("message") + " Please verify your email."));
            }
            else
            {
                Events.DoFireRegsiterationFailed("");
                Events.DoReportMessage(new messageInfo((string)res.SelectToken("message")));
            }
        }
    }
    async public void ProccessDataUpdate(string _email, string _pwd)
    {
        string TokenResult = await GetBearerToken(_email, _pwd);

        if (TokenResult.Contains("error"))
        {
            Debug.Log("somthing went wrong while fetching bearer token : " + TokenResult);
            return;
        }

        StoredTokenResult = TokenResult;
        FirebaseManager.Instance.updatePlayerDataPayload();
        string req;
        
        if(Constants.AllowEnc)
        {
            req = JsonConvert.SerializeObject(FirebaseManager.Instance.PlayerDataPayload.data);
            SetRequestType(RequestType.UPDATE);
            PerfromEnc(req);
        }
        else
        {
            req = JsonConvert.SerializeObject(FirebaseManager.Instance.PlayerDataPayload);
            ProcessDataUpdateAsync(StoredTokenResult, req);
        }
    }
    async public void ProcessDataUpdateAsync(string TokenResult, string req)
    {
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL + "UpdateUserBO", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + TokenResult;
        request.SetRequestHeader("Authorization", _reqToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Events.DoReportMessage(new messageInfo(request.error));
            Debug.Log(request.error);
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            if (request.result == UnityWebRequest.Result.Success)
            {
                FirebaseManager.Instance.OnDocUpdate("");
            }
            else
            {
                Events.DoReportMessage(new messageInfo((string)res.SelectToken("error").SelectToken("message")));
            }
        }
    }
    async public void ProccessLeaderboard(string _email, string _pwd)
    {
        if(Constants.PlayingAsGuest)
        {
            Events.DoReportMessage(new messageInfo("Leaderboard cannot be accessed in guest mode."));
            return;
        }
        string TokenResult = await GetBearerToken(_email, _pwd);

        if (TokenResult.Contains("error"))
        {
            Debug.Log("somthing went wrong while fetching bearer token : " + TokenResult);
            return;
        }

        StoredTokenResult = TokenResult;
        LeaderboardCounter _count = new LeaderboardCounter();
        _count.number = Constants.LeaderboardCount;

        LeaderboardPayload obj = new LeaderboardPayload();
        obj.data = _count;
        string req;
        if (Constants.AllowEnc)
        {
            req = JsonConvert.SerializeObject(obj.data);
            SetRequestType(RequestType.LEADERBOARD);
            PerfromEnc(req);
        }else
        {
            req = JsonConvert.SerializeObject(obj);
            GetLeaderboardAsync(StoredTokenResult,req);
        }
        
    }
    async public void GetLeaderboardAsync(string TokenResult,string req)
    {
        string _mainURL = BaseURL + "Leaderboard";

        if(Constants.AllowEnc)
            _mainURL = BaseURL + "LeaderboardE";

        using UnityWebRequest request = UnityWebRequest.Put(_mainURL, req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + TokenResult;
        request.SetRequestHeader("Authorization", _reqToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (Constants.AllowEnc)
                {
                    SetRequestType(RequestType.LEADERBOARD);
                    PerfromDec(request.downloadHandler.text);
                }
                else
                {
                    FirebaseManager.Instance.OnQueryUpdate(request.downloadHandler.text);
                }            
            }
            else
            {
                //MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
        }
    }
    public void sendVerificationAgain()
    {
        string _token = Constants.ResendTokenID;
        StartCoroutine(sendVerificationLink(_token, true));
    }
    private IEnumerator sendVerificationLink(string _tokenId, bool resendAgain = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "VERIFY_EMAIL");
        form.AddField("idToken", _tokenId);
        using UnityWebRequest request = UnityWebRequest.Post(emailVerification, form);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Events.DoFireRegsiterationFailed("");
            Events.DoReportMessage(new messageInfo(request.error));
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            if (resendAgain)
            {
                FirebaseManager.Instance.ResendEmailSent("");
                Events.DoReportMessage(new messageInfo("Verification link sent at provided email."));
            }
            else
            {
                Events.DoReportMessage(new messageInfo("Verification link sent at provided email."));
            }

            Events.DoFireVerificationSent();
        }
        else
        {
            Events.DoFireRegsiterationFailed("");
            // MainMenuViewController.Instance.SomethingWentWrongMessage();
            // if (resendAgain)
            // MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        }
    }
    private IEnumerator processForgetRequest(string _email)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "PASSWORD_RESET");
        form.AddField("email", _email);
        using UnityWebRequest request = UnityWebRequest.Post(forgetPassword, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //MainMenuViewController.Instance.SomethingWentWrong();
            ////Debug.Log(request.error);
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            FirebaseManager.Instance.OnPassEmailSent("");
        }
        else
        {
            FirebaseManager.Instance.OnPassEmailSentError("");
        }

    }
}
