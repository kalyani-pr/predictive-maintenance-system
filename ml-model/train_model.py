import pandas as pd

# Define correct column names
columns = ['engine_id', 'cycle'] + \
          [f'op_setting_{i}' for i in range(1, 4)] + \
          [f'sensor_{i}' for i in range(1, 22)]

# Load dataset
data = pd.read_csv("train_FD001.txt", sep=" ", header=None)

# Remove empty columns caused by extra spaces
data = data.dropna(axis=1)

# Assign column names
data.columns = columns

print("Dataset loaded successfully.")
print("Shape:", data.shape)
print("\nFirst 5 rows:")
print(data.head())

# Compute maximum cycle for each engine
max_cycles = data.groupby('engine_id')['cycle'].max().reset_index()
max_cycles.columns = ['engine_id', 'max_cycle']

# Merge max_cycle into main dataset
data = data.merge(max_cycles, on='engine_id')

# Compute RUL
data['RUL'] = data['max_cycle'] - data['cycle']

print("\nRUL calculated successfully.")
print(data[['engine_id', 'cycle', 'max_cycle', 'RUL']].head())

# Select only backend-aligned features
selected_features = [
    'cycle',       # OperatingHours
    'sensor_4',    # Pressure
    'sensor_7',    # Vibration
    'sensor_11'    # Temperature
]

X = data[selected_features]
y = data['RUL']

print("\nSelected features:")
print(X.head())

print("\nTarget (RUL):")
print(y.head())

from sklearn.model_selection import train_test_split

# Get unique engine IDs
engine_ids = data['engine_id'].unique()

# Split engines into train and test groups
train_ids, test_ids = train_test_split(engine_ids, test_size=0.2, random_state=42)

# Create train and test datasets
train_data = data[data['engine_id'].isin(train_ids)]
test_data = data[data['engine_id'].isin(test_ids)]

# Extract features and target
X_train = train_data[selected_features]
y_train = train_data['RUL']

X_test = test_data[selected_features]
y_test = test_data['RUL']

print("\nTrain shape:", X_train.shape)
print("Test shape:", X_test.shape)


from sklearn.preprocessing import StandardScaler

# Initialize scaler
scaler = StandardScaler()

# Fit only on training data
X_train_scaled = scaler.fit_transform(X_train)

# Transform test data
X_test_scaled = scaler.transform(X_test)

print("\nScaling completed.")
print("First scaled training sample:")
print(X_train_scaled[0])

# Export Scaler parameters
import json

scaler_params = {
    "mean": scaler.mean_.tolist(),
    "scale": scaler.scale_.tolist()
}

with open("scaler_params.json", "w") as f:
    json.dump(scaler_params, f)

print("Scaler parameters saved as scaler_params.json")

from sklearn.ensemble import RandomForestRegressor

# Initialize model
model = RandomForestRegressor(
    n_estimators=30,      # Reduced from 100
    max_depth=10,         # Limit tree depth
    random_state=42,
    n_jobs=-1
)

# Train model
model.fit(X_train_scaled, y_train)

print("\nModel training completed.")

from sklearn.metrics import mean_absolute_error, mean_squared_error, r2_score
import numpy as np

# Make predictions
y_pred = model.predict(X_test_scaled)

# Calculate metrics
mae = mean_absolute_error(y_test, y_pred)
rmse = np.sqrt(mean_squared_error(y_test, y_pred))
r2 = r2_score(y_test, y_pred)

print("\nModel Evaluation Results:")
print(f"MAE  : {mae:.2f}")
print(f"RMSE : {rmse:.2f}")
print(f"R2   : {r2:.4f}")

max_rul = data['RUL'].max()
print("\nMaximum RUL in dataset:", max_rul)


# Convert predictions into health index
health_index = (y_pred / max_rul) * 100
health_index = np.clip(health_index, 0, 100)

print("\nSample Health Index values:")
print(health_index[:5])


failure_probability = 1 - (y_pred / max_rul)
failure_probability = np.clip(failure_probability, 0, 1)

print("\nSample Failure Probability values:")
print(failure_probability[:5])

def classify_risk(h):
    if h < 40:
        return "Critical"
    elif h < 70:
        return "Warning"
    else:
        return "Healthy"

risk_levels = [classify_risk(h) for h in health_index]

print("\nSample Risk Levels:")
print(risk_levels[:5])


from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

# Define input shape (4 features)
initial_type = [('float_input', FloatTensorType([None, 4]))]

onnx_model = convert_sklearn(model, initial_types=initial_type)

# Save ONNX model
with open("rul_model.onnx", "wb") as f:
    f.write(onnx_model.SerializeToString())

print("\nONNX model saved as rul_model.onnx")