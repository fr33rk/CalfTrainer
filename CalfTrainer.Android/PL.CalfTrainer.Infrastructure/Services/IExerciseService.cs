using System;
using PL.CalfTrainer.Infrastructure.EventArgs;

namespace PL.CalfTrainer.Infrastructure.Services
{
	public interface IExerciseService
	{
		string StateToString();

		void PrepareForNewExercise();

		void Start();

		void Pause();

		void Resume();

		void Stop();

		bool IsRunning { get; }

		void SendExerciseState();

		event EventHandler<ExerciseChangedEventArgs> ExerciseChanged;

		event EventHandler<ActiveSubExerciseChangedEventArgs> ActiveSubExerciseChanged;
	}
}