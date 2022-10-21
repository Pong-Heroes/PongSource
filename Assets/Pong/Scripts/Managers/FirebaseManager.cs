using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

public class UserData
{
    public int TotalScore { get; set; }
    public string WalletAddress { get; set; }
    public object SignupTime { get; set; }
    public int ActiveUser { get; set; }
    public string Email { get; set; }
    public int TotalWins { get; set; }
    public double NumberOfTries { get; set; }
    public int AvatarID { get; set; }
    public string UserName { get; set; }
    public int TotalHits { get; set; }
    public string UID { get; set; }
}

public class AuthCredentials
{
    public string Email { get; set; }
    public string Password { get; set; }

    public string UserName { get; set; }

    public string WalletAddress { get; set; }
    public int AvatarID { get; set; }
}
public class updateDataPayload
{
    public UserData data { get; set; }
}

public class FirebaseManager : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void SetStorage(string key, string val);

    private int key = 129;
    private string UID = "";
    public UserData PlayerData=null;
    public updateDataPayload PlayerDataPayload;


    public UserData[] PlayerDataArray;
    public static FirebaseManager Instance;

    [HideInInspector]
    public AuthCredentials Credentails;

    string DocPath = "users";
    [HideInInspector]
    public bool DocFetched = false;
    [HideInInspector]
    public bool ResultFetched = false;
    bool DataFetchError = false;
    bool FetchUserData = false;
    bool UserDataFetched = false;
    void Awake()
    {
        Credentails = new AuthCredentials();

        if (!Instance)
        {
            Instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }

        //AuthenticateFirebase();
        if (Constants.isUsingFirebaseSDK)
            OnAuthChanged();
    }
    public void updatePlayerDataPayload()
    {
        PlayerDataPayload = new updateDataPayload();
        PlayerDataPayload.data = PlayerData;
    }

    //Setting playerData got from Login API
    //Call this Function if Constants.isUsingSDK is false
    public void SetPlayerData(string _response)
    {
        JToken response = JObject.Parse(_response);
        PlayerData = new UserData();

        if (!Constants.AllowEnc)
        {
            PlayerData.Email = (string)response.SelectToken("data").SelectToken("Email");
            PlayerData.UserName = (string)response.SelectToken("data").SelectToken("UserName");
            PlayerData.WalletAddress = (string)response.SelectToken("data").SelectToken("WalletAddress");
            PlayerData.UID = (string)response.SelectToken("data").SelectToken("UID");
            PlayerData.NumberOfTries = (double)response.SelectToken("data").SelectToken("NumberOfTries");
            PlayerData.AvatarID = (int)response.SelectToken("data").SelectToken("AvatarID");
            PlayerData.TotalWins = (int)response.SelectToken("data").SelectToken("TotalWins");
            PlayerData.TotalHits = (int)response.SelectToken("data").SelectToken("TotalHits");
            PlayerData.TotalScore = (int)response.SelectToken("data").SelectToken("TotalScore");
            PlayerData.SignupTime = (long)response.SelectToken("data").SelectToken("SignupTime");
        }else
        {
            PlayerData.Email = (string)response.SelectToken("Email");
            PlayerData.UserName = (string)response.SelectToken("UserName");
            PlayerData.WalletAddress = (string)response.SelectToken("WalletAddress");
            PlayerData.UID = (string)response.SelectToken("UID");
            PlayerData.NumberOfTries = (double)response.SelectToken("NumberOfTries");
            PlayerData.AvatarID = (int)response.SelectToken("AvatarID");
            PlayerData.TotalWins = (int)response.SelectToken("TotalWins");
            PlayerData.TotalHits = (int)response.SelectToken("TotalHits");
            PlayerData.TotalScore = (int)response.SelectToken("TotalScore");
            PlayerData.SignupTime = (long)response.SelectToken("SignupTime");
        }

        Constants.TotalWins = PlayerData.TotalWins;
        Constants.TotalHits = PlayerData.TotalHits;
        Constants.TotalScore = PlayerData.TotalScore;
        Constants.UserName = PlayerData.UserName;
        Constants.FlagSelectedIndex = PlayerData.AvatarID;
        Constants.WalletAddress = PlayerData.WalletAddress;

        Debug.Log("connected wallet address_____________: " + Constants.WalletAddress);
    }

    public void SetLocalStorage(string key, string data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetStorage(key, data);
#endif
    }

    public void OnAuthSuccess(string info)
    {
        //Debug.Log(info);        
    }


    public void OnAuthError(string error)
    {
        Debug.LogError(error);
    }

    public void CheckEmailForAuth(string _email, string _pass, string _username)
    {
        Credentails.Email = _email;
        Credentails.Password = _pass;
        Credentails.UserName = _username;
        //FirebaseAuth.CheckEmail(_email, gameObject.name, "OnEmailCheck", "OnEmailCheckError");
    }

    public void OnEmailCheck(string info)
    {
        // Debug.Log("Checked email : email existed");

        //if (MainMenuViewController.Instance)
        //MainMenuViewController.Instance.EmailAlreadyExisted();
    }

    public void OnEmailCheckError(string error)
    {
        if (error.Contains("Email Not Registered"))
        {
            // Debug.Log("Checked email : email does not exist, creating one.");
            CreateNewUser(Constants.SavedEmail, Constants.SavedPass);
        }
        else
        {
            Debug.LogError("Email check error : " + error);
        }
    }

    public void CreateNewUser(string _email, string _pass)
    {
        //FirebaseAuth.CreateUserWithEmailAndPassword(_email,_pass, gameObject.name, "OnCreateUser", "OnCreateUserError");
    }

    public void OnCreateUser(string info)
    {
        UID = info;
        FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));
        SendVerEmail();
        //MainMenuViewController.Instance.ShowToast(4f, "Verification link sent to entered email address, please check inbox (or spam) and click on link to verify then login.");
        //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        Invoke("CallWithDelay", 3f);


    }

    public void CallWithDelay()
    {
        //MainMenuViewController.Instance.DisableRegisterScreen();
    }

    public void OnCreateUserError(string error)
    {
        Debug.LogError("Create user error : " + error);
    }

    public void LoginUser(string _email, string _pass, string _username)
    {
        Credentails.Email = _email;
        Credentails.Password = _pass;
        Credentails.UserName = _username;
        //Login with Firebase SDK and API
        if (Constants.isUsingFirebaseSDK)
        {
            //FirebaseAuth.SignInWithEmailAndPassword(_email, _pass, gameObject.name, "OnLoginUser", "OnLoginUserError");
        }
        else
        {
            apiRequestHandler.Instance.signInWithEmail(_email, _pass);
        }
    }

    public void CheckVerification()
    {
        //FirebaseAuth.CheckEmailVerification(gameObject.name, "OnCheckEmail", "OnCheckEmail");
    }

    public void OnCheckEmail(string info)
    {
        if (info == "true")
        {
            //if (MainMenuViewController.Instance)
            //MainMenuViewController.Instance.OnLoginSuccess(false);
        }
        else
        {
            //Debug.Log("Email verification pending");
            //MainMenuViewController.Instance.ShowResendScreen(5f);
            //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            //MainMenuViewController.Instance.ResetRegisterFields();
        }
    }

    public void OnLoginUser(string info)
    {
        CheckVerification();
    }

    public void OnLoginUserError(string error)
    {
        Debug.LogError("Login User error: " + error);
        //if (MainMenuViewController.Instance)
        //MainMenuViewController.Instance.SomethingWentWrong();
    }

    public void LogoutUser()
    {
        ResetStorage();

        //if (MainMenuViewController.Instance)
        //{
        //    MainMenuViewController.Instance.EnableRegisterLogin();
        //    MainMenuViewController.Instance.ToggleMainMenuScreen(false);
        //}

        if (Constants.isUsingFirebaseSDK)
        {
            //FirebaseAuth.SignOut(gameObject.name, "OnSignOut", "OnSignOutError");
        }
    }

    public void ResetStorage()
    {
        string _json = "";
        SetLocalStorage(Constants.CredKey, _json);
        SetLocalStorage(Constants.WalletAccountKey, _json);
    }
    public void OnSignOut(string info)
    {
        //Debug.Log(info);
    }

    public void OnSignOutError(string info)
    {
        Debug.LogError("Logout User error : " + info);
    }

    public void OnAuthChanged()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //FirebaseAuth.OnAuthStateChanged(gameObject.name, "OnAuthChangedSuccess", "OnAuthChangedError");
