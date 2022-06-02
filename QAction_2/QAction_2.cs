using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.MyExtension;

/// <summary>
/// DataMiner QAction Class: After Startup.
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
			var credentialParams = (object[])protocol.GetParameters(new uint[] { Parameter.username_10, Parameter.password_11 });
			var username = Convert.ToString(credentialParams[0]);
			var password = Convert.ToString(credentialParams[1]);

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				protocol.Log("QA" + protocol.QActionID + "|After start up|Please configure the credentials so polling can start", LogType.Information, LogLevel.NoLogging);
				return;
			}

			protocol.SetParameter(Parameter.gettokenreuqestbody_302, JsonConvert.SerializeObject(new TokenBody(username, password)));
			protocol.CheckTrigger((int)Triggers.SendLogingRequest);



		}
        catch (Exception ex)
        {
            protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
        }
    }

	private class TokenBody
	{
		[JsonProperty("LOGIN_TYPE")]
		public string LoginType = "VCP";
		[JsonProperty("LOGIN_NAME")]
		public string LoginName;
		[JsonProperty("LOGIN_PASSWORD")]
		public string LoginPassword;

		public TokenBody(string loginName, string loginPassword)
		{
			LoginName = loginName;
			LoginPassword = loginPassword;
		}
	}
}
