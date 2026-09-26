using GymManagement.Application.Common.Exceptions;
using GymManagement.Application.DTOs.Trainers;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Services;

public class TrainerService : ITrainerService
{
    private readonly GymManagementDbContext _dbContext;

    public TrainerService(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TrainerDto>> GetAllAsync()
    {
        return await _dbContext.Trainers
            .AsNoTracking()
            .Select(t => new TrainerDto
            {
                Id = t.Id,
                Name = t.Name,
                Specialization = t.Specialization,
                ExperienceYears = t.ExperienceYears
            })
            .ToListAsync();
    }

    public async Task<TrainerDto> GetByIdAsync(int id)
    {
        var trainer = await _dbContext.Trainers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
        {
            throw new NotFoundException("Trainer", id);
        }

        return new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Specialization = trainer.Specialization,
            ExperienceYears = trainer.ExperienceYears
        };
    }

    public async Task<TrainerDto> CreateAsync(CreateTrainerDto dto)
    {
        var trainer = new Trainer
        {
            Name = dto.Name.Trim(),
            Specialization = dto.Specialization.Trim(),
            ExperienceYears = dto.ExperienceYears
        };

        _dbContext.Trainers.Add(trainer);
        await _dbContext.SaveChangesAsync();

        return new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Specialization = trainer.Specialization,
            ExperienceYears = trainer.ExperienceYears
        };
    }

    public async Task<TrainerDto> UpdateAsync(int id, UpdateTrainerDto dto)
    {
        var trainer = await _dbContext.Trainers.FindAsync(id);

        if (trainer == null)
        {
            throw new NotFoundException("Trainer", id);
        }

        trainer.Name = dto.Name.Trim();
        trainer.Specialization = dto.Specialization.Trim();
        trainer.ExperienceYears = dto.ExperienceYears;

        await _dbContext.SaveChangesAsync();

        return new TrainerDto
        {
            Id = trainer.Id,
            Name = trainer.Name,
            Specialization = trainer.Specialization,
            ExperienceYears = trainer.ExperienceYears
        };
    }

    public async Task DeleteAsync(int id)
    {
        var trainer = await _dbContext.Trainers
            .Include(t => t.WorkoutPlans)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (trainer == null)
        {
            throw new NotFoundException("Trainer", id);
        }

        if (trainer.WorkoutPlans.Any())
        {
            throw new BadRequestException("Cannot delete trainer who is assigned to existing workout plans.");
        }

        _dbContext.Trainers.Remove(trainer);
        await _dbContext.SaveChangesAsync();
    }
}