#endif
    }

    public void OnAuthChangedSuccess(string user)
    {
        //var parsedUser = StringSerializationAPI.Deserialize(typeof(FirebaseUser), user) as FirebaseUser;
        //UID = parsedUser.uid;
        //DisplayData($"Email: {parsedUser.email}, UserId: {parsedUser.uid}, EmailVerified: {parsedUser.isEmailVerified}");
    }

    public void OnAuthChangedError(string info)
    {
        UID = "";
        Debug.LogError("Auth change error : " + info);
    }

    public void AddFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        //FirebaseFirestore.SetDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnAddData", "OnAddDataError");
    }
    public void OnAddData(string info)
    {
        // Debug.Log("Data successfully added on firestore");
    }

    public void OnAddDataError(string error)
    {
        Debug.LogError("firestore data add error: " + error);
    }

    public void GetFireStoreData(string _collectionID, string _docID)
    {
        DataFetchError = false;
        DocFetched = false;
        ResultFetched = false;
        // FirebaseFirestore.GetDocument(_collectionID, _docID, gameObject.name, "OnDocGet", "OnDocGetError");
    }

    public void OnDocGet(string info)
    {
        if (info == null || info == "null")
        {
            UserDataFetched = false;
            DataFetchError = true;
            DocFetched = false;
            ResultFetched = true;
            FetchUserData = true;
            Debug.LogError("doc was fetched but is null");
        }
        else
        {
            Debug.Log("doc was fetched successfully from firestore");
            DataFetchError = false;
            PlayerData = JsonConvert.DeserializeObject<UserData>(info);
            UserDataFetched = true;
            DocFetched = true;
            ResultFetched = true;
            FetchUserData = true;
        }
    }

    public void OnDocGetError(string error)
    {
        UserDataFetched = false;
        DataFetchError = true;
        DocFetched = false;
        ResultFetched = true;
        FetchUserData = true;
        Debug.Log("Doc fetching error : " + error);
    }

    public IEnumerator FetchUserDB(string _walletID, string _username)
    {
        if (Constants.isUsingFirebaseSDK)
        {
            UserDataFetched = false;
            FetchUserData = false;
            GetFireStoreData(DocPath, _walletID);
        }
        else
        {
            UserDataFetched = true;
            FetchUserData = true;
        }

        yield return new WaitUntil(() => FetchUserData == true);
        if (UserDataFetched)
        {
            //Debug.Log(_walletID);
            //Debug.Log(PlayerData.WalletAddress);
            //Debug.Log(PlayerData.UserName);
            //Debug.Log(PlayerData.TimeSeconds);
            //Debug.Log(PlayerData.UID);
            //Debug.Log(PlayerData.NumberOfTries);
            //Debug.Log(PlayerData.PassBought);
            //Debug.Log(PlayerData.Email);
            //Debug.Log(PlayerData.TournamentEndDate);
            Constants.UserName = PlayerData.UserName;
            Constants.FlagSelectedIndex = PlayerData.AvatarID;
            Constants.TotalWins = PlayerData.TotalWins;
            Constants.TotalHits = PlayerData.TotalHits;
            Constants.TotalScore = PlayerData.TotalScore;

            //if (MainMenuViewController.Instance)
            //MainMenuViewController.Instance.ChangeUserNameText(Constants.UserName);

            //if (Constants.PushingTime)
            //{
            //    Constants.PushingTime = false;
            //    GamePlayUIHandler.Instance.SubmitTime();
            //}
        }
        else
        {
            Debug.LogError("something went wrong with user data fetching, trying again");
            StartCoroutine(FetchUserDB(PlayerPrefs.GetString("Account"), ""));
            yield return null;
        }
    }

    public IEnumerator CheckCreateUserDB(string _walletID, string _username)
    {
        DataFetchError = false;
        DocFetched = false;
        ResultFetched = false;

        if (TournamentManager.Instance)
            TournamentManager.Instance.GetTournamentDataDB();

        if (Constants.isUsingFirebaseSDK)
            GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => ResultFetched == true);

        if (DocFetched == true) //document existed
        {
            //Debug.Log(_walletID);
            //Debug.Log(PlayerData.WalletAddress);
            //Debug.Log(PlayerData.UserName);
            //Debug.Log(PlayerData.TimeSeconds);
            //Debug.Log(PlayerData.UID);
            //Debug.Log(PlayerData.NumberOfTries);
            //Debug.Log(PlayerData.PassBought);
            //Debug.Log(PlayerData.Email);
            //Debug.Log(PlayerData.TournamentEndDate);
            Constants.UserName = PlayerData.UserName;
            Constants.FlagSelectedIndex = PlayerData.AvatarID;
            Constants.TotalWins = PlayerData.TotalWins;
            Constants.TotalHits = PlayerData.TotalHits;
            Constants.TotalScore = PlayerData.TotalScore;

        }
        else
        {
            PlayerData = new UserData();
            PlayerData.WalletAddress = _walletID;
            PlayerData.UserName = Constants.SavedUserName;
            Constants.UserName = PlayerData.UserName;
            PlayerData.TotalWins = Constants.TotalWins;
            PlayerData.TotalHits = Constants.TotalHits;
            PlayerData.TotalScore = Constants.TotalScore;
            PlayerData.UID = UID;
            PlayerData.NumberOfTries = 0;
            PlayerData.Email = Constants.SavedEmail;
            PlayerData.AvatarID = Constants.FlagSelectedIndex;

            if (Constants.isUsingFirebaseSDK)
                AddFireStoreData(PlayerData);
        }

        //if (MainMenuViewController.Instance)
        //    MainMenuViewController.Instance.ChangeUserNameText(Constants.UserName);

        //if(Constants.PushingTime)
        //{
        //    Constants.PushingTime = false;
        //    GamePlayUIHandler.Instance.SubmitTime();
        //}
    }

    public void SendVerEmail()
    {
        //FirebaseAuth.SendEmailVerification(gameObject.name, "OnEmailSent", "OnEmailSentError");
    }

    public void OnEmailSent(string info)
    {
    }

    public void OnEmailSentError(string info)
    {
        Debug.LogError("Sending Verfication email error: " + info);
        //Invoke("SendVerEmail", 1f);
    }

    public IEnumerator CheckWalletDB(string _walletID)
    {
        GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => ResultFetched == true);

        if (DocFetched == true) //document existed
        {
            //MainMenuViewController.Instance.DBChecked(true);
        }
        else
        {
            //MainMenuViewController.Instance.DBChecked(false);
        }

    }

    public void UpdatedFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        if (Constants.isUsingFirebaseSDK)
        {
            //FirebaseFirestore.UpdateDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnDocUpdate","OnDocUpdateError");
        }
        else
        {
            //TODO: call update api to update the data
            apiRequestHandler.Instance.updatePlayerData();
        }
    }

    public void OnDocUpdate(string info)
    {
        if (Constants.PushingTries)
        {
            Constants.PushingTries = false;
            return;
        }

        if (Constants.PushingWins)
        {
            Constants.PushingWins = false;
            return;
        }

        //if (RaceManager.Instance)
        //    RaceManager.Instance.RaceEnded();
    }

    public void OnDocUpdateError(string error)
    {
        Debug.LogError("Doc update error : " + error);
    }

    public void QueryDB(string _field, string _type)
    {
        if (Constants.isUsingFirebaseSDK)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //FirebaseFirestore.QueryDB(DocPath, _field, _type, gameObject.name, "OnQueryUpdate", "OnQueryUpdateError");
#endif
        }
        else
        {
            //Send Leaderboard request to api
            apiRequestHandler.Instance.getLeaderboard();
        }
    }

    public void OnQueryUpdate(string info)
    {
        PlayerDataArray = JsonConvert.DeserializeObject<UserData[]>(info);
        Events.DoFireGetUserData(PlayerDataArray);
    }

    public void OnQueryUpdateError(string error)
    {
        Debug.LogError("Leaderboard query error : " + error);
    }

    public string EncryptDecrypt(string textToEncrypt)
    {
        StringBuilder inSb = new StringBuilder(textToEncrypt);
        StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inSb[i];
            c = (char)(c ^ key);
            outSb.Append(c);
        }
        return outSb.ToString();
    }

    public void SendPasswordResetEmail(string _email)
    {
        Constants.EmailSent = _email;
        if (Constants.isUsingFirebaseSDK)
        {
            //FirebaseAuth.SendPasswordResetEmail(_email, gameObject.name, "OnPassEmailSent", "OnPassEmailSentError");
        }
        else
        {
            apiRequestHandler.Instance.onForgetPassword(_email);
        }
    }
    public void OnPassEmailSent(string info)
    {
        //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        //MainMenuViewController.Instance.BackClicked_PasswordReset();
        //MainMenuViewController.Instance.ShowToast(4f,"Reset password link sent to entered email address, please click the link in inbox to reset password.");
    }

    public void OnPassEmailSentError(string info)
    {
        Debug.LogError("Password resent sending error : " + info);
        //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        //MainMenuViewController.Instance.BackClicked_PasswordReset();
        //MainMenuViewController.Instance.ShowToast(4f, "Something went wrong while sending password reset link, please try again later.");
    }

    public void CallOnError()
    {
        SendPasswordResetEmail(Constants.EmailSent);
    }
    public void showVerificationScreen()
    {
        //Debug.Log("Email verification pending");
        //MainMenuViewController.Instance.ShowResendScreen(5f);
        //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        //MainMenuViewController.Instance.ResetRegisterFields();
    }

    public void ResendVerificationEmail()
    {
        //MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        if (Constants.isUsingFirebaseSDK)
        {
            //FirebaseAuth.SendEmailVerification(gameObject.name, "ResendEmailSent", "ResendEmailSentError");
        }
        else
        {
            apiRequestHandler.Instance.sendVerificationAgain();
        }
    }

    public void ResendEmailSent(string info)
    {
        //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        //MainMenuViewController.Instance.DisableResendScreen();
        //MainMenuViewController.Instance.ShowToast(4f, "Confirmation link sent again, please click the link in inbox (or spam) to confirm.");
    }

    public void ResendEmailSentError(string info)
    {
        Debug.LogError("Resend verification email error : " + info);
        //MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        //MainMenuViewController.Instance.DisableResendScreen();
        //MainMenuViewController.Instance.ShowToast(4f, "Something went wrong while sending confirmation link, please try again later.");
    }

}
