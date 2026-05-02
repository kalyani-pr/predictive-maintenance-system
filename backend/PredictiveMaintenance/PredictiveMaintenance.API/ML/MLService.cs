using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace PredictiveMaintenance.API.ML
{
    public class ScalerParams
    {
        public float[] mean { get; set; }
        public float[] scale { get; set; }
    }
    
    public class MLInferenceService
    {
        private readonly float[] _means;
        private readonly float[] _scales;

        private readonly InferenceSession _session;
        //private const float MaxRUL = 361f; // From training
        private const float MaxRUL = 250f;

        public MLInferenceService()
        {
            var modelPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "ML",
                "rul_model.onnx"
            );

            _session = new InferenceSession(modelPath);

            // 🔹 Load scaler parameters
            var scalerPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "ML",
                "scaler_params.json"
            );

            var json = System.IO.File.ReadAllText(scalerPath);
            var scaler = System.Text.Json.JsonSerializer.Deserialize<ScalerParams>(json);

            _means = scaler.mean;
            _scales = scaler.scale;
        }


        public (float RUL, float HealthIndex, float FailureProbability, string RiskLevel)
            Predict(int cycle, double? pressure, double? vibration, double? temperature)
        {
            // Convert safely to float
            float operatingHours = cycle;
            float pressureVal = (float)(pressure ?? 0);
            float vibrationVal = (float)(vibration ?? 0);
            float temperatureVal = (float)(temperature ?? 0);

            float[] raw = new float[]
            {
                operatingHours,
                pressureVal,
                vibrationVal,
                temperatureVal
            };

            // 🔹 Apply StandardScaler manually
            float[] scaled = new float[4];

            for (int i = 0; i < 4; i++)
            {
                scaled[i] = (raw[i] - _means[i]) / _scales[i];
            }

            var tensor = new DenseTensor<float>(scaled, new[] { 1, 4 });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", tensor)
            };

            using var results = _session.Run(inputs);

            float predictedRUL = results.First().AsEnumerable<float>().First();

            // Derived metrics
            //float healthIndex = (predictedRUL / MaxRUL) * 100f;
            //healthIndex = Math.Clamp(healthIndex, 0f, 100f);

            //float healthIndex = (predictedRUL / 250f) * 100f;
            //healthIndex = Math.Clamp(healthIndex, 0f, 100f);

            float normalized = predictedRUL / MaxRUL;
            normalized = Math.Clamp(normalized, 0f, 1f);

            // Non-linear health curve
            float k = 8f;  // steepness
            float healthIndex = 100f / (1f + MathF.Exp(-k * (normalized - 0.5f)));
            //float healthIndex = 100f * MathF.Pow(normalized, 1.5f);
            healthIndex = Math.Clamp(healthIndex, 0f, 100f);

            float failureProbability = 1f - (predictedRUL / MaxRUL);
            failureProbability = Math.Clamp(failureProbability, 0f, 1f);

            string riskLevel;

            if (healthIndex > 70)
                riskLevel = "Healthy";
            else if (healthIndex > 40)
                riskLevel = "Warning";
            else
                riskLevel = "Critical";

            return (predictedRUL, healthIndex, failureProbability, riskLevel);
        }
    }
}
