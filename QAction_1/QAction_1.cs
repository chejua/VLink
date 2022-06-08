namespace Skyline.Protocol
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using Skyline.DataMiner.Scripting;

	namespace MyExtension
	{
		#region Enumerations

		public enum Triggers
		{
			SendLogingRequest = 2,
			SendPutSelectors = 3,
		}

		#endregion

		#region Structs
		#endregion

		#region Classes


		#endregion

		public class HttpStatusCodeValidator
		{
			private readonly SLProtocol _protocol;
			private string _statusCode;

			public HttpStatusCodeValidator(SLProtocol protocol, string statusCode)
			{
				_protocol = protocol;
				_statusCode = statusCode;
			}

			public bool IsStatusCodeInvalidOrNeedToAuth()
			{
				if (_statusCode.Contains("HTTP/1.1 200 OK"))
				{
					return false;
				}
				else if (_statusCode.Contains("401 Unauthorized"))
				{
					_protocol.CheckTrigger((int)Triggers.SendLogingRequest);
					return true;
				}

				_protocol.Log("QA" + _protocol.QActionID + "|IsStatusCodeInvalidOrNeedToAuth|The status code was: " + _statusCode + " execution was not done for that request", LogType.Information, LogLevel.NoLogging);
				return true;
			}
		}
	}

	namespace HttpClasess
	{
		namespace Token
		{
			public class Root
			{
				public string DISPLAY_NAME { get; set; }

				public string AUTHORIZATION { get; set; }

				public int LABEL_ID { get; set; }

				public int CONNECTION_TIMEOUT_IN_MS { get; set; }

			}
		}

		namespace PutSelectors
		{
			public class Root
			{
				public int ID { get; set; }

				public List<SELECTOR> SELECTORS { get; set; }
			}

			public class SELECTOR
			{
				public string TYPE { get; set; }

				public int ID { get; set; }

				//public string ALIAS { get; set; }

				public static string Converter(string word)
				{
					var wordToLower = word.ToLower();

					switch (wordToLower)
					{
						case "listen":
							return "SELECTOR_LISTEN";

						case "talk":
							return "SELECTOR_TALK";

						case "talk/listen":
							return "SELECTOR";

						default:
							throw new ArgumentException("Parameter value was not one of the following listen, talk, talk/listen " + word);
					}
				}
			}
		}
	}
}