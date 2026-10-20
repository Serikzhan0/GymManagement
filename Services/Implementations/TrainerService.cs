using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IValidator<CreateTrainerDto> _createValidator;
    private readonly IValidator<UpdateTrainerDto> _updateValidator;

    public TrainerService(
        ITrainerRepository trainerRepository,
        IValidator<CreateTrainerDto> createValidator,
        IValidator<UpdateTrainerDto> updateValidator)
    {
        _trainerRepository = trainerRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<TrainerDto>> GetAllAsync()
    {
        var trainers = await _trainerRepository.GetAllAsync();
        return trainers.Select(MapToDto);
    }

    public async Task<ServiceResult<TrainerDto>> GetByIdAsync(int id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null)
        {
            return ServiceResult<TrainerDto>.Failure($"Trainer with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        return ServiceResult<TrainerDto>.Success(MapToDto(trainer));
    }

    public async Task<ServiceResult<TrainerDto>> CreateAsync(CreateTrainerDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<TrainerDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var trainer = new Trainer
        {
            Name = dto.Name.Trim(),
            Specialization = dto.Specialization.Trim(),
            ExperienceYears = dto.ExperienceYears,
            ImageUrl = (dto.ImageUrl ?? "").Trim(),
            Bio = (dto.Bio ?? "").Trim(),
            Rating = 4.9,
            SessionsCount = 120,
            ReviewsCount = 45
        };

        await _trainerRepository.AddAsync(trainer);
        await _trainerRepository.SaveChangesAsync();

        return ServiceResult<TrainerDto>.Success(MapToDto(trainer), StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<TrainerDto>> UpdateAsync(int id, UpdateTrainerDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<TrainerDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer == null)
        {
            return ServiceResult<TrainerDto>.Failure($"Trainer with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        trainer.Name = dto.Name.Trim();
        trainer.Specialization = dto.Specialization.Trim();
        trainer.ExperienceYears = dto.ExperienceYears;
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            trainer.ImageUrl = dto.ImageUrl.Trim();
        }
        if (!string.IsNullOrWhiteSpace(dto.Bio))
        {
            trainer.Bio = dto.Bio.Trim();
        }

        await _trainerRepository.UpdateAsync(trainer);
        await _trainerRepository.SaveChangesAsync();

        return ServiceResult<TrainerDto>.Success(MapToDto(trainer));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var trainer = await _trainerRepository.GetByIdWithPlansAsync(id);
        if (trainer == null)
        {
            return ServiceResult.Failure($"Trainer with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (trainer.WorkoutPlans.Any())
        {
            return ServiceResult.Failure("Cannot delete a trainer who has existing workout plans.", StatusCodes.Status400BadRequest);
        }

        await _trainerRepository.DeleteAsync(trainer);
        await _trainerRepository.SaveChangesAsync();

        return ServiceResult.Ok("Trainer deleted successfully.");
    }

    private static TrainerDto MapToDto(Trainer t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Specialization = t.Specialization,
        ExperienceYears = t.ExperienceYears,
        ImageUrl = t.ImageUrl,
        Bio = t.Bio,
        Rating = t.Rating,
        SessionsCount = t.SessionsCount,
        ReviewsCount = t.ReviewsCount
    };
}
