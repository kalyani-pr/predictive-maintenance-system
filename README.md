# Predictix – AI-Based Predictive Maintenance System

🚀 An end-to-end predictive maintenance system that monitors machine health and predicts failures using machine learning.

---

## 📌 Overview

Predictix is a web-based application that enables real-time monitoring of industrial machines and predicts potential failures using sensor data. The system helps reduce downtime and improve maintenance efficiency through data-driven insights.

---

## ⚙️ Features

- 📊 Real-time machine health monitoring
- 🤖 Machine learning-based prediction (RUL, Health Index, Risk Level)
- 🚨 Alert generation for Warning & Critical states
- 📈 Interactive dashboard with charts
- 🔐 Role-based authentication (Admin / Operator)
- 🧾 Sensor data history tracking
- 🔔 Notification system for active alerts

---

## 🧠 How It Works

1. User enters sensor data (Temperature, Pressure, Vibration, Cycle)
2. Data is sent to backend API
3. ML model predicts Remaining Useful Life (RUL)
4. Health Index & Risk Level are calculated
5. Alerts are generated if needed
6. Results are displayed on dashboard

---

## 🏗️ System Architecture

Frontend → Backend API → ML Model → Database → Dashboard

---

## 🗄️ Database Entities

- Users
- Machines
- SensorReadings
- Predictions
- Alerts

---

## 🔐 Authentication

- JWT-based authentication
- Role-Based Access Control (Admin / Operator)

---

## 📊 ML Model

- Algorithm: Random Forest Regression
- Inputs: Cycle, Temperature, Pressure, Vibration
- Outputs:
  - Remaining Useful Life (RUL)
  - Health Index
  - Failure Probability
  - Risk Level

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/predictive-maintenance-system.git
```

### 2. Run Backend

```bash
cd backend/PredictiveMaintenance.API
dotnet run
```

### 3. Open Frontend

Open in browser:

http://localhost:7262/frontend/login.html

### 🔮 Future Scope
- IoT-based real-time data integration
- Automated email/SMS alerts
- Advanced ML models (LSTM, Deep Learning)
- Cloud deployment (AWS/Azure)
- Mobile application support