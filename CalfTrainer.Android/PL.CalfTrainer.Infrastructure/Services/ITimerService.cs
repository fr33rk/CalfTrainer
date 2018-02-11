﻿using System;

namespace PL.CalfTrainer.Infrastructure.Services
{
	public interface ITimerService
	{
		event EventHandler Elapsed;

		bool IsRunning { get; }

		void Start(int timerIntervalMs);

		void Pause();

		void Resume();

		void Stop();
	}
}