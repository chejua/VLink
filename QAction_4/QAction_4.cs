using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.MyExtension;

/// <summary>
/// DataMiner QAction Class: Parse Statys System Response.
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
			var responseParams = (object[])protocol.GetParameters(new uint[] {Parameter.getstatussystemcode_303, Parameter.getsystemresponse_304 });

			var statusCode = Convert.ToString(responseParams[0]);
			var response = Convert.ToString(responseParams[1]);

			var httpStatusCodeValidator = new HttpStatusCodeValidator(protocol, statusCode);

			if (httpStatusCodeValidator.IsStatusCodeInvalidOrNeedToAuth())
			{
				return;
			}
		
			var statusSystemResponse = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

			var rows = new List<QActionTableRow>();

			foreach (var innerDict in statusSystemResponse)
			{
				foreach (var rowResponse in innerDict)
				{
					var name = rowResponse.Key;
					var value = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(rowResponse.Value));

					var values = new List<string>();

					foreach (var rowValue in value)
					{
						values.Add(Convert.ToString(rowValue.Value));
					}

					rows.Add(new StatussystemQActionRow
					{
						Statussystemname_1001 = name,
						Statussystemtext_1002 = values[0],
						StatusSystemStatus_1003 = values[1],
					});
				}
			}

			if (rows.Any())
			{
				protocol.statussystem.FillArray(rows);
			}
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}