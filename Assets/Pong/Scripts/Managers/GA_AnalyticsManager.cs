using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;

public class BusinessEventPayload
{
    public string CurrencyName; //name of the currency
    public double CurrencyAmount;//amount of currency
    public string ItemType; //The type / category of the item.//GoldPacks
    public string ItemID;//Specific item bought.//1000GoldPack
    public string CartType;//The game location of the purchase.Max 10 unique values. //EndOfLevel
}

public class ProgressionEventPayload
{
    public GAProgressionStatus ProgressionStatus; //Status of added progression (start, complete, fail). //0=undfeiund,1=start,2=complete,3=fail
    public string Mode;//will represent either mode is practice or any other
    public string MapUsed;//represents Map
    public string Difficulty;//will represent Car name
    public int TimeSeconds;
    public Dictionary<string, object> fields = new Dictionary<string, object>();
}
public class GA_AnalyticsManager : MonoBehaviour
{
    public static GA_AnalyticsManager Instance;
    public BusinessEventPayload PayloadBusinessEvent;
    public ProgressionEventPayload PayloadProgressionEvent;
    public ProgressionEventPayload StoredProgression;

    string LogLevel;
    GAErrorSeverity ErrorSeverity;
    Dictionary<string, object> Errorfields = new Dictionary<string, object>();
    void Start()
    {
        if (!Instance)
        {
            Instance = this;
            // DontDestroyOnLoad(this.gameObject);

            PayloadBusinessEvent = new BusinessEventPayload();
            PayloadProgressionEvent = new ProgressionEventPayload();
            StoredProgression= new ProgressionEventPayload();

#if UNITY_WEBGL && !UNITY_EDITOR
	    Application.logMessageReceived += HandleLog;
#endif

            GameAnalytics.Initialize();
        }else
        {
            //Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
	    Application.logMessageReceived -= HandleLog;
#endif
    }

    #region BusinessEvents/TransactionEvents

    public void SetBusinessPayload(double amount, string itemType, string itemId, string cartType)
    {
        PayloadBusinessEvent.CurrencyName = Constants.VirtualNameCurrency;
        PayloadBusinessEvent.CurrencyAmount = amount;
        PayloadBusinessEvent.ItemType = itemType;
        PayloadBusinessEvent.ItemID = itemId;
        PayloadBusinessEvent.CartType = cartType;
    }

    public void RegisterBusinessEvents(BusinessEventPayload Obj)
    {
        GameAnalytics.NewBusinessEvent(Obj.CurrencyName, (int)Obj.CurrencyAmount, Obj.ItemType, Obj.ItemID, Obj.CartType);
    }

    public void TournamentTicketEvent(double _amount)
    {
        SetBusinessPayload(_amount, "Tournament", "TournamentTicket", "MainMenu");
        RegisterBusinessEvents(PayloadBusinessEvent);
    }

    public void TournamentPassEvent(double _amount)
    {
        SetBusinessPayload(_amount, "Tournament", "TournamentPass", "MainMenu");
        RegisterBusinessEvents(PayloadBusinessEvent);
    }

    public void MultiplayerEvent(double _amount)
    {
        SetBusinessPayload(_amount, "Multiplayer", "BidAmount", "MainMenu");
        RegisterBusinessEvents(PayloadBusinessEvent);
    }

    #endregion

    #region ProgressionEvents(Level attempts with Start, Fail & Complete event)
    public void SetProgressionePayload(GAProgressionStatus status, string prog1, string prog2, string prog3, int time, Dictionary<string, object> _fields)
    {
        PayloadProgressionEvent.ProgressionStatus = status;
        PayloadProgressionEvent.Mode = prog1;
        PayloadProgressionEvent.MapUsed = prog2;
        PayloadProgressionEvent.Difficulty = prog3;
        PayloadProgressionEvent.TimeSeconds = time; //can be 0 if there is no logic of time
        PayloadProgressionEvent.fields = _fields;
    }

    public void ProgressionEvents(ProgressionEventPayload Obj)
    {
        GameAnalytics.NewProgressionEvent(Obj.ProgressionStatus, Obj.Mode, Obj.MapUsed, Obj.Difficulty, Obj.TimeSeconds,Obj.fields);
    }

    public void PushProgressionEvent(bool isStart,bool isWon, bool isFail)
    {
        GameProgressionEvent(StoredProgression.Mode, StoredProgression.MapUsed, StoredProgression.Difficulty, StoredProgression.TimeSeconds, StoredProgression.fields,isStart, isWon,isFail);
    }

    public void GameProgressionEvent(string _mode,string _map,string _carName,int _time, Dictionary<string, object> Optionsfields, bool isStart, bool isWon, bool isFail)
    {
        //Dictionary<string, object> Optionsfields = new Dictionary<string, object>();
        //Optionsfields.Add("TimeSeconds", _timeString);
        //Optionsfields.Add("NFTID", nftID);

        if(isStart)
            SetProgressionePayload(GAProgressionStatus.Start, _mode, _map, _carName, _time, Optionsfields);
        else if(isWon)
            SetProgressionePayload(GAProgressionStatus.Complete, _mode, _map, _carName, _time, Optionsfields);
        else if (isFail)
            SetProgressionePayload(GAProgressionStatus.Fail, _mode, _map, _carName, _time, Optionsfields);

        ProgressionEvents(PayloadProgressionEvent);
    }
    #endregion

    #region ErrorEvents
    public void RegisterErrorEvents(GAErrorSeverity severity, string message,Dictionary<string, object> fields )
    {
        GameAnalytics.NewErrorEvent(severity, message, fields);
    }

    public void GameErrorEvent(GAErrorSeverity severity,string msg,string trace)
    {
        Errorfields.Clear();
        Errorfields.Add("Stack_Trace", trace);

        RegisterErrorEvents(severity,msg, Errorfields);
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                ErrorSeverity = GAErrorSeverity.Error;
                break;
            case LogType.Assert:
                ErrorSeverity = GAErrorSeverity.Info;
                break;
            case LogType.Warning:
                ErrorSeverity = GAErrorSeverity.Warning;
                break;
            case LogType.Log:
                ErrorSeverity = GAErrorSeverity.Debug;
                break;
            case LogType.Exception:
                ErrorSeverity = GAErrorSeverity.Critical;
                break;
            default:
                break;
        }

        GameErrorEvent(ErrorSeverity, logString, stackTrace);
    }
    #endregion

    #region DesignEvents
    public void RegisterDesignEvents(string eventName, float eventValue)
    {
        //eventName : The event string can have 1 to 5 parts.The parts are separated by �:� with a max length of 64 each.e.g. �world1:kill:robot:laser�. The parts can be written only with a-zA-Z, 0-9, -_.,:()!? characters.
        //eventValue : Number value of event
        GameAnalytics.NewDesignEvent(eventName, eventValue);
    }

    public void GameDesignEvent(string eventName, float eventValue)
    {
        RegisterDesignEvents(eventName,eventValue);
    }
    #endregion

    #region ResourceEvents
    public void RegisterResourceEvents(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
    {
        //flowType : Add(source) or subtract(sink) resource.
        //currency : One of the available currencies set in GA_Settings(Setup tab).This string can only contain[A - Za - z] characters.
        //amount : Amount sourced or sunk.
        //itemType : One of the available item types set in GA_Settings (Setup tab).
        //itemId : Item id (string max length = 32)

        GameAnalytics.NewResourceEvent(flowType, currency, amount, itemType, itemId);
    }
    #endregion
}
