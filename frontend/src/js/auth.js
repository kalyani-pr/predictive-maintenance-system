// Run authentication check immediately
(function () {

  const token = localStorage.getItem("token");

  if (!token) {
    window.location.replace("login.html");
    return;
  }

})();

// Authenticated fetch helper
async function authFetch(url, options = {}) {

  const token = localStorage.getItem("token");

  options.headers = {
    ...(options.headers || {}),
    "Authorization": "Bearer " + token,
    "Content-Type": "application/json"
  };

  const response = await fetch(url, options);

  if (response.status === 401 || response.status === 403) {

    // token invalid or expired
    localStorage.removeItem("token");
    localStorage.removeItem("role");

    window.location.replace("login.html");
  }

  return response;
}

// Logout
function logout() {

  localStorage.removeItem("token");
  localStorage.removeItem("role");

  window.location.replace("login.html");

}

// Hide Admin-only UI elements
function applyRoleProtection() {

  const role = localStorage.getItem("role");

  if (role !== "Admin") {

    document.querySelectorAll("[data-role='admin']")
      .forEach(el => el.style.display = "none");

  }

}

// Run after page loads
document.addEventListener("DOMContentLoaded", applyRoleProtection);

// Read the role stored during login
function showUserRole(){

  const role = localStorage.getItem("role");

  const roleDisplay = document.getElementById("userRoleDisplay");

  if(roleDisplay){
    roleDisplay.innerText = role;
  }

}

document.addEventListener("DOMContentLoaded", showUserRole);

function toggleUserMenu(){

  const dropdown = document.getElementById("userDropdown");

  if(dropdown.style.display === "flex"){
    dropdown.style.display = "none";
  } else {
    dropdown.style.display = "flex";
  }

}

function toggleAlertMenu(){

  const dropdown = document.getElementById("alertDropdown");

  if(!dropdown) return;

  if(dropdown.style.display === "flex"){
    dropdown.style.display = "none";
  } else {
    dropdown.style.display = "flex";
  }

}

async function loadAlertNotifications(){

  const badge = document.getElementById("alertBadge");
  const alertList = document.getElementById("alertList");

  if(!badge || !alertList) return;

  const response = await authFetch(baseUrl + "/api/Alerts");
  const alerts = await response.json();

  const activeAlerts = alerts.filter(a => !a.isAcknowledged);

  badge.innerText = activeAlerts.length;

  alertList.innerHTML = "";

  const recent = activeAlerts.slice(0,5);

  recent.forEach(alert => {

    alertList.innerHTML += `
      <div class="alert-item">
        Machine ${alert.machineId} - ${alert.riskLevel}
      </div>
    `;

  });

}

document.addEventListener("DOMContentLoaded", () => {

  showUserRole();
  loadAlertNotifications();

  setInterval(loadAlertNotifications, 10000);

});