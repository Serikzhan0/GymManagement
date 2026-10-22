// Global State
let currentUser = null;
let exercisesCache = [];
let trainersCache = [];

// Initialize on DOM load
document.addEventListener("DOMContentLoaded", () => {
  const saved = localStorage.getItem("gym_user");
  if (saved) {
    try {
      currentUser = JSON.parse(saved);
    } catch {
      localStorage.removeItem("gym_user");
    }
  }
  renderAuthState();

  // Set default dates for membership modal
  const today = new Date().toISOString().split("T")[0];
  const nextMonth = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split("T")[0];
  const startInput = document.getElementById("newMemStartDate");
  const endInput = document.getElementById("newMemEndDate");
  if (startInput) startInput.value = today;
  if (endInput) endInput.value = nextMonth;
});

// Toast notification helper
function showToast(message, type = "info") {
  const container = document.getElementById("toastContainer");
  const toast = document.createElement("div");
  toast.className = `toast ${type}`;
  toast.textContent = message;
  container.appendChild(toast);
  setTimeout(() => {
    toast.style.opacity = "0";
    setTimeout(() => toast.remove(), 300);
  }, 4000);
}

// API Fetch Helper
async function api(path, method = "GET", data = null) {
  const headers = { "Content-Type": "application/json" };
  if (currentUser && currentUser.token) {
    headers["Authorization"] = `Bearer ${currentUser.token}`;
  }

  const options = {
    method,
    headers
  };
  if (data) {
    options.body = JSON.stringify(data);
  }

  try {
    const res = await fetch(path, options);
    let body = null;
    const text = await res.text();
    if (text) {
      try {
        body = JSON.parse(text);
      } catch {
        body = text;
      }
    }

    if (!res.ok) {
      if (res.status === 401) {
        showToast("Сессия истекла или требуется авторизация", "error");
        handleLogout();
        throw new Error("Unauthorized");
      }
      const msg = (body && body.message) ? body.message : `Ошибка ${res.status}`;
      throw new Error(msg);
    }

    return body;
  } catch (err) {
    if (err.message !== "Unauthorized") {
      showToast(err.message, "error");
    }
    throw err;
  }
}

// Auth State Rendering
function renderAuthState() {
  const authSection = document.getElementById("authSection");
  const appSection = document.getElementById("appSection");
  const userInfo = document.getElementById("userInfo");
  const userName = document.getElementById("userName");
  const userRole = document.getElementById("userRole");

  if (currentUser) {
    authSection.classList.add("hidden");
    appSection.classList.remove("hidden");
    userInfo.classList.remove("hidden");

    userName.textContent = currentUser.name || currentUser.email;
    userRole.textContent = currentUser.role;
    userRole.className = `badge badge-${currentUser.role}`;

    // Apply role-based button visibility
    const isAdmin = currentUser.role === "Admin";
    const isTrainerOrAdmin = currentUser.role === "Trainer" || currentUser.role === "Admin";

    document.getElementById("addTrainerBtn").classList.toggle("hidden", !isAdmin);
    document.getElementById("addExerciseBtn").classList.toggle("hidden", !isAdmin);
    document.getElementById("addPlanBtn").classList.toggle("hidden", !isTrainerOrAdmin);

    // Load active tab data
    switchMainTab("trainers");
  } else {
    authSection.classList.remove("hidden");
    appSection.classList.add("hidden");
    userInfo.classList.add("hidden");
  }
}

// Quick Login (for exams)
function quickLogin(email, password) {
  document.getElementById("loginEmail").value = email;
  document.getElementById("loginPassword").value = password;
  document.getElementById("loginForm").dispatchEvent(new Event("submit"));
}

// Switch Auth Tab (Login / Register)
function switchAuthTab(tab) {
  document.getElementById("tabLoginBtn").classList.toggle("active", tab === "login");
  document.getElementById("tabRegisterBtn").classList.toggle("active", tab === "register");
  document.getElementById("loginForm").classList.toggle("hidden", tab !== "login");
  document.getElementById("registerForm").classList.toggle("hidden", tab !== "register");
}

// Handle Login
async function handleLogin(e) {
  e.preventDefault();
  const email = document.getElementById("loginEmail").value;
  const password = document.getElementById("loginPassword").value;

  try {
    const res = await api("/api/auth/login", "POST", { email, password });
    currentUser = res;
    localStorage.setItem("gym_user", JSON.stringify(res));
    showToast(`Добро пожаловать, ${res.name}!`, "success");
    renderAuthState();
  } catch (err) {
    // Error toast handled in api()
  }
}

