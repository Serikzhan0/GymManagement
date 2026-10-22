using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IValidator<CreateExerciseDto> _createValidator;
    private readonly IValidator<UpdateExerciseDto> _updateValidator;

    public ExerciseService(
        IExerciseRepository exerciseRepository,
        IValidator<CreateExerciseDto> createValidator,
        IValidator<UpdateExerciseDto> updateValidator)
    {
        _exerciseRepository = exerciseRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<ExerciseDto>> GetAllAsync(string? muscleGroup, string? search)
    {
        var exercises = await _exerciseRepository.GetAllAsync(muscleGroup, search);
        return exercises.Select(MapToDto);
    }

    public async Task<ServiceResult<ExerciseDto>> GetByIdAsync(int id)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(id);
        if (exercise == null)
        {
            return ServiceResult<ExerciseDto>.Failure($"Exercise with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        return ServiceResult<ExerciseDto>.Success(MapToDto(exercise));
    }

    public async Task<ServiceResult<ExerciseDto>> CreateAsync(CreateExerciseDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<ExerciseDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var exercise = new Exercise
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            MuscleGroup = dto.MuscleGroup.Trim(),
            VideoUrl = dto.VideoUrl.Trim(),
            ImageUrl = (dto.ImageUrl ?? "").Trim()
        };

        await _exerciseRepository.AddAsync(exercise);
        await _exerciseRepository.SaveChangesAsync();

        return ServiceResult<ExerciseDto>.Success(MapToDto(exercise), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<ExerciseDto>> UpdateAsync(int id, UpdateExerciseDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<ExerciseDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var exercise = await _exerciseRepository.GetByIdTrackingAsync(id);
        if (exercise == null)
        {
            return ServiceResult<ExerciseDto>.Failure($"Exercise with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        exercise.Name = dto.Name.Trim();
        exercise.Description = dto.Description.Trim();
        exercise.MuscleGroup = dto.MuscleGroup.Trim();
        exercise.VideoUrl = dto.VideoUrl.Trim();
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            exercise.ImageUrl = dto.ImageUrl.Trim();
        }

        await _exerciseRepository.UpdateAsync(exercise);
        await _exerciseRepository.SaveChangesAsync();

        return ServiceResult<ExerciseDto>.Success(MapToDto(exercise));
    }

    public async Task<ServiceResult<ExerciseDto>> UpdateMediaAsync(int id, UpdateExerciseMediaDto dto)
    {
        var exercise = await _exerciseRepository.GetByIdTrackingAsync(id);
        if (exercise == null)
        {
            return ServiceResult<ExerciseDto>.Failure($"Exercise with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            exercise.ImageUrl = dto.ImageUrl.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.VideoUrl))
        {
            exercise.VideoUrl = dto.VideoUrl.Trim();
        }

        await _exerciseRepository.UpdateAsync(exercise);
        await _exerciseRepository.SaveChangesAsync();

        return ServiceResult<ExerciseDto>.Success(MapToDto(exercise));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var exercise = await _exerciseRepository.GetByIdWithWorkoutExercisesAsync(id);
        if (exercise == null)
        {
            return ServiceResult.Failure($"Exercise with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (exercise.WorkoutExercises.Any())
        {
            return ServiceResult.Failure("Cannot delete an exercise that is currently in workout plans.", StatusCodes.Status400BadRequest);
        }

        await _exerciseRepository.DeleteAsync(exercise);
        await _exerciseRepository.SaveChangesAsync();

        return ServiceResult.Ok("Exercise deleted successfully.");
    }

    private static ExerciseDto MapToDto(Exercise e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        MuscleGroup = e.MuscleGroup,
        VideoUrl = e.VideoUrl,
        ImageUrl = e.ImageUrl,
        Difficulty = CalculateDifficulty(e.Name),
        Equipment = CalculateEquipment(e.Name)
    };

    private static string CalculateDifficulty(string name)
    {
        var n = name.ToLowerInvariant();
        if (n.Contains("advanced") || n.Contains("deadlift") || n.Contains("snatch") || n.Contains("pistol") || n.Contains("dragon") || n.Contains("clean") || n.Contains("burpee") || n.Contains("thruster") || n.Contains("pull-up") || n.Contains("dips"))
            return "Advanced";
        if (n.Contains("press") || n.Contains("squat") || n.Contains("row") || n.Contains("curl") || n.Contains("extension") || n.Contains("thrust") || n.Contains("crunch") || n.Contains("twist"))
            return "Intermediate";
        return "Beginner";
    }

    private static string CalculateEquipment(string name)
    {
        var n = name.ToLowerInvariant();
        if (n.Contains("barbell") || n.Contains("deadlift") || (n.Contains("squat") && !n.Contains("bodyweight") && !n.Contains("goblet"))) return "Barbell";
        if (n.Contains("dumbbell") || n.Contains("goblet") || n.Contains("kettlebell")) return "Dumbbell";
        if (n.Contains("cable") || n.Contains("pulldown") || n.Contains("pushdown") || n.Contains("crossover")) return "Cable";
        if (n.Contains("machine") || n.Contains("leg press") || n.Contains("pec deck") || n.Contains("hack") || n.Contains("bike") || n.Contains("rower")) return "Machine";
        return "Bodyweight";
    }
}
