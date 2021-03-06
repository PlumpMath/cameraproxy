﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MJpegCameraProxy.PanTiltZoom
{
	public static class PtzController
	{
		public static string GetHtml(string camId, IPCameraBase cam)
		{
			if (cam.cameraSpec.ptzType == Configuration.PtzType.IPS_EYE01)
				return new IPS_EYE01.IPS_EYE01_PTZ(cam.cameraSpec).GetHtml(camId, cam);
			else if (cam.cameraSpec.ptzType == Configuration.PtzType.TrendnetTVIP400)
				return new TrendNet.TV_IP400(cam.cameraSpec).GetHtml(camId, cam);
			else if (cam.cameraSpec.ptzType == Configuration.PtzType.CustomPTZProfile)
				return new Custom.CustomPTZProfile(cam.cameraSpec, cam).GetHtml(camId, cam);
			else if (cam.cameraSpec.ptzType == Configuration.PtzType.Huisun)
				return new HuisunBullet(cam.cameraSpec, cam).GetHtml(camId, cam);
			return "";
		}
	}
}
