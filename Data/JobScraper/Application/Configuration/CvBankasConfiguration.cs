﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Configuration
{
	public class CvBankasConfiguration : IScrapeConfiguration
	{
		public string Posting { get; set; } = "article";
		public string Name { get; set; } = "h3.list_h3";
		public string Salary { get; set; } = "span.dib.mt5";
		public string Company { get; set; } = "salary_amount";
		public string Url { get; set; } = "a.list_a";
 	}
}