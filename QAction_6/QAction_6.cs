using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using Skyline.Protocol.MyExtension;


/// <summary>
/// DataMiner QAction Class: Parse Party Lines Response.
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
				new uint[] { Parameter.getpartylinescode_307, Parameter.getpartylinesresponse_308 });

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
				object selectorName;
				object image;

				innerDict.TryGetValue("ID", out id);
				innerDict.TryGetValue("LABEL_CATEGORY", out labelCategory);
				innerDict.TryGetValue("LABEL_INDEX", out labelIndex);
				innerDict.TryGetValue("SELECTOR_NAME", out selectorName);
				innerDict.TryGetValue("IMAGE", out image);


				rows.Add(new PartylinesQActionRow
				{
					Partylinesid_3001 = Convert.ToString(id),
					Partylineslabelcategory_3002 = Convert.ToString(labelCategory),
					Partylineslabelindex_3003 = Convert.ToString(labelIndex),
					Partylinesselectorname_3004 = Convert.ToString(selectorName),
					Partylinesimage_3005 = Convert.ToString(image),
				});
			}

			if (rows.Any())
			{
				protocol.partylines.FillArray(rows);
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