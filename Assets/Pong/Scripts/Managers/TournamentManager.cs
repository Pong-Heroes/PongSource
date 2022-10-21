using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class Timestamp
{
    public double seconds { get; set; }
    public double nanoseconds { get; set; }
}
public class StartDate
{
    public double seconds { get; set; }
    public double nanoseconds { get; set; }
}
public class EndDate
{
    public double seconds { get; set; }
    public double nanoseconds { get; set; }
}
public class TournamentData
{
    public Timestamp timestamp { get; set; }
    public int TicketPrice { get; set; }
    public StartDate StartDate { get; set; }
    public EndDate EndDate { get; set; }
    public int Week { get; set; }
    public int PassPrice { get; set; }
    public int DiscountPercentage { get; set; }
    public int DiscountOnCrace { get; set; }
    public bool IsSingleMap { get; set; }
    public int LevelIndex { get; set; }

    public int GTicketPrice { get; set; }
    public StartDate GStartDate { get; set; }
    public EndDate GEndDate { get; set; }
    public int GPassPrice { get; set; }
}
public class TournamentClassData
{
    public TournamentData data { get; set; }
}

public class TournamentManager : MonoBehaviour
{
    [HideInInspector]
    public TournamentData DataTournament;
    public static TournamentManager Instance;

    string CollectionPath = "tournament";
    string DocPath = "TournamentData";

    bool StartTimer = false;
    bool TournamentStartTimer = false;

    string MainTime;
    double RemainingTimeSeconds;
    double StartTimeDiffSeconds;
    TimeSpan RemainingTime;
    TimeSpan TournamentRemainingTime;

    float timeSpanConversionDays;//var to hold days after converstion from seconds
    float timeSpanConversionHours;//var to hold hours after converstion from seconds
    float timeSpanConversiondMinutes;//var to hold minutes after converstion from seconds
    float timeSpanConversionSeconds;//var to hold seconds after converstion from float seconds

    string textfielddays;//string store converstion of days into string for display
    string textfieldHours;//string store converstion of hours into string for display
    string textfieldMinutes;//string store converstion of minutes into string for display
    string textfieldSeconds;//string store converstion of seconds into string for display
    
    private const string firebaseLoginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private const string firebaseApiKey = "AIzaSyDG7C0sHN4-hXhjOyTdyaBpQc3BArIBVsU";
    private  string torunamentDataURL ;
    private void OnEnable()
    {
        Instance = this;
        StartTimer = false;
        TournamentStartTimer = false;

        if (Constants.IsStagging)
            torunamentDataURL = "https://us-central1-pong-tournaments.cloudfunctions.net";
        else
            torunamentDataURL = "https://us-central1-pong-tournaments.cloudfunctions.net";
     
        GetTournamentDataDB();
      
    }
    void Update()
    {
        //for coinracer tournament
        if (StartTimer)
        {
            RemainingTimeSeconds -= Time.deltaTime;
            ConvertTime(RemainingTimeSeconds);
            DisplayTournamentTimer();

            if(RemainingTimeSeconds<=0)
            {
                //MainMenuViewController.Instance.UITournament.TimerText.text = "0:00:00:00";
                StartTimer = false;
                GetTournamentDataDB();
            }
        }else if(TournamentStartTimer)
        {
            StartTimeDiffSeconds -= Time.deltaTime;
            ConvertTime(StartTimeDiffSeconds);
            DisplayTournamentTimer();

            if (StartTimeDiffSeconds <= 0)
            {
                //MainMenuViewController.Instance.UITournament.TimerText.text = "0:00:00:00";
                TournamentStartTimer = false;
                GetTournamentDataDB();
            }
        }
    }

