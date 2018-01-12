using System;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Services
{
	public class TimeProvider : ITimeProvider
	{
		public DateTime Today
		{
			get => DateTime.Today;
			set { }
		}
	}
}