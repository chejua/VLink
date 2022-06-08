using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.HttpClasess.PutSelectors;
using Skyline.Protocol.MyExtension;

/// <summary>
/// DataMiner QAction Class: Remove All Selectors From Client.
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
			var clientID = protocol.RowKey();

			var urlPutRequest = $"api/v1/config/clients/{clientID}";

			var rootBodyRequest = new Root
			{
				ID = int.Parse(clientID),
				SELECTORS = new List<SELECTOR>(),
			};

			protocol.SetParameters(new[] { Parameter.putselectorurl_311, Parameter.putselectorbody_314 }, new object[] { urlPutRequest, JsonConvert.SerializeObject(rootBodyRequest) });
			protocol.CheckTrigger((int)Triggers.SendPutSelectors);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}