    /// <summary>
    /// function called at start to check tournament status from DB and start timer 
    /// </summary>
    public void StartTournamentCounter(bool isError=false, TournamentData _data=null)
    {
            if (isError)
            {
                Constants.TournamentActive = false;
                ManipulateTournamnetUIActivness(false, true, false, false, true,false,false);
                ManipulateTournamnetStartTimer("0:00:00:00");
                StartTimer = false;
                TournamentStartTimer = false;
            }
            else
            {
                RemainingTimeSeconds = _data.EndDate.seconds - _data.timestamp.seconds;
                StartTimeDiffSeconds = _data.timestamp.seconds-_data.StartDate.seconds;

                //Debug.Log(_data.GEndDate.seconds);
                //Debug.Log(_data.timestamp.seconds);
                //Debug.Log(_data.GStartDate.seconds);
                //Debug.Log(GStartTimeDiffSeconds);

                //for coinracer tournament
                if (Mathf.Sign((float)StartTimeDiffSeconds) == -1)
                {
                    StartTimeDiffSeconds = Mathf.Abs((float)StartTimeDiffSeconds);
                    TournamentRemainingTime = TimeSpan.FromSeconds(StartTimeDiffSeconds);
                    ManipulateTournamnetUIActivness(false, true, false, false, true,true,false);
                    StartTimer = false;
                    TournamentStartTimer = true;
                    ManipulateTournamnetStartTimer(TournamentRemainingTime.Days.ToString() + ":" + TournamentRemainingTime.Hours.ToString() + ":" + TournamentRemainingTime.Minutes.ToString() + ":" + TournamentRemainingTime.Seconds.ToString());
                    Constants.TournamentActive = false;

                }
                else
                {
                    if (Mathf.Sign((float)RemainingTimeSeconds) == -1)
                    {
                        Constants.TournamentActive = false;
                        ManipulateTournamnetUIActivness(false, true, false, false, true, false,false);
                        ManipulateTournamnetStartTimer("0:00:00:00");
                        StartTimer = false;
                        TournamentStartTimer = false;
                    }
                    else
                    {
                        Constants.TournamentActive = true;
                        RemainingTime = TimeSpan.FromSeconds(RemainingTimeSeconds);

                        //Debug.LogError(RemainingTime.Days.ToString() + ":" + RemainingTime.Hours.ToString() + ":" + RemainingTime.Minutes.ToString() + ":" + RemainingTime.Seconds.ToString());

                        ManipulateTournamnetUIActivness(true, true, true, false, false, false,true);
                        ManipulateTournamnetUIData("", RemainingTime.Days.ToString() + ":" + RemainingTime.Hours.ToString() + ":" + RemainingTime.Minutes.ToString() + ":" + RemainingTime.Seconds.ToString(), "*Entry Ticket : " + _data.TicketPrice.ToString() + " $PONG", "*"+Constants.DiscountPercentage.ToString()+"% off if you hold "+Constants.DiscountForPong.ToString()+" $PONG or more");
                        StartTimer = true;
                        TournamentStartTimer = false;
                    }
                }
            }
    }

    public void ConvertTime(double _sec)
    {
        //Store TimeSpan into variable.
        timeSpanConversionDays = TimeSpan.FromSeconds(_sec).Days;
        timeSpanConversionHours = TimeSpan.FromSeconds(_sec).Hours;
        timeSpanConversiondMinutes = TimeSpan.FromSeconds(_sec).Minutes;
        timeSpanConversionSeconds = TimeSpan.FromSeconds(_sec).Seconds;

        //Convert TimeSpan variables into strings for textfield display
        textfielddays = timeSpanConversionDays.ToString();
        textfieldHours = timeSpanConversionHours.ToString();
        textfieldMinutes = timeSpanConversiondMinutes.ToString();
        textfieldSeconds = timeSpanConversionSeconds.ToString();
    }


    public void DisplayTournamentTimer()
    {
        MainTime = textfielddays + ":" + textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds;
        //MainMenuViewController.Instance.UITournament.TimerText.text = MainTime;
    }

