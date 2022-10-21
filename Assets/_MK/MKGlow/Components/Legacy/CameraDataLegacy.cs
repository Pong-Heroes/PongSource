//////////////////////////////////////////////////////
// MK Glow Camera Data Legacy	    	    	    //
//					                                //
// Created by Michael Kremmel                       //
// www.michaelkremmel.de                            //
// Copyright © 2020 All rights reserved.            //
//////////////////////////////////////////////////////
/// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MK.Glow.Legacy
{
    internal class CameraDataLegacy : CameraData
    {
        public static implicit operator CameraDataLegacy(UnityEngine.Camera input)
        {
            CameraDataLegacy data = new CameraDataLegacy();
            
            data.width = input.pixelWidth;
            data.height = input.pixelHeight;
            data.stereoEnabled = input.stereoEnabled;
            data.aspect = input.aspect;
            data.worldToCameraMatrix = input.worldToCameraMatrix;
            
            return data;
        }
    }
}
