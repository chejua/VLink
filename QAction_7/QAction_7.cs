using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.MyExtension;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// DataMiner QAction Class: Parse Clients Response.
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
			var responseParams = (object[])protocol.GetParameters(
				new uint[] { Parameter.getclientscode_309, Parameter.getclientsresponse_310 });

			var statusCode = Convert.ToString(responseParams[0]);
			var response = Convert.ToString(responseParams[1]);

			var httpStatusCodeValidator = new HttpStatusCodeValidator(protocol, statusCode);

			if (httpStatusCodeValidator.IsStatusCodeInvalidOrNeedToAuth())
			{
				return;
			}

			var partyLinesResponse = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

			var rows = new List<QActionTableRow>();
			foreach (var innerDict in partyLinesResponse)
			{
				object id;
				object labelCategory;
				object labelIndex;
				object labelType;
				object selectorName;
				object image;
				object lastConnectionTime;

				innerDict.TryGetValue("ID", out id);
				innerDict.TryGetValue("LABEL_CATEGORY", out labelCategory);
				innerDict.TryGetValue("LABEL_INDEX", out labelIndex);
				innerDict.TryGetValue("LABEL_TYPE", out labelType);
				innerDict.TryGetValue("SELECTOR_NAME", out selectorName);
				innerDict.TryGetValue("IMAGE", out image);
				innerDict.TryGetValue("LAST_CONNECT_TIME", out lastConnectionTime);
				DateTime connectionTime = DateTime.MinValue;

				DateTime.TryParse(Convert.ToString(lastConnectionTime), out connectionTime);
				rows.Add(new ClientsQActionRow
				{
					Clientid_4001 = Convert.ToString(id),
					Clientcategory_4002 = Convert.ToString(labelCategory),
					Clientlabelindex_4003 = Convert.ToString(labelIndex),
					Clientlabeltype_4004 = Convert.ToString(labelType),
					Clientselectorname_4005 = Convert.ToString(selectorName),
					Clientimage_4006 = Convert.ToString(image),
					Clientlastconnectiontime_4007 = connectionTime.ToOADate(),

				});
			}

			if (rows.Any())
			{
				protocol.clients.FillArray(rows);
			}

		}
		catch (Exception ex)
		{
			protocol.Log(
				"QA" +
					protocol.QActionID +
					"|" +
					protocol.GetTriggerParameter() +
					"|Run|Exception thrown:" +
					Environment.NewLine +
					ex,
				LogType.Error,
				LogLevel.NoLogging);
		}
	}
}