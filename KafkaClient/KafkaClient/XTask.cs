using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KafkaClient
{
	public class XTask
	{
		[JsonProperty]
		public string Name { get; set; }

		[JsonProperty]
		public  string Description { get; set; }
	}
}
