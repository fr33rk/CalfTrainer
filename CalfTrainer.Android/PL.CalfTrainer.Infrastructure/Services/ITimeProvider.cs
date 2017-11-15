using System;

namespace PL.CalfTrainer.Infrastructure.Services
{
	public interface ITimeProvider
	{
		DateTime Today { get; set; }
	}
}