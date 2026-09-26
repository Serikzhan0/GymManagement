using System.Text.Json.Serialization;
using GymManagementApi.Models;

namespace GymManagementApi.DTOs;

// --- Auth DTOs ---
public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// --- Trainer DTOs ---
public class TrainerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

public class CreateTrainerDto
{
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

public class UpdateTrainerDto
{
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

// --- Exercise DTOs ---
public class ExerciseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
}

public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
}

public class UpdateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
}

// --- WorkoutPlan DTOs ---
public class WorkoutExerciseItemDto
{
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}

public class WorkoutExerciseInputDto
{
    public int ExerciseId { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}

public class WorkoutPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public string TrainerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<WorkoutExerciseItemDto> Exercises { get; set; } = new();
}

public class CreateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TrainerId { get; set; }
    public List<WorkoutExerciseInputDto> Exercises { get; set; } = new();
}

public class UpdateWorkoutPlanDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<WorkoutExerciseInputDto> Exercises { get; set; } = new();
}

// --- Membership DTOs ---
public class MembershipDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreateMembershipDto
{
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
}

public class UpdateMembershipDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipType Type { get; set; }
    public MembershipStatus Status { get; set; }
}

// --- Error Response DTO ---
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }

    public ErrorResponse(int statusCode, string message, IDictionary<string, string[]>? errors = null)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
    }
}