    public void GetTournamentDataDB()
    {
        if (Constants.isUsingFirebaseSDK)
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
        //FirebaseFirestore.GetTournamentData(CollectionPath, DocPath, gameObject.name, "OnGetTournamentData", "OnGetTournamentDataError");
        #endif
        }
        else
        {
            getTournamentData();
        }
    }

    public void OnGetTournamentData(string info)
    {
        //Debug.Log("Data successfully fetched for tournament");

        if (Constants.isUsingFirebaseSDK)
        {
            if (info != null && info != "null")
            {
                DataTournament = JsonConvert.DeserializeObject<TournamentData>(info);

                //Constants.TournamentPassPrice = DataTournament.PassPrice;
                //Constants.DiscountPercentage = DataTournament.DiscountPercentage;
                //Constants.DiscountForCrace = DataTournament.DiscountOnCrace;
                //Constants.TicketPrice = DataTournament.TicketPrice;

                StartTournamentCounter(false, DataTournament);
            }
            else
            {
                OnGetTournamentDataError(info);
            }
        }
        else
        {
            if (DataTournament != null )
            {
               // Debug.Log(DataTournament.PassPrice);
               // DataTournament = JsonConvert.DeserializeObject<TournamentData>(info);

                //Constants.TournamentPassPrice = DataTournament.PassPrice;
                //Constants.DiscountPercentage = DataTournament.DiscountPercentage;
                //Constants.DiscountForCrace = DataTournament.DiscountOnCrace;
                //Constants.TicketPrice = DataTournament.TicketPrice;

                StartTournamentCounter(false, DataTournament);
            }
            else
            {
                OnGetTournamentDataError("Info Not Found");
            }
        }
    }
    public void OnGetTournamentDataError(string error)
    {
        StartTournamentCounter(true,null);
    }
    public void ManipulateTournamnetUIActivness(bool LowerHeaderActive, bool TimerActive, bool FotterActive, bool LoaderObjActive, bool DisclaimerActive, bool DisclaimerActive2,bool _isActive)
    {
        //MainMenuViewController.Instance.UITournament.LowerHeaderText.gameObject.SetActive(LowerHeaderActive);
        //MainMenuViewController.Instance.UITournament.TimerText.gameObject.SetActive(TimerActive);
        //MainMenuViewController.Instance.UITournament.FotterText.gameObject.SetActive(FotterActive);
        //MainMenuViewController.Instance.UITournament.LoaderObj.gameObject.SetActive(LoaderObjActive);
        //MainMenuViewController.Instance.UITournament.NextWeekScreen.gameObject.SetActive(DisclaimerActive);
        //MainMenuViewController.Instance.UITournament.TournamentStartText.gameObject.SetActive(DisclaimerActive2);
        //MainMenuViewController.Instance.UITournament.ActiveScreen.SetActive(_isActive);
    }


    public void ManipulateTournamnetUIData(string LowerHeaderText, string TimerText, string FotterText, string Fotter2Text)
    {
        //MainMenuViewController.Instance.UITournament.LowerHeaderText.text = LowerHeaderText;
        //MainMenuViewController.Instance.UITournament.TimerText.text = TimerText;
        //MainMenuViewController.Instance.UITournament.FotterText.text = FotterText;
        //MainMenuViewController.Instance.UITournament.Fotter2Text.text = Fotter2Text;
    }


    public void ManipulateTournamnetStartTimer(string TimerText)
    {
        //if (MainMenuViewController.Instance) //if instance of UI class is created
        //{
        //    MainMenuViewController.Instance.UITournament.TimerText.text = TimerText;
        //}
    }

    public void getTournamentData()
    {
        StartCoroutine(processTournamentRequest());
    }
     private IEnumerator processTournamentToken(string _email, string _password)
     {
         WWWForm form = new WWWForm();
         form.AddField("email", _email);
         form.AddField("password", _password);
         form.AddField("returnSecureToken", "true");
         using UnityWebRequest request = UnityWebRequest.Post(firebaseLoginUrl + firebaseApiKey, form);
         yield return request.SendWebRequest();
         if (request.result == UnityWebRequest.Result.ConnectionError)
         {
             //Debug.Log(request.error);
         }
         else if (request.result == UnityWebRequest.Result.Success)
         {
             //Debug.Log("Result is: ");
             //Debug.Log(request.result);
             //Debug.Log(request.downloadHandler.text);
             JToken token = JObject.Parse(request.downloadHandler.text);
             string tID = (string) token.SelectToken("idToken");
             StartCoroutine(processTournamentRequest());
             //Debug.Log(tID);
         }
         else
         {
             //MainMenuViewController.Instance.SomethingWentWrong();
         }
     }
     private IEnumerator processTournamentRequest()
    {
        using UnityWebRequest request = UnityWebRequest.Get(torunamentDataURL);
       // request.SetRequestHeader("Authorization","Bearer "+ _token);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //MainMenuViewController.Instance.SomethingWentWrongMessage();
           // Debug.Log(request.error);
        }
        else
        {
            //"message": "Unauthorized" //wrong password
            // JToken response = JObject.Parse(request.downloadHandler.text);
            // string reqResponse = (string)response.SelectToken("data").SelectToken("Email");
            
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            JToken token = JObject.Parse(request.downloadHandler.text);
            
           // JsonConvert.DeserializeObject<TournamentClassData>(token.SelectToken("data"));
            // string tID = (string)token.SelectToken("data");
          
            
            
            if ((string) token.SelectToken("message") == "No User Found.")
            {
               //MainMenuViewController.Instance.SomethingWentWrong();
            }
            else if ((string) token.SelectToken("message") == "Unauthorized")
            {
               // MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) token.SelectToken("message") == "Required parameters are missing")
            {
               // MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) token.SelectToken("message") == "Invalid request.")
            {
               // MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                DataTournament = new TournamentData();
                DataTournament.PassPrice = (int)token.SelectToken("data").SelectToken("PassPrice");
                DataTournament.DiscountPercentage = (int)token.SelectToken("data").SelectToken("DiscountPercentage");
                DataTournament.Week = (int)token.SelectToken("data").SelectToken("Week");
                DataTournament.TicketPrice = (int)token.SelectToken("data").SelectToken("TicketPrice");
                DataTournament.DiscountOnCrace = (int)token.SelectToken("data").SelectToken("DiscountOnCrace");
                DataTournament.timestamp = new Timestamp();
                DataTournament.timestamp.nanoseconds = (double)token.SelectToken("data").SelectToken("timestamp").SelectToken("_nanoseconds");
                DataTournament.timestamp.seconds = (double)token.SelectToken("data").SelectToken("timestamp").SelectToken("_seconds");
                DataTournament.StartDate = new StartDate();
                DataTournament.StartDate.nanoseconds = (double)token.SelectToken("data").SelectToken("StartDate").SelectToken("_nanoseconds");
                DataTournament.StartDate.seconds = (double)token.SelectToken("data").SelectToken("StartDate").SelectToken("_seconds");
                DataTournament.IsSingleMap= (bool)token.SelectToken("data").SelectToken("IsSingleMap");
                DataTournament.LevelIndex = (int)token.SelectToken("data").SelectToken("LevelIndex");

                DataTournament.EndDate = new EndDate();
                DataTournament.EndDate.nanoseconds = (double)token.SelectToken("data").SelectToken("EndDate").SelectToken("_nanoseconds");
                DataTournament.EndDate.seconds = (double)token.SelectToken("data").SelectToken("EndDate").SelectToken("_seconds");

                DataTournament.GTicketPrice = (int)token.SelectToken("data").SelectToken("GTicketPrice");
                DataTournament.GPassPrice = (int)token.SelectToken("data").SelectToken("GPassPrice");
                DataTournament.GStartDate = new StartDate();
                DataTournament.GStartDate.nanoseconds = (double)token.SelectToken("data").SelectToken("GStartDate").SelectToken("_nanoseconds");
                DataTournament.GStartDate.seconds = (double)token.SelectToken("data").SelectToken("GStartDate").SelectToken("_seconds");
                DataTournament.GEndDate = new EndDate();
                DataTournament.GEndDate.nanoseconds = (double)token.SelectToken("data").SelectToken("GEndDate").SelectToken("_nanoseconds");
                DataTournament.GEndDate.seconds = (double)token.SelectToken("data").SelectToken("GEndDate").SelectToken("_seconds");

                OnGetTournamentData("");
            }
            else
            {
                //MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            
            //UserData _player;
            //_player.UserName = 
        }
    }
}
