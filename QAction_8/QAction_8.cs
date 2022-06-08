using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.HttpClasess.PutSelectors;
using Skyline.Protocol.MyExtension;

/// <summary>
/// DataMiner QAction Class: Connect Client to Selector.
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

			var selectors = Convert.ToString(protocol.GetParameter(protocol.GetTriggerParameter())).Split(';');

			var rootBodyRequest = new Root
			{
				ID = int.Parse(clientID),
				SELECTORS = new List<SELECTOR>(),
			};

			foreach (var selector in selectors)
			{
				if (!selector.Contains("-"))
				{
					protocol.Log("QA" + protocol.QActionID + "|Run|Selector with value: " + selector + " was not in the correct format", LogType.Information, LogLevel.NoLogging);
					continue;
				}

				var selectorIAndType = selector.Split('-');

				if (selectorIAndType.Length != 2)
				{
					protocol.Log("QA" + protocol.QActionID + "|Run|Selector configured with ID and Type: " + selector + " was not separated by a '-' invalid format Ex. selector_id1-[listen|talk|talk/listen]", LogType.Information, LogLevel.NoLogging);
					continue;
				}

				rootBodyRequest.SELECTORS.Add(new SELECTOR { ID = int.Parse(selectorIAndType[0]), TYPE = SELECTOR.Converter(selectorIAndType[1])});
			}

			protocol.SetParameters(new[] { Parameter.putselectorurl_311, Parameter.putselectorbody_314 }, new object[] { urlPutRequest, JsonConvert.SerializeObject(rootBodyRequest)});
			protocol.CheckTrigger((int)Triggers.SendPutSelectors);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}