using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.MyExtension;

/// <summary>
/// DataMiner QAction Class: Parse Client Stats.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			var responseParams = (object[])protocol.GetParameters(new uint[] { Parameter.getclientstatscode_305, Parameter.getclientstatsresponse_306 });

			var statusCode = Convert.ToString(responseParams[0]);
			var response = Convert.ToString(responseParams[1]);

			var httpStatusCodeValidator = new HttpStatusCodeValidator(protocol, statusCode);

			if (httpStatusCodeValidator.IsStatusCodeInvalidOrNeedToAuth())
			{
				return;
			}

			var clientStatsResponse = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

			var rows = new List<QActionTableRow>();

			foreach (var innerDict in clientStatsResponse)
			{
				object id;
				object connectionStatus;
				object clientID;
				object clientNumber;
				object ipAddress;
				object connectionStateDuration;
				object disconnectEventCount;
				object disconnectStateCumulativeDuration;

				innerDict.TryGetValue("ID", out id);
				innerDict.TryGetValue("CONNECTION_STATE", out connectionStatus);
				innerDict.TryGetValue("CLIENT_ID", out clientID);
				innerDict.TryGetValue("CLIENT_NUMBER", out clientNumber);
				if (!innerDict.TryGetValue("IP_ADDRESS", out ipAddress))
				{
					ipAddress = "-1";
				}

				if (!innerDict.TryGetValue("CONNECTION_STATE_DURATION", out connectionStateDuration))
				{
					connectionStateDuration = "-1";
				}

				innerDict.TryGetValue("DISCONNECT_EVENT_COUNT", out disconnectEventCount);

				if(!innerDict.TryGetValue("DISCONNECT_STATE_CUMULATIVE_DURATION", out disconnectStateCumulativeDuration))
				{
					disconnectStateCumulativeDuration = -1;
				}

				rows.Add(new ClientstatisticsQActionRow
				{
					Clientstatisticsid_2001 = Convert.ToString(id),
					Clientstatisticsconnectionstatus_2002 = connectionStatus,
					Clientstatisticsclientid_2003 = clientID,
					Clientstatisticsclientnumber_2004 = Convert.ToInt32(clientNumber),
					Clientstatisticsipaddress_2005 = ipAddress,
					Clientstatisticsconnectionstateduration_2006 = connectionStateDuration,
					Clientstatisticsdisconnectcount_2007 = Convert.ToInt32(disconnectEventCount),
					Clientstatisticsdisconnectstateduration_2008 = disconnectStateCumulativeDuration,
				});
			}

			if (rows.Any())
			{
				protocol.clientstatistics.FillArray(rows);
			}
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}