// Handle Register
async function handleRegister(e) {
  e.preventDefault();
  const name = document.getElementById("regName").value;
  const email = document.getElementById("regEmail").value;
  const password = document.getElementById("regPassword").value;
  const role = document.getElementById("regRole").value;

  try {
    const res = await api("/api/auth/register", "POST", { name, email, password, role });
    currentUser = res;
    localStorage.setItem("gym_user", JSON.stringify(res));
    showToast("Регистрация успешна!", "success");
    renderAuthState();
  } catch (err) {
    // Error toast handled in api()
  }
}

// Logout
function handleLogout() {
  currentUser = null;
  localStorage.removeItem("gym_user");
  renderAuthState();
  showToast("Вы вышли из системы", "info");
}
document.getElementById("logoutBtn").addEventListener("click", handleLogout);

// Main Tabs Navigation
function switchMainTab(tab) {
  const tabs = ["trainers", "exercises", "workoutPlans", "memberships"];
  tabs.forEach(t => {
    const btn = Array.from(document.querySelectorAll(".tab-btn")).find(b => b.getAttribute("onclick")?.includes(t));
    const content = document.getElementById(`${t}Tab`);
    if (btn) btn.classList.toggle("active", t === tab);
    if (content) content.classList.toggle("active", t === tab);
  });

  if (tab === "trainers") loadTrainers();
  if (tab === "exercises") loadExercises();
  if (tab === "workoutPlans") loadWorkoutPlans();
  if (tab === "memberships") loadMemberships();
}

// ==================== 1. TRAINERS ====================
async function loadTrainers() {
  try {
    const trainers = await api("/api/trainers");
    trainersCache = trainers;
    const grid = document.getElementById("trainersGrid");
    grid.innerHTML = "";

    if (trainers.length === 0) {
      grid.innerHTML = `<p class="text-muted">Тренеры не найдены.</p>`;
      return;
    }

    trainers.forEach(t => {
      const card = document.createElement("div");
      card.className = "card";
      card.innerHTML = `
        <div>
          <div class="card-title">🧑‍🏫 ${escapeHtml(t.name)}</div>
          <div class="card-meta">🎯 <strong>Специализация:</strong> ${escapeHtml(t.specialization)}</div>
          <div class="card-meta">⏳ <strong>Опыт работы:</strong> ${t.experienceYears} лет</div>
        </div>
        ${currentUser && currentUser.role === "Admin" ? `
          <div class="card-actions">
            <button class="btn btn-danger btn-sm" onclick="handleDeleteTrainer(${t.id})">Удалить</button>
          </div>
        ` : ''}
      `;
      grid.appendChild(card);
    });
  } catch {}
}

function openAddTrainerModal() {
  document.getElementById("newTrainerName").value = "";
  document.getElementById("newTrainerSpec").value = "";
  document.getElementById("newTrainerExp").value = 3;
  openModal("trainerModal");
}

async function handleCreateTrainer(e) {
  e.preventDefault();
  const name = document.getElementById("newTrainerName").value;
  const specialization = document.getElementById("newTrainerSpec").value;
  const experienceYears = parseInt(document.getElementById("newTrainerExp").value);

  try {
    await api("/api/trainers", "POST", { name, specialization, experienceYears });
    closeModal("trainerModal");
    showToast("Тренер успешно добавлен!", "success");
    loadTrainers();
  } catch {}
}

async function handleDeleteTrainer(id) {
  if (!confirm("Вы уверены, что хотите удалить тренера?")) return;
  try {
    await api(`/api/trainers/${id}`, "DELETE");
    showToast("Тренер удален", "success");
    loadTrainers();
  } catch {}
}

// ==================== 2. EXERCISES ====================
async function loadExercises() {
  const muscle = document.getElementById("exerciseMuscleFilter").value;
  const search = document.getElementById("exerciseSearch").value;

  const params = new URLSearchParams();
  if (muscle) params.append("muscleGroup", muscle);
  if (search) params.append("search", search);

  try {
    const exercises = await api(`/api/exercises?${params.toString()}`);
    exercisesCache = exercises;
    const grid = document.getElementById("exercisesGrid");
    grid.innerHTML = "";

    if (exercises.length === 0) {
      grid.innerHTML = `<p class="text-muted">Упражнения не найдены.</p>`;
      return;
    }

    exercises.forEach(e => {
      const card = document.createElement("div");
      card.className = "card";
      card.innerHTML = `
        <div>
          <div class="flex-between">
            <div class="card-title">💪 ${escapeHtml(e.name)}</div>
            <span class="badge badge-Trainer">${escapeHtml(e.muscleGroup)}</span>
          </div>
          <div class="card-body">${escapeHtml(e.description)}</div>
        </div>
        ${currentUser && currentUser.role === "Admin" ? `
          <div class="card-actions">
            <button class="btn btn-danger btn-sm" onclick="handleDeleteExercise(${e.id})">Удалить</button>
          </div>
        ` : ''}
      `;
      grid.appendChild(card);
    });
  } catch {}
}

