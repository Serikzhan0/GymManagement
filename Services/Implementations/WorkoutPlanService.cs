using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class WorkoutPlanService : IWorkoutPlanService
{
    private readonly IWorkoutPlanRepository _planRepository;
    private readonly ITrainerRepository _trainerRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IValidator<CreateWorkoutPlanDto> _createValidator;
    private readonly IValidator<UpdateWorkoutPlanDto> _updateValidator;

    public WorkoutPlanService(
        IWorkoutPlanRepository planRepository,
        ITrainerRepository trainerRepository,
        IExerciseRepository exerciseRepository,
        IValidator<CreateWorkoutPlanDto> createValidator,
        IValidator<UpdateWorkoutPlanDto> updateValidator)
    {
        _planRepository = planRepository;
        _trainerRepository = trainerRepository;
        _exerciseRepository = exerciseRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<WorkoutPlanDto>> GetAllAsync()
    {
        var plans = await _planRepository.GetAllAsync();
        return plans.Select(MapToDto);
    }

    public async Task<ServiceResult<WorkoutPlanDto>> GetByIdAsync(int id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return ServiceResult<WorkoutPlanDto>.Failure($"WorkoutPlan with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        return ServiceResult<WorkoutPlanDto>.Success(MapToDto(plan));
    }

    public async Task<ServiceResult<WorkoutPlanDto>> CreateAsync(CreateWorkoutPlanDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<WorkoutPlanDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var trainer = await _trainerRepository.GetByIdAsync(dto.TrainerId);
        if (trainer == null)
        {
            return ServiceResult<WorkoutPlanDto>.Failure($"Trainer with ID '{dto.TrainerId}' was not found.", StatusCodes.Status404NotFound);
        }

        var requestedExerciseIds = dto.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
        var existingCount = await _exerciseRepository.CountByIdsAsync(requestedExerciseIds);
        if (existingCount != requestedExerciseIds.Count)
        {
            return ServiceResult<WorkoutPlanDto>.Failure("One or more exercises specified were not found.", StatusCodes.Status404NotFound);
        }

        var workoutPlan = new WorkoutPlan
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            TrainerId = dto.TrainerId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var ex in dto.Exercises)
        {
            workoutPlan.WorkoutExercises.Add(new WorkoutExercise
            {
                ExerciseId = ex.ExerciseId,
                Sets = ex.Sets,
                Repetitions = ex.Repetitions
            });
        }

        await _planRepository.AddAsync(workoutPlan);
        await _planRepository.SaveChangesAsync();

        var fresh = await _planRepository.GetByIdAsync(workoutPlan.Id);
        return ServiceResult<WorkoutPlanDto>.Success(MapToDto(fresh!), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<WorkoutPlanDto>> UpdateAsync(int id, UpdateWorkoutPlanDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<WorkoutPlanDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var plan = await _planRepository.GetByIdForUpdateAsync(id);
        if (plan == null)
        {
            return ServiceResult<WorkoutPlanDto>.Failure($"WorkoutPlan with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        var requestedExerciseIds = dto.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
        var existingCount = await _exerciseRepository.CountByIdsAsync(requestedExerciseIds);
        if (existingCount != requestedExerciseIds.Count)
        {
            return ServiceResult<WorkoutPlanDto>.Failure("One or more exercises specified were not found.", StatusCodes.Status404NotFound);
        }

        plan.Name = dto.Name.Trim();
        plan.Description = dto.Description.Trim();

        plan.WorkoutExercises.Clear();
        foreach (var ex in dto.Exercises)
        {
            plan.WorkoutExercises.Add(new WorkoutExercise
            {
                WorkoutPlanId = plan.Id,
                ExerciseId = ex.ExerciseId,
                Sets = ex.Sets,
                Repetitions = ex.Repetitions
            });
        }

        await _planRepository.UpdateAsync(plan);
        await _planRepository.SaveChangesAsync();

        var fresh = await _planRepository.GetByIdAsync(plan.Id);
        return ServiceResult<WorkoutPlanDto>.Success(MapToDto(fresh!));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var plan = await _planRepository.GetByIdForUpdateAsync(id);
        if (plan == null)
        {
            return ServiceResult.Failure($"WorkoutPlan with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        await _planRepository.DeleteAsync(plan);
        await _planRepository.SaveChangesAsync();

        return ServiceResult.Ok("Workout plan deleted successfully.");
    }

    private static WorkoutPlanDto MapToDto(WorkoutPlan wp) => new()
    {
        Id = wp.Id,
        Name = wp.Name,
        Description = wp.Description,
        TrainerId = wp.TrainerId,
        TrainerName = wp.Trainer?.Name ?? string.Empty,
        CreatedAt = wp.CreatedAt,
        Exercises = wp.WorkoutExercises.Select(we => new WorkoutExerciseItemDto
        {
            ExerciseId = we.ExerciseId,
            ExerciseName = we.Exercise?.Name ?? string.Empty,
            Sets = we.Sets,
            Repetitions = we.Repetitions
        }).ToList()
    };
}
