var CryptoJS = window.CryptoJS;
var InitResponse="";
var parsedObjectName="";
var parsedCallback="";


function Client_Enc(_data,_data2) {
	var encrypted = CryptoJS.TripleDES.encrypt(_data, _data2);
	ResponseCallback(encrypted.toString());
};

function Client_Dec(_data,_data2) {
	var bytes = CryptoJS.TripleDES.decrypt(_data, _data2);
    var plainText = bytes.toString(CryptoJS.enc.Utf8);
    var ciphertext2 = plainText;
	ResponseCallback(ciphertext2);
};

function ResponseCallback(_response) {
	UnityCallback(_response);
};

function UnityCallback(_msg) {
	unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, _msg);
};