function handleExerciseSearch() {
  loadExercises();
}

function handleExerciseFilter() {
  loadExercises();
}

function openAddExerciseModal() {
  document.getElementById("newExName").value = "";
  document.getElementById("newExDesc").value = "";
  openModal("exerciseModal");
}

async function handleCreateExercise(e) {
  e.preventDefault();
  const name = document.getElementById("newExName").value;
  const description = document.getElementById("newExDesc").value;
  const muscleGroup = document.getElementById("newExMuscle").value;

  try {
    await api("/api/exercises", "POST", { name, description, muscleGroup });
    closeModal("exerciseModal");
    showToast("Упражнение добавлено!", "success");
    loadExercises();
  } catch {}
}

async function handleDeleteExercise(id) {
  if (!confirm("Удалить это упражнение?")) return;
  try {
    await api(`/api/exercises/${id}`, "DELETE");
    showToast("Упражнение удалено", "success");
    loadExercises();
  } catch {}
}

// ==================== 3. WORKOUT PLANS ====================
async function loadWorkoutPlans() {
  try {
    const plans = await api("/api/workout-plans");
    const grid = document.getElementById("workoutPlansGrid");
    grid.innerHTML = "";

    if (plans.length === 0) {
      grid.innerHTML = `<p class="text-muted">Планы тренировок не найдены.</p>`;
      return;
    }

    const isTrainerOrAdmin = currentUser && (currentUser.role === "Trainer" || currentUser.role === "Admin");

    plans.forEach(p => {
      const card = document.createElement("div");
      card.className = "plan-card";

      let exercisesHtml = "";
      (p.exercises || []).forEach(ex => {
        exercisesHtml += `
          <div class="exercise-tag">
            <span>🏋️ ${escapeHtml(ex.exerciseName)}</span>
            <span><strong>${ex.sets}</strong> подх. × <strong>${ex.repetitions}</strong> повт.</span>
          </div>
        `;
      });

      card.innerHTML = `
        <div style="margin-bottom: 0.8rem;">
          <div class="card-title">📋 ${escapeHtml(p.name)}</div>
          <div class="card-meta">🧑‍🏫 <strong>Тренер:</strong> ${escapeHtml(p.trainerName || 'Не указан')}</div>
          <div class="card-body" style="margin-bottom: 0.8rem;">${escapeHtml(p.description)}</div>
          <div style="font-weight: 600; font-size: 0.85rem; margin-bottom: 0.4rem; color: var(--accent-primary);">
            Упражнения (${(p.exercises || []).length}):
          </div>
          <div>${exercisesHtml}</div>
        </div>
        ${isTrainerOrAdmin ? `
          <div class="card-actions" style="margin-top: 1rem;">
            <button class="btn btn-danger btn-sm" onclick="handleDeletePlan(${p.id})">Удалить план</button>
          </div>
        ` : ''}
      `;
      grid.appendChild(card);
    });
  } catch {}
}

async function openAddPlanModal() {
  document.getElementById("newPlanName").value = "";
  document.getElementById("newPlanDesc").value = "";

  // Populate trainers dropdown
  const trainers = await api("/api/trainers");
  const trainerSelect = document.getElementById("newPlanTrainer");
  trainerSelect.innerHTML = trainers.map(t => `<option value="${t.id}">${escapeHtml(t.name)} (${escapeHtml(t.specialization)})</option>`).join("");

  // Ensure exercises are loaded in cache
  if (exercisesCache.length === 0) {
    exercisesCache = await api("/api/exercises");
  }

  // Clear exercises container and add initial row
  document.getElementById("planExercisesContainer").innerHTML = "";
  addExerciseRowToPlan();

  openModal("workoutPlanModal");
}

function addExerciseRowToPlan() {
  const container = document.getElementById("planExercisesContainer");
  const row = document.createElement("div");
  row.className = "plan-exercise-row";

  const options = exercisesCache.map(e => `<option value="${e.id}">${escapeHtml(e.name)} (${escapeHtml(e.muscleGroup)})</option>`).join("");

  row.innerHTML = `
    <select class="plan-ex-select" style="flex: 2;" required>
      ${options}
    </select>
    <input type="number" class="plan-ex-sets" placeholder="Подходы" value="3" min="1" style="flex: 1;" required>
    <input type="number" class="plan-ex-reps" placeholder="Повторения" value="10" min="1" style="flex: 1;" required>
    <button type="button" class="btn btn-danger btn-sm" onclick="this.parentElement.remove()">✕</button>
  `;
  container.appendChild(row);
}

