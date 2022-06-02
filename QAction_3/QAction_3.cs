using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.HttpClasess.Token;

/// <summary>
/// DataMiner QAction Class: Extract Token.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			var response = Convert.ToString(protocol.GetParameter(Parameter.gettokenresponse_301));
			var loginResponse = JsonConvert.DeserializeObject<Root>(response);
			protocol.SetParameter(Parameter.tokenapi_1, loginResponse.AUTHORIZATION);
			// Auth flag to send other commands
			protocol.SetParameter(Parameter.authflag_5, 1);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}