using GymManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagementApi;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger)
    {
        if (!await db.Users.AnyAsync())
        {
            logger.LogInformation("Seeding initial exam data...");

            // 1. Users
            var adminUser = new User
            {
                Name = "Admin User",
                Email = "admin@gym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            var trainerUser = new User
            {
                Name = "Mike Trainer",
                Email = "trainer@gym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Trainer123!"),
                Role = UserRole.Trainer,
                CreatedAt = DateTime.UtcNow
            };

            var normalUser = new User
            {
                Name = "John Member",
                Email = "user@gym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.AddRange(adminUser, trainerUser, normalUser);
            await db.SaveChangesAsync();

            // 2. Trainers
            var trainer1 = new Trainer
            {
                Name = "Mike Trainer",
                Specialization = "Strength & Conditioning",
                ExperienceYears = 5
            };

            var trainer2 = new Trainer
            {
                Name = "Viktor Stone",
                Specialization = "Cardio & HYROX",
                ExperienceYears = 4
            };

            db.Trainers.AddRange(trainer1, trainer2);
            await db.SaveChangesAsync();

            // 3. Comprehensive Catalog of 50 Exercises with Verified Technique Videos
            var benchPress = new Exercise
            {
                Name = "Bench Press",
                Description = "Классический жим штанги лежа на горизонтальной скамье для развития грудных мышц.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=rT7DgCr-3pg"
            };
            var inclineBenchPress = new Exercise
            {
                Name = "Incline Bench Press",
                Description = "Жим штанги на наклонной скамье (30-45 градусов) с акцентом на верхнюю часть грудных мышц.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=SrqOu55lrYU"
            };
            var declineBenchPress = new Exercise
            {
                Name = "Decline Bench Press",
                Description = "Жим штанги на скамье с обратным наклоном с акцентом на нижнюю часть грудных мышц.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=LfyQBUKR8SE"
            };
            var dumbbellBenchPress = new Exercise
            {
                Name = "Dumbbell Bench Press",
                Description = "Жим гантелей лежа с расширенной амплитудой движения и независимой стабилизацией рук.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=VmB1G1K7v94"
            };
            var inclineDumbbellPress = new Exercise
            {
                Name = "Incline Dumbbell Press",
                Description = "Жим гантелей на наклонной скамье для максимальной гипертрофии верхнего пучка груди.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=8iPEnn-ltC8"
            };
            var dumbbellFly = new Exercise
            {
                Name = "Dumbbell Fly",
                Description = "Изолированное разведение гантелей лежа для растяжения и пампинга грудных мышц.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=eozdVDA78K0"
            };
            var cableCrossover = new Exercise
            {
                Name = "Cable Crossover",
                Description = "Сведение рук в кроссовере на блоках с постоянным мышечным натяжением по всей траектории.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=taI4XduLpTk"
            };
            var pushUp = new Exercise
            {
                Name = "Push-Up",
                Description = "Базовые классические отжимания от пола с собственным весом тела для груди и трицепса.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=IODxDxX7oi4"
            };
            var chestDips = new Exercise
            {
                Name = "Chest Dips",
                Description = "Отжимания на брусьях с наклоном корпуса вперед для мощной проработки низа груди.",
                MuscleGroup = "Chest",
                VideoUrl = "https://www.youtube.com/watch?v=2z8JmcrW-As"
            };

            // BACK (9 exercises)
            var pullUp = new Exercise
            {
                Name = "Pull-Up",
                Description = "Подтягивания на перекладине широким хватом для ширины широчайших мышц спины.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=eGo4IYlbE5g"
            };
            var chinUp = new Exercise
            {
                Name = "Chin-Up",
                Description = "Подтягивания обратным хватом с мощным вовлечением широчайших и бицепсов.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=brhRXlOhsAM"
            };
            var latPulldown = new Exercise
            {
                Name = "Lat Pulldown",
                Description = "Тяга верхнего вертикального блока к груди с контролируемым сведением лопаток.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=CAwf7n6Luuc"
            };
            var barbellRow = new Exercise
            {
                Name = "Barbell Row",
                Description = "Тяга штанги в наклоне для толщины спины, ромбовидных мышц и широчайших.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=FWJR5Ve8gkQ"
            };
            var dumbbellRow = new Exercise
            {
                Name = "Dumbbell Row",
                Description = "Тяга одной гантели к поясу с упором в скамью для устранения мышечного дисбаланса.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=pYcpY20QaE8"
            };
            var seatedCableRow = new Exercise
            {
                Name = "Seated Cable Row",
                Description = "Горизонтальная тяга блока к поясу сидя для средней части спины и плотности широчайших.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=GZbfZ033f74"
            };
            var tBarRow = new Exercise
            {
                Name = "T-Bar Row",
                Description = "Тяга Т-образного грифа для максимальной перегрузки центра спины и трапеций.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=j3Igk5nyZE4"
            };
            var facePull = new Exercise
            {
                Name = "Face Pull",
                Description = "Тяга каната на блоке к лицу для здоровья плечевых суставов и задних дельт.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=rep-qVOkqgk"
            };
            var straightArmPulldown = new Exercise
            {
                Name = "Straight-Arm Pulldown",
                Description = "Пуловер на верхнем блоке прямыми руками для изоляции широчайших мышц.",
                MuscleGroup = "Back",
                VideoUrl = "https://www.youtube.com/watch?v=G3Z7Fcx_0_g"
            };

            // LEGS (10 exercises)
            var squat = new Exercise
            {
                Name = "Barbell Squat",
                Description = "Базовые приседания со штангой на плечах для силы и массы ног.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=bEv6CCg2BC8"
            };
            var frontSquat = new Exercise
            {
                Name = "Front Squat",
                Description = "Фронтальные приседания со штангой на груди с акцентом на квадрицепсы и вертикальный корпус.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=uYumuL_G_V0"
            };
            var gobletSquat = new Exercise
            {
                Name = "Goblet Squat",
                Description = "Кубковые приседания с гантелью или гирей у груди для оттачивания мобильности и глубины.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=MeIiIdhvXT4"
            };
            var legPress = new Exercise
            {
                Name = "Leg Press",
                Description = "Жим ногами в платформенном тренажере для тяжелой нагрузки на квадрицепсы без осевой нагрузки.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=IZxyjW7MPJQ"
            };
            var bulgarianSplitSquat = new Exercise
            {
                Name = "Bulgarian Split Squat",
                Description = "Болгарские сплит-приседания с опорной ногой на скамье для ягодиц и квадрицепсов.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=2C-uNgKwPLE"
            };
            var walkingLunges = new Exercise
            {
                Name = "Walking Lunges",
                Description = "Шагающие выпады с гантелями для гипертрофии квадрицепсов, ягодиц и динамического баланса.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=L8fvypPrzzs"
            };
            var romanianDeadlift = new Exercise
            {
                Name = "Romanian Deadlift",
                Description = "Румынская становая тяга на прямых ногах для растяжения бицепса бедра и ягодиц.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=JCXUYuzwNrM"
            };
            var legCurl = new Exercise
            {
                Name = "Leg Curl",
                Description = "Сгибание ног лежа в тренажере для акцентированной изоляции бицепсов бедер.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=1Tq3QdYUuHs"
            };
            var legExtension = new Exercise
            {
                Name = "Leg Extension",
                Description = "Разгибание ног сидя в тренажере для детализации прямой и латеральной головок квадрицепса.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=YyvSfVjQeL0"
            };
            var calfRaise = new Exercise
            {
                Name = "Calf Raise",
                Description = "Подъемы на носки стоя со штангой или в тренажере для развития икроножных мышц.",
                MuscleGroup = "Legs",
                VideoUrl = "https://www.youtube.com/watch?v=-M4-G8p8fmc"
            };

            // SHOULDERS (7 exercises)
            var overheadPress = new Exercise
            {
                Name = "Overhead Press",
                Description = "Армейский жим штанги стоя для развития дельтовидных мышц плеч.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=2yjwXTZQDDI"
            };
            var dumbbellShoulderPress = new Exercise
            {
                Name = "Dumbbell Shoulder Press",
                Description = "Жим гантелей сидя для передних и средних пучков дельтовидных мышц.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=qEwKCR5JCog"
            };
            var arnoldPress = new Exercise
            {
                Name = "Arnold Press",
                Description = "Жим Арнольда с разворотом кистей во время амплитуды для полной активации всех пучков дельт.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=6Z15_WdXmVw"
            };
            var lateralRaise = new Exercise
            {
                Name = "Lateral Raise",
                Description = "Махи гантелями через стороны для взрывного роста ширины средних дельт.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=3VcKaXpzqRo"
            };
            var frontRaise = new Exercise
            {
                Name = "Front Raise",
                Description = "Подъем гантелей перед собой для прицельного развития передней дельтовидной мышцы.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=-t7fuZ0KhDA"
            };
            var rearDeltFly = new Exercise
            {
                Name = "Rear Delt Fly",
                Description = "Разведение гантелей в наклоне для задних дельт и осанки плечевого пояса.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=0G2_XV7slIg"
            };
            var uprightRow = new Exercise
            {
                Name = "Upright Row",
                Description = "Тяга штанги или троса к подбородку для средних дельт и верха трапеций.",
                MuscleGroup = "Shoulders",
                VideoUrl = "https://www.youtube.com/watch?v=amCU-ziHITM"
            };

            // ARMS (8 exercises)
            var barbellCurl = new Exercise
            {
                Name = "Barbell Curl",
                Description = "Подъем штанги на бицепс стоя — классический золотой стандарт силы рук.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=kwG2ipFRgfo"
            };
            var dumbbellCurl = new Exercise
            {
                Name = "Dumbbell Curl",
                Description = "Поочередный подъем гантелей с супинацией кисти для пика бицепса.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=ykJmrZ5v0Oo"
            };
            var hammerCurl = new Exercise
            {
                Name = "Hammer Curl",
                Description = "Сгибания «Молот» с нейтральным хватом для брахиалиса и предплечий.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=zC3nLlEvin4"
            };
            var preacherCurl = new Exercise
            {
                Name = "Preacher Curl",
                Description = "Сгибания рук на скамье Скотта, исключающие читинг и фиксирующие локти.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=fIWP-FRFNU0"
            };
            var tricepPushdown = new Exercise
            {
                Name = "Tricep Pushdown",
                Description = "Разгибание на трицепс на верхнем блоке с канатной или прямой рукоятью.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=2-LAMcpzODU"
            };
            var overheadTricepExtension = new Exercise
            {
                Name = "Overhead Tricep Extension",
                Description = "Французский жим гантели из-за головы сидя для растяжения длинной головки трицепса.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=_gsUck-7M74"
            };
            var skullCrushers = new Exercise
            {
                Name = "Skull Crushers",
                Description = "Французский жим лежа с EZ-грифом ко лбу для максимального объема трицепса.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=d_KZxkY_0cM"
            };
            var closeGripBenchPress = new Exercise
            {
                Name = "Close-Grip Bench Press",
                Description = "Жим штанги лежа узким хватом — базовое многосуставное упражнение для трицепсов.",
                MuscleGroup = "Arms",
                VideoUrl = "https://www.youtube.com/watch?v=nEF0bv2FW94"
            };

            // CORE (8 exercises)
            var plank = new Exercise
            {
                Name = "Plank",
                Description = "Изометрическая планка на предплечьях для укрепления глубоких мышц кора и поперечной мышцы.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=pSHjTRCQxIw"
            };
            var sidePlank = new Exercise
            {
                Name = "Side Plank",
                Description = "Боковая планка с опорой на одно предплечье для косых мышц живота и стабилизаторов таза.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=K2VljzCC16g"
            };
            var crunch = new Exercise
            {
                Name = "Crunch",
                Description = "Классические скручивания на коврике с округлением спины для прямого пресса.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=Xyd_fa5zoEU"
            };
            var bicycleCrunch = new Exercise
            {
                Name = "Bicycle Crunch",
                Description = "Динамические скручивания «велосипед» с диагональным касанием локтем колена.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=9FGilxCbdz8"
            };
            var legRaise = new Exercise
            {
                Name = "Leg Raise",
                Description = "Подъем прямых ног лежа на полу для нижней части брюшного пресса.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=JB2oyawG9KI"
            };
            var hangingLegRaise = new Exercise
            {
                Name = "Hanging Leg Raise",
                Description = "Подъем ног в висе на турнике — продвинутое упражнение для мощного пресса.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=Pr1ieGZ5atk"
            };
            var russianTwist = new Exercise
            {
                Name = "Russian Twist",
                Description = "Русские скручивания сидя с отрывом стоп для ротационной силы кора.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=wkD8rjkodUI"
            };
            var mountainClimber = new Exercise
            {
                Name = "Mountain Climber",
                Description = "Упражнение «Скалолаз» в упоре лежа — укрепляет кор и дает высокий кардио-отклик.",
                MuscleGroup = "Core",
                VideoUrl = "https://www.youtube.com/watch?v=nmwgirgXLYM"
            };

            // CARDIO / FULL BODY (5 exercises)
            var jumpingJacks = new Exercise
            {
                Name = "Jumping Jacks",
                Description = "Прыжки с разведением рук и ног для активной разминки и сжигания калорий.",
                MuscleGroup = "Cardio",
                VideoUrl = "https://www.youtube.com/watch?v=c4DAnQ6DtF8"
            };
            var burpees = new Exercise
            {
                Name = "Burpees",
                Description = "Бёрпи — функциональный комплекс из падения в упор, отжимания и выпрыгивания вверх.",
                MuscleGroup = "Cardio",
                VideoUrl = "https://www.youtube.com/watch?v=dZgVxmf6jkA"
            };
            var highKnees = new Exercise
            {
                Name = "High Knees",
                Description = "Бег на месте с высоким подниманием коленей для ускорения ЧСС и кардио-выносливости.",
                MuscleGroup = "Cardio",
                VideoUrl = "https://www.youtube.com/watch?v=oDdkytliOqE"
            };
            var boxJump = new Exercise
            {
                Name = "Box Jump",
                Description = "Плиометрические запрыгивания на тумбу для взрывной силы ног и координации.",
                MuscleGroup = "Cardio",
                VideoUrl = "https://www.youtube.com/watch?v=52r_Ul5k03g"
            };
            var bodyweightSquat = new Exercise
            {
                Name = "Bodyweight Squat",
                Description = "Приседания с весом собственного тела в динамическом темпе для выносливости и тонуса.",
                MuscleGroup = "Cardio",
                VideoUrl = "https://www.youtube.com/watch?v=aclHkVaku9U"
            };

            db.Exercises.AddRange(
                benchPress, inclineBenchPress, declineBenchPress, dumbbellBenchPress, inclineDumbbellPress, dumbbellFly, cableCrossover, pushUp, chestDips,
                pullUp, chinUp, latPulldown, barbellRow, dumbbellRow, seatedCableRow, tBarRow, facePull, straightArmPulldown,
                squat, frontSquat, gobletSquat, legPress, bulgarianSplitSquat, walkingLunges, romanianDeadlift, legCurl, legExtension, calfRaise,
                overheadPress, dumbbellShoulderPress, arnoldPress, lateralRaise, frontRaise, rearDeltFly, uprightRow,
                barbellCurl, dumbbellCurl, hammerCurl, preacherCurl, tricepPushdown, overheadTricepExtension, skullCrushers, closeGripBenchPress,
                plank, sidePlank, crunch, bicycleCrunch, legRaise, hangingLegRaise, russianTwist, mountainClimber,
                jumpingJacks, burpees, highKnees, boxJump, bodyweightSquat
            );
            await db.SaveChangesAsync();

            // 4. Workout Plan
            var plan = new WorkoutPlan
            {
                Name = "Full Body Starter",
                Description = "Базовая программа тренировок на все группы мышц для начинающих.",
                TrainerId = trainer1.Id,
                CreatedAt = DateTime.UtcNow,
                WorkoutExercises = new List<WorkoutExercise>
                {
                    new() { ExerciseId = benchPress.Id, Sets = 3, Repetitions = 10 },
                    new() { ExerciseId = squat.Id, Sets = 3, Repetitions = 8 },
                    new() { ExerciseId = pullUp.Id, Sets = 3, Repetitions = 8 }
                }
            };

            db.WorkoutPlans.Add(plan);

            // 5. Active Membership for John Member
            var membership = new Membership
            {
                UserId = normalUser.Id,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(25),
                Type = MembershipType.Monthly,
                Status = MembershipStatus.Active
            };

            db.Memberships.Add(membership);
            await db.SaveChangesAsync();

            // 6. Initial Turnstile Check-In (СКУД)
            var sampleCheckIn = new CheckIn
            {
                UserId = normalUser.Id,
                CheckInTime = DateTime.UtcNow.AddHours(-2),
                Status = "Granted",
                Notes = "Успешный вход по абонементу (Monthly)"
            };

            db.CheckIns.Add(sampleCheckIn);
            await db.SaveChangesAsync();
            logger.LogInformation("Database seeded successfully with exam data.");
        }

        // Ensure comprehensive catalog of exercises is populated
        if (await db.Exercises.CountAsync() < 10)
        {
            logger.LogInformation("Populating comprehensive catalog of exercises...");
            var catalog = new List<Exercise>
            {
                // CHEST
                new() { Name = "Incline Bench Press", Description = "Жим штанги на наклонной скамье (30-45 градусов) с акцентом на верхнюю часть грудных мышц.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=SrqOu55lrYU" },
                new() { Name = "Decline Bench Press", Description = "Жим штанги на скамье с обратным наклоном с акцентом на нижнюю часть грудных мышц.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=LfyQBUKR8SE" },
                new() { Name = "Dumbbell Bench Press", Description = "Жим гантелей лежа с расширенной амплитудой движения и независимой стабилизацией рук.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=VmB1G1K7v94" },
                new() { Name = "Incline Dumbbell Press", Description = "Жим гантелей на наклонной скамье для максимальной гипертрофии верхнего пучка груди.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=8iPEnn-ltC8" },
                new() { Name = "Dumbbell Fly", Description = "Изолированное разведение гантелей лежа для растяжения и пампинга грудных мышц.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=eozdVDA78K0" },
                new() { Name = "Cable Crossover", Description = "Сведение рук в кроссовере на блоках с постоянным мышечным натяжением по всей траектории.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=taI4XduLpTk" },
                new() { Name = "Push-Up", Description = "Базовые классические отжимания от пола с собственным весом тела для груди и трицепса.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=IODxDxX7oi4" },
                new() { Name = "Chest Dips", Description = "Отжимания на брусьях с наклоном корпуса вперед для мощной проработки низа груди.", MuscleGroup = "Chest", VideoUrl = "https://www.youtube.com/watch?v=2z8JmcrW-As" },

                // BACK
                new() { Name = "Chin-Up", Description = "Подтягивания обратным хватом с мощным вовлечением широчайших и бицепсов.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=brhRXlOhsAM" },
                new() { Name = "Lat Pulldown", Description = "Тяга верхнего вертикального блока к груди с контролируемым сведением лопаток.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=CAwf7n6Luuc" },
                new() { Name = "Barbell Row", Description = "Тяга штанги в наклоне для толщины спины, ромбовидных мышц и широчайших.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=FWJR5Ve8gkQ" },
                new() { Name = "Dumbbell Row", Description = "Тяга одной гантели к поясу с упором в скамью для устранения мышечного дисбаланса.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=pYcpY20QaE8" },
                new() { Name = "Seated Cable Row", Description = "Горизонтальная тяга блока к поясу сидя для средней части спины и плотности широчайших.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=GZbfZ033f74" },
                new() { Name = "T-Bar Row", Description = "Тяга Т-образного грифа для максимальной перегрузки центра спины и трапеций.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=j3Igk5nyZE4" },
                new() { Name = "Face Pull", Description = "Тяга каната на блоке к лицу для здоровья плечевых суставов и задних дельт.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=rep-qVOkqgk" },
                new() { Name = "Straight-Arm Pulldown", Description = "Пуловер на верхнем блоке прямыми руками для изоляции широчайших мышц.", MuscleGroup = "Back", VideoUrl = "https://www.youtube.com/watch?v=G3Z7Fcx_0_g" },

                // LEGS
                new() { Name = "Front Squat", Description = "Фронтальные приседания со штангой на груди с акцентом на квадрицепсы и вертикальный корпус.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=uYumuL_G_V0" },
                new() { Name = "Goblet Squat", Description = "Кубковые приседания с гантелью или гирей у груди для оттачивания мобильности и глубины.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=MeIiIdhvXT4" },
                new() { Name = "Leg Press", Description = "Жим ногами в платформенном тренажере для тяжелой нагрузки на квадрицепсы без осевой нагрузки.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=IZxyjW7MPJQ" },
                new() { Name = "Bulgarian Split Squat", Description = "Болгарские сплит-приседания с опорной ногой на скамье для ягодиц и квадрицепсов.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=2C-uNgKwPLE" },
                new() { Name = "Walking Lunges", Description = "Шагающие выпады с гантелями для гипертрофии квадрицепсов, ягодиц и динамического баланса.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=L8fvypPrzzs" },
                new() { Name = "Romanian Deadlift", Description = "Румынская становая тяга на прямых ногах для растяжения бицепса бедра и ягодиц.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=JCXUYuzwNrM" },
                new() { Name = "Leg Curl", Description = "Сгибание ног лежа в тренажере для акцентированной изоляции бицепсов бедер.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=1Tq3QdYUuHs" },
                new() { Name = "Leg Extension", Description = "Разгибание ног сидя в тренажере для детализации прямой и латеральной головок квадрицепса.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=YyvSfVjQeL0" },
                new() { Name = "Calf Raise", Description = "Подъемы на носки стоя со штангой или в тренажере для развития икроножных мышц.", MuscleGroup = "Legs", VideoUrl = "https://www.youtube.com/watch?v=-M4-G8p8fmc" },

                // SHOULDERS
                new() { Name = "Dumbbell Shoulder Press", Description = "Жим гантелей сидя для передних и средних пучков дельтовидных мышц.", MuscleGroup = "Shoulders", VideoUrl = "https://www.youtube.com/watch?v=qEwKCR5JCog" },
                new() { Name = "Arnold Press", Description = "Жим Арнольда с разворотом кистей во время амплитуды для полной активации всех пучков дельт.", MuscleGroup = "Shoulders", VideoUrl = "https://www.youtube.com/watch?v=6Z15_WdXmVw" },
                new() { Name = "Lateral Raise", Description = "Махи гантелями через стороны для взрывного роста ширины средних дельт.", MuscleGroup = "Shoulders", VideoUrl = "https://www.youtube.com/watch?v=3VcKaXpzqRo" },
                new() { Name = "Front Raise", Description = "Подъем гантелей перед собой для прицельного развития передней дельтовидной мышцы.", MuscleGroup = "Shoulders", VideoUrl = "https://www.youtube.com/watch?v=-t7fuZ0KhDA" },
                new() { Name = "Rear Delt Fly", Description = "Разведение гантелей в наклоне для задних дельт и осанки плечевого пояса.", MuscleGroup = "Shoulders", VideoUrl = "https://www.youtube.com/watch?v=0G2_XV7slIg" },
                new() { Name = "Upright Row", Description = "Тяга штанги или троса к подбородку для средних дельт и верха трапеций.", MuscleGroup = "Shoulders", VideoUrl = "https://www.youtube.com/watch?v=amCU-ziHITM" },

                // ARMS
                new() { Name = "Barbell Curl", Description = "Подъем штанги на бицепс стоя — классический золотой стандарт силы рук.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=kwG2ipFRgfo" },
                new() { Name = "Dumbbell Curl", Description = "Поочередный подъем гантелей с супинацией кисти для пика бицепса.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=ykJmrZ5v0Oo" },
                new() { Name = "Hammer Curl", Description = "Сгибания «Молот» с нейтральным хватом для брахиалиса и предплечий.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=zC3nLlEvin4" },
                new() { Name = "Preacher Curl", Description = "Сгибания рук на скамье Скотта, исключающие читинг и фиксирующие локти.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=fIWP-FRFNU0" },
                new() { Name = "Tricep Pushdown", Description = "Разгибание на трицепс на верхнем блоке с канатной или прямой рукоятью.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=2-LAMcpzODU" },
                new() { Name = "Overhead Tricep Extension", Description = "Французский жим гантели из-за головы сидя для растяжения длинной головки трицепса.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=_gsUck-7M74" },
                new() { Name = "Skull Crushers", Description = "Французский жим лежа с EZ-грифом ко лбу для максимального объема трицепса.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=d_KZxkY_0cM" },
                new() { Name = "Close-Grip Bench Press", Description = "Жим штанги лежа узким хватом — базовое многосуставное упражнение для трицепсов.", MuscleGroup = "Arms", VideoUrl = "https://www.youtube.com/watch?v=nEF0bv2FW94" },

                // CORE
                new() { Name = "Plank", Description = "Изометрическая планка на предплечьях для укрепления глубоких мышц кора и поперечной мышцы.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=pSHjTRCQxIw" },
                new() { Name = "Side Plank", Description = "Боковая планка с опорой на одно предплечье для косых мышц живота и стабилизаторов таза.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=K2VljzCC16g" },
                new() { Name = "Crunch", Description = "Классические скручивания на коврике с округлением спины для прямого пресса.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=Xyd_fa5zoEU" },
                new() { Name = "Bicycle Crunch", Description = "Динамические скручивания «велосипед» с диагональным касанием локтем колена.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=9FGilxCbdz8" },
                new() { Name = "Leg Raise", Description = "Подъем прямых ног лежа на полу для нижней части брюшного пресса.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=JB2oyawG9KI" },
                new() { Name = "Hanging Leg Raise", Description = "Подъем ног в висе на турнике — продвинутое упражнение для мощного пресса.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=Pr1ieGZ5atk" },
                new() { Name = "Russian Twist", Description = "Русские скручивания сидя с отрывом стоп для ротационной силы кора.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=wkD8rjkodUI" },
                new() { Name = "Mountain Climber", Description = "Упражнение «Скалолаз» в упоре лежа — укрепляет кор и дает высокий кардио-отклик.", MuscleGroup = "Core", VideoUrl = "https://www.youtube.com/watch?v=nmwgirgXLYM" },

                // CARDIO / FULL BODY
                new() { Name = "Jumping Jacks", Description = "Прыжки с разведением рук и ног для активной разминки и сжигания калорий.", MuscleGroup = "Cardio", VideoUrl = "https://www.youtube.com/watch?v=c4DAnQ6DtF8" },
                new() { Name = "Burpees", Description = "Бёрпи — функциональный комплекс из падения в упор, отжимания и выпрыгивания вверх.", MuscleGroup = "Cardio", VideoUrl = "https://www.youtube.com/watch?v=dZgVxmf6jkA" },
                new() { Name = "High Knees", Description = "Бег на месте с высоким подниманием коленей для ускорения ЧСС и кардио-выносливости.", MuscleGroup = "Cardio", VideoUrl = "https://www.youtube.com/watch?v=oDdkytliOqE" },
                new() { Name = "Box Jump", Description = "Плиометрические запрыгивания на тумбу для взрывной силы ног и координации.", MuscleGroup = "Cardio", VideoUrl = "https://www.youtube.com/watch?v=52r_Ul5k03g" },
                new() { Name = "Bodyweight Squat", Description = "Приседания с весом собственного тела в динамическом темпе для выносливости и тонуса.", MuscleGroup = "Cardio", VideoUrl = "https://www.youtube.com/watch?v=aclHkVaku9U" }
            };

            foreach (var ex in catalog)
            {
                if (!await db.Exercises.AnyAsync(e => e.Name.ToLower() == ex.Name.ToLower()))
                {
                    db.Exercises.Add(ex);
                }
            }
            await db.SaveChangesAsync();
            logger.LogInformation("Comprehensive catalog of exercises seeded successfully.");
        }
    }
}