async function handleCreateWorkoutPlan(e) {
  e.preventDefault();
  const name = document.getElementById("newPlanName").value;
  const description = document.getElementById("newPlanDesc").value;
  const trainerId = parseInt(document.getElementById("newPlanTrainer").value);

  const rows = document.querySelectorAll(".plan-exercise-row");
  const exercises = [];
  rows.forEach(r => {
    exercises.push({
      exerciseId: parseInt(r.querySelector(".plan-ex-select").value),
      sets: parseInt(r.querySelector(".plan-ex-sets").value),
      repetitions: parseInt(r.querySelector(".plan-ex-reps").value)
    });
  });

  try {
    await api("/api/workout-plans", "POST", { name, description, trainerId, exercises });
    closeModal("workoutPlanModal");
    showToast("План тренировок успешно создан!", "success");
    loadWorkoutPlans();
  } catch {}
}

async function handleDeletePlan(id) {
  if (!confirm("Удалить этот план тренировок?")) return;
  try {
    await api(`/api/workout-plans/${id}`, "DELETE");
    showToast("План удален", "success");
    loadWorkoutPlans();
  } catch {}
}

// ==================== 4. MEMBERSHIPS ====================
async function loadMemberships() {
  try {
    const list = await api("/api/memberships");
    const container = document.getElementById("membershipsList");
    container.innerHTML = "";

    const subtitle = document.getElementById("membershipSubtitle");
    if (currentUser.role === "Admin") {
      subtitle.textContent = "Все абонементы клиентов клуба (Администратор)";
    } else {
      subtitle.textContent = "Ваши активные и архивные абонементы";
    }

    if (list.length === 0) {
      container.innerHTML = `<p class="text-muted">Абонементы не найдены. Оформите первый абонемент!</p>`;
      return;
    }

    list.forEach(m => {
      const card = document.createElement("div");
      card.className = "membership-card";

      const start = new Date(m.startDate).toLocaleDateString();
      const end = new Date(m.endDate).toLocaleDateString();

      card.innerHTML = `
        <div>
          <div style="font-weight: 700; font-size: 1.1rem; margin-bottom: 0.2rem;">
            🎟️ Абонемент: ${m.type}
            <span class="badge badge-${m.status}" style="margin-left: 0.5rem;">${m.status}</span>
          </div>
          ${currentUser.role === "Admin" ? `<div class="card-meta">👤 Клиент: <strong>${escapeHtml(m.userName)}</strong> (${escapeHtml(m.userEmail)})</div>` : ''}
          <div class="membership-dates">📅 Период действия: ${start} — ${end}</div>
        </div>
        <div>
          <button class="btn btn-danger btn-sm" onclick="handleDeleteMembership(${m.id})">Отменить / Удалить</button>
        </div>
      `;
      container.appendChild(card);
    });
  } catch {}
}

function openBuyMembershipModal() {
  updateMembershipDates();
  openModal("membershipModal");
}

function updateMembershipDates() {
  const type = document.getElementById("newMemType").value;
  const startInput = document.getElementById("newMemStartDate");
  const endInput = document.getElementById("newMemEndDate");

  let startDate = new Date(startInput.value || Date.now());
  let endDate = new Date(startDate);

  if (type === "Monthly") {
    endDate.setMonth(endDate.getMonth() + 1);
  } else if (type === "Quarterly") {
    endDate.setMonth(endDate.getMonth() + 3);
  } else if (type === "Yearly") {
    endDate.setFullYear(endDate.getFullYear() + 1);
  }

  endInput.value = endDate.toISOString().split("T")[0];
}

async function handleCreateMembership(e) {
  e.preventDefault();
  const type = document.getElementById("newMemType").value;
  const startDate = new Date(document.getElementById("newMemStartDate").value).toISOString();
  const endDate = new Date(document.getElementById("newMemEndDate").value).toISOString();

  try {
    await api("/api/memberships", "POST", {
      userId: currentUser.userId,
      startDate,
      endDate,
      type
    });
    closeModal("membershipModal");
    showToast("Абонемент успешно оформлен!", "success");
    loadMemberships();
  } catch {}
}

async function handleDeleteMembership(id) {
  if (!confirm("Вы уверены, что хотите аннулировать/удалить абонемент?")) return;
  try {
    await api(`/api/memberships/${id}`, "DELETE");
    showToast("Абонемент удален", "success");
    loadMemberships();
  } catch {}
}

// Modal helpers
function openModal(id) {
  document.getElementById(id).classList.remove("hidden");
}

function closeModal(id) {
  document.getElementById(id).classList.add("hidden");
}

function escapeHtml(text) {
  if (!text) return "";
  return text.replace(/[&<>"']/g, m => ({
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': "&quot;",
    "'": "&#039;"
  })[m]);
}
