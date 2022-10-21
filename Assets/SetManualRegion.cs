using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SetManualRegion : MonoBehaviour
{
    public Dropdown RegionDropDown;
    private List<string> RegionString = new List<string>();

    public void SetRegionString(List<string> _region)
    {
        RegionString = _region;
    }

    public List<string> GetRegionString()
    {
        return RegionString;
    }

    public void DropDownValueChanged()
    {
        int index = RegionDropDown.value;
        //Debug.Log(index);
        if (RegionString.Count > 0 && index < RegionString.Count)
        {
            if (Constants.SelectedRegion != RegionString[index])
            {
                PhotonLauncher.DisconnectMaster();
                PhotonLauncher.ConnectMaster();

                Constants.SelectedRegion = RegionString[index];
                var connected = PhotonNetwork.ConnectToRegion(Constants.SelectedRegion);
                Debug.Log("connected to region : " + RegionString[index] + connected);
                Debug.Log("Region changed called");
                // Constants.RegionChanged = true;

                //RegionDropDown.interactable = false;
                //Invoke("ConnectPhotonAgain", 0.1f);
            }
        }
        else
        {
            //Debug.Log("region string is empty");
        }
    }

    public void ConnectPhotonAgain()
    {
        RegionDropDown.interactable = true;
        PhotonLauncher.ConnectMaster();
    }

    // public IEnumerator ShowPingedRegionList_ConnectionUI()
    // {
    //     yield return new WaitUntil(() => PhotonNetwork.best);
    //     UpdatePingList(PhotonNetwork.pingedRegions, PhotonNetwork.pingedRegionPings);
    //     PhotonNetwork.GotPingResult = false;
    // }

    // public void UpdatePingList(string[] regions, string[] pings)
    // {
    //     if (!RegionPinged)
    //     {
    //         RegionPinged = true;
    //         //RegionPingsDropdown
    //         var dropdown = UIConnection.RegionPingsDropdown.GetComponent<Dropdown>();
    //         SetManualRegion _RegionRef = UIConnection.RegionPingsDropdown.GetComponent<SetManualRegion>();
    //         dropdown.options.Clear();
    //         List<string> _regions = new List<string>();
    //         if (pings.Length > 0)
    //         {
    //             int minimumPing = int.Parse(pings[0]);
    //             int currentPing;
    //             dropdown.value = 1;
    //             for (int i = 0; i < regions.Length; i++)
    //             {
    //                 dropdown.options.Add(new Dropdown.OptionData() { text = regions[i] + " " + pings[i] + "ms" });
    //                 _regions.Add(regions[i]);
    //                 currentPing = int.Parse(pings[i]);
    //                 if (currentPing < minimumPing)
    //                 {
    //                     minimumPing = currentPing;
    //                     dropdown.value = i + 1;
    //                 }
    //             }

    //             _RegionRef.SetRegionString(_regions);
    //         }
    //         else
    //         {
    //             //Debug.LogError("region list is empty");
    //         }
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
