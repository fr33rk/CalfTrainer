using System;
using System.Threading;
using Newtonsoft.Json;
using PL.CalfTrainer.Business.Entities;

namespace PL.CalfTrainer.Business.Services
{
	public class ExerciseService
	{
		private Exercise mExercise;
		private ExerciseConfiguration mExerciseConfiguration;
		private Timer mTimer;

		protected ExerciseService(Exercise exercise, ExerciseConfiguration configuration)
		{
			mExercise = exercise;
			mExerciseConfiguration = configuration;
		}

		public static ExerciseService ExerciseServiceFromJson(string asJson, ExerciseConfiguration configuration)
		{
			try
			{
				var exercise = string.IsNullOrEmpty(asJson) 
					? new Exercise(configuration) 
					: JsonConvert.DeserializeObject<Exercise>(asJson);

				return new ExerciseService(exercise, configuration);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(mExercise);
		}

		public void PrepareForNewExercise()
		{
			
		}

		public void Start()
		{
			
		}

		public void Pause()
		{
			
		}

		public void Resume()
		{
			
		}

		public void Stop()
		{
			
		}

		public bool IsRunning;



	}
}